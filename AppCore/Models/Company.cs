namespace AppCore.Models;

public class Company : Contact
{
    public Guid Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string? NIP { get; set; }
    public string? REGON { get; set; }
    public string? KRS { get; set; }

    public List<Person> Employees { get; set; } = new();

    public override string GetDisplayName()
        => Name;
}