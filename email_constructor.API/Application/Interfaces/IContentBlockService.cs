using email_constructor.Application.Models;
using email_constructor.Common.Model;

namespace email_constructor.Application.Interfaces;

public interface IContentBlockService
{
    Task<List<ContentBlock>> GetBlocksAsync(ContentData contentData);
    Task<ContentBlockWrapper> GetWrappedBlockAsync(string id, string storeId);
    // Task<EmailTemplate> AssembleTemplateAsync(string templateId);
}