using FitAplikacja.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitAplikacja.Infrastructure.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasOne(u => u.RefreshToken)
                .WithOne(t => t.User)
                .HasForeignKey<RefreshToken>(t => t.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Workouts)
                .WithOne(w => w.User)
                .HasForeignKey(w => w.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            /*
            builder.HasMany(u => u.AssignedProduct)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);
            */
        }
    }
}
