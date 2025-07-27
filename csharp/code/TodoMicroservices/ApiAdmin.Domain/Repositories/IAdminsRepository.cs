using ApiAdmin.Domain.Entities;
using Todo.Infrastructure.Core;

namespace ApiAdmin.Domain.Repositories;

public interface IAdminsRepository : IRepository<Admins, long>
{
    Task<Admins?> GetByUserNameAsync(string userName, CancellationToken cancellationToken);
}
