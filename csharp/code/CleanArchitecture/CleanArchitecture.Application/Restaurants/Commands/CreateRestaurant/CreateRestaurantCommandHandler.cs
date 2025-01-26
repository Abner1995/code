using AutoMapper;
using CleanArchitecture.Application.Users;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Restaurants.Commands.CreateRestaurant;

public class CreateRestaurantCommandHandler(ILogger<CreateRestaurantCommandHandler> logger,
    IMapper mapper,
    IRestaurantsRepository restaurantsRepository,
    IUserContext userContext) : IRequestHandler<CreateRestaurantCommand, int>
{
    public async Task<int> Handle(CreateRestaurantCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();
        logger.LogInformation("{UserEmail} [{UserId}] 新增餐馆 {@Restaurant}", currentUser.Email,
            currentUser.Id, request);
        var restaurant = mapper.Map<Restaurant>(request);
        restaurant.OwnerId = currentUser.Id;
        int id = await restaurantsRepository.CreateAsync(restaurant);
        return id;
    }
}
