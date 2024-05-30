using System;
using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();


const string ExchangeName = "routing-topic-exchange";

channel.ExchangeDeclare(
    exchange: ExchangeName,
    type: ExchangeType.Topic);

var id = 0;

while (true)
{
    var message = $"Hello I want to broadcast this message with {id}";

    var body = Encoding.UTF8.GetBytes(message);
    channel.BasicPublish(exchange: "routing-exchange", routingKey: "both", body: body);
    Console.WriteLine($"Published: {message}");
    id++;
    Task.Delay(2000).Wait();
}