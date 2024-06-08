using System;
using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "hashingExchange", type: "x-consistent-hash", autoDelete: true);

var random = new Random();
var messageId = 1;

while (true)
{
    var routingKey = random.Next(1, 10).ToString();
    var publishingTime = random.Next(1, 4);
    var message = $"Sending MessageId: {messageId} through hashing exchange with key {routingKey}";
    var body = Encoding.UTF8.GetBytes(message);
    channel.BasicPublish(exchange: "hashingExchange", routingKey: routingKey, basicProperties:null, body: body);
    Console.WriteLine($"Message {message} published.");
    Task.Delay(TimeSpan.FromSeconds(publishingTime)).Wait();
    messageId += 1;
}