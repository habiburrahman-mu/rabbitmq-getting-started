using System;
using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();


const string ExchangeName = "routing-topic-exchange";

channel.ExchangeDeclare(
    exchange: ExchangeName,
    type: ExchangeType.Topic);

var id = 0;

while (true)
{
    var userPaymentsMessage = $"An European user paid something with {id}";

    var userPaymentsMessageBody = Encoding.UTF8.GetBytes(userPaymentsMessage);
    channel.BasicPublish(exchange: ExchangeName, routingKey: "user.europe.payments", body: userPaymentsMessageBody);
    Console.WriteLine($"Published: {userPaymentsMessage}");

    var businessOrderMessage = $"An European business ordered goods with {id}";

    var businessOrderMessageBody = Encoding.UTF8.GetBytes(businessOrderMessage);
    channel.BasicPublish(exchange: ExchangeName, routingKey: "business.europe.order", body: businessOrderMessageBody);
    Console.WriteLine($"Published: {businessOrderMessage}");

    id++;
    Task.Delay(2000).Wait();
}