using System.Threading.Tasks;

namespace ODIN
{
    public interface IOptimizedAsyncHandler<TInput, TOutput>
    {
        Task<TOutput> ExecuteAsync(TInput input);
    }
}
