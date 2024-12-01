using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Configs;

public class DishConfig : IEntityTypeConfiguration<Dish>
{
    public void Configure(EntityTypeBuilder<Dish> builder)
    {
        builder.ToTable($"T_{nameof(Dish)}", t => t.HasComment("菜单"));
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired().HasComment("菜单名");
        builder.Property(t => t.Description).IsRequired().HasComment("菜单介绍");
        builder.Property(t => t.Price).IsRequired().HasComment("价格");
        // 外键配置
        builder.HasOne<Restaurant>()
               //.WithMany()  // 如果 Restaurant 没有导航属性指向 Dish，可以保持这样
               .WithMany(r => r.Dishes)  // 如果 Restaurant 有 Dishes 导航属性
               .HasForeignKey(t => t.RestaurantId)
               .OnDelete(DeleteBehavior.Cascade);  // 可选：级联删除
    }
}
