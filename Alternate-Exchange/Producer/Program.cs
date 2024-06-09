using System;
using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(
    exchange: "alternateExchange", 
    type: ExchangeType.Fanout);

var arguments = new Dictionary<string, object>
{
    { "alternate-exchange", "alternateExchange"}
};

channel.ExchangeDeclare(
    exchange: "mainExchange", 
    type: ExchangeType.Direct,
    arguments: arguments);

var random = new Random();
var messageId = 1;

while (true)
{
    var publishingTime = random.Next(1, 4);
    var message = $"Sending MessageId: {messageId} through exchange";
    var body = Encoding.UTF8.GetBytes(message);
    var routingKey = messageId % 3 == 0 ? "main" : "other";
    channel.BasicPublish(exchange: "mainExchange", routingKey: routingKey, body: body);
    Console.WriteLine($"Message {message} published.");
    Task.Delay(TimeSpan.FromSeconds(publishingTime)).Wait();
    messageId += 1;
}