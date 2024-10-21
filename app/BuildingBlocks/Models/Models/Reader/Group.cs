namespace WTM.Models;

public class Group : BaseEntity
{
    public Group()
    {
        Status = Status.Enabled;
    }
    public Status Status { get; set; }
    public string Name { get; set; }
    public GroupType Type { get; set; }
    public string? Description { get; set; }
    public bool IsArchived { get; set; }
    public bool IsNonArchivable { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsWritableForAdminOnly { get; set; }
    public bool IsWritableAnswersForAll { get; set; }
    public string? ExternalId { get; set; }
}