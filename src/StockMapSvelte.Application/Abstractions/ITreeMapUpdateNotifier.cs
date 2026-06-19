namespace StockMapSvelte.Application.Abstractions;

public interface ITreeMapUpdateNotifier
{
    public void Publish();
    public IAsyncEnumerable<bool> Subscribe(CancellationToken cancellationToken);
    
}