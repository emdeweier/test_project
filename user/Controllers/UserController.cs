using Microsoft.AspNetCore.Mvc;
using user_api.Data.Models;
using user_api.Services.Interfaces;

namespace user_api.Controllers;

[Route("user/")]
public class UserController : Controller
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }
    
    [Route("get-user")]
    [HttpPost]
    public async Task<IActionResult> GetUser([FromBody] GetUserRequest request)
    {
        var users = await _service.GetAllUser(request);
        return Ok(users);
    }
    
    [Route("get-detail-user")]
    [HttpPost]
    public async Task<IActionResult> GetDetailUser([FromBody] string id)
    {
        var users = await _service.GetDetailUser(id);
        return Ok(users);
    }
    
    [Route("create-user")]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var users = await _service.CreateUser(request);
        return Ok(users);
    }
    
    [Route("update-user")]
    [HttpPost]
    public async Task<IActionResult> UpdateRole([FromBody] UpdateUserRequest request)
    {
        var users = await _service.UpdateUser(request);
        return Ok(users);
    }
    
    [Route("delete-user")]
    [HttpPost]
    public async Task<IActionResult> DeleteUser([FromBody] string id)
    {
        var users = await _service.DeleteUser(id);
        return Ok(users);
    }
}