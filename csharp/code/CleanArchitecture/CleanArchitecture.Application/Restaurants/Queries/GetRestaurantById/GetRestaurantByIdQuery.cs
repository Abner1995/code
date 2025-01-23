using CleanArchitecture.Application.Restaurants.Dtos;
using MediatR;

namespace CleanArchitecture.Application.Restaurants.Queries.GetRestaurantById;

public class GetRestaurantByIdQuery(int id) : IRequest<RestaurantDto>
{
    public int Id { get; } = id;
}
