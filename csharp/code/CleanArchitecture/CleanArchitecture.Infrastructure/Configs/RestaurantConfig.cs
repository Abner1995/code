using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Configs;

public class RestaurantConfig : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.ToTable($"T_{nameof(Restaurant)}", t => t.HasComment("餐厅"));
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired().HasComment("餐厅名");
        builder.Property(t => t.Description).IsRequired().HasComment("餐厅介绍");
        builder.Property(t => t.Category).IsRequired().HasComment("餐厅分类");
        builder.Property(t => t.HasDelivery).HasComment("是否配送");
        builder.Property(t => t.ContactEmail).HasComment("邮箱");
        builder.Property(t => t.ContactNumber).HasComment("联系号码");
        builder.OwnsOne(t => t.Address, a =>
        {
            a.Property(ad => ad.Street).HasComment("街道");
            a.Property(ad => ad.City).HasComment("城市");
            a.Property(ad => ad.PostalCode).HasComment("邮政编码");
        });
    }
}
