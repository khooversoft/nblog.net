
Write-Host "Removing container"

& docker stop nblog-store-run

& docker container rm nblog-store-run
