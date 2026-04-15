using AppCore.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Entities;

public class CrmUser : IdentityUser, ISystemUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string FullName
    {
        get => $"{FirstName} {LastName}";
        init => throw new NotImplementedException();
    }

    public required string Department { get; set; }
    public SystemUserStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeactivatedAt { get; private set; }

    public void Activate() {
        if (Status == SystemUserStatus.Inactive) Status = SystemUserStatus.Active;
    }

    public void Deactivate(DateTime now) {
        if (Status == SystemUserStatus.Active) {
            Status = SystemUserStatus.Inactive;
            DeactivatedAt = now;
        }
    }
}