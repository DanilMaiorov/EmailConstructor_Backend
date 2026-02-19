using email_constructor.Common.Model;

namespace email_constructor.Infrastructure.Interfaces;

public interface IContentBlockRepository
{
    public Task<List<ContentBlock>> GetContentBlocks(string storeId, List<string> blockTypes);
}