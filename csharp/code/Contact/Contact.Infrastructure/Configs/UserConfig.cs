using Contact.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Contact.Infrastructure.Configs;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable($"T_{nameof(User)}".ToLower(), t => t.HasComment("用户表"));
        builder.HasKey(t => t.Id);
        builder.Property(t => t.UserName).HasMaxLength(60).IsRequired().HasComment("用户名");
        builder.Property(t => t.PassWord).IsRequired().HasComment("密码");
        builder.Property(t => t.Avatar).HasComment("头像");
        builder.Property(t => t.CreatedAt).HasComment("创建时间");
        builder.Property(t => t.UpdatedAt).HasComment("更新时间");
        builder.HasMany(c => c.RefreshTokens)
        .WithOne()
        .HasForeignKey("UserId");
    }
}
