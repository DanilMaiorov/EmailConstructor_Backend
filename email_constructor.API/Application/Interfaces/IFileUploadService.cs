using email_constructor.Domain.Enums;

namespace email_constructor.Application.Interfaces;

public interface IFileUploadService
{
    Task<string> UploadAndCacheAsync(IFormFile file, UploadFileType type, CancellationToken ct);
}