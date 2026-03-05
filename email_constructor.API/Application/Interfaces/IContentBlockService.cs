using email_constructor.Application.Models;
using email_constructor.Domain.Model;

namespace email_constructor.Application.Interfaces;

public interface IContentBlockService
{
    Task<List<RenderedBlock>> GetRenderedBlocksAsync(ContentData contentData);
    // Task<List<ContentBlock>> GetWrappedBlocksAsync(ContentData contentData);
    RenderedBlock RenderBlock(ContentBlock block, BlockWrapper wrapper);
    // Task<EmailTemplate> AssembleTemplateAsync(string templateId);
}