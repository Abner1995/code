using Basket.API.Basket.GetBasket;
using BuildingBlocks.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Basket.v2.GetBasket;

public record GetBasketResponse(ShoppingCart Cart);

[Authorize]
public class GetBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/v2/basket", async (ISender sender, IUserContext userContext) =>
        {
            // 验证用户已认证
            if (!userContext.IsAuthenticated || string.IsNullOrEmpty(userContext.UserName))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(new GetBasketQuery(userContext.UserName));

            var response = result.Adapt<GetBasketResponse>();

            return Results.Ok(response);
        })
        .WithName("GetBasketV2")
        .Produces<GetBasketResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Get Basket V2")
        .WithDescription("Get Basket V2 - Returns the shopping cart for the authenticated user.");
    }
}