using FitAplikacja.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitAplikacja.Infrastructure.Configurations
{
    public class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
    {
        public void Configure(EntityTypeBuilder<Workout> builder)
        {
            builder.Property(w => w.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Property(w => w.Name).HasMaxLength(100);
            builder.HasMany(w => w.Exercises).WithMany(e => e.Workouts);
        }
    }
}
