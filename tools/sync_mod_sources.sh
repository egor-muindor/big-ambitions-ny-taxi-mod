#!/usr/bin/env bash
# Копирует исходники мода из рабочего клона SDK (sdk/, не в git) в этот
# репозиторий, чтобы опубликованная версия совпадала с собранной.
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
sdk="$root/sdk"

if [[ ! -d "$sdk/Assets/Mods/NYTaxi" ]]; then
  echo "Не найден $sdk/Assets/Mods/NYTaxi — сначала клонируйте SDK (см. README)" >&2
  exit 1
fi

rm -rf "$root/Assets/Mods/NYTaxi"
mkdir -p "$root/Assets/Mods" "$root/Assets/Editor/ModBuilder"
cp -R "$sdk/Assets/Mods/NYTaxi" "$root/Assets/Mods/NYTaxi"
cp "$sdk/Assets/Mods/NYTaxi.meta" "$root/Assets/Mods/NYTaxi.meta"
cp "$sdk/Assets/Editor/ModBuilder/HeadlessModBuild.cs" \
   "$sdk/Assets/Editor/ModBuilder/HeadlessModBuild.cs.meta" \
   "$root/Assets/Editor/ModBuilder/"

echo "Исходники мода синхронизированы из sdk/"
