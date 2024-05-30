using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

const string ExchangeName = "routing-topic-exchange";

channel.ExchangeDeclare(
    exchange: ExchangeName,
    type: ExchangeType.Topic);

var queueName = channel.QueueDeclare().QueueName;

channel.QueueBind(
    queue: queueName,
    exchange: ExchangeName,
    routingKey: "*.europe.*"); // any message with <any-word>.europe.<anyword>

var consumer = new EventingBasicConsumer(channel);

consumer.Received += OnReceivedMessage;

channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

Console.ReadKey();

void OnReceivedMessage(object? sender, BasicDeliverEventArgs ea)
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Analytic Consumer Received {message}");
}