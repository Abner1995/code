using ApiTodo.Domain.Entities;
using ApiTodo.Domain.Enums;
using ApiTodo.Domain.Repositories;
using ApiTodo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Todo.Infrastructure.Core;

namespace ApiTodo.Infrastructure.Repositories;

public class TodosRepository : Repository<Todos, long, TodoDbContext>, ITodosRepository
{
    public TodosRepository(TodoDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Todos>, int)> GetAllMatchingAsync(string? searchPhrase, int pageSize, int pageNumber, string? sortBy, SortDirection sortDirection, CancellationToken cancellationToken = default)
    {
        // 参数验证
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var searchPhraseLower = searchPhrase?.ToLower();

        var baseQuery = DbContext.Set<Todos>()
            .Where(r => searchPhraseLower == null ||
                        r.Title.ToLower().Contains(searchPhraseLower));

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        // 默认排序（必须要有排序才能分页）
        if (string.IsNullOrEmpty(sortBy))
        {
            sortBy = nameof(Todos.Id); // 默认按Id排序
        }

        // 排序字段映射
        var columnsSelector = new Dictionary<string, Expression<Func<Todos, object>>>
        {
            [nameof(Todos.Id)] = r => r.Id,
            [nameof(Todos.Title)] = r => r.Title,
            [nameof(Todos.CreatedAt)] = r => r.CreatedAt,
            [nameof(Todos.DueDate)] = r => r.DueDate,
            [nameof(Todos.Status)] = r => r.Status,
            [nameof(Todos.Priority)] = r => r.Priority
        };

        if (!columnsSelector.TryGetValue(sortBy, out var selectedColumn))
        {
            throw new ArgumentException($"无效的排序字段: {sortBy}");
        }

        baseQuery = sortDirection == SortDirection.Ascending
            ? baseQuery.OrderBy(selectedColumn)
            : baseQuery.OrderByDescending(selectedColumn);

        // 分页计算
        var skip = (pageNumber - 1) * pageSize;
        if (skip >= totalCount)
        {
            return (Enumerable.Empty<Todos>(), totalCount);
        }

        var items = await baseQuery
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
