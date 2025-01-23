using MediatR;

namespace CleanArchitecture.Application.Dishes.Commands.DeleteDishes;

public class DeleteDishesForRestaurantCommand(int restaurantId) : IRequest
{
    public int RestaurantId { get; } = restaurantId;
}
