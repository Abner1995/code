using ApiTodo.Application.Todos.Dtos;
using FluentValidation;

namespace ApiTodo.Application.Todos.Queries.GetAllTodo;

public class GetAllTodoQueryValidator : AbstractValidator<GetAllTodoQuery>
{
    private int[] allowPageSizes = [5, 10, 15, 30];
    private string[] allowedSortByColumnNames = [nameof(TodoDto.Id), nameof(TodoDto.CreatedAt)];

    public GetAllTodoQueryValidator()
    {
        RuleFor(r => r.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(r => r.PageSize)
            .Must(value => allowPageSizes.Contains(value))
            .WithMessage($"Page size must be in [{string.Join(",", allowPageSizes)}]");

        RuleFor(r => r.SortBy)
            .Must(value => allowedSortByColumnNames.Contains(value))
            .When(q => q.SortBy != null)
            .WithMessage($"Sort by is optional, or must be in [{string.Join(",", allowedSortByColumnNames)}]");
    }
}
