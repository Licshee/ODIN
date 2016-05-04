using System;
using System.Threading;
using System.Threading.Tasks;

public abstract class TaskOdinFunction<TInput, TCriteria, TOutput> : OdinFunction<TInput, TCriteria, TOutput>
    where TCriteria : class
    where TOutput : class
{
    #region eliminate synchronous methods

    public override TOutput Invoke(TInput input)
    {
        throw new NotSupportedException();
    }
    protected override TOutput GetCachedOutput(TCriteria criteria, Func<TOutput> gen)
    {
        throw new NotImplementedException();
    }
    protected override Func<TOutput> ReadyOutputGenerator(TInput input, Action<TCriteria> setResult)
    {
        throw new NotImplementedException();
    }

    #endregion

    public virtual async Task<TOutput> Invoke(TInput input, CancellationToken cancellationToken = default(CancellationToken))
    {
        var tcs = new TaskCompletionSource<TCriteria>();
        var task = tcs.Task;

        var gen = await ReadyOutputGenerator(input, tcs.SetResult, cancellationToken);

        var criteria = task.IsCompleted ? task.Result : null;
        if (criteria == null)
            return await gen();

        return await GetCachedOutput(criteria, gen, cancellationToken);
    }

    protected abstract Task<TOutput> GetCachedOutput(TCriteria criteria, Func<Task<TOutput>> gen, CancellationToken cancellationToken = default(CancellationToken));
    protected abstract Task<Func<Task<TOutput>>> ReadyOutputGenerator(TInput input, Action<TCriteria> setResult, CancellationToken cancellationToken = default(CancellationToken));
}
