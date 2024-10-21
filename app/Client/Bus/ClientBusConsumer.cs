using Microsoft.EntityFrameworkCore;
using OtusKdeBus;
using OtusKdeBus.Model.Client;
using WTM.AssistanceDAL;

namespace OtusKdeDAL.BusConsumers;

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
            //Console.wa
            //_producer.SendMessage(new BillingOrderConfirmedEvent() { OrderId = x.OrderId });
        };
        //_consumer.OnOrderCreated("watermelon", action);

        
    }
}