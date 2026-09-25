$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$buildDir = Join-Path $repoRoot "Builds\Windows"
$exe = Join-Path $buildDir "VANTA_BLACK_HORIZON.exe"
$iss = Join-Path $repoRoot "Installer\VANTA_Black_Horizon.iss"

if (-not (Test-Path $exe)) {
    throw "Windows build not found: $exe. Run Unity's VANTA/Build Windows x64 command first."
}

$iscc = Get-Command iscc -ErrorAction SilentlyContinue
if (-not $iscc) {
    $candidates = @(
        "$env:ProgramFiles(x86)\Inno Setup 6\ISCC.exe",
        "$env:ProgramFiles\Inno Setup 6\ISCC.exe"
    )
    $isccPath = $candidates | Where-Object { Test-Path $_ } | Select-Object -First 1
    if (-not $isccPath) {
        throw "Inno Setup 6 (ISCC.exe) was not found."
    }
} else {
    $isccPath = $iscc.Source
}

& $isccPath $iss
if ($LASTEXITCODE -ne 0) {
    throw "Inno Setup failed with exit code $LASTEXITCODE."
}

Write-Host "Installer created under Builds\Installer."
