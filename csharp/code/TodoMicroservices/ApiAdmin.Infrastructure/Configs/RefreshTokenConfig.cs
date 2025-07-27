using ApiAdmin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiAdmin.Infrastructure.Configs;

public class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable($"T_{nameof(RefreshToken)}".ToLower(), t => t.HasComment("管理RefreshToken"));
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Token).HasComment("Token");
        builder.Property(t => t.UserId).HasComment("用户ID");
        builder.Property(t => t.Expiry).HasComment("过期时间");
        builder.Property(t => t.DeviceId).HasComment("设备标识");
    }
}
