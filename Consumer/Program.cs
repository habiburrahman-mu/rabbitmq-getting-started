using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "letterbox",
    durable: false,
    exclusive: false, autoDelete: false, arguments: null);
Console.WriteLine(" [*] Waiting for message");
var consumer = new EventingBasicConsumer(channel);

consumer.Received += OnReceivedMessage;

channel.BasicConsume(queue: "letterbox", autoAck: true, consumer: consumer);
Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

void OnReceivedMessage(object? sender, BasicDeliverEventArgs ea)
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message}");
}