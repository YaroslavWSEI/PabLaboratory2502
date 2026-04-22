using AppCore.Enums;

namespace AppCore.Models;

public abstract class Contact : EntityBase
{
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public Address? Address { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ContactStatus Status { get; set; }

    public List<Tag> Tags { get; set; } = new();
    public List<Note> Notes { get; set; } = new();

    public abstract string GetDisplayName();
}