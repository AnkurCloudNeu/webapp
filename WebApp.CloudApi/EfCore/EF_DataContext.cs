using WebApp.CloudApi.Model;
using Microsoft.EntityFrameworkCore;

namespace WebApp.CloudApi.EfCore;

public class EF_DataContext: DbContext {
    
    public EF_DataContext(DbContextOptions<EF_DataContext> options): base(options)
    {
        
    }

    // public override void OnModelCreating(ModelBuilder modelBuilder) {
    //     modelBuilder.UseSerialColumns();
    // }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>()
            .HasOne(p => p.Account)
            .WithMany(b => b.Documents);
    }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Document> Documents => Set<Document>();
}