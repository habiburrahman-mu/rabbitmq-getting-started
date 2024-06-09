using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

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


channel.QueueDeclare(queue: "alternateExchangeQueue");
channel.QueueBind("alternateExchangeQueue", "alternateExchange", "");

channel.QueueDeclare(queue: "mainQueue");
channel.QueueBind("mainQueue", "mainExchange", "main");

Console.WriteLine(" [*] Waiting for message");
var mainConsumer = new EventingBasicConsumer(channel);
var alternateConsumer = new EventingBasicConsumer(channel);

var random = new Random();

mainConsumer.Received += OnReceivedMessage;
alternateConsumer.Received += OnReceivedMessageAlternate;

channel.BasicConsume(queue: "mainQueue", autoAck: false, consumer: mainConsumer);
channel.BasicConsume(queue: "alternateExchangeQueue", autoAck: false, consumer: alternateConsumer);
Console.WriteLine("Consuming");
Console.ReadKey();


void OnReceivedMessage(object? sender, BasicDeliverEventArgs ea)
{
    var processingTime = random.Next(1, 6);
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message}");
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
}
void OnReceivedMessageAlternate(object? sender, BasicDeliverEventArgs ea)
{
    var processingTime = random.Next(1, 6);
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Alternate Received {message}");
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
}