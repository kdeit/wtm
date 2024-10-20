namespace WTM.Models;

public class Incident : BaseEntity
{
    public int AuthorUserId { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
    
    public bool IsClosed { get; set; }
    public int? ClosedByUserId { get; set; }
    public string ClosingReason { get; set; }
    public DateTime? ClosedDate { get; set; }
    
    public string SalesForceCaseId { get; set; }
    
    public DateTime DateCreated { get; set; }
    public DateTime DateModified { get; set; }
}