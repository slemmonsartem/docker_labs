using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var redis = ConnectionMultiplexer.Connect("redis:6379");
builder.Services.AddSingleton(redis.GetDatabase());

builder.Services.AddSingleton<IConnection>(_ =>
{
    var factory = new ConnectionFactory
    {
        HostName = "rabbitmq",
        UserName = "guest",
        Password = "guest"
    };

    return factory.CreateConnection();
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapHealthChecks("/health");

app.MapPost("/orders", async (Order order, IDatabase redis, IConnection rabbitConnection) =>
{
    var orderId = Guid.NewGuid().ToString();

    await redis.StringSetAsync(
        $"order:{orderId}",
        JsonSerializer.Serialize(order)
    );

    using var channel = rabbitConnection.CreateModel();

    channel.QueueDeclare(
        queue: "orders",
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: null
    );

    var message = JsonSerializer.Serialize(new
    {
        OrderId = orderId,
        order.ProductName,
        order.Quantity
    });

    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(
        exchange: "",
        routingKey: "orders",
        basicProperties: null,
        body: body
    );

    return Results.Ok(new
    {
        OrderId = orderId,
        Status = "Created and sent to RabbitMQ"
    });
});

app.Run();

record Order(string ProductName, int Quantity);