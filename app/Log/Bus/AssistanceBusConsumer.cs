using System.Text.Json;
using OtusKdeBus;
using OtusKdeBus.Model.Client;
using WTM.LogDAL;

namespace Log.BusConsumers;

public class AssistanceBusConsumer
{
    private IBusConsumer _consumer;
    private IBusProducer _producer;
    private Log2023Context _cnt2023;
    private Log2024Context _cnt2024;

    public AssistanceBusConsumer(IBusConsumer busConsumer,
        Log2023Context context2023,
        Log2024Context context2024,
        IBusProducer producer)
    {
        _consumer = busConsumer;
        _cnt2023 = context2023;
        _cnt2024 = context2024;
        _producer = producer;
    }

    public void Init()
    {
        Action<AssistanceIncidentCreatedEvent> action = async (x) =>
        {
            var year = DateTime.Now.Year;
            var nv = new WTM.Models.Log()
            {
                DateCreated = DateTime.UtcNow,
                EventName = typeof(AssistanceIncidentCreatedEvent).FullName,
                Payload = JsonSerializer.Serialize(x)
            };
            switch (year)
            {
                case (2024):
                    _cnt2024.Add(nv);
                    _cnt2024.SaveChanges();
                    break;
                case (2023):
                    _cnt2023.Add(nv);
                    _cnt2023.SaveChanges();
                    break;
            }
        };
        _consumer.OnAssistanceIncidentCreated("oooorange", action);
    }
}