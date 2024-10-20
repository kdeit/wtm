namespace WTM.Models;

public class Posting : BaseEntity
{
    public Status Status { get; set; }
    public int GroupId { get; set; }
    public int? ReplyToPostingId { get; set; }
    public int AuthorUserId { get; set; }
    public string Content { get; set; }
    public string ContentPreview { get; set; }
    public bool IsVisible { get; set; }
    public int? EventId { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateModified { get; set; }
}