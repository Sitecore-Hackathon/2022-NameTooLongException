#Requires -RunAsAdministrator
[CmdletBinding(DefaultParameterSetName = "no-arguments")]
Param (	
    [Parameter(HelpMessage = "Build Containers")]
    [Alias("b")]
    [switch]$Build
)



$dockerCompose = "docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.mvp.yml"
if ($Build) {
    # Restore dotnet tool for sitecore login and serialization
    dotnet tool restore
    # Build all containers in the Sitecore instance, forcing a pull of latest base containers
    Write-Host "BUILD the docker containers..." -ForegroundColor Green
    Write-Host "$dockerCompose build"
    Invoke-Expression "$dockerCompose build"
}
$composeFiles = @(".\docker-compose.yml", ".\docker-compose.override.yml", ".\docker-compose.mvp.yml")
$HostDomain = "sc-mvp.localhost"

Start-Docker -ComposeFiles $composeFiles

Push-Items -IdHost "https://id.$($HostDomain)" -CmHost "https://cm.$($HostDomain)"

Write-Host "`nMVP site is accessible on https://mvp.$HostDomain/`n`nUse the following command to monitor:"  -ForegroundColor Magenta
Write-PrePrompt
Write-Host "docker logs -f mvp-rendering`n"

Write-Host "Opening cm in browser..." -ForegroundColor Green
Start-Process https://cm.$HostDomain/sitecore/
Write-Host "Opening mvp site in browser..." -ForegroundColor Green
Start-Process "https://mvp.sc-mvp.localhost"


Write-Host "Use the following command to bring your docker environment down again:" -ForegroundColor Green
Write-Host ".\Stop-Environment.ps1"