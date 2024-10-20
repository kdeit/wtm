namespace WTM.Models;

public class GroupMembers : BaseEntity
{
    public int GroupId { get; set; }
    public int UserId { get; set; }
    public int CompanyId { get; set; }
}