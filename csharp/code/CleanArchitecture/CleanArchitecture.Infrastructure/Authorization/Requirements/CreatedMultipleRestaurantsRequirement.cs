using Microsoft.AspNetCore.Authorization;

namespace CleanArchitecture.Infrastructure.Authorization.Requirements;

public class CreatedMultipleRestaurantsRequirement(int minimumRestaurantsCreated) : IAuthorizationRequirement
{
    public int MinimumRestaurantsCreated { get; } = minimumRestaurantsCreated;
}
