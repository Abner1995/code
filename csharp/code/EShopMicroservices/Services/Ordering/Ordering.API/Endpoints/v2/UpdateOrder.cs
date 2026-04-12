using BuildingBlocks.Identity;
using Microsoft.AspNetCore.Authorization;
using Ordering.Application.Data;
using Ordering.Application.Orders.Commands.UpdateOrder;
using Ordering.Domain.ValueObjects;

namespace Ordering.API.Endpoints.v2;

[Authorize]
public class UpdateOrder : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/v2/orders", async (UpdateOrderRequest request, ISender sender, IUserContext userContext, IApplicationDbContext dbContext) =>
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

            // 查找订单并验证所属用户
            var orderId = OrderId.Of(request.Order.Id);
            var order = await dbContext.Orders.FindAsync([orderId], cancellationToken: default);
            if (order is null)
            {
                return Results.NotFound();
            }

            // 验证订单是否属于当前用户
            if (order.CustomerId.Value != customerId)
            {
                return Results.Forbid();
            }

            // 使用来自用户上下文的CustomerId覆盖请求中的CustomerId
            var orderDto = request.Order with { CustomerId = customerId };

            var command = new UpdateOrderCommand(orderDto);

            var result = await sender.Send(command);

            var response = result.Adapt<UpdateOrderResponse>();

            return Results.Ok(response);
        })
        .WithName("UpdateOrderV2")
        .Produces<UpdateOrderResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Update Order V2")
        .WithDescription("Update Order V2 - Only allow authenticated user to update their own orders, CustomerId is overridden with authenticated user's ID.");
    }
}