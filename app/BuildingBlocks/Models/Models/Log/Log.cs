namespace WTM.Models;

public class Notification : BaseEntity
{
    public NotificationType NotificationType { get; set; }
    public string Title { get; set; }
    public string ShortDescription { get; set; }
    public string FullDescription { get; set; }
    public bool IsAllUsers { get; set; }
    public bool IsAttentionRequired { get; set; }
    public int CreatedByUserId { get; set; }
    public string ButtonTitle { get; set; }
    public string ExternalLink { get; set; }

    public DateTime DateCreated { get; set; }
    public DateTime DateModified { get; set; }
}