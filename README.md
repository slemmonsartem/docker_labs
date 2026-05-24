**Лаб №1**

**1. Почему для консоли runtime, а для web aspnet?**
Для консольного приложения достаточно обычного .NET Runtime. Для web-приложения нужен образ aspnet, потому что в нём есть компоненты ASP.NET Core для запуска сайта.

**2. Зачем нужен multi-stage build?**
Чтобы сначала собрать приложение в SDK образе, а потом перенести только готовые файлы в маленький runtime-образ. Так итоговый Docker-образ получается меньше и чище.

**3. Как передать переменную окружения через docker run?**
docker run -d -p 5000:5000 -e ASPNETCORE_URLS=http://+:5000 web-dotnet

запуск самостоятельного задания
```
docker run -d `
  -p 5000:5000 `
  -e ASPNETCORE_URLS=http://+:5000 `
  -v ${PWD}\logs:/app/logs `
  --name myweb `
  web-dotnet
```

**Лаб №2**
**1. Как в Docker Compose передать строку подключения к БД?**

Через секцию environment в docker-compose.yml:

```
services:
  api:
    environment:
      ConnectionStrings__DefaultConnection: "Host=db;Port=5432;Database=myapidb;Username=postgres;Password=postg
```

**2. Зачем нужен volume для PostgreSQL?**

Volume сохраняет данные базы данных вне контейнера.

Без volume:
контейнер удалили -> база данных пропала.

С volume:
контейнер удалили и создали заново -> данные остались.

**3. Как выполнить команду внутри контейнера (docker exec)?**
docker exec -it имя_контейнера команда
