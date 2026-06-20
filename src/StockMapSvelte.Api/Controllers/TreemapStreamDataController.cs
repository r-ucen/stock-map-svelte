using System.Runtime.CompilerServices;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.DTOs;

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
                var updates = GetUpdatesAsync(portfolioId, treeMapFacade, treeMapNotifier, cancellationToken);
                return Results.ServerSentEvents(updates, eventType: "treemap-update");
            }
        )
        .RequireAuthorization()
        .RequireRateLimiting("TreeMapDataPolicy");
        
        return app;
    }
    
    private static async IAsyncEnumerable<TreemapDataDto> GetUpdatesAsync(
        Guid portfolioId,
        ITreeMapFacade treeMapFacade,
        ITreeMapUpdateNotifier treeMapNotifier,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        yield return await treeMapFacade.GetTreemapDataViewModelByIdAsync(portfolioId, cancellationToken);

        await foreach (var _ in treeMapNotifier.Subscribe(cancellationToken))
        {
            if (cancellationToken.IsCancellationRequested) yield break;
            yield return await treeMapFacade.GetTreemapDataViewModelByIdAsync(portfolioId, cancellationToken);
        }
    }
}