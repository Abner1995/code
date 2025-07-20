using ApiTodo.Domain.Repositories;
using MediatR;
using Todo.Core;
using TodosEntitie = ApiTodo.Domain.Entities.Todos;

namespace ApiTodo.Application.Todos.Commands.AddTodo;

public class AddTodoCommandHandler(ITodosRepository todosRepository) : IRequestHandler<AddTodoCommand, ApiResponse<long>>
{
    public async Task<ApiResponse<long>> Handle(AddTodoCommand request, CancellationToken cancellationToken)
    {
        long UserId = 1;
        var todo = new TodosEntitie(request.Title, request.DueDate, UserId);
        await todosRepository.AddAsync(todo);
        await todosRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<long>.Success(todo.Id);
    }
}
