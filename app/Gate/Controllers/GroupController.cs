using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WTM.Models;

namespace WinTeamGate.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class GroupController : Controller
{
    private readonly HttpClient _http;
    private readonly HttpClient _http2;
    private readonly IHostEnvironment _env;

    public GroupController(HttpClient http, HttpClient http2, IHostEnvironment env)
    {
        _env = env;
        _http = http;
        _http.BaseAddress = _env.IsDevelopment()
            ? new Uri("http://localhost:5015")
            : new Uri("http://client-api-asp.default.svc.cluster.local");
        
        _http2 = http2;
        _http2.BaseAddress = _env.IsDevelopment()
            ? new Uri("http://localhost:5297")
            : new Uri("http://reader-api-asp.default.svc.cluster.local");
    }

    [HttpGet("{groupId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Group>> GetById(int groupId)
    {
        var userId = await GetUserId();
        _http2.DefaultRequestHeaders.Add("X-user-id", userId.ToString());
        
        var res = await _http2.GetAsync($"group/{groupId}");
        if (!res.IsSuccessStatusCode) return NotFound();
        var content = await res.Content.ReadFromJsonAsync<Group>();

        return Ok(content);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<int?>> Create(GroupCreateRequest model)
    {
        var userId = await GetUserId();
        _http2.DefaultRequestHeaders.Add("X-user-id", userId.ToString());
        
        JsonContent content = JsonContent.Create(model);
        var res = await _http2.PostAsync($"group", content);
        
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