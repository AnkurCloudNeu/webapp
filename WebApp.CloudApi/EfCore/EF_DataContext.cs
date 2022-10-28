using WebApp.CloudApi.Model;
using Microsoft.EntityFrameworkCore;

namespace WebApp.CloudApi.EfCore;

public class EF_DataContext : DbContext
{
    public virtual DbSet<Account> Accounts { get; set; }
    public virtual DbSet<Document> Documents { get; set; }

    public EF_DataContext(DbContextOptions<EF_DataContext> options)
        : base(options)
    {
    }
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     // optionsBuilder.UseNpgsql(@"Host=" +
    //     // GlobalData.Application.Where(s => s.Key == "Database").FirstOrDefault().Value +
    //     // ";Database=" + GlobalData.Application.Where(s => s.Key == "DatabaseName").FirstOrDefault().Value +
    //     // ";Port=" + GlobalData.Application.Where(s => s.Key == "DatabasePort").FirstOrDefault().Value +
    //     // ";Username=" + GlobalData.Application.Where(s => s.Key == "MasterUsername").FirstOrDefault().Value + ";Password=" +
    //     // GlobalData.Application.Where(s => s.Key == "MasterPassword").FirstOrDefault().Value);
        
    //      optionsBuilder.UseNpgsql(@"Host=testdb1.cbd0o3qojchd.us-east-1.rds.amazonaws.com;Database=postgrestest;Port=5432;Username=postgres;Password=postgres;");
    // }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.UseSerialColumns();
        modelBuilder.Entity<Document>()
            .HasOne(p => p.Account)
            .WithMany(b => b.Documents);
    }

}