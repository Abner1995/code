using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Restaurants.Commands.DeleteRestaurant;

public class DeleteRestaurantCommandHandler(ILogger<DeleteRestaurantCommandHandler> logger,
    IRestaurantsRepository restaurantsRepository) : IRequestHandler<DeleteRestaurantCommand>
{
    public async Task Handle(DeleteRestaurantCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting restaurant with id: {RestaurantId}", request.Id);
        var restaurant = await restaurantsRepository.GetIdAsync(request.Id);
        if (restaurant is null)
            throw new NotFoundException(nameof(Restaurant), request.Id.ToString());

        await restaurantsRepository.DeleteAsync(restaurant);
    }
}
