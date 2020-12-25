param (
    [string] $ResourceGroupName = "prod-nblog-rg",

    [string] $Name = "prod-nblog-aks",

    [int] $NodeCount = 2,

    [string] $AcrName = "prodnblogreg"
)

& az aks create --resource-group $ResourceGroupName --name $Name --node-count $NodeCount --generate-ssh-keys --attach-acr $AcrName;

