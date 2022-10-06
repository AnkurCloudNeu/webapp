using CloudApi.Model;
using Microsoft.EntityFrameworkCore;

namespace CloudApi.EfCore;

public class EF_DataContext: DbContext {
    
    public EF_DataContext(DbContextOptions<EF_DataContext> options): base(options)
    {
        
    }

    // public override void OnModelCreating(ModelBuilder modelBuilder) {
    //     modelBuilder.UseSerialColumns();
    // }

    public DbSet<Account> Accounts { get; set; }
}