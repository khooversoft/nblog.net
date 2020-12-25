param (
    [string] $Repository = "prodnblogreg.azurecr.io",

    [string] $Version = "v1"
)

# & docker login -u prodnblogreg -p $RegistryPwd $Repository;

$containers = @(
    "nblog-server-image",
    "nblog-store-image"
)

foreach($item in $containers.GetEnumerator())
{
    Write-Host "Removing image from register $Repository";
    & docker image rm $Repository/${item}:$Version

    Write-Host "Tag image to register $Repository";
    & docker tag $item $Repository/${item}:$Version

    Write-Host "Push image to register $Repository";
    & docker push $Repository/${item}:$Version
}
