
[CmdletBinding(DefaultParameterSetName = "no-arguments")]
Param (	
    [Parameter(HelpMessage = "Build Containers")]
    [Alias("b")]
    [switch]$Build
)

# Restore dotnet tool for sitecore login and serialization
dotnet tool restore
$HostDomain = "sc-mvp.localhost"

$dockerCompose = "docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.mvp.yml"
if ($Build) {
    # Restore dotnet tool for sitecore login and serialization
    dotnet tool restore
    # Build all containers in the Sitecore instance, forcing a pull of latest base containers
    Write-Host "BUILD the docker containers..." -ForegroundColor Green
    Write-Host "$dockerCompose build"
    Invoke-Expression "$dockerCompose build"
}

if ($LASTEXITCODE -ne 0)
{
    Write-Error "Container build failed, see errors above."
}

# Run the docker containers
Write-Host "Run the docker containers..." -ForegroundColor Green
Write-Host "$dockerCompose up -d"
Invoke-Expression "$dockerCompose up -d"

# Wait for Traefik to expose CM route
Write-Host "Waiting for CM to become available..." -ForegroundColor Green
$startTime = Get-Date
do {
    Start-Sleep -Milliseconds 1000
    try {
        $status = Invoke-RestMethod "http://localhost:8079/api/http/routers/cm-secure@docker"
    } catch {
        if ($_.Exception.Response.StatusCode.value__ -ne "404") {
            throw
        }
    }
	$WaitIndicator = $WaitIndicator + "."
	Write-Host "`r$WaitIndicator" -NoNewline
} while ($status.status -ne "enabled" -and $startTime.AddSeconds(600) -gt (Get-Date))

Write-Host "`r$WaitIndicator"
if (-not $status.status -eq "enabled") {
    $status
    Write-Error "Timeout waiting for Sitecore CM to become available via Traefik proxy. Check CM container logs."
}

dotnet sitecore login --cm "https://cm.$($HostDomain)" --auth "https://id.$($HostDomain)/" --allow-write true
if ($LASTEXITCODE -ne 0) {
    Write-Error "Unable to log into Sitecore, did the Sitecore environment start correctly? See logs above."
}

Write-Host "Checking SOLR schema" -ForegroundColor Green
$solrfields = Invoke-RestMethod -Uri "http://localhost:8984/solr/sc-mvp_master_index/schema/fields?wt=json"
if ($solrfields.fields.name -contains '_template') {
	Write-Host "Managed schema seems to be populated already!"
} else {

	Write-Host "Populating Solr managed schema..." -ForegroundColor Green
	$token = (Get-Content .\.sitecore\user.json | ConvertFrom-Json).endpoints.default.accessToken
	Invoke-RestMethod "https://cm.$($HostDomain)/sitecore/admin/PopulateManagedSchema.aspx?indexes=all" -Headers @{Authorization = "Bearer $token"} -UseBasicParsing | Out-Null
	
	$WaitIndicator = ""
	$startTime = Get-Date
	do {
		Start-Sleep -Milliseconds 1000
		$WaitIndicator = $WaitIndicator + "."
		Write-Host "`r$WaitIndicator" -NoNewline
	} while ($startTime.AddSeconds(60) -gt (Get-Date))
	Write-Host "`r$WaitIndicator"
}

Write-Host "Pushing latest items to Sitecore..." -ForegroundColor Green

dotnet sitecore ser push
if ($LASTEXITCODE -ne 0) {
    Write-Error "Serialization push failed, see errors above."
}

dotnet sitecore publish
if ($LASTEXITCODE -ne 0) {
    Write-Error "Item publish failed, see errors above."
}

Write-Host "Opening site..." -ForegroundColor Green

Start-Process "https://cm.$($HostDomain)/sitecore/"

Start-Process "https://mvp.$HostDomain/"

Write-Host "Use the following command to bring your docker environment down again:" -ForegroundColor Green
Write-Host ".\Stop-Environment.ps1"