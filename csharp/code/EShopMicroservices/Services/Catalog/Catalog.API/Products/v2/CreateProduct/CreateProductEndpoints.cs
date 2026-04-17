using BuildingBlocks.Identity;
using Catalog.API.Products.CreateProduct;
using Microsoft.AspNetCore.Authorization;

namespace Catalog.API.Products.v2.CreateProduct;

public record CreateProductRequest(string Name, List<string> Category, string Description, string ImageFile, decimal Price);
public record CreateProductResponse(Guid Id);

[Authorize]
public class CreateProductEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/v2/products", async (CreateProductRequest request, ISender sender, IUserContext userContext) =>
        {
            // 验证用户已认证
            if (!userContext.IsAuthenticated)
            {
                return Results.Unauthorized();
            }

            var command = request.Adapt<CreateProductCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<CreateProductResponse>();

            return Results.Created($"/v2/products/{response.Id}", response);
        })
        .WithName("CreateProductV2")
        .Produces<CreateProductResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Create Product V2")
        .WithDescription("Create Product V2 - Creates a new product. Requires authentication.");
    }
}