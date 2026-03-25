namespace AppCore.Models;

public class Note : EntityBase
{
    public string Content { get; set; } = string.Empty;
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
}