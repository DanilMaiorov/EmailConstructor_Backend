using email_constructor.Api.Model.Request;
using email_constructor.Application.Interfaces;
using email_constructor.Application.Services;
using email_constructor.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace email_constructor.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TestController : ControllerBase
{
    private readonly IParserService _parserService;
    private readonly ILogger<TestController> _logger;
    
    public TestController(IParserService parserService, ILogger<TestController> logger)
    {
        _parserService = parserService;
        _logger = logger;
    }
    
    

    

    [HttpGet]
    public IActionResult Get()
    {
        _logger.LogInformation("Test endpoint called");
        return Ok(new { message = "Hello from backend!" });
    }
    
    [HttpPost("GetEmailContentBlock")]
    public IActionResult GetEmailContentBlock(
        [FromBody] GetBlockRequest request, 
        CancellationToken ct)
    {
        Console.WriteLine(request);
        _logger.LogInformation("Test endpoint called");
        return Ok(new { message = "Hello from GetEmailContentBlock!" });
    }
    
    [HttpPost("upload/excel")]
    public async Task<IActionResult> UploadExcel(
        IFormFile file,
        [FromForm] UploadFileType fileType,
        CancellationToken ct)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest("File cannot be null or empty");

            var products = await _parserService.GetProducts(file, fileType, ct);
            var json = products.ToJson();
        
            _logger.LogInformation("UploadExcel endpoint called");
            return Ok(json);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}