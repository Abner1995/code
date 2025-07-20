using ApiTodo.Domain.Entities;
using ApiTodo.Domain.Enums;
using Todo.Infrastructure.Core;

namespace ApiTodo.Domain.Repositories;

public interface ITodosRepository : IRepository<Todos, long>
{
    Task<(IEnumerable<Todos>, int)> GetAllMatchingAsync(string? searchPhrase, int pageSize, int pageNumber, string? sortBy, SortDirection sortDirection, CancellationToken cancellationToken = default);
}
