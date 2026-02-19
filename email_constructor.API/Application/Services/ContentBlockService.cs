using email_constructor.Application.Interfaces;
using email_constructor.Common.Model;
using email_constructor.Infrastructure.Interfaces;

namespace email_constructor.Application.Services;

public class ContentBlockService : IContentBlockService
{
    private readonly IContentBlockRepository _contentBlockRepository;

    public ContentBlockService(IContentBlockRepository contentBlockRepository)
    {
        _contentBlockRepository = contentBlockRepository;
    }
    
    public Task<ContentBlock> GetBlockAsync(string id, string storeId)
    {
        _contentBlockRepository.GetContentBlock("default_btn", "ru");
        ;
        throw new NotImplementedException();
    }

    public Task<ContentBlockWrapper> GetWrappedBlockAsync(string id, string storeId)
    {
        ;
        throw new NotImplementedException();
    }
}