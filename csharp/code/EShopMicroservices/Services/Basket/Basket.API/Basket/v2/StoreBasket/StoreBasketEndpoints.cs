using Basket.API.Basket.StoreBasket;
using BuildingBlocks.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Basket.v2.StoreBasket;

public record StoreBasketRequest(ShoppingCart Cart);

public record StoreBasketResponse(string UserName);

[Authorize]
public class StoreBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/v2/basket", async (StoreBasketRequest request, ISender sender, IUserContext userContext) =>
        {
            // 验证用户已认证
            if (!userContext.IsAuthenticated || string.IsNullOrEmpty(userContext.UserName))
            {
                return Results.Unauthorized();
            }

            // 验证请求中的用户名与当前认证用户匹配
            if (request.Cart.UserName != userContext.UserName)
            {
                return Results.BadRequest("User name in request does not match authenticated user");
            }

            var command = request.Adapt<StoreBasketCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<StoreBasketResponse>();

            return Results.Created($"/v2/basket/{response.UserName}", response);
        })
        .WithName("StoreBasketV2")
        .Produces<StoreBasketResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Store Basket V2")
        .WithDescription("Store Basket V2 - Stores or updates the shopping cart for the authenticated user.");
    }
}