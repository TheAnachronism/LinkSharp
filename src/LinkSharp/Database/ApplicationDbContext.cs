using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LinkSharp.Database;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<ApplicationUser>()
            .OwnsMany<ApplicationUserCollectionAccess>(user => user.Collections)
            .WithOwner(x => x.ApplicationUser);

        builder.Entity<Collection>()
            .OwnsMany<ApplicationUserCollectionAccess>(collection => collection.UserAccesses)
            .WithOwner(x => x.Collection);
    }

    public DbSet<Collection> Collections { get; set; }
    public DbSet<ApplicationUserCollectionAccess> ApplicationUserCollectionAccesses { get; set; }
    public DbSet<Link> Links { get; set; }
    public DbSet<Tag> Tags { get; set; }
}