using System.ComponentModel.DataAnnotations;

namespace WTM.Models;

public class IncidentCreateRequest
{
    public string Subject { get; set; }
    public string Content { get; set; }
}