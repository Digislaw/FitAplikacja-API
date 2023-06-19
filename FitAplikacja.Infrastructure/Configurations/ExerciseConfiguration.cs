using FitAplikacja.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitAplikacja.Infrastructure.Configurations
{
    public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
    {
        public void Configure(EntityTypeBuilder<Exercise> builder)
        {
            builder.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Property(e => e.Name).IsRequired().HasMaxLength(50);
            builder.Property(e => e.BurnedCalories).IsRequired();
            builder.Property(e => e.IsWeightTraining).HasDefaultValue(false);
            builder.Property(e => e.Difficulty).HasDefaultValue((byte)0);
            builder.Property(e => e.Repetition).HasDefaultValue((byte)0);
            builder.Property(e => e.Weight).HasDefaultValue(0);
            builder.Property(e => e.VideoURL).HasMaxLength(60);
            builder.Property(e => e.Description).HasMaxLength(300);
            builder.Property(e => e.BodyPart).HasMaxLength(50);
        }
    }
}
