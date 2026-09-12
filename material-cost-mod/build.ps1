param(
    [string]$GameDir = 'F:\SteamLibrary\steamapps\common\Old Market Simulator',
    [string]$BepInExDir,
    [ValidateSet('BepInEx', 'MelonLoader')][string]$Loader = 'BepInEx',
    [string]$MelonLoaderDir
)
$ErrorActionPreference = 'Stop'
$projectRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$workDir = Join-Path $projectRoot 'work'
$outputDir = Join-Path $projectRoot 'outputs'
New-Item -ItemType Directory -Force $workDir, $outputDir | Out-Null
if ($Loader -eq 'BepInEx' -and -not $BepInExDir) {
    $BepInExDir = Join-Path $workDir 'bepinex\BepInEx\core'
    if (-not (Test-Path -LiteralPath (Join-Path $BepInExDir 'BepInEx.dll'))) {
        $archive = Join-Path $workDir 'BepInEx_win_x64_5.4.23.5.zip'
        Invoke-WebRequest 'https://github.com/BepInEx/BepInEx/releases/download/v5.4.23.5/BepInEx_win_x64_5.4.23.5.zip' -OutFile $archive
        if ((Get-FileHash -LiteralPath $archive).Hash -ne '82F9878551030F54657792C0740D9D51A09500EEAE1FBA21106B0C441E6732C4') { throw 'BepInEx checksum mismatch.' }
        Expand-Archive -LiteralPath $archive -DestinationPath (Join-Path $workDir 'bepinex') -Force
    }
}
if ($Loader -eq 'MelonLoader' -and -not $MelonLoaderDir) {
    $MelonLoaderDir = Join-Path $workDir 'melonloader-0.7.3\MelonLoader\net35'
    if (-not (Test-Path -LiteralPath (Join-Path $MelonLoaderDir 'MelonLoader.dll'))) {
        $archive = Join-Path $workDir 'MelonLoader-0.7.3-x64.zip'
        Invoke-WebRequest 'https://github.com/LavaGang/MelonLoader/releases/download/v0.7.3/MelonLoader.x64.zip' -OutFile $archive
        if ((Get-FileHash -LiteralPath $archive).Hash -ne '5B2B2F3D1CD42B59EC886C5BDC2663EDAE87A0097A4F4A8F58C0965A99DDA416') { throw 'MelonLoader checksum mismatch.' }
        Expand-Archive -LiteralPath $archive -DestinationPath (Join-Path $workDir 'melonloader-0.7.3') -Force
    }
}
dotnet run --project (Join-Path $PSScriptRoot 'tests\CostTests.csproj') -c Release
if ($LASTEXITCODE -ne 0) { throw 'Accounting tests failed.' }
$buildArgs = @('-c', 'Release', '--nologo', "-p:GameDir=$GameDir", "-p:Loader=$Loader", "-p:BaseIntermediateOutputPath=obj/$Loader/")
if ($BepInExDir) { $buildArgs += "-p:BepInExDir=$BepInExDir" }
if ($MelonLoaderDir) { $buildArgs += "-p:MelonLoaderDir=$MelonLoaderDir" }
dotnet build (Join-Path $PSScriptRoot 'MaterialCost.csproj') @buildArgs
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
# Stage an explicit allowlist only. Never redistribute game reference assemblies.
$package = Join-Path $workDir "material-cost-package-$Loader"
$pluginRelative = if ($Loader -eq 'MelonLoader') { 'Mods/OldMarket.MaterialCost.dll' } else { 'BepInEx/plugins/OldMarket.MaterialCost/OldMarket.MaterialCost.dll' }
$pluginDir = Split-Path -Parent (Join-Path $package $pluginRelative)
New-Item -ItemType Directory -Force $pluginDir | Out-Null
$dll = Join-Path $PSScriptRoot "bin\$Loader\Release\netstandard2.1\OldMarket.MaterialCost.dll"
Copy-Item -LiteralPath $dll -Destination $pluginDir
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'README.md') -Destination (Join-Path $package 'README.md')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'LICENSE') -Destination (Join-Path $package 'LICENSE')
$zip = Join-Path $outputDir "OldMarket.MaterialCost-0.5.0-$Loader.zip"
# Pass file paths, not a recursive staging directory, to prevent stale file inclusion.
# ZipFile preserves the installation directory structure with an explicit entry list.
Add-Type -AssemblyName System.IO.Compression
$stream = [System.IO.File]::Open($zip, [System.IO.FileMode]::Create)
$zipFile = [System.IO.Compression.ZipArchive]::new($stream, [System.IO.Compression.ZipArchiveMode]::Create)
try {
    foreach ($entryPath in @($pluginRelative, 'README.md', 'LICENSE')) {
        $entry = $zipFile.CreateEntry($entryPath)
        $entryStream = $entry.Open()
        $sourceStream = [System.IO.File]::OpenRead((Join-Path $package $entryPath))
        try { $sourceStream.CopyTo($entryStream) }
        finally { $sourceStream.Dispose(); $entryStream.Dispose() }
    }
} finally { $zipFile.Dispose(); $stream.Dispose() }
Copy-Item -LiteralPath $dll -Destination (Join-Path $outputDir "OldMarket.MaterialCost-$Loader.dll")
Get-FileHash -LiteralPath $zip -Algorithm SHA256
Write-Host "Built $zip. Game installation was only read."
