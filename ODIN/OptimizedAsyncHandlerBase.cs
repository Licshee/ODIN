using System;
using System.Threading.Tasks;

namespace ODIN
{
#if !NET40
    using TaskEx = Task;
#endif

    public abstract class OptimizedAsyncHandlerBase<TInput, TArguments, TOutput>
        : OptimizedHandlerBase<TInput, TArguments, TOutput>
    {
        protected readonly static Task<TOutput> DefaultResultTask = TaskEx.FromResult(default(TOutput));

        public async Task<TOutput> ExecuteAsync(TInput input)
        {
            var args = await ExtractArgumentsAsync(input);

            var result = await RetriveOptimizedAsync(args, input);
            if (result != null)
                return result;

            return await ProcessAsync(args);
        }

        private bool _IsSyncExtractArgumentsOverridden = true;
        private bool _IsAsyncExtractArgumentsOverridden = true;
        private void EnsureExtractArgumentsOverridden()
        {
            if (_IsSyncExtractArgumentsOverridden || _IsAsyncExtractArgumentsOverridden)
                return;
            throw new NotImplementedException();
        }
        public override TArguments ExtractArguments(TInput input)
        {
            _IsSyncExtractArgumentsOverridden = false;
            EnsureExtractArgumentsOverridden();

            return TaskEx.Run(() => ExtractArgumentsAsync(input)).Result;
        }
        public virtual Task<TArguments> ExtractArgumentsAsync(TInput input)
        {
            _IsAsyncExtractArgumentsOverridden = false;
            EnsureExtractArgumentsOverridden();

            return TaskEx.Run(() => ExtractArguments(input));
        }

        protected override TOutput RetriveOptimized(TArguments args, TInput input)
            => TaskEx.Run(() =>RetriveOptimizedAsync(args, input)).Result;
        protected virtual Task<TOutput> RetriveOptimizedAsync(TArguments args, TInput input)
            => DefaultResultTask;

        private bool _IsSyncProcessOverridden = true;
        private bool _IsAsyncProcessOverridden = true;
        private void EnsureProcessOverridden()
        {
            if (_IsSyncProcessOverridden || _IsAsyncProcessOverridden)
                return;
            throw new NotImplementedException();
        }
        protected override TOutput Process(TArguments args)
        {
            _IsSyncProcessOverridden = false;
            EnsureProcessOverridden();

            return TaskEx.Run(() => ProcessAsync(args)).Result;
        }
        protected virtual Task<TOutput> ProcessAsync(TArguments args)
        {
            _IsAsyncProcessOverridden = false;
            EnsureProcessOverridden();

            return TaskEx.Run(() => Process(args));
        }
    }
}
