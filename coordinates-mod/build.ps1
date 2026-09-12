param([string]$GameDir = 'F:\SteamLibrary\steamapps\common\Old Market Simulator')
$ErrorActionPreference = 'Stop'
$taskRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
dotnet build (Join-Path $PSScriptRoot 'Coordinates.csproj') -c Release --nologo "-p:GameDir=$GameDir"
if ($LASTEXITCODE) { throw 'Coordinates build failed.' }
$taskOutput = Join-Path $taskRoot 'outputs'
New-Item -ItemType Directory -Force $taskOutput | Out-Null
$taskZip = Join-Path $taskOutput 'OldMarket.Coordinates-0.1.2.zip'
$taskDll = Join-Path $PSScriptRoot 'bin/Release/netstandard2.1/OldMarket.Coordinates.dll'
Add-Type -AssemblyName System.IO.Compression
$taskStream = [IO.File]::Open($taskZip, [IO.FileMode]::Create)
$taskArchive = [IO.Compression.ZipArchive]::new($taskStream, [IO.Compression.ZipArchiveMode]::Create)
try {
    # Explicit allowlist: do not package reference assemblies or local game data.
    foreach ($taskEntry in @(
        @{ Source = $taskDll; Entry = 'BepInEx/plugins/OldMarket.Coordinates/OldMarket.Coordinates.dll' },
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
