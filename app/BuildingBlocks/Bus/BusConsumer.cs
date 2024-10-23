using System.Text;
using System.Text.Json;
using OtusKdeBus.Model;
using OtusKdeBus.Model.Client;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OtusKdeBus;

public class BusConsumer : IBusConsumer
{
    private IModel _channel;

    public BusConsumer()
    {
        /**
         * TODO:: get other
         */

        var isProduction = Environment.GetEnvironmentVariable("DB_PASSWORD") is not null;
        var HostName = isProduction
            ? "rabbit-rabbitmq.default.svc.cluster.local"
            : "localhost";
        Console.WriteLine($"Try connect to {HostName}...");

        var factory = new ConnectionFactory()
            { HostName = HostName, VirtualHost = "otus", Port = 5672, UserName = "admin", Password = "sEcret" };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
    }

    private void Consume<T>(string queue_name, MessageType type, Action<T> fn)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var res = JsonSerializer.Deserialize<T>(message);
            fn(res);
        };
        _channel.QueueDeclare(queue: $"Queue_{type}.{queue_name}", durable: true, exclusive: false, autoDelete: false,
            arguments: null);
        _channel.QueueBind(queue: $"Queue_{type}.{queue_name}", exchange: "user_exchange",
            routingKey: $"Routing_key_{type}");

        _channel.BasicConsume($"Queue_{type}.{queue_name}",
            autoAck: true,
            consumer: consumer);
    }

    public void OnClientUserCreated(string queue_name, Action<ClientUserCreatedEvent> fn)
    {
        this.Consume(queue_name, MessageType.USER_CREATED, fn);
    }

    public void OnAssistanceIncidentCreated(string queue_name, Action<AssistanceIncidentCreatedEvent> fn)
    {
        this.Consume(queue_name, MessageType.INCIDENT_CREATED, fn);
    }
    
    public void OnAssistanceIncidentConfirmed(string queue_name, Action<AssistanceIncidentConfirmedEvent> fn)
    {
        this.Consume(queue_name, MessageType.INCIDENT_CONFIRMED, fn);
    }
    
    public void OnAssistanceIncidentReverted(string queue_name, Action<AssistanceIncidentRevertedEvent> fn)
    {
        this.Consume(queue_name, MessageType.INCIDENT_REVERTED, fn);
    }

    public void OnAssistanceSfSyncSuccessCreated(string queue_name, Action<AssistanceSfSyncSuccessEvent> fn)
    {
        this.Consume(queue_name, MessageType.SF_SYNC_SUCCESS, fn);
    }

    public void OnAssistanceSfSyncErrorCreated(string queue_name, Action<AssistanceSfSyncErrorEvent> fn)
    {
        this.Consume(queue_name, MessageType.SF_SYNC_ERROR, fn);
    }
    
    public void OnClientSupportSuccessChecked(string queue_name, Action<ClientSupportSuccessCheckedEvent> fn)
    {
        this.Consume(queue_name, MessageType.PHARMACY_SUPPORT_CHECK_SUCCESS, fn);
    }
    public void OnClientSupportErrorChecked(string queue_name, Action<ClientSupportErrorCheckedEvent> fn)
    {
        this.Consume(queue_name, MessageType.PHARMACY_SUPPORT_CHECK_ERROR, fn);
    }
}