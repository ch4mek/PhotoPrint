# План хирургии MMS-базы (по инвентаризации от агента)

## 1. Удаление модулей: Booking, Merch, Cv, Certificate, TimeTrack, TeamWork (Game отсутствует)

Папки src/Modules/<Модуль>.* (по 2–6 csproj) + правки:
- `api/src/Modsen.Portal.Api/Modules/ApiServiceModule.cs` — usings (L10–70), вызовы RegisterBookingModules/TimeTrack/Certificates/TeamWork/Merch/Cv (L86–100) и их определения (L129,157,170,189,225)
- `api/src/Modsen.Portal.Api/Program.cs` — миграции (L45–46, 52–53), PublishExisting*-таски (L60–108), сиды (L90–96, 120–121)
- References: `Modsen.Portal.Api.csproj` L37–58; `Infrastructure/Migrator/...csproj` L16–27; `Excel/...csproj` L11 (TimeTrack); `Csv/...csproj` L9 (TimeTrack)
- Tests: удалить Booking/Certificate/Cv/Merch/TeamWork/TimeTrack.UnitTests; проверить ArchitectureTests
- web: убрать роуты/страницы Booking, Merch, TimeSheet, Recruitment(MFE) из routes/index.tsx + меню

## 2. MySQL (Pomelo) вместо PostgreSQL

- Directory.Packages.props: + Pomelo.EntityFrameworkCore.MySql 9.0.0, − Npgsql.*, HealthChecks.NpgSql→MySql
- UseNpgsql → UseMySql в 32 файлах: 14 DataAccessModule.cs + 14 DesignTime*DbContext.cs + MigratorModule/PortalMigrator/DesignTimeMigratorDbContext/HangfireModule
- Npgsql-специфика в entity-конфигах — 3 места: MicrofrontendsConfigEntity.cs:28 (jsonb→json), EquipmentReport.cs:37, NotificationEntity.cs:35
- Миграции (~90 файлов, 14 папок): НЕ переносим — удалить папки Migrations, пересоздать базу EnsureCreated (хакатон) или сгенерировать свежие dotnet ef
- ConnectionStrings:TrackDb → Server=localhost;3306;kontur;root (пароль в appsettings.Development.json, gitignored)

## 3. Логин: локальный вместо Azure AD

- AccountController.cs: есть только Azure AD (login L56, microsoftLogin L81, callback L113). Добавить POST /auth/local-login (login+password → JWT) по образцу legacy-slice/api TokenService; сиды 10 демо-учёток (director, prod-head, analyst, hr, marketing, support, moderator, prod-worker, employee, client / demo1234)

## 4. Web: @mms/mms-ui-library → своя тема «Тёмная комната»

- 874 файла импортируют ~281 компонент/иконку — ПОФАЙЛОВАЯ ЗАМЕНА НЕРЕАЛЬНА.
- Стратегия: локальный пакет web/packages/mms-ui/ (file:..), реэкспорт из @mui/material + наши тёмные токены (Unbounded/Manrope, #0C0E13/#FFB020 — см. legacy-slice/web/src/styles.css); alias в vite.config + tsconfig на '@mms/mms-ui-library' → этот пакет; theme/index.tsx переписать на getDesignTokens своей палитры
- .yarnrc.yml: убрать gitlab-registry/токен; npm вместо yarn (package-lock)
- Портирование: Пульс + Решения из legacy-slice/web/src/pages (Pulse.tsx, Decisions.tsx, Audit.tsx) в mms-fe роутинг/страницы; API-слой — axios-интерцепторы уже есть

## 5. Портирование логики среза в модули MMS (шаблон Contract/Controllers/Requests/Npgsql)

- Analytics (Пульс: KpiEngine из legacy-slice/api), Decisions, Support, Production, Catalog, Assistant, DataFactory (датасет dataset/seed + генератор)
- Аудит: своя таблица + middleware (образец legacy-slice AuditService)

## Порядок работ
1) API: удалить модули + починить sln/references → build
2) MySQL-замена → build + запуск + EnsureCreated + сиды
3) Local-login + демо-учётки
4) Web: shim-пакет темы → npm i → build
5) Портирование Пульс/Решения в mms-fe
6) Новые API-модули (Analytics/Decisions...) + Smoke-тест из scripts/api_smoke.py
