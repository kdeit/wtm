using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using OtusKdeBus.Model;
using OtusKdeBus.Model.Client;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Assistance;

public enum SagaStatus
{
    PENDING,
    CONFIRMED,
    REVERTED
}

public class Saga
{
    public bool IsMustBeReverted { get; set; }
    public SagaStatus Access { get; set; }
    public SagaStatus SfSync { get; set; }
    public string SfId { get; set; }
}

public class IncidentCreatedSaga
{
    private IModel _channel;
    IDistributedCache _cache;

    public IncidentCreatedSaga(IDistributedCache distributedCache)
    {
        _cache = distributedCache;
    }

    public void handle()
    {
        var isProduction = Environment.GetEnvironmentVariable("DB_PASSWORD") is not null;

        var HostName = isProduction
            ? "rabbit-rabbitmq.default.svc.cluster.local"
            : "localhost";
        Console.WriteLine($"Try connect to {HostName}...");

        var factory = new ConnectionFactory()
            { HostName = HostName, VirtualHost = "otus", Port = 5672, UserName = "admin", Password = "sEcret" };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        string queue_name = "OrderTransactionSaga";
        string exchange = "user_exchange";

        // ORDER_CREATED
        var type = MessageType.INCIDENT_CREATED;
        _channel.QueueDeclare(queue: $"Queue_{type}.{queue_name}", durable: true, exclusive: false, autoDelete: false,
            arguments: null);
        _channel.QueueBind(queue: $"Queue_{type}.{queue_name}", exchange, routingKey: $"Routing_key_{type}");


        var consumerOrderCreated = new EventingBasicConsumer(_channel);
        consumerOrderCreated.Received += async (model, ea) =>
        {
            Console.WriteLine("Incident created consumer");
            var _ = GetEventPayload<AssistanceIncidentCreatedEvent>(ea);
            var saga = new Saga();
            saga.IsMustBeReverted = false;
            saga.Access = SagaStatus.PENDING;
            saga.SfSync = SagaStatus.PENDING;
            var serialised = JsonSerializer.Serialize(saga);

            await _cache.SetStringAsync(_.IncidentId.ToString(), serialised);
            Console.WriteLine("Incident created consumer");
        };
        _channel.BasicConsume($"Queue_{type}.{queue_name}", autoAck: true, consumerOrderCreated);

        //Client check pharmacy support access
        type = MessageType.PHARMACY_SUPPORT_CHECK_SUCCESS;
        _channel.QueueDeclare(queue: $"Queue_{type}.{queue_name}", durable: true, exclusive: false, autoDelete: false,
            arguments: null);
        _channel.QueueBind(queue: $"Queue_{type}.{queue_name}", exchange, routingKey: $"Routing_key_{type}");

        var consumerOrderCreated2 = new EventingBasicConsumer(_channel);
        consumerOrderCreated2.Received += async (model, ea) =>
        {
            Console.WriteLine("Company access success consumed");
            var _ = GetEventPayload<ClientSupportSuccessCheckedEvent>(ea);
            var saga = await GetSaga(_.IncidentId);
            saga.Access = SagaStatus.CONFIRMED;
            await CheckForComplete(saga, _.IncidentId);
        };
        _channel.BasicConsume($"Queue_{type}.{queue_name}", autoAck: true, consumerOrderCreated2);

        type = MessageType.PHARMACY_SUPPORT_CHECK_ERROR;
        _channel.QueueDeclare(queue: $"Queue_{type}.{queue_name}", durable: true, exclusive: false, autoDelete: false,
            arguments: null);
        _channel.QueueBind(queue: $"Queue_{type}.{queue_name}", exchange, routingKey: $"Routing_key_{type}");

        var consumerOrderCreated3 = new EventingBasicConsumer(_channel);
        consumerOrderCreated3.Received += async (model, ea) =>
        {
            Console.WriteLine("Company access error consumed");
            var _ = GetEventPayload<ClientSupportErrorCheckedEvent>(ea);
            var saga = await GetSaga(_.IncidentId);
            saga.Access = SagaStatus.REVERTED;
            saga.IsMustBeReverted = true;

            await CheckForComplete(saga, _.IncidentId);
        };
        _channel.BasicConsume($"Queue_{type}.{queue_name}", autoAck: true, consumerOrderCreated3);

        //EOF Client

        //SF check
        type = MessageType.SF_SYNC_SUCCESS;
        _channel.QueueDeclare(queue: $"Queue_{type}.{queue_name}", durable: true, exclusive: false, autoDelete: false,
            arguments: null);
        _channel.QueueBind(queue: $"Queue_{type}.{queue_name}", exchange, routingKey: $"Routing_key_{type}");

        var consumerOrderCreated4 = new EventingBasicConsumer(_channel);
        consumerOrderCreated4.Received += async (model, ea) =>
        {
            Console.WriteLine("SF success consumed");
            var _ = GetEventPayload<AssistanceSfSyncSuccessEvent>(ea);
            var saga = await GetSaga(_.IncidentId);
            saga.SfSync = SagaStatus.CONFIRMED;
            saga.SfId = _.SfId;
            await CheckForComplete(saga, _.IncidentId);
        };
        _channel.BasicConsume($"Queue_{type}.{queue_name}", autoAck: true, consumerOrderCreated4);

        type = MessageType.SF_SYNC_ERROR;
        _channel.QueueDeclare(queue: $"Queue_{type}.{queue_name}", durable: true, exclusive: false, autoDelete: false,
            arguments: null);
        _channel.QueueBind(queue: $"Queue_{type}.{queue_name}", exchange, routingKey: $"Routing_key_{type}");

        var consumerOrderCreated5 = new EventingBasicConsumer(_channel);
        consumerOrderCreated5.Received += async (model, ea) =>
        {
            Console.WriteLine("SF error consumed");
            var _ = GetEventPayload<AssistanceSfSyncErrorEvent>(ea);
            var saga = await GetSaga(_.IncidentId);
            saga.SfSync = SagaStatus.REVERTED;
            saga.IsMustBeReverted = true;
            await CheckForComplete(saga, _.IncidentId);
        };
        _channel.BasicConsume($"Queue_{type}.{queue_name}", autoAck: true, consumerOrderCreated5);

        //EOF SF check
    }

    private T GetEventPayload<T>(BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        return JsonSerializer.Deserialize<T>(message);
    }

    private void Send<T>(MessageType type, T message)
    {
        var json = JsonSerializer.Serialize(message);
        var body = System.Text.Encoding.UTF8.GetBytes(json);
        _channel.BasicPublish(exchange: "user_exchange", routingKey: $"Routing_key_{type}", basicProperties: null,
            body: body);
    }

    private async Task<Saga> GetSaga(int incidentId)
    {
        var cv = await _cache.GetStringAsync(incidentId.ToString());
        return JsonSerializer.Deserialize<Saga>(cv);
    }

    private async Task CheckForComplete(Saga saga, int incidentId)
    {
        Console.WriteLine(
            $"{saga.Access.ToString()} {saga.SfSync.ToString()}  {saga.IsMustBeReverted.ToString()} {incidentId}");
        bool isAllComplete = saga.Access != SagaStatus.PENDING && saga.SfSync != SagaStatus.PENDING;
        if (!isAllComplete)
        {
            ;
            var c = JsonSerializer.Serialize<Saga>(saga, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            Console.WriteLine($"CheckforComplete:: continue {c}");

            await _cache.SetStringAsync(incidentId.ToString(), c);
            return;
        }

        if (saga.IsMustBeReverted)
        {
            Send(MessageType.INCIDENT_REVERTED, new AssistanceIncidentRevertedEvent() { IncidentId = incidentId });
            Console.WriteLine($"Transaction error");
        }
        else
        {
            Console.WriteLine($"Transaction success");
            Send(MessageType.INCIDENT_CONFIRMED, new AssistanceIncidentConfirmedEvent()
                { IncidentId = incidentId, SfId = saga.SfId });
        }

        await _cache.RemoveAsync(incidentId.ToString());
    }
}