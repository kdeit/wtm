using OtusKdeBus.Model.Client;

namespace OtusKdeBus;

public interface IBusConsumer
{
    public void OnAssistanceIncidentCreated(string queue_name, Action<AssistanceIncidentCreatedEvent> fn);
    public void OnAssistanceIncidentConfirmed(string queue_name, Action<AssistanceIncidentConfirmedEvent> fn);
    public void OnAssistanceIncidentReverted(string queue_name, Action<AssistanceIncidentRevertedEvent> fn);

    
    public void OnAssistanceSfSyncSuccessCreated(string queue_name, Action<AssistanceSfSyncSuccessEvent> fn);
    public void OnAssistanceSfSyncErrorCreated(string queue_name, Action<AssistanceSfSyncErrorEvent> fn);
    
    public void OnClientUserCreated(string queue_name, Action<ClientUserCreatedEvent> fn);
    public void OnClientSupportSuccessChecked(string queue_name, Action<ClientSupportSuccessCheckedEvent> fn);
    public void OnClientSupportErrorChecked(string queue_name, Action<ClientSupportErrorCheckedEvent> fn);
    
}