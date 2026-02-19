using email_constructor.Api;
using email_constructor.Api.Extensions;
using email_constructor.Application.Interfaces;
using email_constructor.Application.Models;
using email_constructor.Infrastructure.Interfaces;
using Grpc.Core;

namespace email_constructor.Services;

public class EmailContentBlockService : EmailContentBlock.EmailContentBlockBase
{
    private readonly IContentBlockService _contentBlockService;
    private readonly Mapper _mapper;

    public EmailContentBlockService(IContentBlockService contentBlockService, Mapper mapper)
    {
        _contentBlockService = contentBlockService;
        _mapper = mapper;
    }

    public override async Task<GetEmailContentBlockResponse> GetEmailContentBlock(
        GetEmailContentBlockRequest request, ServerCallContext context)
    {

        var mappedRequest = new ContentData()
        {
            
        };
        
        var block = _contentBlockService.GetBlockAsync("tu", "tu");
        return new GetEmailContentBlockResponse { };
    }
}