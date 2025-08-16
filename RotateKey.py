import requests
import json

# Replace with your actual values
tenant_id = "<Tenant ID>"
client_id = "<Client ID>"
client_secret = "<Client Secret>"
vault_name = "<Key Vault Name>"
key_name = "<Key Name>"

# Step 1: Get access token
token_url = f"https://login.microsoftonline.com/{tenant_id}/oauth2/v2.0/token"
token_data = {
    "grant_type": "client_credentials",
    "client_id": client_id,
    "client_secret": client_secret,
    "scope": "https://vault.azure.net/.default"
}
token_response = requests.post(token_url, data=token_data)
token_response.raise_for_status()
access_token = token_response.json()["access_token"]

# Step 2: Rotate the key by creating a new version
rotate_url = f"https://{vault_name}.vault.azure.net/keys/{key_name}/create?api-version=7.4"
headers = {
    "Authorization": f"Bearer {access_token}",
    "Content-Type": "application/json"
}
payload = {
    "kty": "RSA"
}
response = requests.post(rotate_url, headers=headers, json=payload)
response.raise_for_status()
result = response.json()

print(f"Key rotated successfully. New version: {result['key']['kid']}")
