#Requires -RunAsAdministrator
param (
    [Switch]
    $IdeaThree
)

Import-Module -Name (Join-Path $PSScriptRoot "_StarterKit\tools\StarterKitCLi") -Force

Show-HackLogo

if (Test-Path .\.idea3) {
    Write-Host "Duuhh.. rtfm; You cannot use this script anymore since you've selected idea #3...`n`n" -ForegroundColor Cyan
    Write-Host "Now get back to work!...`n`n" -ForegroundColor DarkGray
    exit 0
}

if ($IdeaThree.IsPresent -and (Confirm "This will download and extract the files needed for idea #3 into this folder.`nPlease confirm?")) {
    Write-Host "Downloading..." -ForegroundColor Green

    $url = Get-FetchUrl
    Invoke-WebRequest -Uri $url -OutFile .\archive.zip
    if (!(Test-Path .\archive.zip)) {
        Write-Host "Could not download required files.. Please try again or download it manually from Github..." -ForegroundColor Red
        exit 0
    }
    mkdir .\_tmp
    Expand-Archive .\archive.zip -DestinationPath .\_tmp

    Move-Item .\README.md .\README-HACKATHON.md -Force
    Remove-Item .\License -Recurse -Force

    Get-ChildItem .\_tmp\ | Where-Object { $_.PSIsContainer } | ForEach-Object{ Copy-Item -Path "$($_.FullName)\*" -Destination .\ -Force -Recurse  }
    Write-Host "Data fetched and extacted.. Cleaning up.." -ForegroundColor Magenta
    Remove-Item .\Stop-Hackathon.ps1 -Force
    Remove-Item .\Remove-Starterkit.ps1 -Force
    Remove-Item .\_tmp -Recurse -Force
    Remove-Item .\archive.zip -Force
    Write-Output "[image of cute kitten]" > .\.idea3
    Write-Host "Done.. Now follow the instructions found in .\README.md " -ForegroundColor Green
    Write-Host "`nor simply run `n`n.\Start-Environment -LicensePath {path to valid license.xml}`n`nand follow the on-screen instructions..`n" -ForegroundColor Yellow
    Write-Host "`nRemember to commit and push the setup so other team members can get started.`n " -ForegroundColor Magenta
    exit 0
}


if (Test-IsEnvInitialized -FilePath ".\docker\.env" ) {
    Write-Host "Docker environment is present, starting docker.." -ForegroundColor Green

    if (!(Test-Path ".\docker\traefik\certs\cert.pem")) {
        Write-Host "TLS certificate for Traefik not found, generating and adding hosts file entries" -ForegroundColor Green
        Install-SitecoreDockerTools
        $hostDomain = Get-EnvValueByKey "HOST_DOMAIN"
        if ($hostDomain -eq "") {
            throw "Required variable 'HOST_DOMAIN' not set in .env file."
        }
        Initialize-HostNames $hostDomain
        Start-Docker -Url "$($hostDomain)/sitecore" -Build
        exit 0
    }
    Start-Docker -Url "$(Get-EnvValueByKey "CM_HOST")/sitecore"
    exit 0
}

if (!$IdeaThree.IsPresent) {
    Write-Host "`n[IMPORTANT] " -ForegroundColor Blue -NoNewline
    Write-Host "If you plan to work on idea #3 - please exit this script by pressing [ctrl-c] and run it again with the switch -IdeaThree `n`n" -ForegroundColor Yellow
    Write-Host "`like this:" -ForegroundColor DarkGray
    Write-PrePrompt
    Write-Host ".\Start-Hackathon.ps1 -IdeaThree`n`n" -ForegroundColor White
}

if ( !(Test-Path ".\_StarterKit\docker")) {
    Write-Host "Starter-kit docker setups not found - you're on your own.." -ForegroundColor Green
    exit 0
}

if ((Test-Path ".\*.sln")) {
    if (!(Confirm "A solution file already exist but no initialized docker environmnent was found.`n`nWould you like to install a Docker environment preset??")) {
        exit 0
    }
    if (Test-Path (Join-Path $PSScriptRoot "docker")) {
        Remove-Item (Join-Path $PSScriptRoot "docker") -Force -Recurse
    }
}

Stop-IisIfRunning

$solutionName = Read-ValueFromHost -Question "Please enter a valid solution name`n(Capital first letter, letters and numbers only, min. 3 char)" -ValidationRegEx "^[A-Z]([a-z]|[A-Z]|[0-9]){2}([a-z]|[A-Z]|[0-9])*$" -Required

if (!(Test-Path ".\*.sln") -and !(Confirm -Question "Would you like to install a Docker environment preset?" -DefaultYes)) {
    Write-Host "Okay, No Docker preset will be installed.." -ForegroundColor Yellow
    Copy-Item ".\_StarterKit\_Boilerplate.sln" ".\" -Force
    Rename-SolutionFile $solutionName
    exit 0
}

$esc = [char]27
$dockerPreset = Select-DockerStarterKit -Disclaimer @"
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                   ->>  $esc[4mDISCLAIMER$esc[24m <<- 
    We highly recommend that you do NOT attempt to use this setup 
        if you are completely new to running Sitecore on Docker. 

        $esc[1$esc[4mThe Hackathon is not the right time to learn Docker.$esc[24m$esc[22m

           The setup is optional to use and delivered as-is. 
    The community judges will not provide support for it during the Hackathon
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
"@ 

Write-Host "$($dockerPreset) selected.." -ForegroundColor Magenta

Write-Host "`nIncluding a basic solution msbuild setup" -ForegroundColor Green
Install-DockerStarterKit -Name $dockerPreset -IncludeSolutionFiles $true

Rename-SolutionFile $solutionName
Install-SitecoreDockerTools

$hostDomain = "$($solutionName.ToLower()).localhost"
$hostDomain = Read-ValueFromHost -Question "Domain Hostname (press enter for $($hostDomain))" -DefaultValue $hostDomain -Required
Initialize-HostNames $hostDomain

do {
    $licenseFolderPath = Read-ValueFromHost -Question "Path to a folder that contains your Sitecore license.xml file `n- must contain a file named license.xml file (press enter for .\License\)" -DefaultValue ".\License\" -Required
} while (!(Test-Path (Join-Path $licenseFolderPath "license.xml")))

Copy-Item (Join-Path $licenseFolderPath "license.xml") ".\docker\license\"
Write-Host "Copied license.xml to .\docker\license\" -ForegroundColor Magenta

Push-Location ".\docker"
Set-EnvFileVariable "COMPOSE_PROJECT_NAME" -Value $solutionName.ToLower() 
Set-EnvFileVariable "REGISTRY" -Value (Read-ValueFromHost -Question "Local container registry (leave empty if none, must end with /)")
Set-EnvFileVariable "HOST_LICENSE_FOLDER" -Value ".\license"
Set-EnvFileVariable "HOST_DOMAIN"  -Value $hostDomain
Set-EnvFileVariable "CM_HOST" -Value "cm.$($hostDomain)"
Set-EnvFileVariable "ID_HOST" -Value "id.$($hostDomain)"
Set-EnvFileVariable "RENDERING_HOST" -Value "www.$($hostDomain)"

Set-EnvFileVariable "TELERIK_ENCRYPTION_KEY" -Value (Get-SitecoreRandomString 128)
Set-EnvFileVariable "MEDIA_REQUEST_PROTECTION_SHARED_SECRET" -Value (Get-SitecoreRandomString 64 -DisallowSpecial)
Set-EnvFileVariable "SITECORE_IDSECRET" -Value (Get-SitecoreRandomString 64 -DisallowSpecial)
$idCertPassword = Get-SitecoreRandomString 8 -DisallowSpecial
Set-EnvFileVariable "SITECORE_ID_CERTIFICATE" -Value (Get-SitecoreCertificateAsBase64String -DnsName "localhost" -Password (ConvertTo-SecureString -String $idCertPassword -Force -AsPlainText))
Set-EnvFileVariable "SITECORE_ID_CERTIFICATE_PASSWORD" -Value $idCertPassword
Set-EnvFileVariable "SQL_SA_PASSWORD" -Value (Get-SitecoreRandomString 19 -DisallowSpecial -EnforceComplexity)
Set-EnvFileVariable "SITECORE_VERSION" -Value (Read-ValueFromHost -Question "Sitecore image version`n(10.2-ltsc2019, 10.2-2009, 10.2-20H2 - press enter for 10.2-ltsc2019)" -DefaultValue "10.2-ltsc2019" -Required)
Set-EnvFileVariable "SITECORE_ADMIN_PASSWORD" -Value (Read-ValueFromHost -Question "Sitecore admin password (press enter for 'b')" -DefaultValue "b" -Required)

if (Confirm -Question "Would you like to adjust container memory limits?") {
    Set-EnvFileVariable "MEM_LIMIT_SQL" -Value (Read-ValueFromHost -Question "SQL Server memory limit (default: 4GB)" -DefaultValue "4GB" -Required)
    Set-EnvFileVariable "MEM_LIMIT_SOLR" -Value (Read-ValueFromHost -Question "Solr memory limit (default: 2GB)" -DefaultValue "2GB" -Required)
    Set-EnvFileVariable "MEM_LIMIT_CM" -Value (Read-ValueFromHost -Question "CM Server memory limit (default: 4GB)" -DefaultValue "4GB" -Required)
}

dotnet tool restore

Start-Docker -Url "cm.$($hostDomain)/sitecore" -Build


Pop-Location

Write-Host "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+" -ForegroundColor Magenta
Write-Host "                       ->> $esc[4mIMPORTANT NEXT STEPS$esc[24m <<-" -ForegroundColor Cyan
Write-Host @"
    If you are more than one team member - then test, commit and push the environment.
 
      Other team members then have to pull the latest and run `.\Start-Hackathon.ps1`.

          This will generate locally trusted certificates and add hosts entries

"@ -ForegroundColor Yellow
Write-Host "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+" -ForegroundColor Magenta