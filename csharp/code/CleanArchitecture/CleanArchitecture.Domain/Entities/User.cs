using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Domain.Entities;

public class User : IdentityUser
{
    public DateTime? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
}
