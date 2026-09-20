using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Interfaces;
using PatientManagement.Domain.Entities;
using PatientManagement.Infrastructure.Persistence;

namespace PatientManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        _context.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
}
