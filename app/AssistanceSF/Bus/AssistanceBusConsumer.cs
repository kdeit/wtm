using Microsoft.EntityFrameworkCore;
using OtusKdeBus;
using OtusKdeBus.Model.Client;
using WTM.AssistanceDAL;

namespace AssistanceSF.BusConsumers;

public class ClientBusConsumer
{
    private IBusConsumer _consumer;
    private IBusProducer _producer;
    private AssistanceContext _cnt;

    public ClientBusConsumer(IBusConsumer busConsumer, AssistanceContext context, IBusProducer producer)
    {
        _consumer = busConsumer;
        _cnt = context;
        _producer = producer;
    }

    public void Init()
    {
        Action<AssistanceIncidentCreatedEvent> action = async (x) =>
        {
            Console.WriteLine($"Assistance SF:: assistance incident created with {x.IncidentId} - {x.UserId}");
        };
        _consumer.OnAssistanceIncidentCreated("banan", action);
        
    }
}