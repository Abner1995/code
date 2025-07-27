using ApiAdmin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiAdmin.Infrastructure.Configs;

public class AdminConfig : IEntityTypeConfiguration<Admins>
{
    public void Configure(EntityTypeBuilder<Admins> builder)
    {
        builder.ToTable($"T_{nameof(Admins)}".ToLower(), t => t.HasComment("管理员表"));
        builder.HasKey(t => t.Id);
        builder.Property(t => t.UserName).HasMaxLength(60).IsRequired().HasComment("用户名");
        builder.Property(t => t.PassWord).IsRequired().HasComment("密码");
        builder.Property(t => t.Avatar).HasComment("头像");
        builder.Property(t => t.CreatedAt).HasComment("创建时间");
        builder.Property(t => t.UpdatedAt).HasComment("更新时间");
        builder.Ignore(c => c.RefreshTokens);
    }
}
