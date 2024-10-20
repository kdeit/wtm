using System.ComponentModel.DataAnnotations;

namespace WTM.Models;

public class UserCreateUpdateRequest
{
    public UserCreateUpdateRequest()
    {
        Enabled = true;
    }

    [EmailAddress] public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
    public bool Enabled { get; set; }
}