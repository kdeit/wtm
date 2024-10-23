namespace OtusKdeBus.Model.Client;

public class AssistanceSfSyncSuccessEvent : BaseEvent
{
    public AssistanceSfSyncSuccessEvent() : base(MessageType.SF_SYNC_SUCCESS)
    {
    }

    public int IncidentId { get; set; }
    public string SfId { get; set; }
}