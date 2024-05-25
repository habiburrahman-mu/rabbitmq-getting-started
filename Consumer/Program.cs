﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "letterbox",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

Console.WriteLine(" [*] Waiting for message");
var consumer = new EventingBasicConsumer(channel);

var random = new Random();

consumer.Received += OnReceivedMessage;

channel.BasicConsume(queue: "letterbox", autoAck: false, consumer: consumer);
Console.WriteLine("Consuming");
Console.ReadKey();


void OnReceivedMessage(object? sender, BasicDeliverEventArgs ea)
{
    var processingTime = random.Next(1, 6);
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message}");
    Task.Delay(TimeSpan.FromSeconds(processingTime)).Wait();
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
}