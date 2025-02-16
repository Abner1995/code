using Contact.Domain.Entities;
using Contact.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Contact.Infrastructure.Seeders;

internal class UserSeeder(ContactDbContexts dbContexts) : IUserSeeder
{
    public async Task Seed()
    {
        User user = GetUser();
        var res = await dbContexts.Users.FirstOrDefaultAsync(x=>x.UserName.Equals(user.UserName));
        if (res == null) { 
            await dbContexts.Users.AddAsync(user);
            await dbContexts.SaveChangesAsync();
        }
    }

    private User GetUser()
    {
        return new User
        {
            UserName = "xuzizheng",
            PassWord = "111111"
        };
    }
}