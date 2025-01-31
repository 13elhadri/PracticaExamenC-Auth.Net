using examn.Funkos.Dto;
using examn.Funkos.Models;
using examn.Funkos.Services;
using examn.Funkos.Storage.Config;
using examn.User.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace examn.Funkos.Controller;

[ApiController]
[Route("api/funkos")]
public class FunkoController:ControllerBase
{
    private readonly IFunkoService _funkoService;

    public FunkoController(IFunkoService funkoService)
    {
        _funkoService = funkoService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Funko>>> Getall()
    {
        
        var result = await _funkoService.GetAllAsync();
            
        return Ok(result);
        
    }
    
    [HttpGet("{id}")]
    [Authorize(Policy = "UserPolicy")]
    public async Task<ActionResult<Funko>> GetById(long id)
    {
        
        var result = await _funkoService.GetByIdAsync(id);
        if (result == null) return NotFound(new { message = $"El funko no se a encontrado con id: {id}" });
        
        return Ok(result);
        
    }

    [HttpPost]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<ActionResult<Funko>> Create([FromBody] FunkoRequest userRequest)
    {
        try
        {
            var result = await _funkoService.CreateAsync(userRequest);

            return Ok(result);

        }
        catch (Exception e)
        {
            return BadRequest(new {  message = e.Message });
        }
    }

    [HttpPatch("{id}")]
    [Authorize(Policy = "UserPolicy")]
    public async Task<ActionResult<FunkoRequest>> Update(long id,IFormFile file)
    {
        try
        {
            var result = await _funkoService.UpdateFunkoFotoAsync(id,file);

            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(new {  message = e.Message });
        }
        
            
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<ActionResult<Funko>> Delete(long id)
    {
        var result = await _funkoService.DeleteByGuidAsync(id);
        if (result == null) return NotFound(new { message = $"El funko no se a encontrado con id: {id}" });

        return Ok(result);
            
    }
    
    [HttpGet("photo/{fileName}")]
    [Authorize("AdminPolicy")]
    public async Task<IActionResult> GetPhotoByFileNameAsync(string fileName)
    {

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return BadRequest(new { message = "File name must be provided." });
        }

        var fileStream = await _funkoService.GetFileAsync(fileName);
        if (fileStream == null)
        {
            return NotFound(new { message = $"File with name {fileName} not found." });
        }

        var fileExtension = Path.GetExtension(fileName);
        var mimeType = MimeType.GetMimeType(fileExtension);

        return File(fileStream, mimeType, fileName);
    }
    
}