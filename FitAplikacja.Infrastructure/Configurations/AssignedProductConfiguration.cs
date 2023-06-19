using FitAplikacja.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitAplikacja.Infrastructure.Configurations
{
    public class AssignedProductConfiguration : IEntityTypeConfiguration<AssignedProduct>
    {
        public void Configure(EntityTypeBuilder<AssignedProduct> builder)
        {
            builder.Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Property(p => p.Added).IsRequired();
            builder.Property(p => p.Count).HasDefaultValue(1);
        }
    }
}
