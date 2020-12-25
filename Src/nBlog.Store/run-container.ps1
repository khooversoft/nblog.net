
# param(
#     [Parameter(Mandatory=$true)]
#     [string] $AccountKey,

#     [Parameter(Mandatory=$true)]
#     [string] $ApiKey

# )

& docker container rm nblog-store-run

# & docker run -it -p 5010:80 --name nblog-store-run nblog-store-image "Store:AccountKey=$AccountKey" "ApiKey=$ApiKey"
& docker run -it -p 5010:80 --name nblog-store-run nblog-store-image

