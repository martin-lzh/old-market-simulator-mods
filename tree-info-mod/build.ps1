param([Parameter(Mandatory = $true)][string]$GameDir)
$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'tests/Tests.csproj') -c Release
if ($LASTEXITCODE) { throw 'Tree Info tests failed.' }
dotnet build (Join-Path $PSScriptRoot 'TreeInfo.csproj') -c Release --nologo "-p:GameDir=$GameDir"
if ($LASTEXITCODE) { throw 'Tree Info build failed.' }
$taskRoot = Split-Path $PSScriptRoot -Parent
$taskDll = Join-Path $PSScriptRoot 'bin/Release/netstandard2.1/OldMarket.TreeInfo.dll'
dotnet run --project (Join-Path $PSScriptRoot 'tests/ContractChecks.csproj') -c Release "-p:GameDir=$GameDir" -- $GameDir $taskDll
if ($LASTEXITCODE) { throw 'Tree Info contracts failed.' }
$taskZip = Join-Path $taskRoot 'outputs/OldMarket.TreeInfo-0.1.1.zip'
python (Join-Path $taskRoot 'tools/ci.py') package --mod tree-info --variant BepInEx --dll $taskDll --output $taskZip
if ($LASTEXITCODE) { throw 'Tree Info packaging failed.' }
Get-FileHash -Algorithm SHA256 -LiteralPath $taskZip
