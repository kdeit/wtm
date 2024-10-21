namespace OtusKdeBus.Model.Client;

public class AssistanceSfSyncErrorEvent : BaseEvent
{
    public AssistanceSfSyncErrorEvent() : base(MessageType.SF_SYNC_ERROR)
    {
    }

    public int IncidentId { get; set; }
}