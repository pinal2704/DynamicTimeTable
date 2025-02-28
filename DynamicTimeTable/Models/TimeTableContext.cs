using DynamicTimeTable.Models;
using Microsoft.EntityFrameworkCore;

public class TimeTableContext : DbContext
{
    public TimeTableContext(DbContextOptions<TimeTableContext> options)
            : base(options)
    {
    }

    public DbSet<TimetableConfiguration> TimetableConfigurations { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<TimetableData> TimetableDatas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure one-to-many relationship between TimetableConfiguration and Subject
        modelBuilder.Entity<TimetableConfiguration>()
            .HasMany(t => t.Subjects)
            .WithOne(s => s.TimetableConfiguration)
            .HasForeignKey(s => s.TimetableConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure one-to-one relationship between TimetableConfiguration and TimetableData
        modelBuilder.Entity<TimetableConfiguration>()
            .HasOne(t => t.TimetableData)
            .WithOne(td => td.TimetableConfiguration)
            .HasForeignKey<TimetableData>(td => td.TimetableConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}