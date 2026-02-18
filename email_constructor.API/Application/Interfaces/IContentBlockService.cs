using email_constructor.Common.Model;

namespace email_constructor.Application.Interfaces;

public interface IContentBlockService
{
    Task<ContentBlock> GetBlockAsync(string id, string storeId);
    Task<ContentBlockWrapper> GetWrappedBlockAsync(string id, string storeId);
    // Task<EmailTemplate> AssembleTemplateAsync(string templateId);
}