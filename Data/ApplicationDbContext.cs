using Assessment.Models;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostTag> PostTags { get; set; }    
    public DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //configure PostTag as a composite key
        modelBuilder.Entity<PostTag>()
            .HasKey(pt => new { pt.PostId, pt.TagId});

        // configure User-UserProfile one to one relationship
        modelBuilder.Entity<User>()
            .HasOne(u => u.UserProfile)
            .WithOne(up => up.User)
            .HasForeignKey<UserProfile>(up => up.UserId);
    }
}