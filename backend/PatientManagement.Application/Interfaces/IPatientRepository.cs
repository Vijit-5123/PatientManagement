using PatientManagement.Domain.Entities;

namespace PatientManagement.Application.Interfaces;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Patient> Items, int TotalCount)> SearchAsync(string? query, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<bool> MedicalRecordNumberExistsAsync(string medicalRecordNumber, int? excludePatientId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Patient patient, CancellationToken cancellationToken = default);
    void Remove(Patient patient);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
