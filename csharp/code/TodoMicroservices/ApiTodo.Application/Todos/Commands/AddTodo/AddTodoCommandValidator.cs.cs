using FluentValidation;

namespace ApiTodo.Application.Todos.Commands.AddTodo;

public class AddTodoCommandValidator : AbstractValidator<AddTodoCommand>
{
    public AddTodoCommandValidator()
    {
        RuleFor(x => x.Title)
            .Length(1, 10)
            .WithMessage("请输入内容");
    }
}
