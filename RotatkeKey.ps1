# To rotate a key in Azure Key Vault, the service principal must have the following Key Vault access policy permissions:
# 
# get – to read key properties
# create – to generate a new version of the key
# update – to modify key attributes

# Variables
$tenantId = "<Tenant ID>"
$clientId = "<Client ID>"
$clientSecret = "<Client Secret>"
$vaultName = "<Key Vault Name>"
$keyName = "<Key Name>"

# Get access token for Azure Key Vault
$body = @{
    grant_type    = "client_credentials"
    client_id     = $clientId
    client_secret = $clientSecret
    scope         = "https://vault.azure.net/.default"
}
$tokenResponse = Invoke-RestMethod -Method Post -Uri "https://login.microsoftonline.com/$tenantId/oauth2/v2.0/token" -Body $body
$accessToken = $tokenResponse.access_token

# Rotate the key by creating a new version
$uri = "https://$vaultName.vault.azure.net/keys/$keyName/create?api-version=7.4"
$headers = @{
    Authorization = "Bearer $accessToken"
    "Content-Type"  = "application/json"
}
$body = @{
    kty = "RSA"
} | ConvertTo-Json -Depth 3

$response = Invoke-RestMethod -Method Post -Uri $uri -Headers $headers -Body $body
Write-Host "Key rotated successfully. New version: $($response.key.kid)"
