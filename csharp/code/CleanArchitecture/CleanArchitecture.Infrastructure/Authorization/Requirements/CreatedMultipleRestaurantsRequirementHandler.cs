﻿using CleanArchitecture.Application.Users;
using CleanArchitecture.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace CleanArchitecture.Infrastructure.Authorization.Requirements;

internal class CreatedMultipleRestaurantsRequirementHandler(IRestaurantsRepository restaurantsRepository,
        IUserContext userContext) : AuthorizationHandler<CreatedMultipleRestaurantsRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CreatedMultipleRestaurantsRequirement requirement)
    {
        var currentUser = userContext.GetCurrentUser();

        var restaurants = await restaurantsRepository.GetAllAsync();

        var userRestaurantsCreated = restaurants.Count(r => r.OwnerId == currentUser!.Id);

        if (userRestaurantsCreated >= requirement.MinimumRestaurantsCreated)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}
