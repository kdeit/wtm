using System.ComponentModel.DataAnnotations;

namespace WTM.Models;

public class BaseEntity
{
    [Key] public int Id { get; set; }
}