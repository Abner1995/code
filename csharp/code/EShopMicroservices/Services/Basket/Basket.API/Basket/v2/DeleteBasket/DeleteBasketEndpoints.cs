using Basket.API.Basket.DeleteBasket;
using BuildingBlocks.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Basket.v2.DeleteBasket;

public record DeleteBasketResponse(bool IsSuccess);

[Authorize]
public class DeleteBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/v2/basket", async (ISender sender, IUserContext userContext) =>
        {
            // 验证用户已认证
            if (!userContext.IsAuthenticated || string.IsNullOrEmpty(userContext.UserName))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(new DeleteBasketCommand(userContext.UserName));

            var response = result.Adapt<DeleteBasketResponse>();

            return Results.Ok(response);
        })
        .WithName("DeleteBasketV2")
        .Produces<DeleteBasketResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Delete Basket V2")
        .WithDescription("Delete Basket V2 - Deletes the shopping cart for the authenticated user.");
    }
}