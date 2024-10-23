using Microsoft.EntityFrameworkCore;
using OtusKdeBus;
using OtusKdeBus.Model.Client;
using WTM.AssistanceDAL;
using WTM.Models;

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
        Action<AssistanceIncidentConfirmedEvent> action = async (x) =>
        {
            Console.WriteLine("Incident confirmed consumer");
            var i = await _cnt.Incidents.FirstOrDefaultAsync(_ => _.Id == x.IncidentId);
            if (i is null)
            {
                return;
            }

            i.Status = IncidentStatus.OPEN;
            i.SalesForceCaseId = x.SfId;
            await _cnt.SaveChangesAsync();
        };
        _consumer.OnAssistanceIncidentConfirmed("watermelona", action);

        Action<AssistanceIncidentRevertedEvent> action2 = async (x) =>
        {
            Console.WriteLine("Incident confirmed reverted");
            var i = await _cnt.Incidents.FirstOrDefaultAsync(_ => _.Id == x.IncidentId);
            if (i is null)
            {
                return;
            }

            i.Status = IncidentStatus.ERROR;
            await _cnt.SaveChangesAsync();
        };
        _consumer.OnAssistanceIncidentReverted("watermelona", action2);
    }
}