using CleanArchitecture.Application.Restaurants.Dtos;
using FluentValidation;

namespace CleanArchitecture.Application.Restaurants.Validators;

public class CreateRestaurantDtoValidators : AbstractValidator<CreateRestaurantDto>
{
    private readonly List<string> validCategorys = ["蔬菜"];

    public CreateRestaurantDtoValidators()
    {
        RuleFor(x => x.Name).Length(2, 100);

        //自定义
        RuleFor(x => x.Category)
            .Must(validCategorys.Contains)
            .WithMessage("分类不对");

        RuleFor(x => x.ContactNumber)
            .EmailAddress()
            .WithMessage("请输入正确手机号码");
    }
}
