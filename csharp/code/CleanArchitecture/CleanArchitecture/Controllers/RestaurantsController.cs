using CleanArchitecture.Application.Restaurants;
using CleanArchitecture.Application.Restaurants.Commands.CreateRestaurant;
using CleanArchitecture.Application.Restaurants.Commands.DeleteRestaurant;
using CleanArchitecture.Application.Restaurants.Commands.UpdateRestaurant;
using CleanArchitecture.Application.Restaurants.Dtos;
using CleanArchitecture.Application.Restaurants.Queries.GetAllRestaurants;
using CleanArchitecture.Application.Restaurants.Queries.GetRestaurantById;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RestaurantsController(IRestaurantsService restaurantsService, IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [Authorize(Policy = PolicyNames.CreatedAtleast2Restaurants)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllRestaurantsQuery query)
    {
        //var restaurants = await restaurantsService.GetAllRestaurants();
        var restaurants = await mediator.Send(query);
        return Ok(restaurants);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = PolicyNames.HasNationality)]
    public async Task<IActionResult> GetId([FromRoute] int id)
    {
        //var restaurant = await restaurantsService.GetId(id);
        //if (restaurant is null)
        //    return NotFound();
        var restaurant = await mediator.Send(new GetRestaurantByIdQuery(id));
        return Ok(restaurant);
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Owner)]
    public async Task<IActionResult> CreateRestaurant([FromBody] CreateRestaurantCommand command)
    {
        //int id = await restaurantsService.Create(createRestaurantDto);
        //使用MediatR
        int id = await mediator.Send(command);
        return CreatedAtAction(nameof(GetId), new { id }, null);
    }

    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRestaurant([FromRoute] int id, UpdateRestaurantCommand command)
    {
        command.Id = id;
        await mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRestaurant([FromRoute] int id)
    {
        await mediator.Send(new DeleteRestaurantCommand(id));
        return NoContent();
    }
}
