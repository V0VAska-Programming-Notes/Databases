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
