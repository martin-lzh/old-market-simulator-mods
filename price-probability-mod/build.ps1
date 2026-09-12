param([string]$GameDir = 'F:\SteamLibrary\steamapps\common\Old Market Simulator')
$ErrorActionPreference = 'Stop'
$taskRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
dotnet run --project (Join-Path $PSScriptRoot 'tests/Tests.csproj') -c Release
if ($LASTEXITCODE) { throw 'Pricing checks failed.' }
dotnet build (Join-Path $PSScriptRoot 'PriceProbability.csproj') -c Release --nologo "-p:GameDir=$GameDir"
if ($LASTEXITCODE) { throw 'Pricing build failed.' }
dotnet run --project (Join-Path $PSScriptRoot 'tests/ContractChecks.csproj') -c Release "-p:GameDir=$GameDir" '-p:BaseIntermediateOutputPath=obj/Contracts/' -- $GameDir (Join-Path $PSScriptRoot 'bin/Release/netstandard2.1/OldMarket.PriceProbability.dll')
if ($LASTEXITCODE) { throw 'Game patch contract checks failed.' }
$taskOutput = Join-Path $taskRoot 'outputs'
New-Item -ItemType Directory -Force $taskOutput | Out-Null
$taskZip = Join-Path $taskOutput 'OldMarket.PriceProbability-0.1.0.zip'
$taskDll = Join-Path $PSScriptRoot 'bin/Release/netstandard2.1/OldMarket.PriceProbability.dll'
Add-Type -AssemblyName System.IO.Compression
$taskStream = [IO.File]::Open($taskZip, [IO.FileMode]::Create)
$taskArchive = [IO.Compression.ZipArchive]::new($taskStream, [IO.Compression.ZipArchiveMode]::Create)
try {
    # Explicit allowlist: do not package reference assemblies or local game data.
    foreach ($taskEntry in @(
        @{ Source = $taskDll; Entry = 'BepInEx/plugins/OldMarket.PriceProbability/OldMarket.PriceProbability.dll' },
        @{ Source = (Join-Path $PSScriptRoot 'README.md'); Entry = 'README.md' }
    )) {
        $taskTarget = $taskArchive.CreateEntry($taskEntry.Entry).Open()
        $taskSource = [IO.File]::OpenRead($taskEntry.Source)
        try { $taskSource.CopyTo($taskTarget) }
        finally { $taskSource.Dispose(); $taskTarget.Dispose() }
    }
} finally { $taskArchive.Dispose(); $taskStream.Dispose() }
Get-FileHash -LiteralPath $taskZip -Algorithm SHA256
Write-Output "Package: $taskZip (game installation only read)"
