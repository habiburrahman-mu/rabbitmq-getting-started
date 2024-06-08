using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "hashingExchange", type: "x-consistent-hash", autoDelete: true);
channel.QueueDeclare(queue: "letterbox1");
channel.QueueDeclare(queue: "letterbox2");

channel.QueueBind("letterbox1", "hashingExchange", "25");
channel.QueueBind("letterbox2", "hashingExchange", "75");

Console.WriteLine(" [*] Waiting for message");
var consumer1 = new EventingBasicConsumer(channel);
var consumer2 = new EventingBasicConsumer(channel);

var random = new Random();

consumer1.Received += OnReceivedMessage1;
consumer2.Received += OnReceivedMessage2;

channel.BasicConsume(queue: "letterbox1", autoAck: false, consumer: consumer1);
channel.BasicConsume(queue: "letterbox2", autoAck: false, consumer: consumer2);
Console.WriteLine("Consuming");
Console.ReadKey();


void OnReceivedMessage1(object? sender, BasicDeliverEventArgs ea)
{
    var processingTime = random.Next(1, 6);
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message} from queue 1");
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
}

void OnReceivedMessage2(object? sender, BasicDeliverEventArgs ea)
{
    var processingTime = random.Next(1, 6);
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message} from queue 2");
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
}