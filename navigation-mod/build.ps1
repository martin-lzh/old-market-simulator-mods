param([string]$GameDir = 'F:\SteamLibrary\steamapps\common\Old Market Simulator')
$ErrorActionPreference = 'Stop'
$modVersion = ([xml](Get-Content -LiteralPath (Join-Path $PSScriptRoot 'Navigation.csproj') -Raw)).Project.PropertyGroup.Version
$taskRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$taskPhysics = Join-Path $GameDir 'Old Market Simulator_Data/Managed/UnityEngine.PhysicsModule.dll'
if (-not (Test-Path -LiteralPath $taskPhysics -PathType Leaf)) { throw "Required read-only game dependency is missing: $taskPhysics" }
$taskMaps = (Get-Content -LiteralPath (Join-Path $PSScriptRoot 'map-pack.json') -Raw | ConvertFrom-Json).files | Where-Object { $_.name.EndsWith('.json') } | ForEach-Object { Join-Path $PSScriptRoot ('maps/' + $_.name) }
dotnet run --project (Join-Path $PSScriptRoot 'tests/Navigation.Tests.csproj') -c Release -- @taskMaps
if ($LASTEXITCODE) { throw 'Navigation checks failed.' }
dotnet build (Join-Path $PSScriptRoot 'Navigation.csproj') -c Release --nologo "-p:GameDir=$GameDir"
if ($LASTEXITCODE) { throw 'Navigation build failed.' }
$taskOutput = Join-Path $taskRoot 'outputs'
New-Item -ItemType Directory -Force $taskOutput | Out-Null
$taskZip = Join-Path $taskOutput "OldMarket.Navigation-$modVersion.zip"
$taskDll = Join-Path $PSScriptRoot 'bin/Release/netstandard2.1/OldMarket.Navigation.dll'
# Use the same explicit DLL/document/map allowlist and validation as CI releases.
python (Join-Path $taskRoot 'tools/ci.py') package --mod navigation --dll $taskDll --output $taskZip
if ($LASTEXITCODE) { throw 'Navigation package validation failed.' }
Get-FileHash -LiteralPath $taskZip -Algorithm SHA256
Write-Output "Package: $taskZip (game installation only read)"
