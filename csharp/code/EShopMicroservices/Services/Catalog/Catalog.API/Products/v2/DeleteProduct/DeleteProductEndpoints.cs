using BuildingBlocks.Identity;
using Catalog.API.Products.DeleteProduct;
using Microsoft.AspNetCore.Authorization;

namespace Catalog.API.Products.v2.DeleteProduct;

public record DeleteProductResponse(bool IsSuccess);

[Authorize]
public class DeleteProductEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/v2/products/{id}", async (Guid id, ISender sender, IUserContext userContext) =>
        {
            // 验证用户已认证
            if (!userContext.IsAuthenticated)
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(new DeleteProductCommand(id));

            var response = result.Adapt<DeleteProductResponse>();

            return Results.Ok(response);
        })
        .WithName("DeleteProductV2")
        .Produces<DeleteProductResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Delete Product V2")
        .WithDescription("Delete Product V2 - Deletes a product. Requires authentication.");
    }
}