using FluentValidation;

namespace ApiTodo.Application.Todos.Commands.EditTodo;

public class EditTodoCommandValidator : AbstractValidator<EditTodoCommand>
{
    public EditTodoCommandValidator()
    {
        RuleFor(x => x.Title)
            .Length(1, 10)
            .WithMessage("请输入内容");
    }
}
