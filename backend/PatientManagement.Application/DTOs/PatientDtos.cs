using System.ComponentModel.DataAnnotations;

namespace PatientManagement.Application.DTOs;

public class PatientDto
{
    public int Id { get; set; }
    public string MedicalRecordNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreatePatientRequest
{
    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of birth is required.")]
    public DateOnly DateOfBirth { get; set; }

    [Required(ErrorMessage = "Gender is required.")]
    [MaxLength(30)]
    public string Gender { get; set; } = string.Empty;

    [MaxLength(30)]
    [Phone(ErrorMessage = "Phone number is not in a valid format.")]
    public string? Phone { get; set; }

    [MaxLength(200)]
    [EmailAddress(ErrorMessage = "Email is not in a valid format.")]
    public string? Email { get; set; }

    [MaxLength(300)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }
}

public class UpdatePatientRequest
{
    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of birth is required.")]
    public DateOnly DateOfBirth { get; set; }

    [Required(ErrorMessage = "Gender is required.")]
    [MaxLength(30)]
    public string Gender { get; set; } = string.Empty;

    [MaxLength(30)]
    [Phone(ErrorMessage = "Phone number is not in a valid format.")]
    public string? Phone { get; set; }

    [MaxLength(200)]
    [EmailAddress(ErrorMessage = "Email is not in a valid format.")]
    public string? Email { get; set; }

    [MaxLength(300)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }
}

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
