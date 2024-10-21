using System.ComponentModel.DataAnnotations;

namespace WTM.Models;

public class GroupCreateRequest
{
    public GroupCreateRequest()
    {
        Status = Status.Enabled;
    }

    public Status Status { get; set; }
    public string Name { get; set; }
}