using email_constructor.Application.Interfaces;
using email_constructor.Cache;
using email_constructor.Domain.Enums;

namespace email_constructor.Application.Services;

public class FileUploadService : IFileUploadService
{
    private readonly IParserService _excelParserService;
    private readonly IProductCache _productCache;
    
    public FileUploadService(IParserService excelParserService, IProductCache productCache)
    {
        _excelParserService = excelParserService;
        _productCache = productCache;
    }
    
    public async Task<string> UploadAndCacheAsync(IFormFile file, UploadFileType type, CancellationToken ct)
    {
        var products = await _excelParserService.GetProducts(file, type, ct);
        return await _productCache.SetCacheProducts(products, ct);
    }
}