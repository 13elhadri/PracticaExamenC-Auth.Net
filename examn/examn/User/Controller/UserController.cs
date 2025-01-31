using examn.User.Dto;
using examn.User.Models;
using examn.User.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace examn.User.Controller;

[ApiController]
[Route("api/usuarios")]
public class UserController:ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    
    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        try
        {
            var result = await _userService.GetByUsernameAsync(loginRequest.Username);
            if (result == null) return NotFound(new { message = "Usuario no encontrado" });
            
            var token = _userService.Authenticate(loginRequest.Username, loginRequest.Password);
            return Ok(new { Token = token });
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(e.Message);
        }
    }
    
    [HttpGet]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<ActionResult<IEnumerable<UserResponse>>> Getall()
    {
        
        var result = await _userService.GetAllAsync();
            
        return Ok(result);
        
    }
    
    [HttpGet("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<ActionResult<UserResponse>> GetById(long id)
    {
        
        
        var result = await _userService.GetByIdAsync(id);
        if (result == null) return NotFound(new { message = $"El usuario no se a encontrado con id: {id}" });
        
        return Ok(result);
        
    }
    
    [HttpGet("me")]
    [Authorize(Policy = "UserPolicy")]
    public async Task<ActionResult<UserResponse>> GetMe()
    {
        var userAuth = _userService.GetAuthenticatedUser();
        if (userAuth is null) return NotFound("No se ha podido identificar al usuario logeado");
        
        var result = await _userService.GetByIdAsync(userAuth.Id);
        if (result == null) return NotFound(new { message = $"El usuario no se a encontrado con id: {userAuth.Id}" });
        
        return Ok(result);
        
    }

    [HttpPost]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<ActionResult<UserResponse>> Create([FromBody] UserRequest userRequest)
    {
        try
        {
            var result = await _userService.CreateAsync(userRequest);

            return Ok(result);

        }
        catch (Exception e)
        {
            return BadRequest(new {  message = e.Message });
        }
    }
    
    [HttpPatch("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<ActionResult<UserResponse>> Update(long id,[FromBody] UserRequestUpdate userRequest)
    {
            var result = await _userService.UpdateAsync(id,userRequest);
            if (result == null) return NotFound(new { message = $"El usuario no se a encontrado con id: {id}" });

            return Ok(result);
            
    }
    
    [HttpPatch("me")]
    [Authorize(Policy = "UserPolicy")]
    public async Task<ActionResult<UserResponse>> UpdateMe([FromBody] UserRequestUpdate userRequest)
    {
        
        var userAuth = _userService.GetAuthenticatedUser();
        if (userAuth is null) return NotFound("No se ha podido identificar al usuario logeado");
        
        var result = await _userService.UpdateAsync(userAuth.Id,userRequest);
        if (result == null) return NotFound(new { message = $"El usuario no se a encontrado con id: {userAuth.Id}" });

        return Ok(result);
            
    }
    
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<ActionResult<UserResponse>> Delete(long id)
    {
        var result = await _userService.DeleteByGuidAsync(id);
        if (result == null) return NotFound(new { message = $"El usuario no se a encontrado con id: {id}" });

        return Ok(result);
            
    }
}