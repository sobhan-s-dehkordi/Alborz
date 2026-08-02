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
        builder.Property(x => x.TotalDiscount).HasColumnType("decimal(18,2)");

        builder.Property(x => x.ReferenceNumber)
               .HasMaxLength(50);

        builder.Property(x => x.ReceiptDate)
               .IsRequired();

        builder.Property(x => x.AdditionalCharges)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Remarks)
            .HasMaxLength(1000).IsRequired(false);

        builder.HasOne(x => x.Party)
               .WithMany()
               .HasForeignKey(x => x.PartyId)
               .OnDelete(DeleteBehavior.Restrict);

        var navigation = builder.Metadata.FindNavigation(nameof(PurchaseReceipt.Items));
        navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Items)
               .WithOne()
               .HasForeignKey(x => x.PurchaseReceiptId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
