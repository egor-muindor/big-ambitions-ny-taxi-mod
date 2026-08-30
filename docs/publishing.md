# Публикация NY Taxi в Steam Workshop

Готовые тексты для формы Mod Creator и порядок действий.

## Порядок действий (в игре, на вашем ПК)

1. Собрать мод (см. [building.md](building.md)).
2. Убедиться, что папка мода в `ModsLocal` содержит `thumbnail.png` в корне —
   **Mod Builder его не копирует**, кладём вручную из
   `sdk/Assets/Mods/NYTaxi/thumbnail.png`.
   Windows: `%UserProfile%\AppData\LocalLow\Hovgaard Games\Big Ambitions\ModsLocal\NYTaxi\`,
   macOS: `~/Library/Application Support/Hovgaard Games/Big Ambitions/ModsLocal/NYTaxi/`.
3. Запустить игру через Steam (Steam должен быть залогинен — загрузка идёт под
   вашей учётной записью через Steamworks UGC).
4. Главное меню → Mods → Mod Creator → Create new mod.
5. Browse mod folder → проверить, что NYTaxi на месте; выбрать NYTaxi в
   выпадающем списке.
6. Заполнить форму текстами ниже, выбрать thumbnail, нажать Upload.
7. После загрузки игра сама подпишется на предмет; проверить страницу мода в
   Workshop и что мод работает при загрузке из Workshop.

Обновление версии позже: тот же путь, но Edit mod → Update (изменить Version в
`ModManifest.asset` и заполнить changelog).

## Title

```
NY Taxi - Call a Cab by Phone
```

## Description (EN)

```
Tired of chasing taxis down the street? Now you can just call one.

NY Taxi adds a taxi dispatch contact to your phone. Step outside, give them a
call, and a cab will be sent to your location.

HOW IT WORKS
- A new contact appears in your phone: NY Taxi.
- Call from the street and the dispatcher sends a cab your way.
- The cab takes 5-10 in-game minutes to reach you - you wait, just like in real
  life.
- When it arrives, the usual taxi map opens and you pick your destination.
- Prices are exactly the same as hailing a taxi on the street. No markup.
- Calling from inside a building, an underground parking or a vehicle won't
  work - the dispatcher will politely ask you to step outside.

Rides booked through NY Taxi count towards your taxi ride statistics, just like
regular ones.

Available in English and Russian.
```

## Description (RU, если захотите отдельно)

```
Надоело ловить такси на улице? Теперь можно просто позвонить.

NY Taxi добавляет в телефон контакт диспетчерской службы такси. Выходите на
улицу, звоните — и машину подадут к вам.

КАК РАБОТАЕТ
- В телефоне появляется контакт NY Taxi.
- Звоните с улицы — диспетчер высылает машину.
- Такси едет к вам 5–10 игровых минут: ожидание, как в жизни.
- Когда машина приезжает, открывается обычная карта выбора точки назначения.
- Цены ровно те же, что у такси, пойманного на улице. Без наценок.
- Из здания, подземного паркинга или машины вызвать не получится — диспетчер
  вежливо попросит выйти на улицу.

Поездки через NY Taxi учитываются в статистике поездок на такси, как обычные.

Available in all 22 game languages.
```

## Changelog для версии 1.0.0

```
Initial release.
```
