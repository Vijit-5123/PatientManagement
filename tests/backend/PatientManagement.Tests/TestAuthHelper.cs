using System.Net.Http.Headers;
using System.Net.Http.Json;
using PatientManagement.Application.DTOs;

namespace PatientManagement.Tests;

public static class TestAuthHelper
{
    public static async Task<string> LoginAsync(HttpClient client, string username, string password)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Username = username,
            Password = password
        });

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return body!.AccessToken;
    }

    public static async Task<HttpClient> CreateAuthenticatedClientAsync(CustomWebApplicationFactory factory, string username, string password)
    {
        var client = factory.CreateClient();
        var token = await LoginAsync(client, username, password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
