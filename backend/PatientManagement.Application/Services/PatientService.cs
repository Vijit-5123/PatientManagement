using PatientManagement.Application.Common;
using PatientManagement.Application.DTOs;
using PatientManagement.Application.Interfaces;
using PatientManagement.Domain.Entities;

namespace PatientManagement.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _repository;

    public PatientService(IPatientRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<PatientDto>> SearchAsync(string? query, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;

        var (items, totalCount) = await _repository.SearchAsync(query, page, pageSize, cancellationToken);

        return new PagedResult<PatientDto>
        {
            Items = items.Select(ToDto).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PatientDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var patient = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Patient with id {id} was not found.");

        return ToDto(patient);
    }

    public async Task<PatientDto> CreateAsync(CreatePatientRequest request, CancellationToken cancellationToken = default)
    {
        if (request.DateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new ValidationAppException(nameof(request.DateOfBirth), "Date of birth must not be in the future.");
        }

        var medicalRecordNumber = await GenerateMedicalRecordNumberAsync(cancellationToken);

        var now = DateTime.UtcNow;
        var patient = new Patient
        {
            MedicalRecordNumber = medicalRecordNumber,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender.Trim(),
            Phone = request.Phone?.Trim(),
            Email = request.Email?.Trim(),
            Address = request.Address?.Trim(),
            City = request.City?.Trim(),
            State = request.State?.Trim(),
            PostalCode = request.PostalCode?.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        await _repository.AddAsync(patient, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return ToDto(patient);
    }

    public async Task<PatientDto> UpdateAsync(int id, UpdatePatientRequest request, CancellationToken cancellationToken = default)
    {
        if (request.DateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new ValidationAppException(nameof(request.DateOfBirth), "Date of birth must not be in the future.");
        }

        var patient = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Patient with id {id} was not found.");

        patient.FirstName = request.FirstName.Trim();
        patient.LastName = request.LastName.Trim();
        patient.DateOfBirth = request.DateOfBirth;
        patient.Gender = request.Gender.Trim();
        patient.Phone = request.Phone?.Trim();
        patient.Email = request.Email?.Trim();
        patient.Address = request.Address?.Trim();
        patient.City = request.City?.Trim();
        patient.State = request.State?.Trim();
        patient.PostalCode = request.PostalCode?.Trim();
        patient.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);

        return ToDto(patient);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var patient = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Patient with id {id} was not found.");

        _repository.Remove(patient);
        await _repository.SaveChangesAsync(cancellationToken);
    }

    private async Task<string> GenerateMedicalRecordNumberAsync(CancellationToken cancellationToken)
    {
        string candidate;
        do
        {
            candidate = $"MRN-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
        }
        while (await _repository.MedicalRecordNumberExistsAsync(candidate, null, cancellationToken));

        return candidate;
    }

    private static PatientDto ToDto(Patient patient) => new()
    {
        Id = patient.Id,
        MedicalRecordNumber = patient.MedicalRecordNumber,
        FirstName = patient.FirstName,
        LastName = patient.LastName,
        DateOfBirth = patient.DateOfBirth,
        Gender = patient.Gender,
        Phone = patient.Phone,
        Email = patient.Email,
        Address = patient.Address,
        City = patient.City,
        State = patient.State,
        PostalCode = patient.PostalCode,
        CreatedAt = patient.CreatedAt,
        UpdatedAt = patient.UpdatedAt
    };
}
