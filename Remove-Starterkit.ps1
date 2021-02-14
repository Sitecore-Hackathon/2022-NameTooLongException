#Requires -RunAsAdministrator

Import-Module -Name (Join-Path $PSScriptRoot "_StarterKit\\tools\\_StarterKitFunctions") -Force

Show-HackLogo

if (!(Confirm -Question "This will delete .\docker\**\*, .\Dockerfile and msbuild files from the starterkit.`n`nAre you completely sure you want to do that?")) {
    Write-Host "Okay, nevermind then.." -ForegroundColor Cyan
    exit 0
}

Write-Host "Okay, deleting all the stuff.." -ForegroundColor Red

if (Test-Path .\docker) 
{
    Stop-Docker -DockerRoot ".\\docker"
    Remove-EnvHostsEntry "CM_HOST"
    Remove-EnvHostsEntry "ID_HOST"
    Remove-EnvHostsEntry "RENDERING_HOST"
    Remove-Item ".\\docker" -Force -Recurse
}

Get-ChildItem directory.build.* | ForEach-Object { Remove-Item $_ -Force }

if (Test-Path .\publishsettings.targets) 
{
    Remove-Item .\publishsettings.targets -Force 
}

if (Test-Path .\Dockerfile) 
{
    Remove-Item .\Dockerfile -Force 
}

Write-Host "Job's done.." -ForegroundColor Green