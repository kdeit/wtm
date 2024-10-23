namespace OtusKdeBus.Model.Client;

public class AssistanceIncidentConfirmedEvent : BaseEvent
{
    public AssistanceIncidentConfirmedEvent() : base(MessageType.INCIDENT_CONFIRMED)
    {
    }

    public int IncidentId { get; set; }
    public string SfId { get; set; }
}