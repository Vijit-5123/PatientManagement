using PatientManagement.Application.Interfaces;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Enums;
using PatientManagement.Infrastructure.Persistence;

namespace PatientManagement.Infrastructure.Seed;

public static class DbSeeder
{
    public static void Seed(ApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        if (!context.Users.Any())
        {
            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            context.Users.AddRange(
                new User
                {
                    Username = "admin",
                    PasswordHash = passwordHasher.Hash("Admin123!"),
                    Role = UserRole.Admin,
                    CreatedAt = seedDate
                },
                new User
                {
                    Username = "doctor",
                    PasswordHash = passwordHasher.Hash("Doctor123!"),
                    Role = UserRole.Doctor,
                    CreatedAt = seedDate
                });
        }

        if (!context.Patients.Any())
        {
            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var patients = new[]
            {
                new Patient { MedicalRecordNumber = "MRN-00000001", FirstName = "Alice", LastName = "Anderson", DateOfBirth = new DateOnly(1985, 3, 14), Gender = "Female", Phone = "555-0101", Email = "alice.anderson@example.com", Address = "12 Maple St", City = "Springfield", State = "IL", PostalCode = "62701", CreatedAt = seedDate, UpdatedAt = seedDate },
                new Patient { MedicalRecordNumber = "MRN-00000002", FirstName = "Brian", LastName = "Baker", DateOfBirth = new DateOnly(1978, 7, 22), Gender = "Male", Phone = "555-0102", Email = "brian.baker@example.com", Address = "45 Oak Ave", City = "Springfield", State = "IL", PostalCode = "62702", CreatedAt = seedDate, UpdatedAt = seedDate },
                new Patient { MedicalRecordNumber = "MRN-00000003", FirstName = "Carla", LastName = "Cruz", DateOfBirth = new DateOnly(1992, 11, 3), Gender = "Female", Phone = "555-0103", Email = "carla.cruz@example.com", Address = "78 Pine Rd", City = "Riverdale", State = "IL", PostalCode = "62703", CreatedAt = seedDate, UpdatedAt = seedDate },
                new Patient { MedicalRecordNumber = "MRN-00000004", FirstName = "David", LastName = "Diaz", DateOfBirth = new DateOnly(1965, 1, 30), Gender = "Male", Phone = "555-0104", Email = "david.diaz@example.com", Address = "23 Cedar Ln", City = "Riverdale", State = "IL", PostalCode = "62704", CreatedAt = seedDate, UpdatedAt = seedDate },
                new Patient { MedicalRecordNumber = "MRN-00000005", FirstName = "Elena", LastName = "Evans", DateOfBirth = new DateOnly(2001, 9, 18), Gender = "Female", Phone = "555-0105", Email = "elena.evans@example.com", Address = "90 Birch Blvd", City = "Springfield", State = "IL", PostalCode = "62705", CreatedAt = seedDate, UpdatedAt = seedDate },
                new Patient { MedicalRecordNumber = "MRN-00000006", FirstName = "Frank", LastName = "Foster", DateOfBirth = new DateOnly(1955, 5, 9), Gender = "Male", Phone = "555-0106", Email = "frank.foster@example.com", Address = "16 Elm St", City = "Millbrook", State = "IL", PostalCode = "62706", CreatedAt = seedDate, UpdatedAt = seedDate },
                new Patient { MedicalRecordNumber = "MRN-00000007", FirstName = "Grace", LastName = "Green", DateOfBirth = new DateOnly(1988, 12, 25), Gender = "Female", Phone = "555-0107", Email = "grace.green@example.com", Address = "34 Willow Way", City = "Millbrook", State = "IL", PostalCode = "62707", CreatedAt = seedDate, UpdatedAt = seedDate },
                new Patient { MedicalRecordNumber = "MRN-00000008", FirstName = "Henry", LastName = "Hughes", DateOfBirth = new DateOnly(1970, 4, 2), Gender = "Male", Phone = "555-0108", Email = "henry.hughes@example.com", Address = "61 Spruce Ct", City = "Riverdale", State = "IL", PostalCode = "62708", CreatedAt = seedDate, UpdatedAt = seedDate },
                new Patient { MedicalRecordNumber = "MRN-00000009", FirstName = "Isla", LastName = "Ibarra", DateOfBirth = new DateOnly(1996, 6, 11), Gender = "Female", Phone = "555-0109", Email = "isla.ibarra@example.com", Address = "8 Aspen Dr", City = "Springfield", State = "IL", PostalCode = "62709", CreatedAt = seedDate, UpdatedAt = seedDate },
                new Patient { MedicalRecordNumber = "MRN-00000010", FirstName = "Jack", LastName = "Jensen", DateOfBirth = new DateOnly(1982, 8, 27), Gender = "Male", Phone = "555-0110", Email = "jack.jensen@example.com", Address = "52 Poplar Pl", City = "Millbrook", State = "IL", PostalCode = "62710", CreatedAt = seedDate, UpdatedAt = seedDate },
            };

            context.Patients.AddRange(patients);
        }

        context.SaveChanges();
    }
}
