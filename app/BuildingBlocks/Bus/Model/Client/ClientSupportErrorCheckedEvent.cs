namespace OtusKdeBus.Model.Client;

public class ClientSupportErrorCheckedEvent : BaseEvent
{
    public ClientSupportErrorCheckedEvent() : base(MessageType.PHARMACY_SUPPORT_CHECK_ERROR)
    {
    }

    public int IncidentId { get; set; }
}