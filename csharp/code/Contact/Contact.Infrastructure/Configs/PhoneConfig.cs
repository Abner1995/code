using Contact.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Contact.Infrastructure.Configs;

public class PhoneConfig : IEntityTypeConfiguration<Phone>
{
    public void Configure(EntityTypeBuilder<Phone> builder)
    {
        builder.ToTable($"T_{nameof(Phone)}".ToLower(), t => t.HasComment("联系号码表"));
        builder.HasNoKey();  // 禁用主键
        builder.Property(t => t.ContactId).HasComment("联系人ID");
        builder.Property(t => t.UserId).HasComment("用户ID");
        builder.Property(t => t.Mobile).IsRequired().HasComment("联系号码");
    }
}
