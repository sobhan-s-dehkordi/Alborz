using Alborz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alborz.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);

        builder.Property(p => p.Barcode).IsRequired().HasMaxLength(50);
        builder.HasIndex(p => p.Barcode).IsUnique();

        builder.Property(p => p.PurchasePrice).HasColumnType("decimal(18,2)");
        builder.Property(p => p.SellPrice).HasColumnType("decimal(18,2)");
    }
}
