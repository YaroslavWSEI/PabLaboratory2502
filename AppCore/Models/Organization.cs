using AppCore.Enums;

namespace AppCore.Models;

public class Organization : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public OrganizationType Type { get; set; }

    public List<Person> Members { get; set; } = new();
}