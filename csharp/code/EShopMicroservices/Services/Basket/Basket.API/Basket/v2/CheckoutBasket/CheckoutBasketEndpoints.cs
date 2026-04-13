using Basket.API.Basket.CheckoutBasket;
using BuildingBlocks.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Basket.v2.CheckoutBasket;

public record CheckoutBasketRequest(BasketCheckoutDto BasketCheckoutDto);
public record CheckoutBasketResponse(bool IsSuccess);

[Authorize]
public class CheckoutBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/v2/basket/checkout", async (CheckoutBasketRequest request, ISender sender, IUserContext userContext) =>
        {
            // 验证用户已认证
            if (!userContext.IsAuthenticated || string.IsNullOrEmpty(userContext.UserName))
            {
                return Results.Unauthorized();
            }

            // 验证请求中的用户名与当前认证用户匹配（如果提供了用户名）
            if (!string.IsNullOrEmpty(request.BasketCheckoutDto.UserName) &&
                request.BasketCheckoutDto.UserName != userContext.UserName)
            {
                return Results.BadRequest("User name in request does not match authenticated user");
            }

            // 从用户上下文设置用户名（覆盖请求中的任何值）
            request.BasketCheckoutDto.UserName = userContext.UserName;

            // TODO: 从用户上下文或其他地方获取CustomerId，如果需要的话
            // 目前我们保持现有的CustomerId，如果未设置则可能需要处理

            var command = request.Adapt<CheckoutBasketCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<CheckoutBasketResponse>();

            return Results.Ok(response);
        })
        .WithName("CheckoutBasketV2")
        .Produces<CheckoutBasketResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Checkout Basket V2")
        .WithDescription("Checkout Basket V2 - Checkout the shopping cart for the authenticated user.");
    }
}