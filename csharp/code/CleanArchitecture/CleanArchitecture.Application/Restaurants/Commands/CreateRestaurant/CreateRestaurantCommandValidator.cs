using FluentValidation;

namespace CleanArchitecture.Application.Restaurants.Commands.CreateRestaurant;

public class CreateRestaurantCommandValidator : AbstractValidator<CreateRestaurantCommand>
{
    private readonly List<string> validCategorys = ["蔬菜"];

    public CreateRestaurantCommandValidator()
    {
        RuleFor(x => x.Name).Length(2, 100);

        //自定义
        RuleFor(x => x.Category)
            .Must(validCategorys.Contains)
            .WithMessage("分类不对");

        RuleFor(x => x.ContactNumber)
            .NotEmpty()
            .WithMessage("请输入正确手机号码");
    }
}
