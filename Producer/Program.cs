using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

var replyQueue = channel.QueueDeclare(queue: "", exclusive: true);
channel.QueueDeclare("request-queue", exclusive: false);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += Consumer_Received;

void Consumer_Received(object? sender, BasicDeliverEventArgs e)
{
    var body = e.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Reply received: {message}");
}

channel.BasicConsume(queue: replyQueue.QueueName, autoAck: true, consumer: consumer);

var message = "Can I request a reply";
var body = Encoding.UTF8.GetBytes(message);

var properties = channel.CreateBasicProperties();
properties.ReplyTo = replyQueue.QueueName;
properties.CorrelationId = Guid.NewGuid().ToString();

channel.BasicPublish("", "request-queue", properties, body);

Console.WriteLine($"Sending Request: {properties.CorrelationId}");

Console.WriteLine("Started Client");
Console.ReadKey();