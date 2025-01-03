using Grpc.Core;
using Node;

namespace PublicMailbox.Node.Services;

public class GrpcHandlerService(ILogger<GrpcHandlerService> logger) : NodeService.NodeServiceBase
{
    public sealed override Task<QueryResponse> Query(QueryRequest request, ServerCallContext context)
    {
        return base.Query(request, context);
    }

    public sealed override Task<PurgeResponse> Purge(PurgeRequest request, ServerCallContext context)
    {
        return base.Purge(request, context);
    }
}