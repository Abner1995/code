using ContactDomainEntities = Contact.Domain.Entities.Contact;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Contact.Infrastructure.Configs;

public class ContactConfig : IEntityTypeConfiguration<ContactDomainEntities>
{
    public void Configure(EntityTypeBuilder<ContactDomainEntities> builder)
    {
        builder.ToTable($"T_{nameof(ContactDomainEntities)}".ToLower(), t => t.HasComment("联系人表"));
        builder.HasKey(t => t.Id);
        builder.Property(t => t.UserId).HasComment("用户ID");
        builder.Property(t => t.UserName).HasMaxLength(60).IsRequired().HasComment("姓名");
        builder.HasMany(c => c.Phones)
        .WithOne()
        .HasForeignKey("ContactId");
        //添加这句无法使用Include
        //builder.Ignore(c => c.Phones);
    }
}
