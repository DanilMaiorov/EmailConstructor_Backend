using email_constructor.Api;
using email_constructor.Application.Interfaces;
using email_constructor.Infrastructure.Interfaces;
using Grpc.Core;

namespace email_constructor.Services;

public class EmailContentBlockService : EmailContentBlock.EmailContentBlockBase
{
    private readonly IContentBlockService _contentBlockService;

    public EmailContentBlockService(IContentBlockService contentBlockService)
    {
        _contentBlockService = contentBlockService;
    }

    public override async Task<GetEmailContentBlockResponse> GetEmailContentBlock(
        GetEmailContentBlockRequest request, ServerCallContext context)
    {
        var block = _contentBlockService.GetBlockAsync("tu", "tu");
        return new GetEmailContentBlockResponse { Id = request.Id };
    }
}