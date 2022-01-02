using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool Verified { get; set; } = false;
    public bool Disabled { get; set; } = false;
    public DateTimeOffset LastLogin { get; set; }
}