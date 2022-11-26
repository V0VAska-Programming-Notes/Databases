# EF Code First

На всякий случай пусть будут установлены совместимые с Помелом, авось пригодятся:
```
dotnet tool uninstall --global dotnet-aspnet-codegenerator
dotnet tool install --global dotnet-aspnet-codegenerator --version 6.0.10
dotnet tool uninstall --global dotnet-ef
dotnet tool install --global dotnet-ef
```

## Начало

Создаём консольное приложение:

```
dotnet new console -o EF_Code_First -f net6.0
```

Data/MyRecord.cs

```c#
namespace EF_Code_First.Data;

public class MyRecord
{
    public int ID { get; set; }
    public string Message { get; set; } = default!;
}
```

Устанавливаем пакет с Помело:

```
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 6.0.2
```

Data/MyContext.cs

```c#
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
```

Program.cs приведём к привычному виду:

```c#
using EF_Code_First.Data;
internal class Program
{
    private static void Main(string[] args)
    {
        Initialize();
        PrintData();
    }

    private static void Initialize()
    {
        using (var context = new MyContext())
        {
            context.Database.EnsureCreated();

            if (context.MyRecords.Any())
            {
                return;
            }

            var item1 = new MyRecord
            {
                Message = "One"
            };
            var item2 = new MyRecord
            {
                Message = "Two"
            };

            var items = new MyRecord[]
            { item1, item2 };

            context.AddRange(items);

            context.SaveChanges();
        }
    }

    private static void PrintData()
    {
        using (var context = new MyContext())
        {
            foreach (var item in context.MyRecords)
            {
                Console.WriteLine(item.Message);
            }
        }
    }
}
```

Запущаем 'dotnet run'.
