using BuildingBlocks.Identity;
using Microsoft.AspNetCore.Authorization;
using Ordering.Application.Data;
using Ordering.Application.Orders.Commands.DeleteOrder;
using Ordering.Domain.ValueObjects;

namespace Ordering.API.Endpoints.v2;

[Authorize]
public class DeleteOrder : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/v2/orders/{id}", async (Guid id, ISender sender, IUserContext userContext, IApplicationDbContext dbContext) =>
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
            var orderId = OrderId.Of(id);
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

            // 删除订单
            var result = await sender.Send(new DeleteOrderCommand(id));

            var response = result.Adapt<DeleteOrderResponse>();

            return Results.Ok(response);
        })
        .WithName("DeleteOrderV2")
        .Produces<DeleteOrderResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Delete Order V2")
        .WithDescription("Delete Order V2 - Only allow authenticated user to delete their own orders.");
    }
}