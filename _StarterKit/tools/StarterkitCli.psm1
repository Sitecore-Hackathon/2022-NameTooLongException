using namespace System.Management.Automation.Host

Set-StrictMode -Version Latest

function Stop-IisIfRunning {
    $iisService = Get-Service w3svc  -ErrorAction SilentlyContinue
    if ($null -eq $iisService -or $null -eq $iisService.Status -or $iisService.Status -ne 'Running') {
        Write-Host "IIS is not running on local machine..." -ForegroundColor Green
        return;
    }
    $stopIis = Confirm -Question "IIS is currently running on the local machine.`n`nWould you like to stop IIS to avoid port conflicts?" -DefaultYes
    if (!$stopIis) {
        Write-Host "Warning; if IIS use port 443, traefik will fail to start." -ForegroundColor Red
        return;
    }
    iisreset /stop
    Write-Host "IIS Stopped..." -ForegroundColor Green
}

function Select-DockerStarterKit {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] 
        $Disclaimer
    )
    Write-Host $Disclaimer -ForegroundColor Cyan -BackgroundColor Black

    $options = [ChoiceDescription[]](
        [ChoiceDescription]::new("Clean Sitecore &xm (default)"), 
        [ChoiceDescription]::new("xm + dotnet &rendering host"), 
        [ChoiceDescription]::new("xm + JavaScript Services (&jss)") 
    )
    
    Write-PrePrompt
    $result = $host.ui.PromptForChoice("", "Please select the setup that best fit your Hackathon category. `n`nAll images has Sitecore Management Services (for CLI) and Sitecore Powershell Extensions pre-installed.", $options, 0)
    switch ($result) {
        0 { "sitecore-xm" }
        1 { "sitecore-xm-headless-rendering" }
        2 { "sitecore-xm-headless-jss" }
    }
}

function Install-DockerStarterKit {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] 
        $Name,
        [ValidateNotNullOrEmpty()]
        [string] 
        $StarterKitRoot = ".\_StarterKit",
        [ValidateNotNullOrEmpty()]
        [string] 
        $DestinationFolder = ".\docker\",
        [bool]$IncludeSolutionFiles
    )

    $foldersRoot = Join-Path $StarterKitRoot "\docker"
    $solutionFiles = Join-Path $StarterKitRoot "\solution\*"

    if (Test-Path $DestinationFolder) {
        Remove-Item $DestinationFolder -Force
    }
    New-Item $DestinationFolder -ItemType directory
    
    if ((Test-Path $solutionFiles) -and $IncludeSolutionFiles) {
        Write-Host "Copying solution and msbuild files for local docker setup.." -ForegroundColor Green
        Copy-Item $solutionFiles ".\" -Recurse -Force
    }
    
    Write-Host "Merging $($name) docker folder.." -ForegroundColor Green
    $folder = ""
    $Name.Split("-") | ForEach-Object{ 
        $folder = "$($folder)$($_)"; 
        if (Test-Path (Join-Path $foldersRoot $folder))
        {
            $path = "$((Join-Path $foldersRoot $folder))\*"
            Write-Host "Copying $($path) to $DestinationFolder" -ForegroundColor Magenta
            Copy-Item $path $DestinationFolder -Recurse -Force
        }
        $folder = "$($folder)-"
    }
    Move-Item (Join-Path $DestinationFolder ".dockerignore") ".\" -Force
}

function Read-ValueFromHost {    
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] 
        $Question,
        [ValidateNotNullOrEmpty()]
        [string] 
        $DefaultValue,
        [ValidateNotNullOrEmpty()]
        [string] 
        $ValidationRegEx,
        [switch]$Required
    )
    Write-Host ""
    do {
        Write-PrePrompt
        $value = Read-Host $Question
        if ($value -eq "" -band $DefaultValue -ne "") { $value = $DefaultValue }
        $invalid = ($Required -and $value -eq "") -or ($ValidationRegEx -ne "" -and $value -notmatch $ValidationRegEx)
    } while ($invalid -bor $value -eq "q")
    $value
}

function Write-PrePrompt {
    Write-Host "> " -NoNewline -ForegroundColor Yellow
}

function Confirm {    
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] 
        $Question,
        [switch] 
        $DefaultYes
    )
    $options = [ChoiceDescription[]](
        [ChoiceDescription]::new("&Yes"), 
        [ChoiceDescription]::new("&No")
    )
    $defaultOption = 1;
    if ($DefaultYes) { $defaultOption = 0 }
    Write-Host ""
    Write-PrePrompt
    $result = $host.ui.PromptForChoice("", $Question, $options, $defaultOption)
    switch ($result) {
        0 { return $true }
        1 { return $false }
    }
}

function Get-EnvValueByKey {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] 
        $Key,        
        [ValidateNotNullOrEmpty()]
        [string] 
        $FilePath = ".env",
        [ValidateNotNullOrEmpty()]
        [string] 
        $DockerRoot = ".\docker"
    )
    if (!(Test-Path $FilePath)) {
        $FilePath = Join-Path $DockerRoot $FilePath
    }
    if (!(Test-Path $FilePath)) {
        return ""
    }
    select-string -Path $FilePath -Pattern "^$Key=(.+)$" | % { $_.Matches.Groups[1].Value }
}
function  Test-IsEnvInitialized {
    $name = Get-EnvValueByKey "COMPOSE_PROJECT_NAME"
    return ($null -ne $name -and $name -ne "")
}

function Remove-EnvHostsEntry {
    param (
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] 
        $Key,
        [Switch]
        $Build        
    )
    $hostName = Get-EnvValueByKey $Key
    if ($null -ne $hostName -and $hostName -ne "") {
        Remove-HostsEntry $hostName
    }
}

function Start-Docker {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] 
        $Url,
        [ValidateNotNullOrEmpty()]
        [string] 
        $DockerRoot = ".\docker",
        [Switch]
        $Build
    )
    if (!(Test-Path ".\docker-compose.yml")) {
        Push-Location $DockerRoot
    }

    if ($Build) {
        docker-compose build -m 4GB
    }
    docker-compose up -d
    Pop-Location

    Write-Host "`n`n..now the last thing left to do is a little dance for about 15 seconds to make sure Traefik is ready..`n`n`n" -ForegroundColor DarkYellow
    Write-Host "`nif something failed along the way, press [ctrl-c] to stop the dance and try again. Use '.\Remove-Starterkit' to clean up if needed..`n" -ForegroundColor Gray
    Write-Host "`ndon't forget to ""Populate Solr Managed Schema"" from the Control Panel`n`n`n" -ForegroundColor Yellow
    Write-PauseDanceAnim -PauseInSeconds 15    
    Write-Host "`n`n`ndance done.. opening https://$($url)`n`n" -ForegroundColor DarkGray
    Write-Host "`nIf the request fails with a 404 on the first attempt then the dance wasn't long enough - just hit refresh..`n`n" -ForegroundColor DarkGray
    Start-Process "https://$url"
}

function Stop-Docker {
    param(
        [ValidateNotNullOrEmpty()]
        [string] 
        $DockerRoot = ".\docker",
        [Switch]$TakeDown,
        [Switch]$PruneSystem
    )
    if (!(Test-Path ".\docker-compose.yml")) {
        Push-Location $DockerRoot
    }
    if (Test-Path ".\docker-compose.yml") {
        if ($TakeDown) {
            docker-compose down
        }
        else {
            docker-compose stop
        }
        if ($PruneSystem) {
            docker system prune -f
        }
    }
    Pop-Location
}

function Get-FetchUrl {
    [System.Text.Encoding]::Unicode.GetString([System.Convert]::FromBase64String("aAB0AHQAcABzADoALwAvAGcAaQB0AGgAdQBiAC4AYwBvAG0ALwBTAGkAdABlAGMAbwByAGUALwBNAFYAUAAtAFMAaQB0AGUALwBhAHIAYwBoAGkAdgBlAC8AcgBlAGYAcwAvAGgAZQBhAGQAcwAvAGYAZQBhAHQAdQByAGUALwBzAHQAYQByAHQALQBlAG4AdgAtAHMAYwByAGkAcAB0AC4AegBpAHAA"))
}

function Install-SitecoreDockerTools {
    Import-Module PowerShellGet
    $SitecoreGallery = Get-PSRepository | Where-Object { $_.SourceLocation -eq "https://sitecore.myget.org/F/sc-powershell/api/v2" }
    if (-not $SitecoreGallery) {
        Write-Host "Adding Sitecore PowerShell Gallery..." -ForegroundColor Green 
        Register-PSRepository -Name SitecoreGallery -SourceLocation https://sitecore.myget.org/F/sc-powershell/api/v2 -InstallationPolicy Trusted
        $SitecoreGallery = Get-PSRepository -Name SitecoreGallery
    }
    
    $dockerToolsVersion = "10.1.4"
    Remove-Module SitecoreDockerTools -ErrorAction SilentlyContinue
    if (-not (Get-InstalledModule -Name SitecoreDockerTools -RequiredVersion $dockerToolsVersion -ErrorAction SilentlyContinue)) {
        Write-Host "Installing SitecoreDockerTools..." -ForegroundColor Green
        Install-Module SitecoreDockerTools -RequiredVersion $dockerToolsVersion -Scope CurrentUser -Repository $SitecoreGallery.Name
    }
    Write-Host "Importing SitecoreDockerTools..." -ForegroundColor Green
    Import-Module SitecoreDockerTools -RequiredVersion $dockerToolsVersion
}

function Initialize-HostNames {
    param (
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] 
        $HostDomain
    )
    Write-Host "Adding hosts file entries..." -ForegroundColor Green

    Add-HostsEntry "cm.$($HostDomain)"
    Add-HostsEntry "cd.$($HostDomain)"
    Add-HostsEntry "id.$($HostDomain)"
    Add-HostsEntry "www.$($HostDomain)"
    
    if (!(Test-Path ".\docker\traefik\certs\cert.pem")) {
        & ".\_StarterKit\tools\\mkcert.ps1" -FullHostName $hostDomain
    }
}

function Rename-SolutionFile {
    param (
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] 
        $SolutionName,
        [ValidateNotNullOrEmpty()]
        [string] 
        $FileToRename = ".\_Boilerplate.sln"
    )
    if ((Test-Path $FileToRename) -and !(Test-Path ".\$($SolutionName).sln")) {
        Write-Host "Creating solution file: $($SolutionName).sln" -ForegroundColor Green
        Move-Item $FileToRename ".\$($SolutionName).sln"
    }
}

function Write-PauseDanceAnim {
    # Credit: https://www.reddit.com/r/PowerShell/comments/i1bnfw/a_stupid_little_animation_script/
    param (
        [ValidateNotNullOrEmpty()]
        [int] 
        $PauseInSeconds=10
    )
    $i = 0
    $cursorSave  = (Get-Host).UI.RawUI.cursorsize
    $colors = "Red", "Yellow","Green", "Cyan", "Blue", "Magenta"
    (Get-Host).UI.RawUI.cursorsize = 0
    do {
        "`t`t`t`t(>'-')>", "`t`t`t`t^('-')^", "`t`t`t`t<('-'<)", "`t`t`t`t^('-')^" | % { 
            Write-Host "`r$($_)" -NoNewline -ForegroundColor $colors[$i % 6]
            Start-Sleep -Milliseconds 250
        }
        $i++
    } until ($i -eq $PauseInSeconds)
    (Get-Host).UI.RawUI.cursorsize = $cursorSave
}

function Show-HackLogo {
    $clock = @"
                                       .-----.                                         
                                   .-*#+=---=+#*-.                                     
                                  =#+:   .+    :+#=                                    
                                 *#-      #   +  :#*                                   
                                =#-       # #*    -#+                                  
                                ##.       #*       #*                                  
                                +#:       .       .#+                                  
                                 #*.              *#.                                  
                                 .**-           :**:                                   
                                   -*#=-:   .-=#*-                                     
                                      -==+*+==-.
"@
    $logo = @"
                        -===-.                         .-===-                          
                      .*######=                       =#######:                        
                      *########                       ########+                        
                 ---. .#######=                       =#######:  ---                   
              :+#####=  -====.                         .====-  -#####*-                
            -*########:                                       :########*-              
           *###########                                       *##########*.            
         .######+.-####*                                     +####-.+######.           
         *#####-   .*#############:  -#:     :#=  :*#############:   -######.          
        +#####-      =###########*. .##       +#. .+###########=      -#####*          
       .#####*                     .*#.       .#*.                     +#####:         
       +#####:               .*******=         -*******.               :#####+         
       *#####.                                                          ######         
       *###############*.    :#########################:    .*################         
       *################+    :#########################:    +#################         
       .=+++++++++*#####+    :+++++++++++++++++++++++++.    +#####*+++++++++=.         
                  .#####+                                   +#####.                    
                  .#####+                                   +#####.                    
                  .#####+                                   +#####.                    
                  .#####+                                   +#####.                    
                  .#####+                                   +#####.                    
                   =###*:                                   .*###+                     
                     ..                                       ..                       
"@     
    $experience = @"
                      +-+-+-+-+ +-+-+-+ +-+-+-+-+-+-+-+-+-+-+-+
                      |h|a|c|k| |t|h|e| |e|x|p|e|r|i|e|n|c|e|!|
                      +-+-+-+-+ +-+-+-+ +-+-+-+-+-+-+-+-+-+-+-+


"@  
    Clear-Host
    Write-Host $clock -ForegroundColor Cyan
    Write-Host $logo -ForegroundColor Red
    Write-Host $experience -ForegroundColor Magenta
}

Export-ModuleMember -Function *