using ApiTodo.Domain.Repositories;
using MediatR;
using Todo.Core;
using Todo.Core.Exceptions;

namespace ApiTodo.Application.Todos.Commands.DeleteTodo;

public class DeleteTodoCommandHandler(ITodosRepository todosRepository) : IRequestHandler<DeleteTodoCommand, ApiResponse<long>>
{
    public async Task<ApiResponse<long>> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = todosRepository.Get(request.Id);
        if (todo is null)
        {
            throw new NotFoundException(nameof(todo), request.Id.ToString());
        }

        todosRepository.Delete(todo.Id);
        await todosRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<long>.Success(todo.Id);
    }
}
