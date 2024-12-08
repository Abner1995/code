using CleanArchitecture.Application.Restaurants;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RestaurantsController(IRestaurantsService restaurantsService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var restaurants = await restaurantsService.GetAllRestaurants();
        return Ok(restaurants);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetId([FromRoute]int id)
    {
        var restaurant = await restaurantsService.GetId(id);
        if (restaurant is null)
            return NotFound();

        return Ok(restaurant);
    }
}
