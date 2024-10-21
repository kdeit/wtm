using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WTM.Models;

namespace WinTeamGate.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IncidentController : Controller
{
    private readonly HttpClient _http;
    private readonly HttpClient _http2;
    private readonly IHostEnvironment _env;

    public IncidentController(HttpClient http, HttpClient http2, IHostEnvironment env)
    {
        _env = env;
        _http = http;
        _http.BaseAddress = _env.IsDevelopment()
            ? new Uri("http://localhost:5015")
            : new Uri("http://client-api-asp.default.svc.cluster.local");
        
        _http2 = http2;
        _http2.BaseAddress = _env.IsDevelopment()
            ? new Uri("http://localhost:5241")
            : new Uri("http://assistance-api-asp.default.svc.cluster.local");
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<int?>> Create(IncidentCreateRequest model)
    {
        var userId = await GetUserId();
        _http2.DefaultRequestHeaders.Add("X-user-id", userId.ToString());
        
        JsonContent content = JsonContent.Create(model);
        var res = await _http2.PostAsync($"incident", content);
        
        return Ok(await res.Content.ReadAsStringAsync());
    }
    
    private async Task<int> GetUserId()
    {
        var email = HttpContext.User.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        var res0 = await _http.GetAsync($"user/{email}");
        User content = await res0.Content.ReadFromJsonAsync<User>();

        return content.Id;
    }
}