using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare("request-queue", exclusive: false);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += Consumer_Received;

void Consumer_Received(object? sender, BasicDeliverEventArgs e)
{
    Console.WriteLine($"Received Request: {e.BasicProperties.CorrelationId}");

    var replyMessage = $"This is your reply for {e.BasicProperties.CorrelationId}";
    Task.Delay(2000).Wait();
    var body = Encoding.UTF8.GetBytes(replyMessage);
    channel.BasicPublish("", e.BasicProperties.ReplyTo, null, body);
}

channel.BasicConsume(queue: "request-queue", autoAck: true, consumer: consumer);

Console.ReadLine();