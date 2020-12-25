<#
.DESCRIPTION
Update images and/or deploy containers to local or azure hosts

#>

param(
    [switch] $BuildStore,
    [switch] $BuildServer,
    [switch] $BuildAll,
    [switch] $Compose,
    [switch] $Register
)

$activities = @(
    @(($BuildAll -or $BuildStore), { .\Build-container.ps1 -Service "Store" }),
    @(($BuildAll -or $BuildServer), { .\Build-container.ps1 -Service "Server" }),
    @($Register, { .\push-registry.ps1 }),
    @($Compose, { docker-compose.exe -f nblog-compose.yaml up })
)

foreach($activity in $activities)
{
    if( $activity[0] -eq $true )
    {
        & $activity[1];
    }
}

Write-Host "Completed";

