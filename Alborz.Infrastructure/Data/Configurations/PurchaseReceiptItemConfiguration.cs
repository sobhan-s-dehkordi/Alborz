using Alborz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alborz.Infrastructure.Data.Configurations;

public class PurchaseReceiptItemConfiguration : IEntityTypeConfiguration<PurchaseReceiptItem>
{
    public void Configure(EntityTypeBuilder<PurchaseReceiptItem> builder)
    {
        builder.ToTable("PurchaseReceiptItems");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(x => x.DiscountAmount).HasColumnType("decimal(18,2)");

        builder.HasOne(x => x.Product)
               .WithMany()
               .HasForeignKey(x => x.ProductId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}