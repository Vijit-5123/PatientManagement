using PatientManagement.Domain.Entities;

namespace PatientManagement.Application.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}
