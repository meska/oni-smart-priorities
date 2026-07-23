#!/bin/zsh
set -euo pipefail

project_dir=${0:A:h}
mod_dir="$HOME/Library/Application Support/unity.Klei.Oxygen Not Included/mods/Local/OniSmartPriorities"
dll_path="$project_dir/src/OniSmartPriorities/bin/Release/net48/OniSmartPriorities.dll"

# Compila prima de tocar la copia che carica el zogo.
dotnet build "$project_dir/src/OniSmartPriorities/OniSmartPriorities.csproj" -c Release
mkdir -p "$mod_dir"
cp "$project_dir/package/mod.yaml" "$mod_dir/mod.yaml"
cp "$project_dir/package/mod_info.yaml" "$mod_dir/mod_info.yaml"
if [[ ! -f "$mod_dir/config.json" ]]; then
  cp "$project_dir/package/config.json" "$mod_dir/config.json"
fi
cp "$dll_path" "$mod_dir/OniSmartPriorities.dll"

echo "Installed Smart Priorities in: $mod_dir"
