# Modsen Management System FE

**Ссылка на доску**: https://jira.modsen.app/secure/RapidBoard.jspa
**Ссылка на документацию**: https://confluence.modsen.app/spaces/TT/overview
**Ссылка на Figma макет**: https://www.figma.com/design/BJe17uJDiUWQ5vnnUpmbDe/MMS-for-dev

**Ссылка на dev версию**: https://mms-fe.development.modsen.app
**Ссылка на qa версию**: https://mms-fe.test.modsen.app
**Ссылка на staging версию**: https://mms-fe.staging.modsen.app

**Ссылка на Swagger**: https://mms-api.development.modsen.app/swagger/

**Ссылка на тестовые аккаунты**: https://confluence.modsen.app/spaces/TT/pages/14319845/Environments+and+Credentials

**Ссылка на онбоардинг**: https://confluence.modsen.app/spaces/TT/pages/14319835/FrontEnd+Newcomer+onboarding

## Начало разработки

Проект необходимо клонировать по ssh ссылке. Для этого необходимо настроить ssh key в своём аккаунте GitLab (docs: https://docs.gitlab.com/ee/user/ssh.html).

###### Release FLOW : https://confluence.modsen.app/spaces/TT/pages/14319785/Release+Flow

## Настройка окружения

Для корректной работы библиотеки `@mms/mms-ui-library` в локальном проекте:

1. В корне проекта создайте файл `.env.yarn` и добавьте в него персональный токен GitLab:

```env
# персональный токен, сгенерированный в GitLab
MMS_UI_LIBRARY_TOKEN=glpat-4wHPKy*****VBHJ3J
```

2. Установите библиотеку:

```bash
yarn add @mms/mms-ui-library
```

> Для подробной инструкции по настройке библиотеки смотрите [UI Library Setup Documentation](https://confluence.modsen.app/spaces/TT/pages/14319808/Add+Modsen+UI+Library+to+MMS).

### UI Kit

###### Локальный запуск проекта с локально собранной ui kit

Для корректной работы рекомендуется запускать скрипт в git bash, в данном случает возможно потребуется авторизация в git. При создании access token в permission достаточно указать только read_repository, это понадобится для клонирования mms-ui-library

```sh
# Пример запуска команды
yarn start:ui main && yarn remove:ui
```

```sh
# Запуск проекта с предварительным запуском сборки ui kit
yarn start:ui # принимает один необязательный аргумент (ветка в mms-ui-library для сборки)
```

```sh
# Возврат проекта в исходное состояние (переустановка mms-ui-library на версию из package.json)
yarn remove:ui
```

## Создание ветки

Новые ветки создаём от существующей ветки develop!

При разработке новой функциональности ветки создаём от существующей ветки develop. Если есть необходимость в исправлении bugs или внести hotfixes, то тогда, ветка создаётся от qa или stage (зависит от того, на каком этапе было найдена ошибка). Каждая новая функциональность на Jira-board имеет эпик, если эпик находится в работе, то ветку надо создавать от ветки которая соответствует эпику.

```sh
git checkout develop
git checkout -b 'new-branch-name'
```

Название новой ветки должно состоять из префикса MMS- и затем номер таски из Jira:

```sh
git checkout -b 'MMS-123'
```

## Именование коммитов

Первый коммит на ветке именуем согласно правилу:

```sh
название-ветки - название-таски-из-Jira
```

Пример:

```sh
MMS-123 - [Certificates] Logic of "Apply" button
```

Для всех последующих коммитов на этой же ветке используем префикс из названия ветки (MMS-123 - ...)

## Pull/Merge request'ы

При открытии pull request'а обязательно в поле base указывать ветку откуда вы открывали свою ветку. Это может быть qa, stage, develop, MMS-<Number> - эпик!

## Вливание PR'ов и MR'ов в develop

Вливание своих пулл реквестов в develop ветку после успешного ревью каждого разработчика из команды производится самостоятельно

## Вливание PR'ов и MR'ов в QA

Вливание большей части функциональности в QA происходит в соответствии с релиз планом. Обращаем внимание на "версию исправления" в задаче на Jira-board. Если там версия исправления соответствует текущей версии, то изменения происходит на QA окружении, если версия исправления будущих периодов, то речь идет о develop окружении.
После того как разработчики, заканчивают спринт/релиз-план, техлид или тимлид команды открывает PR из ветки develop в QA и далее на Jira-board задачи которые, вошли в спринт перемещает в колонку "Ready for testing".

## Jira epics

Большая функциональность выносится в отдельные эпики. Таким образом, когда разработчики начинают работу над эпиком, необходимо создать ветку, название которой будет соответствовать MMS-<Number> где Number это номер задачи, которая включает в себя все задачи эпика. Далее нужно внести соответствующее название For example: [Desk booking module] и далее все задачи, которые относятся к эпику, будут создаваться от этой ветки и вливаться в неё.

## Jira remarks

Development flow Jira: https://confluence.modsen.app/spaces/TT/pages/14319756/Jira+Usage+Guidelines
