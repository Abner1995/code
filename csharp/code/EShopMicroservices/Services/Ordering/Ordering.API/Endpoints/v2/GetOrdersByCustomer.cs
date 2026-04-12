using BuildingBlocks.Identity;
using Microsoft.AspNetCore.Authorization;
using Ordering.Application.Orders.Queries.GetOrdersByCustomer;

namespace Ordering.API.Endpoints.v2;

[Authorize]
public class GetOrdersByCustomer : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/v2/orders/customer/{customerId}", async (Guid customerId, ISender sender, IUserContext userContext) =>
        {
            // 验证用户已认证
            if (!userContext.IsAuthenticated)
            {
                return Results.Unauthorized();
            }

            // 解析当前用户ID为Guid
            if (!Guid.TryParse(userContext.UserId, out var currentCustomerId))
            {
                return Results.BadRequest("Invalid user ID format");
            }

            // 验证请求的customerId与当前用户匹配
            if (customerId != currentCustomerId)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new GetOrdersByCustomerQuery(customerId));

            var response = new GetOrdersByCustomerResponse(result.Orders);

            return Results.Ok(response);
        })
        .WithName("GetOrdersByCustomerV2")
        .Produces<GetOrdersByCustomerResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .WithSummary("Get Orders By Customer V2")
        .WithDescription("Get Orders By Customer V2 - Requires authenticated user to match requested customerId");
    }
}