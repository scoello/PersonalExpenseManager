using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Domain;
namespace TaskManagement.Api.Data;
public sealed class TaskDbContext(DbContextOptions<TaskDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<User> Users => Set<User>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        var task = builder.Entity<TaskItem>();
        task.ToTable("Tasks"); task.HasKey(x => x.Id);
        task.Property(x => x.Title).HasMaxLength(200).IsRequired();
        task.Property(x => x.Description).HasColumnType("nvarchar(max)");
        task.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
        task.Property(x => x.DueDate).HasColumnType("datetimeoffset");
        task.Property(x => x.CreatedAt).HasColumnType("datetimeoffset");
        task.Property(x => x.UpdatedAt).HasColumnType("datetimeoffset");
        task.HasIndex(x => new { x.UserId, x.Status });
        task.HasIndex(x => new { x.UserId, x.DueDate });
        task.HasOne(x => x.User).WithMany(x => x.Tasks).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<User>().ToTable("Users").HasKey(x => x.Id);
    }
}
