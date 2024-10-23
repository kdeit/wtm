using OtusKdeBus;
using OtusKdeBus.Model.Client;
using WTM.AssistanceDAL;

namespace AssistanceSF.BusConsumers;

public class AssistanceBusConsumer
{
    private IBusConsumer _consumer;
    private IBusProducer _producer;
    private AssistanceContext _cnt;

    public AssistanceBusConsumer(IBusConsumer busConsumer, AssistanceContext context, IBusProducer producer)
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
            var random = new Random();
            Thread.Sleep(random.Next(300, 1000));
            var b = random.Next(2);
            Console.WriteLine($"Send to SF :: {x.IncidentId} with result ${b}");
            if (b == 1)
            {
                _producer.SendMessage(new AssistanceSfSyncSuccessEvent()
                {
                    IncidentId = x.IncidentId,
                    SfId = Guid.NewGuid().ToString()
                });
            }
            else
            {
                _producer.SendMessage(new AssistanceSfSyncErrorEvent() { IncidentId = x.IncidentId });
            }
        };
        _consumer.OnAssistanceIncidentCreated("banana", action);
        
        Action<AssistanceIncidentRevertedEvent> action2 = async (x) =>
        {
            Console.WriteLine($"Revert incident {x.IncidentId}");
        };
        _consumer.OnAssistanceIncidentReverted("kokoc", action2);
    }
}