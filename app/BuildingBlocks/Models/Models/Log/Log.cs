namespace WTM.Models;

public class Log : BaseEntity
{
    public string EventName { get; set; }
    public string Payload { get; set; }
    public DateTime DateCreated { get; set; }
}