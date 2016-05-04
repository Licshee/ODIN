#if NETFX
using System;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

public abstract class OmegaTaskOdinFunction<TInput, TOutput> : AlphaTaskOdinFunction<TInput, TOutput>
    where TOutput : class
{
    public OmegaTaskOdinFunction(ObjectCache cacheSet, string regionName = null)
        : base(cacheSet, regionName)
    { }

    public override async Task<TOutput> Invoke(TInput input, CancellationToken cancellationToken = default(CancellationToken))
    {
        var tcs = new TaskCompletionSource<object>();
        var task = tcs.Task;

        var gen = await ReadyOutputGenerator(input, tcs.SetResult, cancellationToken);

        var key = task.IsCompleted ? task.Result : null;
        if (key == null)
            return await gen();

        return await GetCachedOutput(key, gen, cancellationToken);
    }

    protected abstract Task<Func<Task<TOutput>>> ReadyOutputGenerator(TInput input, Action<object> setResult, CancellationToken cancellationToken = default(CancellationToken));
    protected sealed override Task<Func<Task<TOutput>>> ReadyOutputGenerator(TInput input, Action<Tuple<string, CacheItemPolicy>> setResult, CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotSupportedException();
    }

    protected abstract Task<CacheItemPolicy> GetCachePolicy(object key);

    protected virtual async Task<TOutput> GetCachedOutput(object key, Func<Task<TOutput>> gen, CancellationToken cancellationToken)
        => await GetCachedOutput(Tuple.Create(key.ToString(), await GetCachePolicy(key)), gen, cancellationToken);
}
#endif
