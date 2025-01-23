using CleanArchitecture.Application.Common;
using CleanArchitecture.Application.Restaurants.Dtos;
using CleanArchitecture.Domain.Constants;
using MediatR;

namespace CleanArchitecture.Application.Restaurants.Queries.GetAllRestaurants;

public class GetAllRestaurantsQuery : IRequest<PagedResult<RestaurantDto>>
{
    public string? SearchPhrase { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; set; }
    public SortDirection SortDirection { get; set; }
}
