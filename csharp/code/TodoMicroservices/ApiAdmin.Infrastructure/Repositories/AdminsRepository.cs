using ApiAdmin.Domain.Entities;
using ApiAdmin.Domain.Repositories;
using ApiAdmin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Todo.Infrastructure.Core;

namespace ApiAdmin.Infrastructure.Repositories;

public class AdminsRepository : Repository<Admins, long, AdminDbContext>, IAdminsRepository
{
    public AdminsRepository(AdminDbContext context) : base(context)
    {
    }

    public async Task<Admins?> GetByUserNameAsync(string userName, CancellationToken cancellationToken)
    {
        return await DbContext.Set<Admins>()
            .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
    }
}
