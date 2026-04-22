using email_constructor.Api.Extensions;
using email_constructor.Api.Model.Request;
// using email_constructor.Api.Services;
using email_constructor.Application.Interfaces;
using email_constructor.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace email_constructor.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TestController : ControllerBase
{
    private readonly IContentBlockService _contentBlockService;
    private readonly IFileUploadService _fileUploadService;
    private readonly IMapper _mapper;
    private readonly ILogger<TestController> _logger;
    
    public TestController(
        IFileUploadService fileUploadService, 
        IContentBlockService contentBlockService,
        IMapper mapper,
        ILogger<TestController> logger)
    {
        _contentBlockService = contentBlockService;
        _fileUploadService = fileUploadService;
        _mapper = mapper;
        _logger = logger;
    }
    
    [HttpPost("GetEmailContentBlock")]
    public IActionResult GetEmailContentBlock(
        [FromBody] GetBlockRequest request, 
        CancellationToken ct)
    {
        Console.WriteLine(request);
        _logger.LogInformation("Test endpoint called");
        return Ok(new { message = "Hello from РУЧКА ДЛЯ КОНСТРУКТОРА!" });
    }
    
    
    [HttpPost("GetEmailTemplate")]
    public async Task<IActionResult> GetEmailTemplate(
        [FromBody] GetEmailTemplateRequest request, 
        CancellationToken ct)
    {
        var mappedRequest = _mapper.MapToContentData(request);
        var template = await _contentBlockService.GetRenderedBlocksAsync(mappedRequest, ct);
        var result = template.ToJson();
        return Ok(result);
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

            var products = await _fileUploadService.UploadAndCacheAsync(file, fileType, ct);
            var result = products.ToJson();
        
            _logger.LogInformation("UploadExcel endpoint called");
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}