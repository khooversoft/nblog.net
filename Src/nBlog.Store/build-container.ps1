
Write-Host "Building Container"

Remove-Item .\bin -Force -Recurse;

& dotnet publish -c Release --framework net5.0 --self-contained --runtime linux-x64

& docker container rm nblog-store-run

& docker image rm nblog-store-image

& docker build -f DockerFile -t nblog-store-image .

