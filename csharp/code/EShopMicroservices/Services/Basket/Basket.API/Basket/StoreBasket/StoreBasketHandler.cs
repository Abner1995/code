using Discount.Grpc;

namespace Basket.API.Basket.StoreBasket;

public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;

public record StoreBasketResult(string UserName);

public class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
{
    public StoreBasketCommandValidator()
    {
        RuleFor(x => x.Cart).NotNull().WithMessage("Cart can not be null");
        RuleFor(x => x.Cart.UserName).NotEmpty().WithMessage("UserName is required");
    }
}

public class StoreBasketHandler(IBasketRepository basketRepository, DiscountProtoService.DiscountProtoServiceClient discountProto) : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(StoreBasketCommand request, CancellationToken cancellationToken)
    {
        await DeductDiscount(request.Cart, cancellationToken);

        await basketRepository.StoreBasket(request.Cart, cancellationToken); ;

        return new StoreBasketResult(request.Cart.UserName);
    }

    public async Task DeductDiscount(ShoppingCart Cart, CancellationToken cancellationToken)
    {
        foreach (var item in Cart.Items)
        {
            var coupon = await discountProto.GetDiscountAsync(new GetDiscountRequest
            {
                ProductName = item.ProductName,
            }, cancellationToken: cancellationToken);
            item.Price -= coupon.Amount;
        }
    }
}
