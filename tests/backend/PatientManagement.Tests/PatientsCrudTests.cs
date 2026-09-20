using System.Net;
using System.Net.Http.Json;
using PatientManagement.Application.DTOs;
using Xunit;

namespace PatientManagement.Tests;

public class PatientsCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PatientsCrudTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_ThenGet_ReturnsSamePatient()
    {
        var client = await TestAuthHelper.CreateAuthenticatedClientAsync(_factory, "admin", "Admin123!");

        var request = new CreatePatientRequest
        {
            FirstName = "Crud",
            LastName = "Create",
            DateOfBirth = new DateOnly(1995, 5, 5),
            Gender = "Female",
            Email = "crud.create@example.com"
        };

        var createResponse = await client.PostAsJsonAsync("/api/patients", request);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<PatientDto>();
        Assert.NotNull(created);
        Assert.False(string.IsNullOrWhiteSpace(created!.MedicalRecordNumber));

        var getResponse = await client.GetAsync($"/api/patients/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var fetched = await getResponse.Content.ReadFromJsonAsync<PatientDto>();
        Assert.Equal(created.Id, fetched!.Id);
        Assert.Equal("Crud", fetched.FirstName);
    }

    [Fact]
    public async Task Get_NonexistentPatient_ReturnsNotFound()
    {
        var client = await TestAuthHelper.CreateAuthenticatedClientAsync(_factory, "admin", "Admin123!");

        var response = await client.GetAsync("/api/patients/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ExistingPatient_ChangesFields()
    {
        var client = await TestAuthHelper.CreateAuthenticatedClientAsync(_factory, "admin", "Admin123!");

        var created = await CreateSamplePatientAsync(client);

        var updateRequest = new UpdatePatientRequest
        {
            FirstName = "Updated",
            LastName = created.LastName,
            DateOfBirth = created.DateOfBirth,
            Gender = created.Gender,
            City = "NewCity"
        };

        var response = await client.PutAsJsonAsync($"/api/patients/{created.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<PatientDto>();
        Assert.Equal("Updated", updated!.FirstName);
        Assert.Equal("NewCity", updated.City);
    }

    [Fact]
    public async Task Update_NonexistentPatient_ReturnsNotFound()
    {
        var client = await TestAuthHelper.CreateAuthenticatedClientAsync(_factory, "admin", "Admin123!");

        var updateRequest = new UpdatePatientRequest
        {
            FirstName = "X",
            LastName = "Y",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Gender = "Other"
        };

        var response = await client.PutAsJsonAsync("/api/patients/999999", updateRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingPatient_ReturnsNoContent_ThenNotFound()
    {
        var client = await TestAuthHelper.CreateAuthenticatedClientAsync(_factory, "admin", "Admin123!");

        var created = await CreateSamplePatientAsync(client);

        var deleteResponse = await client.DeleteAsync($"/api/patients/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/patients/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_NonexistentPatient_ReturnsNotFound()
    {
        var client = await TestAuthHelper.CreateAuthenticatedClientAsync(_factory, "admin", "Admin123!");

        var response = await client.DeleteAsync("/api/patients/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("", "LastName")]
    [InlineData("FirstName", "")]
    public async Task Create_MissingRequiredField_ReturnsBadRequest(string firstName, string lastName)
    {
        var client = await TestAuthHelper.CreateAuthenticatedClientAsync(_factory, "admin", "Admin123!");

        var request = new CreatePatientRequest
        {
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = new DateOnly(1990, 1, 1),
            Gender = "Other"
        };

        var response = await client.PostAsJsonAsync("/api/patients", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_InvalidEmail_ReturnsBadRequest()
    {
        var client = await TestAuthHelper.CreateAuthenticatedClientAsync(_factory, "admin", "Admin123!");

        var request = new CreatePatientRequest
        {
            FirstName = "Bad",
            LastName = "Email",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Gender = "Other",
            Email = "not-an-email"
        };

        var response = await client.PostAsJsonAsync("/api/patients", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static async Task<PatientDto> CreateSamplePatientAsync(HttpClient client)
    {
        var request = new CreatePatientRequest
        {
            FirstName = "Sample",
            LastName = "Patient",
            DateOfBirth = new DateOnly(1988, 3, 3),
            Gender = "Male"
        };

        var response = await client.PostAsJsonAsync("/api/patients", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<PatientDto>())!;
    }
}
