using AppCore.Enums;

namespace AppCore.Models;

public class Person : Contact
{
    
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public DateTime? BirthDate { get; set; }
    public Gender Gender { get; set; }

    public string? Position { get; set; }

    public Guid? EmployerId { get; set; }
    public Guid? OrganizationId { get; set; }
    public Company? Employer { get; set; }
    public object Organization { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public override string GetDisplayName()
        => $"{FirstName} {LastName}";
}