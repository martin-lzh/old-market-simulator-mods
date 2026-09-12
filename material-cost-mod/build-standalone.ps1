param([string]$GameDir = 'F:\SteamLibrary\steamapps\common\Old Market Simulator')
$ErrorActionPreference = 'Stop'
$taskRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$taskBep = Join-Path $taskRoot 'work/bepinex'
$taskCore = Join-Path $taskBep 'BepInEx/core'
# Reuse verified, unmodified third-party libraries from the official archive.
# No BepInEx framework, preloader, config manager or MelonLoader files are packaged.
if (!(Test-Path (Join-Path $taskBep 'winhttp.dll'))) { throw 'Prepare the verified BepInEx 5.4.23.5 archive in work/bepinex first.' }
if ((Get-FileHash (Join-Path $taskBep 'winhttp.dll')).Hash -ne '8C6CDBC38836DEE87E3368F5DE1994D7C0CCEBF29E4CE7ABA3C0981F9375412C') { throw 'Unexpected Doorstop binary.' }
dotnet build (Join-Path $PSScriptRoot 'Bootstrap.csproj') -c Release '-p:BaseIntermediateOutputPath=obj/Bootstrap/'
if ($LASTEXITCODE) { throw 'Bootstrap build failed.' }
dotnet build (Join-Path $PSScriptRoot 'MaterialCost.csproj') -c Release '-p:Loader=Standalone' '-p:BaseIntermediateOutputPath=obj/Standalone/' "-p:GameDir=$GameDir" "-p:BepInExDir=$taskCore"
if ($LASTEXITCODE) { throw 'Cost module build failed.' }
dotnet run --project (Join-Path $PSScriptRoot 'tests/CostTests.csproj') -c Release
if ($LASTEXITCODE) { throw 'Cost tests failed.' }
$taskStage = Join-Path $taskRoot ('work/standalone-package-' + (Get-Date -Format 'yyyyMMdd-HHmmss'))
$taskRuntime = Join-Path $taskStage 'OldMarketMod'
New-Item -ItemType Directory -Path $taskRuntime -Force | Out-Null
Copy-Item (Join-Path $taskBep 'winhttp.dll') $taskStage
Copy-Item (Join-Path $PSScriptRoot 'bin/Bootstrap/Release/netstandard2.1/OldMarket.Bootstrap.dll') $taskRuntime
Copy-Item (Join-Path $PSScriptRoot 'bin/Standalone/Release/netstandard2.1/OldMarket.MaterialCost.dll') $taskRuntime
foreach ($taskLib in @('0Harmony.dll','Mono.Cecil.dll','MonoMod.RuntimeDetour.dll','MonoMod.Utils.dll')) {
    Copy-Item (Join-Path $taskCore $taskLib) $taskRuntime
}
Copy-Item (Join-Path $PSScriptRoot 'STANDALONE.md') (Join-Path $taskStage 'README.md')
Copy-Item (Join-Path $PSScriptRoot 'LICENSE') (Join-Path $taskStage 'LICENSE')
[IO.File]::WriteAllText((Join-Path $taskRuntime 'enabled.txt'), 'true')
[IO.File]::WriteAllText((Join-Path $taskRuntime 'materials-only.txt'), 'false')
@'
[General]
enabled = true
target_assembly = OldMarketMod\OldMarket.Bootstrap.dll
redirect_output_log = false
boot_config_override =
ignore_disable_switch = false
[UnityMono]
dll_search_path_override =
debug_enabled = false
debug_address = 127.0.0.1:10000
debug_suspend = false
'@ | Set-Content -LiteralPath (Join-Path $taskStage 'doorstop_config.ini') -Encoding ascii
$taskManifest = Get-ChildItem $taskStage -File -Recurse | ForEach-Object {
    [pscustomobject]@{Path=[IO.Path]::GetRelativePath($taskStage,$_.FullName); SHA256=(Get-FileHash $_.FullName).Hash}
}
$taskManifest | ConvertTo-Json | Set-Content (Join-Path $taskStage 'manifest.json')
dotnet run --project (Join-Path $PSScriptRoot 'tests/BootstrapChecks.csproj') -c Release '-p:BaseIntermediateOutputPath=obj/BootstrapChecks/' -- $taskStage ($taskStage + '-checks')
if ($LASTEXITCODE) { throw 'Bootstrap checks failed.' }
$taskOutput = Join-Path $taskRoot 'outputs'
New-Item -ItemType Directory $taskOutput -Force | Out-Null
$taskZip = Join-Path $taskOutput 'OldMarket.MaterialCost-0.5.0-Standalone-0.1.0.zip'
Compress-Archive -Path (Join-Path $taskStage '*') -DestinationPath $taskZip -Force
Write-Output "Package: $taskZip"
Write-Output "Stage: $taskStage"
