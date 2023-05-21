##Инструкция для создания и накатывания миграций 

для создания миграции для WpfAppDbContext:  
```
...\WpfApp\WpfApp.UI> dotnet ef migrations add "MigrationName" --context WpfAppDbContext --project ..\WpfApp.DataAccess

```  

или через Package Manager Console (выбираем WpfApp.UI в качестве запускаемого проекта):
```
Add-Migration -Name "MigationName" -Context WpfAppDbContext -Project WpfApp.DataAccess
```
  
для ручного применения новых миграций WpfAppDbContext:
```
...\WpfApp\WpfApp.UI>  dotnet ef database update --context WpfAppDbContext
```
или через Package Manager Console (выбираем WpfApp.UI в качестве запускаемого проекта):
```
Update-Database -Context WpfAppDbContext -Project WpfApp.DataAccess
```

