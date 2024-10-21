using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WTM.ReaderDAL;
using WTM.Models;

namespace WTM.Reader.Controllers;

[ApiController]
[Route("[controller]")]
public class GroupController : Controller
{
    private readonly ReaderContext _cnt;

    public GroupController(ReaderContext context)
    {
        _cnt = context;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Group>> GetById(
        [FromHeader(Name = "X-user-id")] [Required]
        int UserId,
        int id)
    {
        var group = await _cnt.Groups.FirstOrDefaultAsync(_ => _.Id == id && _.Status != Status.Deleted);
        if (group is null) return NotFound();
        var groupMember = await _cnt.GroupMembers
            .FirstOrDefaultAsync(_ => _.GroupId == id && _.UserId == UserId);
        if (groupMember is null) return NotFound();

        return Ok(group);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> Create(
        [FromHeader(Name = "X-user-id")] [Required]
        int UserId,
        [FromBody] GroupCreateRequest model)
    {
        Console.WriteLine("Create group");

        var newGroup = new Group()
        {
            Name = model.Name
        };
        await _cnt.AddAsync(newGroup);
        await _cnt.SaveChangesAsync();
        var newGroupMember = new GroupMembers()
        {
            GroupId = newGroup.Id,
            UserId = UserId
        };
        await _cnt.AddAsync(newGroupMember);
        await _cnt.SaveChangesAsync();

        return Ok(newGroup.Id);
    }
}