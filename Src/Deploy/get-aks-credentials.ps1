param (
    [string] $ResourceGroupName = "prod-nblog-rg",

    [string] $Name = "prod-nblog-aks"
)

& az aks get-credentials --resource-group $ResourceGroupName --name $Name;

