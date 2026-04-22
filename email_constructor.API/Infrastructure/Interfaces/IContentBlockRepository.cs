using email_constructor.Application.Models;
using email_constructor.Domain.Model;

namespace email_constructor.Infrastructure.Interfaces;

public interface IContentBlockRepository
{
    public Task<List<DefaultBlock>> GetBlocks(string storeId, List<BlockData> blockTypes);
    public Task<List<DefaultBlockData>> GetBlocksDefaultData(string storeId, string languageId, List<BlockData> blockTypes);
    public Task<List<BlockWrapper>> GetBlockWrappers();
}