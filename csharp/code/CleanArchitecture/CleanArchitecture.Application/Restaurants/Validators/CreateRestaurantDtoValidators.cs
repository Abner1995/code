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
        //.Custom((value, context) =>
        //{
        //    var isValidCategory = validCategories.Contains(value);
        //    if(!isValidCategory)
        //    {
        //        context.AddFailure("Category", "Invalid category. Please choose from the valid categories.");
        //    }
        //});

        RuleFor(x => x.ContactNumber)
            .EmailAddress()
            .WithMessage("请输入正确手机号码");
    }
}
