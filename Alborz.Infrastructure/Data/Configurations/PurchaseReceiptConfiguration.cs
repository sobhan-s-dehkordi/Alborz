using Alborz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alborz.Infrastructure.Data.Configurations;

public class PurchaseReceiptConfiguration : IEntityTypeConfiguration<PurchaseReceipt>
{
    public void Configure(EntityTypeBuilder<PurchaseReceipt> builder)
    {
        builder.ToTable("PurchaseReceipts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.SupplierName).HasMaxLength(200);

        var navigation = builder.Metadata.FindNavigation(nameof(PurchaseReceipt.Items));
        navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Items)
               .WithOne()
               .HasForeignKey(x => x.PurchaseReceiptId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
