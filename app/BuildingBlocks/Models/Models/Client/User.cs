using System.ComponentModel.DataAnnotations;

namespace WTM.Models;

public class User : BaseEntity
{
    public Status Status { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    [EmailAddress] public string Email { get; set; }
    public UserRoleEnum RolesFlag { get; set; }

    public int CompanyId { get; set; }
}