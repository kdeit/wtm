namespace OtusKdeBus.Model.Client;

public class AssistanceIncidentCreatedEvent : BaseEvent
{
    public AssistanceIncidentCreatedEvent() : base(MessageType.INCIDENT_CREATED)
    {
    }

    public int UserId { get; set; }
    public int IncidentId { get; set; }
}