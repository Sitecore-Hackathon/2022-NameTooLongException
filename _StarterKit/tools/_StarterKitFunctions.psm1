using namespace System.Management.Automation.Host

function Select-DockerStarterKit {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] 
        $Title,
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] 
        $Message
    )
    $options = [ChoiceDescription[]](
        [ChoiceDescription]::new("&XP0 (default)"), 
        [ChoiceDescription]::new("&Dotnet rendering host"), 
        [ChoiceDescription]::new("&jss"), 
        [ChoiceDescription]::new("&sxa"), 
        [ChoiceDescription]::new("&None (Skip)")
    )
    $result = $host.ui.PromptForChoice($Title, $Message , $options, 0)

    switch ($result) {
        0 { "sitecore-xp0" }
        1 { "sitecore-xp0-rendering" }
        2 { "sitecore-xp0-jss" }
        3 { "sitecore-xp0-sxa" }
        4 { "none" }
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
        $StarterKitRoot = ".\\_StarterKit",
        [ValidateNotNullOrEmpty()]
        [string] 
        $DestinationFolder = ".\\docker"
    )

    $dockerPresetsPath = Join-Path $StarterKitRoot "\\docker\\$($Name)"
    $solutionFiles = Join-Path $StarterKitRoot "\\solution\\*"

    if (!(Test-Path $dockerPresetsPath)) {
        throw "Docker preset not found on path $($dockerPresetsPath)"
    }
    
    if ((Test-Path $solutionFiles) -and !(Test-Path ".\\Directory.build.props")) {
        Write-Host "Copying msbuild props and targets files for docker deploys.." -ForegroundColor Green
        Copy-Item $solutionFiles ".\\" -Recurse -Force
    }
    
    Write-Host "Creating docker folder.." -ForegroundColor Green
    Copy-Item $dockerPresetsPath $DestinationFolder -Recurse -Force

    Write-Host "Creating solution Dockerfile.." -ForegroundColor Green
    Move-Item (Join-Path $DestinationFolder "Dockerfile") ".\\" -Force
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
    do {
        $value = Read-Host $Question
        if ($value -eq "" -band $DefaultValue -ne "") { $value = $DefaultValue }
        
        $invalid = ($Required -and $value -eq "") -or ($ValidationRegEx -ne "" -and $value -notmatch $ValidationRegEx)
    } while ($invalid -bor $value -eq "q")
    $value
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
        $DockerRoot = ".\\docker"
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
        $DockerRoot = ".\\docker",
        [Switch]
        $Build
    )
    if (!(Test-Path ".\\docker-compose.yml")) {
        Push-Location $DockerRoot
    }

    if ($Build) {
        docker-compose build
    }
  
    docker-compose up -d    
    Start-Process "https://$url"
    Pop-Location
}

function Stop-Docker {
    param(
        [ValidateNotNullOrEmpty()]
        [string] 
        $DockerRoot = ".\\docker",
        [Switch]$TakeDown,
        [Switch]$PruneSystem
    )
    if (!(Test-Path ".\\docker-compose.yml")) {
        Push-Location $DockerRoot
    }
    if (Test-Path ".\\docker-compose.yml") {
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

function Install-SitecoreDockerTools {
    Import-Module PowerShellGet
    $SitecoreGallery = Get-PSRepository | Where-Object { $_.SourceLocation -eq "https://sitecore.myget.org/F/sc-powershell/api/v2" }
    if (-not $SitecoreGallery) {
        Write-Host "Adding Sitecore PowerShell Gallery..." -ForegroundColor Green 
        Register-PSRepository -Name SitecoreGallery -SourceLocation https://sitecore.myget.org/F/sc-powershell/api/v2 -InstallationPolicy Trusted
        $SitecoreGallery = Get-PSRepository -Name SitecoreGallery
    }
    
    $dockerToolsVersion = "10.0.5"
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
    
    if (!(Test-Path ".\\docker\\traefik\\certs\\cert.pem")) {
        & ".\\_StarterKit\\tools\\\\mkcert.ps1" -FullHostName $hostDomain
    }
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
    Write-Host $clock -ForegroundColor Cyan
    Write-Host $logo -ForegroundColor Red
    Write-Host $experience -ForegroundColor Magenta
}

Export-ModuleMember -Function *