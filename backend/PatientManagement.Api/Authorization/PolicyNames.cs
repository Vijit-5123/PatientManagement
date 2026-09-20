namespace PatientManagement.Api.Authorization;

public static class PolicyNames
{
    public const string PatientRead = "PatientRead";
    public const string PatientWrite = "PatientWrite";
    public const string PatientDelete = "PatientDelete";
    public const string AdminOnly = "AdminOnly";
}
