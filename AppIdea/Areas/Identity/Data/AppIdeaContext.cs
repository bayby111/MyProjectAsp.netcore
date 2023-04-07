using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppIdea.Areas.Identity.Data;

public class AppIdeaContext : IdentityDbContext<AppIdeaUser>
{
    public AppIdeaContext(DbContextOptions<AppIdeaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppIdeaUser> Users { get; set; } = null!;
    //public virtual DbSet<AppIdeaRole> Roles { get; set; } = null!;
    public virtual DbSet<Department> Departments { get; set; } = null!;
    public virtual DbSet<Category> Categories { get; set; } = null!;
    public virtual DbSet<Idea> Ideas { get; set; } = null!;
    public virtual DbSet<Topic> Topics { get; set; } = null!;
    public virtual DbSet<React> Reacts { get; set; } = null!;
    public virtual DbSet<View> Views { get; set; }
    public virtual DbSet<Comment> Comments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        builder.ApplyConfiguration(new AppIdeaEnityConfiguration());
    }


}

public class AppIdeaEnityConfiguration : IEntityTypeConfiguration<AppIdeaUser>
{
    public void Configure(EntityTypeBuilder<AppIdeaUser> builder)
    {
        builder.Property(u => u.Firstname).HasMaxLength(255);
    }
}
