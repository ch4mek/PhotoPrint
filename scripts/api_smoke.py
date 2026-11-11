#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Smoke-тест платформы «Пульс»: проверяет обязательные критерии трека 1 на живом API.
Запуск: python scripts/api_smoke.py [base_url]   (по умолчанию http://localhost:5253)
Проверяет: ОТ-02 (роли), УП-02 (паспорт KPI), УП-03 (drill-down), УП-04 (решение+журнал), УП-05 (scope).
"""
import json
import sys
import urllib.request
import urllib.error

BASE = sys.argv[1] if len(sys.argv) > 1 else "http://localhost:5253"
PASSED, FAILED = [], []


def call(method, path, token=None, body=None):
    req = urllib.request.Request(BASE + path, method=method)
    req.add_header("Content-Type", "application/json")
    if token:
        req.add_header("Authorization", "Bearer " + token)
    data = json.dumps(body, ensure_ascii=False).encode("utf-8") if body else None
    try:
        with urllib.request.urlopen(req, data=data) as r:
            return r.status, json.loads(r.read().decode("utf-8"))
    except urllib.error.HTTPError as e:
        try:
            return e.code, json.loads(e.read().decode("utf-8"))
        except Exception:
            return e.code, {}


def check(name, cond, detail=""):
    (PASSED if cond else FAILED).append(name)
    print(f"  {'✅' if cond else '❌'} {name}" + (f" — {detail}" if detail else ""))


def login(login, password="demo1234"):
    status, body = call("POST", "/auth/login", body={"login": login, "password": password})
    return body.get("token") if status == 200 else None


print(f"Smoke-тест «Пульса» против {BASE}\n")

# --- ОТ-02: вход под двумя ролями ---
tok_dir = login("director")
tok_head = login("prod-head")
check("ОТ-02 вход директора", tok_dir is not None)
check("ОТ-02 вход руководителя подразделения", tok_head is not None)
check("ОТ-02 неверный пароль отклоняется", login("director", "wrong") is None)

# --- Пульс: KPI и алерты ---
st, pulse = call("GET", "/metrics/pulse", tok_dir)
kpis = {k["code"]: k for k in pulse.get("kpis", [])}
check("Пульс отдаёт ≥8 KPI", st == 200 and len(kpis) >= 8, f"{len(kpis)} метрик")
reds = [a["kpiCode"] for a in pulse.get("criticalAlerts", [])]
check("Красные отклонения попадают в алерты", len(reds) >= 1, ", ".join(reds))

# --- УП-02: стеклянная метрика ---
st, m = call("GET", "/metrics/defect_rate", tok_dir)
check("УП-02 паспорт KPI с формулой и источником",
      st == 200 and m.get("formula") and m.get("source") and m.get("refresh"),
      m.get("formula", ""))

# --- УП-03: drill-down до причины ---
st, br = call("GET", "/metrics/defect_rate/breakdown?by=station", tok_dir)
items = br.get("items", [])
top = items[0]["key"] if items else ""
check("УП-03 drill-down брака по станкам", st == 200 and len(items) >= 1,
      " → ".join(f"{i['key']}:{i['value']}" for i in items[:3]))
st, brw = call("GET", "/metrics/sla_percent/breakdown?by=week", tok_dir)
check("УП-03 drill-down SLA по неделям", st == 200 and len(brw.get("items", [])) >= 2,
      "worst=" + str(min((i["value"] for i in brw.get("items", [])), default="?")))

# --- УП-04: решение создаётся, меняется, журлируется ---
st, dec = call("POST", "/decisions", tok_dir, {
    "title": "Разобрать просрочки доставки",
    "problem": "Заказы в срок 78.8% — красная зона",
    "cause": "CDEK-инцидент на неделе -2",
    "action": "Переключить поток на Яндекс Доставку в пиковые недели",
    "ownerId": "emp-032", "ownerName": "Логист",
    "deadline": "2026-11-18", "priority": 1,
    "kpiCode": "on_time_percent", "refEntity": "cause:delivery",
})
dec_id = dec.get("id", "")
check("УП-04 карточка решения создана", st == 201 and dec_id != "", dec_id)
check("УП-04 зафиксирован KPI на момент создания", dec.get("factAtCreation") is not None,
      f"on_time_percent={dec.get('factAtCreation')}")

st, patched = call("PATCH", f"/decisions/{dec_id}", tok_dir, {"status": "in_progress"})
check("УП-04 статус изменён", st == 200 and patched.get("status") == "in_progress")

st, hist = call("GET", f"/decisions/{dec_id}/history", tok_dir)
actions = [e["action"] for e in hist] if isinstance(hist, list) else []
check("УП-04 история в журнале", "decision.created" in actions and "decision.updated" in actions,
      ", ".join(actions))

# --- Повторная проверка эффекта ---
st, verify = call("POST", f"/decisions/{dec_id}/verify", tok_dir)
check("Проверка эффекта возвращает до/после",
      st == 200 and verify.get("before") is not None and verify.get("after") is not None,
      verify.get("note", ""))

# --- УП-05: изоляция подразделений ---
st, _ = call("GET", "/metrics/pulse?department=dep-mkt", tok_head)
check("УП-05 чужое подразделение запрещено (403)", st == 403)
st, _ = call("GET", "/metrics/pulse?department=dep-prod", tok_head)
check("УП-05 своё подразделение доступно", st == 200)

# --- Аудит фиксирует отказ ---
st, audit = call("GET", "/audit?objectType=department&limit=10", tok_dir)
denied = [e for e in audit.get("items", []) if e["result"] == "denied"] if isinstance(audit, dict) else []
check("Аудит фиксирует попытки доступа", len(denied) >= 1,
      f"{len(denied)} denied-записей")

# --- Аналитику журнал доступен, сотруднику нет ---
tok_emp = login("prod-worker")
st, _ = call("GET", "/audit", tok_emp)
check("Роли различаются: журнал закрыт для оператора", st == 403)

print(f"\nИтог: {len(PASSED)} passed, {len(FAILED)} failed")
if FAILED:
    print("Провалены:", ", ".join(FAILED))
    sys.exit(1)
print("Все обязательные проверки трека 1 (минимальный срез) — PASS")
