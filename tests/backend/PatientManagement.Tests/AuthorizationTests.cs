using System.Net;
using System.Net.Http.Json;
using PatientManagement.Application.DTOs;
using Xunit;

namespace PatientManagement.Tests;

public class AuthorizationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthorizationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private static CreatePatientRequest NewPatientRequest(string firstName = "Auth", string lastName = "TestCase") => new()
    {
        FirstName = firstName,
        LastName = lastName,
        DateOfBirth = new DateOnly(1990, 1, 1),
        Gender = "Other"
    };

    [Fact]
    public async Task Admin_CanListReadCreateUpdateAndDelete()
    {
        var client = await TestAuthHelper.CreateAuthenticatedClientAsync(_factory, "admin", "Admin123!");

        var listResponse = await client.GetAsync("/api/patients");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var createResponse = await client.PostAsJsonAsync("/api/patients", NewPatientRequest());
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<PatientDto>();

        var getResponse = await client.GetAsync($"/api/patients/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var updateRequest = new UpdatePatientRequest
        {
            FirstName = "AuthUpdated",
            LastName = created.LastName,
            DateOfBirth = created.DateOfBirth,
            Gender = created.Gender
        };
        var updateResponse = await client.PutAsJsonAsync($"/api/patients/{created.Id}", updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/patients/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task Doctor_CanListReadCreateAndUpdate_ButNotDelete()
    {
        var client = await TestAuthHelper.CreateAuthenticatedClientAsync(_factory, "doctor", "Doctor123!");

        var listResponse = await client.GetAsync("/api/patients");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var createResponse = await client.PostAsJsonAsync("/api/patients", NewPatientRequest("Doctor", "Created"));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<PatientDto>();

        var updateRequest = new UpdatePatientRequest
        {
            FirstName = "DoctorUpdated",
            LastName = created!.LastName,
            DateOfBirth = created.DateOfBirth,
            Gender = created.Gender
        };
        var updateResponse = await client.PutAsJsonAsync($"/api/patients/{created.Id}", updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/patients/{created.Id}");
        Assert.Equal(HttpStatusCode.Forbidden, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task UnauthenticatedRequest_IsRejected()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/patients", NewPatientRequest());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
