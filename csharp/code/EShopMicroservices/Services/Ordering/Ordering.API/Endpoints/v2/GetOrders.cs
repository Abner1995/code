using BuildingBlocks.Identity;
using BuildingBlocks.Pagination;
using Microsoft.AspNetCore.Authorization;
using Ordering.API.Endpoints;
using Ordering.Application.Dtos;
using Ordering.Application.Orders.Queries.GetOrdersByCustomer;

namespace Ordering.API.Endpoints.v2;

[Authorize]
public class GetOrders : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/v2/orders", async ([AsParameters] PaginationRequest request, ISender sender, IUserContext userContext) =>
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

            // 使用GetOrdersByCustomerQuery获取当前用户的订单
            // 注意：GetOrdersByCustomerQuery当前不支持分页，我们需要处理分页
            // 为简单起见，暂时忽略分页，直接返回所有用户订单
            var result = await sender.Send(new GetOrdersByCustomerQuery(customerId));

            // 实现内存分页
            var ordersList = result.Orders.ToList();
            var totalItems = ordersList.Count;
            var pageIndex = request.PageIndex;
            var pageSize = Math.Max(request.PageSize, 1); // 确保至少1

            var pagedOrders = ordersList
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedResult = new PaginatedResult<OrderDto>(
                pageIndex,
                pageSize,
                totalItems,
                pagedOrders);

            var response = new GetOrdersResponse(paginatedResult);

            return Results.Ok(response);
        })
        .WithName("GetOrdersV2")
        .Produces<GetOrdersResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Get Orders V2")
        .WithDescription("Get Orders V2 - Returns orders for authenticated user only with pagination support.");
    }
}