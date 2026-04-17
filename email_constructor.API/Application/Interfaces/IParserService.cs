using email_constructor.Domain.Enums;
using email_constructor.Domain.Model;

namespace email_constructor.Application.Interfaces;

public interface IParserService
{
    Task<IEnumerable<Product>> GetProducts(IFormFile file, UploadFileType fileType, CancellationToken cancellationToken);
}