using AppCore.Enums;

namespace AppCore.Models;

public class Organization : Contact
{
    public string Name { get; set; } = string.Empty;
    public string? RegistrationNumber { get; set; }
    public OrganizationType Type { get; set; }

    public List<Person> Members { get; set; } = new();
    public override string GetDisplayName()
    {
        return Name ?? "Unknown Organization";
    }
}