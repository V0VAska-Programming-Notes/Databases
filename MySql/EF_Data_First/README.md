# EF Data First

**Important:**

- It is very important to dispose the DbContext after use. This ensures both that any unmanaged resources are freed, and that any events or other hooks are unregistered so as to prevent memory leaks in case the instance remains referenced.
- DbContext is not thread-safe. Do not share contexts between threads. Make sure to await all async calls before continuing to use the context instance.
- An InvalidOperationException thrown by EF Core code can put the context into an unrecoverable state. Such exceptions indicate a program error and are not designed to be recovered from.

На всякий случай пусть будут установлены совместимые с Помелом, авось пригодятся:

```
dotnet tool uninstall --global dotnet-aspnet-codegenerator
dotnet tool install --global dotnet-aspnet-codegenerator --version 6.0.10
dotnet tool uninstall --global dotnet-ef
dotnet tool install --global dotnet-ef
```

`dotnet-ef` точно в данном примере пригодится.

## Создаём тестовую БД

Для полной кросс-платформенности пользуемся общедоступной в любой ОС mysql-утилитой:

```
./mysql -h ub2204-srv-dev.basyambia.home -u vovaska -p
```

Вводим пароль и создаём БД:

```
mysql> create database to_scaffold;
mysql> use to_scaffold;
mysql> create table `User` (
-> `User_ID` int not null auto_increment,
-> `Name` char(30) not null,
-> primary key (`User_ID`));
mysql> describe User;
+---------+----------+------+-----+---------+----------------+
| Field   | Type     | Null | Key | Default | Extra          |
+---------+----------+------+-----+---------+----------------+
| User_ID | int      | NO   | PRI | NULL    | auto_increment |
| Name    | char(30) | NO   |     | NULL    |                |
+---------+----------+------+-----+---------+----------------+
2 rows in set (0.00 sec)
```

Можно и заполнить несколькими записями:

```
mysql> insert into User (Name) value('user 1');
mysql> insert into User (Name) value('user 2');
mysql> exit
```

Наша консолька:

```
dotnet new console -o EF_Data_First -f net6.0
code -r EF_Data_First
```

Приводим Program.cs к обычному виду:

```c#
internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }
}
```

Ставим наше Pomelo и, в дополнение к уже установленному dotnet-ef, Microsoft.EntityFrameworkCore.Design:

```
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 6.0.2
dotnet add package Microsoft.EntityFrameworkCore.Design --version 6.0.11
dotnet restore
dotnet build
```

Подробную информацию о параметрах 'реверса' можно посмотреть в [мелкософтовских доках](https://learn.microsoft.com/en-us/ef/core/managing-schemas/scaffolding/?tabs=dotnet-core-cli)

Запускаем:

```
dotnet ef dbcontext scaffold "server=ub2204-srv-dev.basyambia.home;user=vovaska;password=passw0rd!;database=to_scaffold" Pomelo.EntityFrameworkCore.MySql -c EFDataFirstContext --context-dir Data -o Models -f
```

Дальнейшие пояснения излишни... Окромя естественного - ежели после внесения изменений ОПЯТЬ 'реверснуть', то изменения похерятся.
