// using email_constructor.Api.Extensions;
// using email_constructor.Api.Model.Request;
// using email_constructor.Application.Interfaces;
// using Grpc.Core;
//
// namespace email_constructor.Api.Services;
//
// public class EmailContentBlockService : EmailContentBlock.EmailContentBlockBase
// {
//     private readonly IContentBlockService _contentBlockService;
//     private readonly IMapper _mapper;
//
//     public EmailContentBlockService(IContentBlockService contentBlockService, IMapper mapper)
//     {
//         _contentBlockService = contentBlockService;
//         _mapper = mapper;
//     }
//
//     public override async Task<GetEmailTemplateResponse> GetEmailTemplate(
//         GetEmailTemplateRequest request, ServerCallContext context)
//     {
//         var mappedRequest = _mapper.MapToContentData(request);
//         
//         var result = await _contentBlockService.GetRenderedBlocksAsync(mappedRequest, context.CancellationToken);
//         
//         var mappedResponse = _mapper.MapToResponse(result);
//
//         var response = new GetEmailTemplateResponse();
//         
//         response.Blocks.AddRange(mappedResponse.Blocks);
//         
//         return response;
//     }
// }