using BuildingBlocks.Identity;
using Microsoft.AspNetCore.Authorization;
using Ordering.API.Endpoints;
using Ordering.Application.Dtos;
using Ordering.Application.Orders.Commands.CreateOrder;
using Ordering.Domain.Enums;

namespace Ordering.API.Endpoints.v2;

[Authorize]
public class CreateOrder : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/v2/orders", async (CreateOrderRequest request, ISender sender, IUserContext userContext) =>
        {
            // 验证用户已认证
            if (!userContext.IsAuthenticated)
            {
                return Results.Unauthorized();
            }

            // 解析用户ID为Guid
            if (!Guid.TryParse(userContext.UserId, out var customerId))
            {
                return Results.BadRequest("Invalid user ID format");
            }

            // 使用来自用户上下文的CustomerId覆盖请求中的CustomerId
            var orderDto = request.Order with { CustomerId = customerId };

            var command = new CreateOrderCommand(orderDto);

            var result = await sender.Send(command);

            var response = result.Adapt<CreateOrderResponse>();

            return Results.Created($"/v2/orders/{response.Id}", response);
        })
        .WithName("CreateOrderV2")
        .Produces<CreateOrderResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Create Order V2")
        .WithDescription("Create Order V2 - Uses authenticated user's CustomerId (overrides any provided CustomerId)");
    }
}