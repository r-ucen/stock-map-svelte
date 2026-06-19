using System.Threading.Channels;
using StockMapSvelte.Application.Abstractions;

namespace StockMapSvelte.Infrastructure.Services;

public class TreeMapUpdateNotifier : ITreeMapUpdateNotifier
{
    private readonly Lock _lock = new();
    private readonly List<ChannelWriter<bool>> _subscribers = [];
    
    public void Publish()
    {
        lock (_lock)
        {
            foreach (var writer in _subscribers)
            {
                writer.TryWrite(true);
            }
        }
    }
    
    public IAsyncEnumerable<bool> Subscribe(CancellationToken cancellationToken)
    {
        var channel = Channel.CreateBounded<bool>(new BoundedChannelOptions(1)
        {
            FullMode = BoundedChannelFullMode.DropOldest
        });
        
        lock (_lock)
        {
            _subscribers.Add(channel.Writer);
        }
        
        cancellationToken.Register(() =>
        {
            lock (_lock)
            {
                _subscribers.Remove(channel.Writer);
                channel.Writer.TryComplete();
            }
        });
        
        return channel.Reader.ReadAllAsync(cancellationToken);
    }
}