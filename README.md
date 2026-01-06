<p align="center">
  <img src="docs/images/icon.svg" alt="Octopus Project Logo" width="250" style="padding: 20px">
</p>

<div align="center">

[![License](https://img.shields.io/badge/license-AGPL--3.0-orange?style=for-the-badge)](LICENSE)
![Last Commit](https://img.shields.io/github/last-commit/meme-bank/api?style=for-the-badge)
![Repo Size](https://img.shields.io/github/repo-size/meme-bank/api?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-10.0-blueviolet?style=for-the-badge)
![Build Status](https://img.shields.io/github/check-runs/meme-bank/api/main?style=for-the-badge)

[![Мы ВКонтукле](https://img.shields.io/badge/Мы_ВКонтукле-4680C2?style=for-the-badge&logo=vk&logoColor=white)](https://vk.com/membank_mm)
[![Website](https://img.shields.io/badge/Сайтик-333333?style=for-the-badge&logo=link&logoColor=white)](https://membank.ru)

</div>

# <p align="center">Ядро НБМ (API) (Проект Octopus)</p>

Исходный код ядра, то бишь API, Народного Банка Мемов, сердца экономики Мемного мира

## Лицензия
Этот проект распространяется под лицензией [GNU Affero General Public License v3.0 (GNU AGPLv3)](LICENSE). Согласно условиям, если вы используете это ПО для предоставления сетевых услуг (SaaS), вы обязаны предоставить исходный код своей версии пользователям.

Проще говоря: Пиздить разрешается, закрывать нельзя

## Экономическая модель (РП документация)
- Стоимость валюты расчитывается от стоимости главной валюты (в нашем случае - Левро).
- Игровой год тождественнен 12 дням, соответственно 1 игровой месяц = 1 день.
- Торговля строится на предметах и услугах.
  - Также имеются депозиты и кредиты
  - Вложения (они же акции)
- Экономическая модель пытается быть реалистичной, при этом сохраняя относительную простоту.
- Имеются блоги и посты в них, с помощью  этого могут быть реализованы новости и дайджесты

## Технологический стек
| Для чего? | Технология |
| :--- | :--- |
| **Язык программирования** | С# |
| **Runtime** | .NET10 (может быть использован и новее) |
| **База данных** | PostgreSQL |
| **Архитектура** | Монолитная, основанная на сервисах, отдельно сервер авторизации |
| **Лицензия** | GNU AGPLv3 |

## !!! Предупреждение !!!
При публикациях форков, не оставляйте, пожалуйста секретных ключей и прочих штук, которые должны оставаться конфидициальными. Используйте для этого Переменные окружение, они же Environment Variables

## Развёртка
1. Для начала убедитесь, что у вас скачан .NET и ASP.NET. Скачать можно через пакетный менеджер Вашего дистрибутива, либо [отсюда](https://dotnet.microsoft.com/ru-ru/download). Также при развёртке нам понадобится [git](https://git-scm.com/install). Также понадобится PostgreSQL
2. Клонируйте репозиторий:
```sh
git clone git@github.com:meme-bank/api.git MembankAPI # Клонируем из github.com:meme-bank/api.gitgithub.com:meme-bank/api.git в папку MembankAPI
```
3. Перейдите в директорию:
```sh
cd MembankAPI # Переходим в директорию MembankAPI
```
4. Создайте базу данных и примените миграции:
```sh
dotnet ef database update
```
  - з.ы. Если не работает, то выполните: `dotnet tool install --global dotnet-ef`
5. Запустите проект:
```sh
dotnet run # Всё само скомпилируется и запустится
```

## Спасибы:
- Прошлые проекты-реализации Народного Банка Мемов (раннее Банка Мемного Социалистического Интернационала):
  - [Urtyom-Alyanov/bank_msi](https://github.com/Urtyom-Alyanov/bank_msi)
  - [Urtyom-Alyanov/pbm-project](https://github.com/Urtyom-Alyanov/pbm-project)
  - [Urtyom-Alyanov/nbm-1.1s](https://github.com/Urtyom-Alyanov/nbm-1.1s)
  - [Urtyom-Alyanov/octopus-bank](https://github.com/Urtyom-Alyanov/octopus-bank)
  - И потерянные навсегда версии...
- Системы Автоматизированной Бюрократии (онги же бот-банки, боты-паспортисты):
  - [Urtyom-Alyanov/lovuschkinsk-bot](https://github.com/Urtyom-Alyanov/lovuschkinsk-bot)
  - [Urtyom-Alyanov/umsr-bot](https://github.com/Urtyom-Alyanov/umsr-bot)
  - А также боты созданные на всяких проприетарных платформах (Бот-банк Гардернии, ранние версии бота Ловушниского)
- И, конечно же, Торговой Мемной Лиге (ренне Банк Кекляндии)
  - [rival-politics/tml-classic](https://github.com/rival-politics/tml-classic)
    - Ну и, конечно же, новым банкам от этого же разработчика, которые тоже утеряны и заброшены...