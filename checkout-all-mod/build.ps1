param([string]$GameDir = 'F:\SteamLibrary\steamapps\common\Old Market Simulator')
$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'localization/tests/Localization.Tests.csproj') -c Release
if ($LASTEXITCODE) { throw 'Localization checks failed.' }
$taskRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
dotnet run --project (Join-Path $PSScriptRoot 'tests/Tests.csproj') -c Release
if ($LASTEXITCODE) { throw 'Checkout hold checks failed.' }
dotnet build (Join-Path $PSScriptRoot 'CheckoutAll.csproj') -c Release --nologo "-p:GameDir=$GameDir"
if ($LASTEXITCODE) { throw 'Checkout build failed.' }
$taskOutput = Join-Path $taskRoot 'outputs'
New-Item -ItemType Directory -Force $taskOutput | Out-Null
$taskZip = Join-Path $taskOutput 'OldMarket.CheckoutAll-0.1.4.zip'
$taskDll = Join-Path $PSScriptRoot 'bin/Release/netstandard2.1/OldMarket.CheckoutAll.dll'
Add-Type -AssemblyName System.IO.Compression
$taskStream = [IO.File]::Open($taskZip, [IO.FileMode]::Create)
$taskArchive = [IO.Compression.ZipArchive]::new($taskStream, [IO.Compression.ZipArchiveMode]::Create)
try {
    # Explicit allowlist: do not package reference assemblies or local game data.
    foreach ($taskEntry in @(
        @{ Source = $taskDll; Entry = 'BepInEx/plugins/OldMarket.CheckoutAll/OldMarket.CheckoutAll.dll' },
        @{ Source = (Join-Path $PSScriptRoot 'README.md'); Entry = 'README.md' },
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
