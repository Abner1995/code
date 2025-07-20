using ApiTodo.Domain.Repositories;
using MediatR;
using Todo.Core;
using Todo.Core.Exceptions;

namespace ApiTodo.Application.Todos.Commands.EditTodo;

public class EditTodoCommandHandler(ITodosRepository todosRepository) : IRequestHandler<EditTodoCommand, ApiResponse<long>>
{
    public async Task<ApiResponse<long>> Handle(EditTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = todosRepository.Get(request.Id);
        if (todo is null)
        {
            throw new NotFoundException(nameof(todo), request.Title);
        }

        long UserId = 1;

        todo.Update(
            title: request.Title,
            status: request.Status,
            priority: request.Priority,
            dueDate: request.DueDate
        );

        todosRepository.Update(todo);
        await todosRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<long>.Success(todo.Id);
    }
}
