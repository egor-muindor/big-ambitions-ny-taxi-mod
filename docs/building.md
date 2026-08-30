# Сборка мода

## Что нужно

- Unity Hub и Unity **2022.3.62f2** (версия SDK, другие не подойдут)
- Установленная Big Ambitions — из неё берутся игровые DLL
- Активированная лицензия Unity (хватит бесплатной Personal)

## Подготовка

```bash
# официальный SDK кладём рядом с этим репозиторием, в папку sdk/
git clone https://github.com/hovgaardgames/bigambitions.git sdk

# исходники мода — в SDK
cp -R Assets/Mods/NYTaxi sdk/Assets/Mods/
cp Assets/Mods/NYTaxi.meta sdk/Assets/Mods/
cp Assets/Editor/ModBuilder/HeadlessModBuild.cs* sdk/Assets/Editor/ModBuilder/
```

Если правите мод в `sdk/` (так удобнее — Unity сразу видит изменения), верните
исходники в репозиторий перед коммитом:

```bash
./tools/sync_mod_sources.sh
```

## Вариант 1: через Unity

1. Откройте папку `sdk/` в Unity 2022.3.62f2.
2. В приветственном окне укажите путь установки игры и импортируйте DLL.
3. Меню **Big Ambitions → Mod Builder → Build & Install**.

Мод соберётся в `sdk/Output/NYTaxi/` и установится в папку локальных модов.

## Вариант 2: без GUI, из командной строки

```bash
UNITY=/Applications/Unity/Hub/Editor/2022.3.62f2/Unity.app/Contents/MacOS/Unity
GAME="/path/to/Big Ambitions"   # папка, внутри которой лежит Big Ambitions_Data

# 1. импорт игровых DLL
BA_INSTALL_PATH="$GAME" "$UNITY" -batchmode -nographics -projectPath sdk \
  -executeMethod BAModTemplate.Editor.HeadlessModBuild.ImportGameDlls \
  -logFile unity-import.log -quit

# 2. сборка мода
BA_MOD_ID=NYTaxi "$UNITY" -batchmode -nographics -projectPath sdk \
  -executeMethod BAModTemplate.Editor.HeadlessModBuild.BuildMod \
  -logFile unity-build.log
```

**Важно на свежем клоне SDK:** в `sdk/ProjectSettings/ProjectSettings.asset`
дефайн `Standalone: BA_GAME_DLLS_IMPORTED` записан заранее, поэтому Unity в
batchmode пытается компилировать моды до импорта DLL и падает. Очистите
значение (`Standalone: `) перед первым импортом — импортёр вернёт дефайн сам.

## Проверка кода без запуска Unity

Быстрая проверка типов против настоящих DLL игры. Положите
`Big Ambitions_Data/Managed` из установленной игры в `game/` и запустите:

```bash
dotnet build tools/typecheck/NYTaxiTypecheck.csproj
```

Компилирует скрипты мода за пару секунд — удобно, чтобы не гонять Unity на
каждую правку. Если тип не находится, добавьте нужную DLL в список `Reference`
внутри csproj.

## Иконки

`nytaxi-contactname.png` (иконка контакта) и `thumbnail.png` (обложка мода)
генерируются скриптом:

```bash
python3 tools/make_icons.py
```

## Локализация

Каждый язык — отдельный файл в `Assets/Mods/NYTaxi/Locales/`, имя файла
совпадает с кодом языка игры в нижнем регистре (`de.json`, `zh-cn.json`).
Все 22 языка игры уже на месте. Если ключа в языке нет, игра подставит
английский, поэтому новый ключ достаточно добавить хотя бы в `en.json`.

## Готовый мод

Собранная папка `sdk/Output/NYTaxi/` содержит DLL, локали, `enums.txt` и
ассет-бандлы под Windows и macOS. Обложку `thumbnail.png` Mod Builder не
копирует — для публикации положите её в корень папки мода вручную.
