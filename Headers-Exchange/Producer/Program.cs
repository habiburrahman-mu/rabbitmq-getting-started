using System;
using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "headersExchange", type: ExchangeType.Headers);

var properties = channel.CreateBasicProperties();
properties.Headers = new Dictionary<string, object>
{
    {"name", "test"}
};

var random = new Random();
var messageId = 1;

while (true)
{
    var publishingTime = random.Next(1, 4);
    var message = $"Sending MessageId: {messageId} through header exchange";
    var body = Encoding.UTF8.GetBytes(message);
    channel.BasicPublish(exchange: "headersExchange", routingKey: "", properties, body: body);
    Console.WriteLine($"Message {message} published.");
    Task.Delay(TimeSpan.FromSeconds(publishingTime)).Wait();
    messageId += 1;
}