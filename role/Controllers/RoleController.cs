using Microsoft.AspNetCore.Mvc;
using role_api.Data.Models;
using role_api.Services.Interfaces;

namespace role_api.Controllers;

[Route("role/")]
public class RoleController : Controller
{
    private readonly IRoleService _service;

    public RoleController(IRoleService service)
    {
        _service = service;
    }
    
    [Route("get-role")]
    [HttpPost]
    public async Task<IActionResult> GetRole([FromBody] GetRoleRequest request)
    {
        var roles = await _service.GetAllRole(request);
        return Ok(roles);
    }
    
    [Route("get-detail-role")]
    [HttpPost]
    public async Task<IActionResult> GetDetailRole([FromBody] int id)
    {
        var roles = await _service.GetDetailRole(id);
        return Ok(roles);
    }
    
    [Route("create-role")]
    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        var roles = await _service.CreateRole(request);
        return Ok(roles);
    }
    
    [Route("update-role")]
    [HttpPost]
    public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleRequest request)
    {
        var roles = await _service.UpdateRole(request);
        return Ok(roles);
    }
    
    [Route("delete-role")]
    [HttpPost]
    public async Task<IActionResult> DeleteRole([FromBody] int id)
    {
        var roles = await _service.DeleteRole(id);
        return Ok(roles);
    }
    
    [Route("create-user-role")]
    [HttpPost]
    public async Task<IActionResult> CreateUserRole([FromBody] CreateUserRoleRequest request)
    {
        var userRole = await _service.CreateUserRole(request);
        return Ok(userRole);
    }
    
    [Route("get-user-role")]
    [HttpPost]
    public async Task<IActionResult> GetUserRole([FromBody] GetUserRoleRequest request)
    {
        var userRole = await _service.GetAllUserRole(request);
        return Ok(userRole);
    }
}