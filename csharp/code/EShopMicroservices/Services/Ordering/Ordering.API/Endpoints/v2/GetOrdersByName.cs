using BuildingBlocks.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Data;
using Ordering.Application.Extensions;

namespace Ordering.API.Endpoints.v2;

[Authorize]
public class GetOrdersByName : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/v2/orders/{orderName}", async (string orderName, ISender sender, IUserContext userContext, IApplicationDbContext dbContext) =>
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

            // 查询属于当前用户且名称包含orderName的订单
            var orders = await dbContext.Orders
                .Include(o => o.OrderItems)
                .AsNoTracking()
                .Where(o => o.CustomerId.Value == customerId && o.OrderName.Value.Contains(orderName))
                .OrderBy(o => o.OrderName.Value)
                .ToListAsync();

            var orderDtos = orders.ToOrderDtoList();

            var response = new GetOrdersByNameResponse(orderDtos);

            return Results.Ok(response);
        })
        .WithName("GetOrdersByNameV2")
        .Produces<GetOrdersByNameResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Get Orders By Name V2")
        .WithDescription("Get Orders By Name V2 - Returns orders for authenticated user only, filtered by order name.");
    }
}