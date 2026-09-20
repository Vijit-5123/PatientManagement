using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Interfaces;
using PatientManagement.Domain.Entities;
using PatientManagement.Infrastructure.Persistence;

namespace PatientManagement.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly ApplicationDbContext _context;

    public PatientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Patient?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Patients.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<Patient> Items, int TotalCount)> SearchAsync(string? query, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var patients = _context.Patients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalized = query.Trim().ToLower();
            patients = patients.Where(p =>
                p.FirstName.ToLower().Contains(normalized) ||
                p.LastName.ToLower().Contains(normalized) ||
                p.MedicalRecordNumber.ToLower().Contains(normalized) ||
                (p.Email != null && p.Email.ToLower().Contains(normalized)));
        }

        var totalCount = await patients.CountAsync(cancellationToken);

        var items = await patients
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> MedicalRecordNumberExistsAsync(string medicalRecordNumber, int? excludePatientId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Patients.Where(p => p.MedicalRecordNumber == medicalRecordNumber);
        if (excludePatientId.HasValue)
        {
            query = query.Where(p => p.Id != excludePatientId.Value);
        }
        return await query.AnyAsync(cancellationToken);
    }

    public Task AddAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        _context.Patients.Add(patient);
        return Task.CompletedTask;
    }

    public void Remove(Patient patient) => _context.Patients.Remove(patient);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
