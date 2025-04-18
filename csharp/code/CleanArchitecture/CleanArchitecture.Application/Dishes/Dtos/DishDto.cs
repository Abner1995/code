﻿using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Dishes.Dtos;

public class DishDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; }
    public int? KiloCalories { get; set; }
    public int RestaurantId { get; set; }

    public static DishDto FromEntity(Dish d)
    {
        return new DishDto()
        {
            Id = d.Id,
            Name = d.Name,
            Description = d.Description,
            Price = d.Price,
            KiloCalories = d.KiloCalories,
        };
    }
}
