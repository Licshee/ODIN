namespace ODIN
{
    public interface IOptimizedHandler<TInput, TOutput>
    {
        TOutput Execute(TInput input);
    }
}
