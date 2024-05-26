using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(
    exchange: "pubsub",
    type: ExchangeType.Fanout);

var queueName = channel.QueueDeclare().QueueName;

Console.WriteLine(" [*] Waiting for message");
var consumer = new EventingBasicConsumer(channel);

channel.QueueBind(queueName, exchange: "pubsub", routingKey: "");

consumer.Received += OnReceivedMessage;

channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

Console.ReadKey();

void OnReceivedMessage(object? sender, BasicDeliverEventArgs ea)
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Second Consumer Received {message}");
}