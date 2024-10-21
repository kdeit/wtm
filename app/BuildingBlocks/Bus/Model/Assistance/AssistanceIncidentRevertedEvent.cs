namespace OtusKdeBus.Model.Client;

public class AssistanceIncidentRevertedEvent : BaseEvent
{
    public AssistanceIncidentRevertedEvent() : base(MessageType.INCIDENT_REVERTED)
    {
    }

    public int IncidentId { get; set; }
}