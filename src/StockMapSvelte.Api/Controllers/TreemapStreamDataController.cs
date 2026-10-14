using System.Runtime.CompilerServices;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.UseCases.TreeMapUseCases.Queries;

namespace StockMapSvelte.Api.Controllers;

public static class TreemapStreamDataController
{
    public static IEndpointRouteBuilder MapPortfolioStreamEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("treemap-data/{portfolioId:guid}/stream", (
                    Guid portfolioId,
                    ITreeMapFacade treeMapFacade,
                    ITreeMapUpdateNotifier treeMapNotifier,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetTreemapDataQuery(portfolioId);
                    var updates = GetUpdatesAsync(query, treeMapFacade, treeMapNotifier, cancellationToken);
                    return Results.ServerSentEvents(updates, eventType: "treemap-update");
                }
            )
            .RequireAuthorization()
            .RequireRateLimiting("TreeMapDataPolicy")
            .WithDescription(
                "SSE stream which automatically sends new data when a stock update occurs (StockDataUpdateTimedService). The stream sends data ready to build the treemap for the given portfolio.");
        
        return app;
    }
    
    private static async IAsyncEnumerable<TreemapDataDto> GetUpdatesAsync(
        GetTreemapDataQuery query,
        ITreeMapFacade treeMapFacade,
        ITreeMapUpdateNotifier treeMapNotifier,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        yield return await treeMapFacade.GetTreemapDataViewModelByIdAsync(query, cancellationToken);

        await foreach (var _ in treeMapNotifier.Subscribe(cancellationToken))
        {
            if (cancellationToken.IsCancellationRequested) yield break;
            yield return await treeMapFacade.GetTreemapDataViewModelByIdAsync(query, cancellationToken);
        }
    }
}