using EF_Code_First.Data;
using Microsoft.EntityFrameworkCore;

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
            //context.Database.EnsureCreated();

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

public class MyContextFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<MyContext>
{
    public MyContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MyContext>();
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));
        optionsBuilder.UseMySql("server=ub2204-srv-dev.basyambia.home;user=vovaska;password=passw0rd!;database=ef_code_first", serverVersion);

        return new MyContext(optionsBuilder.Options);
    }
}
