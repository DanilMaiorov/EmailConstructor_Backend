using email_constructor.Domain.Model;

namespace email_constructor.Infrastructure.Interfaces;

public interface IContentBlockRepository
{
    public Task<List<DefaultBlock>> GetDefaultBlocks(string storeId, List<string> blockTypes);
    public Task<List<DefaultBlockData>> GetDefaultBlocksData(string storeId, string languageId, List<string> blockTypes);
    public Task<List<BlockWrapper>> GetBlockWrappers();
}