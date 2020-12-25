
param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("Store", "Server")]
    [string] $Service
)

Write-Host "Building Container for $Service";

$currentFolder = $PSScriptRoot;

$path = @{
    "Server" = Join-Path -Path $currentFolder -ChildPath "..\\nBlog.Server"
    "Store" = Join-Path -Path $currentFolder -ChildPath "..\\nBlog.Store"
}

Set-Location $path[$Service];

$Service = $Service.ToLower();

Remove-Item .\bin -Force -Recurse;

& dotnet publish -c Release --framework net5.0 --self-contained --runtime linux-x64

& docker container rm "nblog-$Service-run"

& docker image rm "nblog-$Service-image"

& docker build -f DockerFile -t "nblog-$Service-image" .

Set-Location $currentFolder;

Write-Host "Completed $Service";
