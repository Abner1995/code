using CleanArchitecture.Application.Restaurants;
using CleanArchitecture.Application.Restaurants.Commands.CreateRestaurant;
using CleanArchitecture.Application.Restaurants.Commands.DeleteRestaurant;
using CleanArchitecture.Application.Restaurants.Commands.UpdateRestaurant;
using CleanArchitecture.Application.Restaurants.Dtos;
using CleanArchitecture.Application.Restaurants.Queries.GetAllRestaurants;
using CleanArchitecture.Application.Restaurants.Queries.GetRestaurantById;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RestaurantsController(IRestaurantsService restaurantsService, IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllRestaurantsQuery query)
    {
        //var restaurants = await restaurantsService.GetAllRestaurants();
        var restaurants = await mediator.Send(query);
        return Ok(restaurants);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetId([FromRoute] int id)
    {
        //var restaurant = await restaurantsService.GetId(id);
        //if (restaurant is null)
        //    return NotFound();
        var restaurant = await mediator.Send(new GetRestaurantByIdQuery(id));
        return Ok(restaurant);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRestaurant([FromBody] CreateRestaurantCommand command)
    {
        //int id = await restaurantsService.Create(createRestaurantDto);
        //使用MediatR
        int id = await mediator.Send(command);
        return CreatedAtAction(nameof(GetId), new { id }, null);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateRestaurant([FromRoute] int id, UpdateRestaurantCommand command)
    {
        command.Id = id;
        await mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRestaurant([FromRoute] int id)
    {
        await mediator.Send(new DeleteRestaurantCommand(id));
        return NoContent();
    }
}
