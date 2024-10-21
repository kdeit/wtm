using OtusKdeBus;
using OtusKdeBus.Model.Client;
using WTM.ClientDAL;

namespace Client.BusConsumers;

public class AssistanceBusConsumer
{
    private IBusConsumer _consumer;
    private IBusProducer _producer;
    private ClientContext _cnt;

    public AssistanceBusConsumer(IBusConsumer busConsumer, ClientContext context, IBusProducer producer)
    {
        _consumer = busConsumer;
        _cnt = context;
        _producer = producer;
    }

    public void Init()
    {
        Action<AssistanceIncidentCreatedEvent> action = async (x) =>
        {
            Console.WriteLine($"Client:: assistance incident created with {x.IncidentId} - {x.UserId}");
            var random = new Random();
            Thread.Sleep(random.Next(300, 1000));
            var b = random.Next(2);
            Console.WriteLine($"Is pharmacy has Access to support :: {x.IncidentId} with result ${b}");
            if (b == 1)
            {
                _producer.SendMessage(new ClientSupportSuccessCheckedEvent() { IncidentId = x.IncidentId });
            }
            else
            {
                _producer.SendMessage(new ClientSupportErrorCheckedEvent() { IncidentId = x.IncidentId });
            }
        };
        _consumer.OnAssistanceIncidentCreated("banan", action);
    }
}