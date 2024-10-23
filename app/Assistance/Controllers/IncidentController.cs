using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OtusKdeBus;
using OtusKdeBus.Model.Client;
using WTM.AssistanceDAL;
using WTM.Models;

namespace WTM.Reader.Controllers;

[ApiController]
[Route("[controller]")]
public class IncidentController : Controller
{
    private readonly AssistanceContext _cnt;
    private readonly IBusProducer _busProducer;

    public IncidentController(AssistanceContext context, IBusProducer busProducer)
    {
        _cnt = context;
        _busProducer = busProducer;
    }

    [HttpGet("{incidentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Incident>> Create(
        [FromHeader(Name = "X-user-id")] [Required]
        int UserId,
        int incidentId)
    {
        Console.WriteLine($"Get incident with Id {incidentId} and user {UserId}");

        var res = await _cnt.Incidents.FirstOrDefaultAsync(_ => _.Id == incidentId && _.AuthorUserId == UserId);

        return res is not null ? Ok(res) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> Create(
        [FromHeader(Name = "X-user-id")] [Required]
        int UserId,
        [FromBody] IncidentCreateRequest model)
    {
        Console.WriteLine("Create incident");

        var incident = new Incident()
        {
            Subject = model.Subject,
            Content = model.Content,
            AuthorUserId = UserId
        };
        await _cnt.AddAsync(incident);
        await _cnt.SaveChangesAsync();

        _busProducer.SendMessage(new AssistanceIncidentCreatedEvent()
        {
            IncidentId = incident.Id,
            UserId = UserId
        });

        return Ok(incident.Id);
    }
}