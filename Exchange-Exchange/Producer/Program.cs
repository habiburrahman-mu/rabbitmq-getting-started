using System;
using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "firstExchange", type: ExchangeType.Direct);
channel.ExchangeDeclare(exchange: "secondExchange", type: ExchangeType.Direct);

channel.ExchangeBind("secondExchange", "firstExchange", "");

var random = new Random();
var messageId = 1;

while (true)
{
    var publishingTime = random.Next(1, 4);
    var message = $"Sending MessageId: {messageId}";
    var body = Encoding.UTF8.GetBytes(message);
    channel.BasicPublish(exchange: "firstExchange", routingKey: "", body: body);
    Console.WriteLine($"Message {message} published.");
    Task.Delay(TimeSpan.FromSeconds(publishingTime)).Wait();
    messageId += 1;
}