using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

class Program
{
    static async Task Main()
    {
        // Replace with your actual values
        string tenantId = "<Tenant ID>";
        string clientId = "<Client ID>";
        string clientSecret = "<Client Secret>";
        string vaultName = "<Key Vault Name>";
        string keyName = "<Key Name>";

        // Get access token
        var token = await GetAccessTokenAsync(tenantId, clientId, clientSecret);

        // Rotate the key by creating a new version
        await RotateKeyAsync(vaultName, keyName, token);
    }

    static async Task<string> GetAccessTokenAsync(string tenantId, string clientId, string clientSecret)
    {
        using var client = new HttpClient();
        var body = new StringContent($"grant_type=client_credentials&client_id={clientId}&client_secret={clientSecret}&scope=https://vault.azure.net/.default", Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await client.PostAsync($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token", body);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        dynamic result = JsonConvert.DeserializeObject(json);
        return result.access_token;
    }

    static async Task RotateKeyAsync(string vaultName, string keyName, string accessToken)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var uri = $"https://{vaultName}.vault.azure.net/keys/{keyName}/create?api-version=7.4";
        var payload = JsonConvert.SerializeObject(new { kty = "RSA" });

        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(uri, content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        dynamic result = JsonConvert.DeserializeObject(json);
        Console.WriteLine($"Key rotated successfully. New version: {result.key.kid}");
    }
}

