# Pomelo

По мотивам https://learn.microsoft.com/en-us/aspnet/core/tutorials/razor-pages/?view=aspnetcore-6.0

Можно и в VS 2022 извращаться, только все действия типа скаффолда выполнять из терминала. Преимущество VS2022 только в удобстве при подборе версий нугетов на совместимось по сравнению с Code "из коробки".

Зачинаем как обычно под net6.0:

```
dotnet new webapp -o PomeloTest -f net6.0
code -r PomeloTest
```

Добавляем 'Models/Movie.cs'

```c#
using System.ComponentModel.DataAnnotations;

namespace RazorPagesMovie.Models
{
    public class Movie
    {
        public int ID { get; set; }
        public string Title { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        public string Genre { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
```

В VS Code ловим глюк с жалобами на имя класса Movie. Перезагружаем проектик и всё будет пучком.

В терминале стучим:

```
dotnet tool uninstall --global dotnet-aspnet-codegenerator
dotnet tool install --global dotnet-aspnet-codegenerator
dotnet tool uninstall --global dotnet-ef
dotnet tool install --global dotnet-ef
```

> Пока и самые свежие (седьмые) версии под 6.0 встают. Если что-то проапгрейдят и полезут ошибки, то под net6.0 хоть как оставят `dotnet tool install --global dotnet-aspnet-codegenerator --version 6.0.10` и `dotnet tool install --global dotnet-ef --version 6.0.11`

А вот дальше точно разброс. Прёмся в [нугетнятину](https://www.nuget.org/). Интересующий нас Pomelo.EntityFrameworkCore.MySql в стабильной версии 6.0.2, забираем:

```
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 6.0.2
```

И запоминаем зависимости:

```
net6.0
Microsoft.EntityFrameworkCore.Relational (>= 6.0.7 && < 7.0.0)
Microsoft.Extensions.DependencyInjection (>= 6.0.0)
MySqlConnector (>= 2.1.2)
```

Ищем Microsoft.EntityFrameworkCore.Design. Последняя версия точно нам воткнёт  с собой Microsoft.EntityFrameworkCore.Relational версии >= 7.0.0, а у нас ограничение в Помеле. Поэтому грузим:

```
dotnet add package Microsoft.EntityFrameworkCore.Design --version 6.0.11
```

Далее по списку, опять же после проверки:

```
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design --version 6.0.10
```

Ну, и напоследок:

```
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 6.0.11
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 6.0.11
```

Проверимся:

```
dotnet restore
```

Генерим странички:

```
dotnet aspnet-codegenerator razorpage -m Movie -dc RazorPagesMovieContext -udl -outDir Pages/Movies --referenceScriptLibraries -sqlite
```

> При использовании кода из репозитария данную команду выполнять не надоть - странички уже сгенерены.

<details><summary>Примечание</summary>


Фактически нам под net6.0 нужны лишь помимо "глобальных":
- Pomelo.EntityFrameworkCore.MySql --version 6.0.2
- Microsoft.EntityFrameworkCore.Design --version 6.0.11
- Microsoft.VisualStudio.Web.CodeGeneration.Design --version 6.0.10
- Microsoft.EntityFrameworkCore.SqlServer --version 6.0.11

Ведь пользоваться sqlite не планируем. Из самой команды генерации страниц выкинем упоминание о sqlite и на всякий вонючий добавим указание на требуемую версию net:

```
dotnet aspnet-codegenerator razorpage -m Movie -dc RazorPagesMovieContext -udl -outDir Pages/Movies --referenceScriptLibraries -tfm net6.0
```

</details>

Всё прошло штатно и свои Pages получили. Осталось настроить соединение с РЕАЛЬНОЙ бд. Поэтому прёмся в appsettings.json и добавляем RazorPagesMovieContext_MySql:

```
  "ConnectionStrings": {
    "RazorPagesMovieContext": "Data Source=.db",
    "RazorPagesMovieContext_MySql": "server=localhost;user=root;password=password;database=rpMovie"
  }
```

В Program.cs подшаманиваем:

```
//builder.Services.AddDbContext<RazorPagesMovieContext>(options =>
//    options.UseSqlite(builder.Configuration.GetConnectionString("RazorPagesMovieContext") ?? throw new InvalidOperationException("Connection string 'RazorPagesMovieContext' not found.")));
var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));
builder.Services.AddDbContext<RazorPagesMovieContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("RazorPagesMovieContext_MySql") ?? throw new InvalidOperationException("Connection string 'RazorPagesMovieContext' not found."), serverVersion));
```

Компилируемся или строимся... относительно NET я так и не вкуриваю строгой точности терминов - ведь получаем ни рыбу, ни мясо - какой-то полуисполняемый код.

```
dotnet build
```

Ошибок нет? Зашибись! Осталось:

```
dotnet ef migrations add InitialCreate
dotnet ef database update
```

> Если используется готовый код из репозитария, то выполнять `dotnet ef migrations add *` не надо. Всё, что следует сделать, так это лишь проупдатить базу данных.

Мона запущать dotnet run. В браузере идём 'https://localhost:7296' - порт может быть и другой (но в терминале будут ссылки на http и https соединения. которые можно активировать ctrl + клик мышером. Добавляем в адресной строке '/Movies'. Вуаля, мы работаем с MySql.

