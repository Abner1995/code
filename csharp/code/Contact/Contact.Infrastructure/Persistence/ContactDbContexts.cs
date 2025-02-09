using ContactDomainEntities = Contact.Domain.Entities.Contact;
using Contact.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Contact.Infrastructure.Persistence;

internal class ContactDbContexts : DbContext
{
    internal DbSet<User> Users { get; set; }
    internal DbSet<ContactDomainEntities> Contacts { get; set; }
    internal DbSet<Phone> Phones { get; set; }

    public ContactDbContexts(DbContextOptions<ContactDbContexts> options) : base(options)
    {
        
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseMySql("server=localhost;database=contact;user=contact;password=contact", new MySqlServerVersion(new Version(5, 6, 20)));
    //}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasCharSet("utf8mb4");
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
