using FluentValidation;

namespace ApiTodo.Application.Todos.Commands.AddTodo;

public class AddTodoCommandValidator : AbstractValidator<AddTodoCommand>
{
    public AddTodoCommandValidator()
    {
        RuleFor(x => x.Title)
            .Length(1, 100)
            .WithMessage("请输入内容");
    }
}
