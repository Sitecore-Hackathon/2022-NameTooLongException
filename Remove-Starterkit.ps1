#Requires -RunAsAdministrator

Import-Module -Name (Join-Path $PSScriptRoot "_StarterKit\\tools\\StarterKitCLi") -Force

Show-HackLogo

if (!(Confirm -Question "This will delete .\docker\**\*, .\Dockerfile and msbuild files from the starterkit.`n`nAre you really completely sure you want to do that?")) {
    Write-Host "Okay, nevermind then.." -ForegroundColor Cyan
    exit 0
}

Write-Host "Okay, no way back - deleting all the stuff.." -ForegroundColor Red

if (Test-Path .\docker) 
{
    Stop-Docker -DockerRoot ".\\docker" -TakeDown -PruneSystem
    Remove-EnvHostsEntry "CM_HOST"
    Remove-EnvHostsEntry "ID_HOST"
    Remove-EnvHostsEntry "RENDERING_HOST"
    Remove-Item ".\\docker" -Force -Recurse
}


if (Test-Path .\Docker.pubxml) 
{
    Remove-Item .\Docker.pubxml -Force 
}

if (Test-Path .\HelixTemplates) 
{
    Remove-Item .\HelixTemplates -Force -Recurse
}

if (Test-Path .\.helixtemplates) 
{
    Remove-Item .\.helixtemplates -Force
}

if (Test-Path .\Dockerfile) 
{
    Remove-Item .\Dockerfile -Force 
}

if ((Test-Path .\*.sln) -and (Confirm -Question "Would you also like to delete the solution files?")) 
{
    Remove-Item .\*.sln -Force 
    Get-ChildItem directory.build.* | ForEach-Object { Remove-Item $_ -Force }
    Remove-Item .\Packages.props -Force 
}

Write-Host "Job's done.." -ForegroundColor Green