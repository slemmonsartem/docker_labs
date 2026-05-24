using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapHealthChecks("/health");

app.MapGet("/notify", () =>
{
    return Results.Ok(new
    {
        Message = "Notification service is working"
    });
});

Task.Run(() =>
{
    Thread.Sleep(10000);

    var factory = new ConnectionFactory
    {
        HostName = "rabbitmq",
        UserName = "guest",
        Password = "guest"
    };

    var connection = factory.CreateConnection();
    var channel = connection.CreateModel();

    channel.QueueDeclare(
        queue: "orders",
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: null
    );

    var consumer = new EventingBasicConsumer(channel);

    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        Console.WriteLine($"Notification received order: {message}");
    };

    channel.BasicConsume(
        queue: "orders",
        autoAck: true,
        consumer: consumer
    );
});

app.Run();