namespace OtusKdeBus.Model.Client;

public class ClientSupportSuccessCheckedEvent : BaseEvent
{
    public ClientSupportSuccessCheckedEvent() : base(MessageType.PHARMACY_SUPPORT_CHECK_SUCCESS)
    {
    }

    public int IncidentId { get; set; }
}