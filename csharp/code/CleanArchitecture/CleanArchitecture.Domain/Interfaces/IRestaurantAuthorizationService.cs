using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface IRestaurantAuthorizationService
{
    bool Authorize(Restaurant restaurant, ResourceOperation resourceOperation);
}
