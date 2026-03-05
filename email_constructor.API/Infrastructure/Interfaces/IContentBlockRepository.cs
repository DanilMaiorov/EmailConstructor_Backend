using email_constructor.Domain.Model;

namespace email_constructor.Infrastructure.Interfaces;

public interface IContentBlockRepository
{
    public Task<List<ContentBlock>> GetContentBlocks(string storeId, List<string> blockTypes);
    public Task<List<BlockWrapper>> GetContentBlockWrappers();
}