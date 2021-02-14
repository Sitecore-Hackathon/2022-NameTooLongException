#Requires -RunAsAdministrator

Import-Module -Name (Join-Path $PSScriptRoot "_StarterKit\\tools\\_StarterKitFunctions") -Force

Show-HackLogo

if (Test-IsEnvInitialized -FilePath ".\\docker\\.env" ) {
    Write-Host "Docker environment is present, starting docker.." -ForegroundColor Green

    if (!(Test-Path ".\\docker\\traefik\\certs\\cert.pem")) {
        Write-Host "TLS certificate for Traefik not found, generating and adding hosts file entries" -ForegroundColor Green
        Install-SitecoreDockerTools
        $hostDomain = Get-EnvValueByKey "HOST_DOMAIN"
        if ($hostDomain -eq "") 
        {
            throw "Required variable 'HOST_DOMAIN' not set in .env file."
        }
        Initialize-HostNames $hostDomain
    }
    Start-Docker -Url "$(Get-EnvValueByKey "CM_HOST")/sitecore"
    exit 0
}

if ( !(Test-Path ".\\_StarterKit\\docker")) {
    Write-Host "Boilerplate docker setups not found - you're on your own.." -ForegroundColor Green
    exit 0
}

if (!(Test-Path (Join-Path $PSScriptRoot "_Boilerplate.sln"))) {
    if (!(Confirm "Solution file has been renamed but no docker-compose environmnent found. `n`nWould you like to install one?")) {
        exit 0
    }
}

$solutionName = Read-ValueFromHost -Question "Please enter a valid solution name (a-z|A-Z - min. 3 char)" -ValidationRegEx "^([a-z]|[A-Z]){3}([a-z]|[A-Z])*$" -Required

if (Test-Path (Join-Path $PSScriptRoot "_Boilerplate.sln")) {
    Write-Host "Renaming solution file to: $($solutionName).sln" -ForegroundColor Green
    Move-Item (Join-Path $PSScriptRoot "_Boilerplate.sln") (Join-Path $PSScriptRoot "$($solutionName).sln")
}

$dockerPreset = Select-DockerStarterKit -Title "^^^^^^^^^^^^^^^^^^^^ Docker environment 'starter-kit' ^^^^^^^^^^^^^^^^^^^^^^^^`n`n" -Message @"
+-+-+-+-+ +-+-+-+ +-+-+-+-+-+-+-+-+-+-+-++-+-+-+-+ +-+-+-+ +-+-+-+-+-+-+-+-+-+
                        ->> DISCLAIMER <<- 
    We highly recommend that you do NOT attempt to use this setup 
        if you are unfamiliar with running Sitecore on Docker. 

            The setup is optional to use and delivered as-is. 
    The community judges will not provide support for it during the Hackathon
+-+-+-+-+ +-+-+-+ +-+-+-+-+-+-+-+-+-+-+-++-+-+-+-+ +-+-+-+ +-+-+-+-+-+-+-+-+-+

Select the Sitecore topology that match your Hackathon category. 

Choose 'None' if you'd prefer to use your own setup.`n
"@ 

if ($dockerPreset -eq "none") {
    exit 0
}

Install-DockerStarterKit $dockerPreset
Install-SitecoreDockerTools

$hostDomain = "$($solutionName.ToLower()).localhost"
$hostDomain = Read-ValueFromHost -Question "Domain Hostname (press enter for $($hostDomain))" -DefaultValue $hostDomain -Required
Initialize-HostNames $hostDomain

$licenseFolderPath = Read-ValueFromHost -Question "Path to the folder that contains your Sitecore license.xml (press enter for .\License\)" -DefaultValue ".\License\)" -Required

Push-Location ".\\docker"
Set-DockerComposeEnvFileVariable "COMPOSE_PROJECT_NAME" -Value $solutionName.ToLower() 
Set-DockerComposeEnvFileVariable "HOST_LICENSE_FOLDER" -Value $licenseFolderPath
Set-DockerComposeEnvFileVariable "HOST_DOMAIN"  -Value $hostDomain
Set-DockerComposeEnvFileVariable "CM_HOST" -Value "cm.$($hostDomain)"
Set-DockerComposeEnvFileVariable "ID_HOST" -Value "id.$($hostDomain)"
Set-DockerComposeEnvFileVariable "RENDERING_HOST" -Value "www.$($hostDomain)"

Set-DockerComposeEnvFileVariable "REPORTING_API_KEY" -Value (Get-SitecoreRandomString 128 -DisallowSpecial)
Set-DockerComposeEnvFileVariable "TELERIK_ENCRYPTION_KEY" -Value (Get-SitecoreRandomString 128)
Set-DockerComposeEnvFileVariable "SITECORE_IDSECRET" -Value (Get-SitecoreRandomString 64 -DisallowSpecial)
$idCertPassword = Get-SitecoreRandomString 8 -DisallowSpecial
Set-DockerComposeEnvFileVariable "SITECORE_ID_CERTIFICATE" -Value (Get-SitecoreCertificateAsBase64String -DnsName "localhost" -Password (ConvertTo-SecureString -String $idCertPassword -Force -AsPlainText))
Set-DockerComposeEnvFileVariable "SITECORE_ID_CERTIFICATE_PASSWORD" -Value $idCertPassword
Set-DockerComposeEnvFileVariable "SQL_SA_PASSWORD" -Value (Get-SitecoreRandomString 19 -DisallowSpecial -EnforceComplexity)
Set-DockerComposeEnvFileVariable "SITECORE_ADMIN_PASSWORD" -Value $AdminPassword

if (Confirm -Question "Would you like to adjust common environment settings?") {
    Set-DockerComposeEnvFileVariable "SITECORE_VERSION" -Value (Read-ValueFromHost -Question "Sitecore image version (press enter for 10.0.1-ltsc2019)" -DefaultValue "10.0.1-ltsc2019" -Required)
    Set-DockerComposeEnvFileVariable "SPE_VERSION" -Value (Read-ValueFromHost -Question "Sitecore Powershell Extensions version (press enter for 6.1.1-1809)" -DefaultValue "6.1.1-1809" -Required)
    Set-DockerComposeEnvFileVariable "SITECORE_ADMIN_PASSWORD" -Value (Read-ValueFromHost -Question "Sitecore admin password (press enter for 'b')" -DefaultValue "b" -Required)
    Set-DockerComposeEnvFileVariable "REGISTRY" -Value (Read-ValueFromHost -Question "Local container registry (leave empty if none, must end with /)")
    Set-DockerComposeEnvFileVariable "ISOLATION" -Value (Read-ValueFromHost -Question "Container isolation mode (press enter for default)" -DefaultValue "default" -Required)

    if (Confirm -Question "Would you like to adjust container memory limits?") {
        Set-DockerComposeEnvFileVariable "MEM_LIMIT_SQL" -Value (Read-ValueFromHost -Question "SQL Server memory limit (default: 4GB)" -DefaultValue "4GB" -Required)
        Set-DockerComposeEnvFileVariable "MEM_LIMIT_SOLR" -Value (Read-ValueFromHost -Question "Solr memory limit (default: 2GB)" -DefaultValue "2GB" -Required)
        Set-DockerComposeEnvFileVariable "MEM_LIMIT_CM" -Value (Read-ValueFromHost -Question "CM Server memory limit (default: 4GB)" -DefaultValue "4GB" -Required)
    }
}

Write-Host "Environment configuration done..." -ForegroundColor Green
Start-Docker -Url "cm.$($hostDomain)/sitecore" -Build
Pop-Location