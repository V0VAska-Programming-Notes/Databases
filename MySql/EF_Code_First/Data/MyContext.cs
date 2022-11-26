using Microsoft.EntityFrameworkCore;

namespace EF_Code_First.Data;

public class MyContext : DbContext
{
    public MyContext() : base ()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));
        optionsBuilder.UseMySql("server=ub2204-srv-dev.basyambia.home;user=vovaska;password=passw0rd!;database=ef_code_first", serverVersion);
    }

    public DbSet<MyRecord> MyRecords { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MyRecord>(entity =>
        {
            entity.Property(e => e.Message).IsRequired();
            entity.ToTable(nameof(MyRecord));
        });
    }
}
