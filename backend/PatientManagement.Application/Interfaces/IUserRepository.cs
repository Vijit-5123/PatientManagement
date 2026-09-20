using PatientManagement.Domain.Entities;

namespace PatientManagement.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}
