# UI Library

## 1. Структура библиотеки

[Architecture](https://confluence.modsen.app/spaces/TT/pages/14319898/Architecture+Tech+Design)

## 2. Версионирование (Semantic Versioning)

Мы используем [SemVer](https://semver.org/):

- **MAJOR** (x.0.0) - обратно несовместимые изменения
- **MINOR** (0.x.0) - новая функциональность без нарушения обратной совместимости
- **PATCH** (0.0.x) - исправления багов без изменения API

### Процесс обновления версии:

1. Определить тип изменений (MAJOR/MINOR/PATCH)
2. После мержа изменений в main запустить в pipeline необходимый stage для обновления версии и деплоя
3. Актуализация CHANGELOG.md происходит автоматически при запуске stage (patch, minor, major)

## 3. Внесение изменений

### 3.1. Рабочий процесс

1. Создать новую ветку от `main` по шаблону `feature/name` или `fix/name`
2. Реализовать изменения
3. Написать тесты (если требуется)
4. Обновить документацию (Storybook)
5. Создать Pull Request
6. Дождаться мержа (мерж производит мейнтейнер библиотеки)

### 3.2. Правила именования коммитов

#### Формат коммитов

```
<type>(<scope>): <subject>
<BLANK LINE>
<body>
<BLANK LINE>
<footer>
```

Пример:

```
refactor(api): redesign service interface

Changed authentication service to use JWT tokens
instead of session cookies for better security

BREAKING CHANGE: New initialization requires config object
Closes #112
```

#### Типы коммитов

| Тип      | Описание                                   | Пример                              |
| -------- | ------------------------------------------ | ----------------------------------- |
| feat     | Новая функциональность                     | feat: add login form                |
| fix      | Исправление ошибки                         | fix: button click handler           |
| docs     | Изменения документации                     | docs: update README                 |
| style    | Изменения форматирования                   | style: format code                  |
| refactor | Рефакторинг кода                           | refactor: auth module               |
| test     | Тесты                                      | test: add unit tests                |
| chore    | Вспомогательные изменения                  | chore: update deps                  |
| perf     | Изменения для улучшения производительности | perf: optimize render               |
| build    | Изменения в системе сборки                 | build: update webpack config        |
| ci       | Изменения CI-конфигурации                  | ci: add github actions              |
| revert   | Откат предыдущего коммита                  | revert: remove experimental feature |

#### Область изменений

Указывается в скобках после типа. Примеры:

```
feat(button): add keyboard support
fix(header): correct mobile layout
```

#### Тело коммита

Формат:

- Начинается после пустой строки
- Объясняет что и почему было изменено

Пример:

```
feat(theme): add dark mode support

Implemented new ThemeProvider component with context API.
Added theme toggle button in header component.
All components now support dynamic theme switching.
```

#### Футер коммита

Содержит:

- Ссылки на задачи (Closes, Fixes)
- BREAKING CHANGES для несовместимых изменений
- Ссылки на связанные коммиты

Пример:

```
Closes #123, #45
See also: #78
BREAKING CHANGE: Drop IE11 support
```

### 3.3. Правила разработки компонентов

- Каждый компонент должен быть самодостаточным
- Документировать все публичные API
- Реализовать responsive-поведение

### 3.4. Тестирование

- Unit-тесты (Jest) для логики компонентов
- Визуальное (скриншотное) тестирование Storybook через Playwright — см. [3.5](#35-визуальные-скриншотные-тесты)

### 3.5. Визуальные (скриншотные) тесты

Кратко: Playwright проходит по stories из собранного Storybook и сравнивает страницу с сохранёнными эталонами. Два размера вьюпорта на story (мобильный и десктоп), детали — в `tests/test-visual.spec.ts` и `playwright.config.ts`.

**Где лежат эталонные скриншоты:** каталог **`tests/test-visual.spec.ts-snapshots/`** — один PNG на комбинацию «story + ширина вьюпорта + проект Playwright». Скриншоты меняются только осознанно (через update-скрипты или CI) и коммитятся вместе с кодом. Отчёты и артефакты прогона: **`playwright-report/`**.

#### Скрипты (`package.json`) — как пользоваться

| Скрипт                              | Когда запускать                        | Что сделает                                                                                                        |
| ----------------------------------- | -------------------------------------- | ------------------------------------------------------------------------------------------------------------------ |
| **`yarn test:docker`**              | Проверка против эталонов               | Скриншотный прогон в контейнере: сборка Storybook, сравнение страниц с **`tests/test-visual.spec.ts-snapshots/`**. |
| **`yarn test:docker:clean`**        | Нужен «чистый» прогон                  | Сначала полная пересборка образа контейнера, затем снова **`yarn test:docker`**.                                   |
| **`yarn test:docker:update`**       | Обновить эталоны после смены UI        | В контейнере перезапись PNG в **`tests/test-visual.spec.ts-snapshots/`**; дальше **закоммить** файлы.              |
| **`yarn test:docker:update:clean`** | То же после пересборки образа без кэша | Пересборка образа без кэша, затем обновление эталонов в контейнере.                                                |
| **`yarn test:report`**              | После любого прогона Playwright        | Открывает HTML-отчёт из **`playwright-report/`**.                                                                  |

#### Теги в stories

Задаются в **`*.stories.tsx`** у `meta` или у конкретной story (массив **`tags`**).

| Тег                  | Поведение                                                                    |
| -------------------- | ---------------------------------------------------------------------------- |
| **`no-test`**        | Story **не участвует** в скриншотных тестах (ни 390, ни 1920).               |
| **`no-test-mobile`** | На вьюпорте **390×844** снимок **не** делается; на **1920×1080** — делается. |

Пример:

```ts
tags: ['no-test'];
tags: ['no-test-mobile'];
```

#### GitLab CI — как взаимодействовать

| Что                        | Как устроено                                                                                                                                                                                                                       |
| -------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **`test-screenshots`**     | Проверяет сгенерированные скриншоты с эталонными, запускается автоматически на пуш в ветку. внутри — **`yarn test:ci`**. Упала — скачай артефакты: **`playwright-report/`**, **`test-results/`** и проверь визуальные расхождения. |
| **`generate-screenshots`** | **Ручная** джоба: выполняет **`yarn test:update`**, при изменении файлов в **`tests/test-visual.spec.ts-snapshots/`** делает коммит **`ci: update screenshots`** и пуш. При необходимости подтяни в проекте запушенные скриншоты   |

---

#### Типичный workflow при смене визуала

1. Локально запусти **`yarn test:docker`** и посмотри результат: тесты либо зелёные, либо покажут отличия от эталона.
2. Если картинка **должна** измениться — выполни **`yarn test:docker:update`**, проверь diff в **`tests/test-visual.spec.ts-snapshots/`** и **закоммить** эти PNG **вместе** с правками кода/stories.
3. Запушь ветку и дождись пайплайна: джоба **`test-screenshots`** должна пройти. Если она красная — открой артефакты **`playwright-report/`** / **`test-results/`** и добейся совпадения с эталонами.

Альтернатива без локального Docker: ручная джоба **`generate-screenshots`** в GitLab (обновит эталоны в репозитории сама — не забудь подтянуть ветку).

## 4. Документирование

Используем Storybook для:

- Демонстрации компонентов
- Документирования пропсов и API
- Отображения состояний компонентов

Для каждого компонента:

1. Создать storybook-файл
2. Описать назначение компонента
3. Показать примеры использования
4. Документировать все пропсы

## 5. Обратная совместимость

- Стараться не ломать существующий API
- При необходимости изменений API:
  - Добавить депрекейшн-варнинг
  - Поддерживать старый API хотя бы 2 минорных версии
  - Указать в документации альтернативы
  - Перенести стори в группу депрекейтед

## 6. Зависимости

- Минимизировать внешние зависимости
- Использовать peerDependencies для основных библиотек (React и т.д.)
- Регулярно обновлять зависимости (`yarn audit`)

## 7. Поддержка

- Вести CHANGELOG.md с описанием изменений
- Обеспечить документацию по миграции между major-версиями
- Поддерживать issue-трекер для багов и feature-реквестов

## 8. Рекомендации

- Следить за производительностью компонентов
- Оптимизировать перерисовки (React.memo, useCallback)
- Использовать CSS-переменные для тем
- Тестировать в разных браузерах

## 9. Post CSS Styles

- В проекте используется **PostCSS** со сборкой переменных через `@import` в `src/styles/variables.css`.
- Тёмная тема реализована через **CSS-переменные** и атрибут `data-theme` на корневом элементе:
- Если у `<html>` стоит `data-theme="dark"` — CSS-переменные переопределяются на тёмные значения.
  Все компоненты, использующие `var(--*)`, автоматически переключаются.

Продуктовые темы поставляются как **standalone CSS-бандлы** с foundational Figma-токенами (`--color-*`, `--size-*`, `--typography-*`, `--opacity-*`, `--shadow-*`).

### Подключение

```tsx
import '@mms/mms-ui-library/themes/mms.css';
```

| Бандл             | Продукт | Accent |
| ----------------- | ------- | ------ |
| `themes/mms.css`  | MMS     | orange |
| `themes/mems.css` | MEMS    | blue   |
| `themes/mtms.css` | MTMS    | teal   |
| `themes/asmt.css` | ASMT    | orange |

### Light / Dark

Два измерения: **продукт** (импорт CSS-файла) и **режим** (атрибут `data-theme` на `<html>`).

```tsx
useEffect(() => {
	document.documentElement.setAttribute(
		'data-theme',
		isDark ? 'dark' : 'light'
	);
}, [isDark]);
```

### Сборка

```bash
yarn build:themes  # postcss-cli → dist/themes/*.css (использует postcss.config.mjs)
```

Исходники редактируются по значениям из Figma:

- `src/styles/shared/` — общие токены (spaces, typography, shadows, …)
- `src/styles/themes/{product}/light.css` и `dark.css` — palette + core
- `src/styles/themes/asmt/` — product-specific overrides (typography, shadows, gaps)

### PostCSS и брейкпоинты

- Брейкпоинты задаются через `@custom-media` в `src/styles/shared/breakpoints.css`, а не через CSS-переменные. На этапе сборки `@csstools/postcss-global-data` подключает этот файл ко всем CSS (включая CSS Modules):

```css
@media (--breakpoint-tablet) {
	.component {
		padding: var(--size-gaps-8);
	}
}
```
