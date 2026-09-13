param([string]$GameDir = 'F:\SteamLibrary\steamapps\common\Old Market Simulator')
$ErrorActionPreference = 'Stop'
$modVersion = ([xml](Get-Content -LiteralPath (Join-Path $PSScriptRoot 'Navigation.csproj') -Raw)).Project.PropertyGroup.Version
$taskRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$taskPhysics = Join-Path $GameDir 'Old Market Simulator_Data/Managed/UnityEngine.PhysicsModule.dll'
if (-not (Test-Path -LiteralPath $taskPhysics -PathType Leaf)) { throw "Required read-only game dependency is missing: $taskPhysics" }
dotnet run --project (Join-Path $PSScriptRoot 'tests/Navigation.Tests.csproj') -c Release
if ($LASTEXITCODE) { throw 'Navigation checks failed.' }
dotnet build (Join-Path $PSScriptRoot 'Navigation.csproj') -c Release --nologo "-p:GameDir=$GameDir"
if ($LASTEXITCODE) { throw 'Navigation build failed.' }
$taskOutput = Join-Path $taskRoot 'outputs'
New-Item -ItemType Directory -Force $taskOutput | Out-Null
$taskZip = Join-Path $taskOutput "OldMarket.Navigation-$modVersion.zip"
$taskDll = Join-Path $PSScriptRoot 'bin/Release/netstandard2.1/OldMarket.Navigation.dll'
Add-Type -AssemblyName System.IO.Compression
$taskStream = [IO.File]::Open($taskZip, [IO.FileMode]::Create)
$taskArchive = [IO.Compression.ZipArchive]::new($taskStream, [IO.Compression.ZipArchiveMode]::Create)
try {
    # Explicit allowlist: do not package reference assemblies or local game data.
    foreach ($taskEntry in @(
        @{ Source = $taskDll; Entry = 'BepInEx/plugins/OldMarket.Navigation/OldMarket.Navigation.dll' },
        @{ Source = (Join-Path $PSScriptRoot 'README.md'); Entry = 'README.md' },
        @{ Source = (Join-Path $PSScriptRoot 'CHANGELOG.md'); Entry = 'CHANGELOG.md' },
        @{ Source = (Join-Path $PSScriptRoot 'LICENSE'); Entry = 'LICENSE' }
    )) {
        $taskTarget = $taskArchive.CreateEntry($taskEntry.Entry).Open()
        $taskSource = [IO.File]::OpenRead($taskEntry.Source)
        try { $taskSource.CopyTo($taskTarget) }
        finally { $taskSource.Dispose(); $taskTarget.Dispose() }
    }
} finally { $taskArchive.Dispose(); $taskStream.Dispose() }
Get-FileHash -LiteralPath $taskZip -Algorithm SHA256
Write-Output "Package: $taskZip (game installation only read)"

