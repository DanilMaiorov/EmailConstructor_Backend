using email_constructor.Application.Interfaces;
using email_constructor.Domain.Enums;
using email_constructor.Domain.Model;
using ClosedXML.Excel;
namespace email_constructor.Application.Services;

/// <summary>
/// Сервис парсинга из эксель
/// </summary>
public class ExcelParserService : IParserService
{
    
    public async Task<IEnumerable<Product>> GetProducts(IFormFile file, UploadFileType fileType, CancellationToken ct)
    {
        if (fileType != UploadFileType.Excel) throw new ArgumentException("Invalid file type");
        return await ExtractValues(file);
    }

    /// <summary>
    /// Извлекает данные продуктов из эксель файла
    /// </summary>
    /// <param name="file">Файл</param>
    /// <returns>Коллекция продуктов</returns>
    private static async Task<IEnumerable<Product>> ExtractValues(IFormFile file)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        ms.Position = 0;
    
        using var workbook = new XLWorkbook(ms);
        var worksheet = workbook.Worksheet(1);
        var usedRange = worksheet.RangeUsed();
        
        if (!usedRange.Columns().Any()) throw new ArgumentException("Invalid number of used range columns");

        var hasText = usedRange.Row(1).Cells().Any(c => c.Value.ToString().Contains("text"));
        
        return usedRange.Rows()
            .Skip(1)
            .Select(row => new Product
            {
                Link = row.Cell(2).Value.ToString(),
                Type = row.Cell(3).Value.ToString().ToUpper(),
                Brand = row.Cell(4).Value.ToString().ToUpper(),
                Name = row.Cell(5).Value.ToString().ToLower(),
                Description = hasText ? row.Cell(7).Value.ToString() : null,
                Price = hasText ? row.Cell(8).Value.ToString() : row.Cell(7).Value.ToString(),
                OldPrice = hasText ? row.Cell(9).Value.ToString() : row.Cell(8).Value.ToString(),
                Sku = hasText ? row.Cell(10).Value.ToString() : row.Cell(9).Value.ToString(),
                Img = hasText ? row.Cell(11).Value.ToString() : row.Cell(10).Value.ToString(),
            })
            .ToList();
    }
}