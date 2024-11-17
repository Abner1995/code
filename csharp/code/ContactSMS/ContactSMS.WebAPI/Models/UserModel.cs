using System.ComponentModel.DataAnnotations;

namespace ContactSMS.WebAPI.Models;

public class UserModel
{
    [Required]
    public int Id { get; set; }

    [Required]
    [MinLength(2)]
    [MaxLength(10)]
    public string FirstName { get; set; }

    [Required]
    [MinLength(2)]
    [MaxLength(10)]
    public string LastName { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    [Phone]
    public string Phone { get; set; }

    [Url]
    public string HomePage { get; set; }

    [Range(0, 5)]
    public int NumberOfVehicles { get; set; }
}
