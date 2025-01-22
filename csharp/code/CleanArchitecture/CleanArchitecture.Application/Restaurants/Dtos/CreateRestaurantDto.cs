using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.Restaurants.Dtos;

public class CreateRestaurantDto
{
    //[StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Category { get; set; } = default!;
    public bool HasDelivery { get; set; }
    public string? ContactEmail { get; set; }
    //[Phone(ErrorMessage = "请填写正确号码")]
    public string? ContactNumber { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? PostalCode { get; set; }
}
