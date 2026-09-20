using PatientManagement.Domain.Enums;

namespace PatientManagement.Api.Authorization;

public static class AuthorizationPolicyExtensions
{
    public static IServiceCollection AddPatientManagementAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(PolicyNames.PatientRead, policy =>
                policy.RequireRole(UserRole.Admin.ToString(), UserRole.Doctor.ToString()))
            .AddPolicy(PolicyNames.PatientWrite, policy =>
                policy.RequireRole(UserRole.Admin.ToString(), UserRole.Doctor.ToString()))
            .AddPolicy(PolicyNames.PatientDelete, policy =>
                policy.RequireRole(UserRole.Admin.ToString()))
            .AddPolicy(PolicyNames.AdminOnly, policy =>
                policy.RequireRole(UserRole.Admin.ToString()));

        return services;
    }
}
