using Alborz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alborz.Infrastructure.Data.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.TotalAmount).HasColumnType("decimal(18,2)");
        builder.Property(i => i.DiscountAmount).HasColumnType("decimal(18,2)");

        builder.HasOne(i => i.Customer)
               .WithMany()
               .HasForeignKey(i => i.CustomerId)
               .OnDelete(DeleteBehavior.SetNull);

        var navigation = builder.Metadata.FindNavigation(nameof(Invoice.Items));
        navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(i => i.Items)
               .WithOne()
               .HasForeignKey(i => i.InvoiceId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
