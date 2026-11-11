param([switch]$Publish)
$ErrorActionPreference = 'Stop'
Set-Location -LiteralPath $PSScriptRoot
dotnet run --project tests/Bugger.Checks/Bugger.Checks.csproj -c Release
if ($LASTEXITCODE -ne 0) { throw 'Behavior checks failed.' }
dotnet build src/Bugger.App/Bugger.App.csproj -c Release
if ($LASTEXITCODE -ne 0) { throw 'Application build failed.' }
if ($Publish) {
    dotnet publish src/Bugger.App/Bugger.App.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=None -o outputs/Bugger
    if ($LASTEXITCODE -ne 0) { throw 'Publishing failed.' }
    if (Test-Path sounds) {
        New-Item -ItemType Directory -Path outputs/Bugger/sounds -Force | Out-Null
        Copy-Item sounds/*.wav -Destination outputs/Bugger/sounds -Force
    }
}
