using PatientManagement.Application.DTOs;

namespace PatientManagement.Application.Interfaces;

public interface IPatientService
{
    Task<PagedResult<PatientDto>> SearchAsync(string? query, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<PatientDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PatientDto> CreateAsync(CreatePatientRequest request, CancellationToken cancellationToken = default);
    Task<PatientDto> UpdateAsync(int id, UpdatePatientRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
