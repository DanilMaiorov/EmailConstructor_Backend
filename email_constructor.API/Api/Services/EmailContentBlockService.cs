using email_constructor.Api.Extensions;
using email_constructor.Application.Interfaces;
using Grpc.Core;

namespace email_constructor.Api.Services;

public class EmailContentBlockService : EmailContentBlock.EmailContentBlockBase
{
    private readonly IContentBlockService _contentBlockService;
    private readonly IMapper _mapper;

    public EmailContentBlockService(IContentBlockService contentBlockService, IMapper mapper)
    {
        _contentBlockService = contentBlockService;
        _mapper = mapper;
    }

    public override async Task<GetEmailContentBlockResponse> GetEmailContentBlock(
        GetEmailContentBlockRequest request, ServerCallContext context)
    {
        var mappedRequest = _mapper.MapToContentData(request);
        
        var result = await _contentBlockService.GetRenderedBlocksAsync(mappedRequest);
        
        var mappedResponse = _mapper.MapToResponse(result);

        var response = new GetEmailContentBlockResponse();
        
        response.Blocks.AddRange(mappedResponse.Blocks);
        
        return response;
    }
}