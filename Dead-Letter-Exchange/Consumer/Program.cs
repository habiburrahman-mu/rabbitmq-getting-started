using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(
    exchange: "dead-letter-exchange",
    type: ExchangeType.Fanout);

channel.ExchangeDeclare(
    exchange: "mainExchange",
    type: ExchangeType.Direct);

var arguments = new Dictionary<string, object>
{
    { "x-dead-letter-exchange", "dead-letter-exchange"},
    { "x-message-ttl", 1000} // any message which are in queue more than 1 second goes to dead letter exchange
};
channel.QueueDeclare(queue: "mainQueue", arguments: arguments);
channel.QueueBind("mainQueue", "mainExchange", "main");

channel.QueueDeclare(queue: "dead-letter-exchangeQueue");
channel.QueueBind("dead-letter-exchangeQueue", "dead-letter-exchange", "");


Console.WriteLine(" [*] Waiting for message");
var mainConsumer = new EventingBasicConsumer(channel);
var deadLetterConsumer = new EventingBasicConsumer(channel);

var random = new Random();

mainConsumer.Received += OnReceivedMessage;
deadLetterConsumer.Received += OnReceivedMessageDeadLetter;

//channel.BasicConsume(queue: "mainQueue", autoAck: false, consumer: mainConsumer);
channel.BasicConsume(queue: "dead-letter-exchangeQueue", autoAck: false, consumer: deadLetterConsumer);
Console.WriteLine("Consuming");
Console.ReadKey();


void OnReceivedMessage(object? sender, BasicDeliverEventArgs ea)
{
    var processingTime = random.Next(1000, 6000);
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message}");
    Task.Delay(processingTime).Wait();
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
}
void OnReceivedMessageDeadLetter(object? sender, BasicDeliverEventArgs ea)
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Dead Letter Received {message}");
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
}