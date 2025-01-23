using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Repositories;

public interface IDishesRepository
{
    Task<int> Create(Dish entity);
    Task Delete(IEnumerable<Dish> entities);
}
