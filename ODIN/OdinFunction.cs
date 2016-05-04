using System;
using System.Threading.Tasks;

public abstract class OdinFunction<TInput, TCriteria, TOutput>
    where TCriteria : class
    where TOutput : class
{
    public virtual TOutput Invoke(TInput input)
    {
        var tcs = new TaskCompletionSource<TCriteria>();
        var task = tcs.Task;

        var gen = ReadyOutputGenerator(input, tcs.SetResult);

        var criteria = task.IsCompleted ? task.Result : null;
        if (criteria == null)
            return gen();

        return GetCachedOutput(criteria, gen);
    }

    protected abstract TOutput GetCachedOutput(TCriteria criteria, Func<TOutput> gen);
    protected abstract Func<TOutput> ReadyOutputGenerator(TInput input, Action<TCriteria> setResult);
}
