var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () =>
{
    string currentTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

    Directory.CreateDirectory("logs");

    File.AppendAllText(
        "logs/log.txt",
        $"Запрос: {currentTime}{Environment.NewLine}"
    );

    return $"Текущее время: {currentTime}";
});

app.Run();