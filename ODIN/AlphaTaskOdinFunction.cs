#if NETFX
using System;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

public abstract class AlphaTaskOdinFunction<TInput, TOutput> : TaskOdinFunction<TInput, Tuple<string, CacheItemPolicy>, TOutput>
    where TOutput : class
{
    private readonly ObjectCache _Cache;
    private readonly string _Region;

    public AlphaTaskOdinFunction(ObjectCache cacheSet, string regionName = null)
    {
        _Cache = cacheSet;
    }

    protected override Task<TOutput> GetCachedOutput(Tuple<string, CacheItemPolicy> criteria, Func<Task<TOutput>> gen, CancellationToken cancellationToken = default(CancellationToken))
    {
        var value = new Lazy<Task<TOutput>>(gen);
        value = (Lazy<Task<TOutput>>)_Cache.AddOrGetExisting(criteria.Item1, value, criteria.Item2, _Region);
        return value.Value;
    }
}
#endif
