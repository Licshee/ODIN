using System;
using System.Threading.Tasks;

namespace ODIN
{
    public abstract class OptimizedAsyncHandlerBase<TInput, TArguments, TOutput>
        : OptimizedHandlerBase<TInput, TArguments, TOutput>
    {
        protected readonly static Task<TOutput> DefaultResultTask;

        static OptimizedAsyncHandlerBase()
        {
            var defaultTCS = new TaskCompletionSource<TOutput>();
            defaultTCS.SetResult(default(TOutput));
            DefaultResultTask = defaultTCS.Task;
        }

        public Task<TOutput> ExecuteAsync(TInput input)
        {
            return ExtractArgumentsAsync(input)
                .ContinueWith(argsTask =>
                {
                    var args = argsTask.Result;
                    return RetriveOptimizedAsync(args, input)
                    .ContinueWith(resultTask =>
                    {
                        var result = resultTask.Result;
                        if (result != null)
                            return resultTask;

                        return ProcessAsync(args);
                    }).Unwrap();
                }).Unwrap();
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

            return Task.Factory.StartNew(() => ExtractArgumentsAsync(input)).Unwrap().Result;
        }
        public virtual Task<TArguments> ExtractArgumentsAsync(TInput input)
        {
            _IsAsyncExtractArgumentsOverridden = false;
            EnsureExtractArgumentsOverridden();

            return Task.Factory.StartNew(() => ExtractArguments(input));
        }

        protected override TOutput RetriveOptimized(TArguments args, TInput input)
            => Task.Factory.StartNew(() => RetriveOptimizedAsync(args, input)).Unwrap().Result;
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

            return Task.Factory.StartNew(() => ProcessAsync(args)).Unwrap().Result;
        }
        protected virtual Task<TOutput> ProcessAsync(TArguments args)
        {
            _IsAsyncProcessOverridden = false;
            EnsureProcessOverridden();

            return Task.Factory.StartNew(() => Process(args));
        }
    }
}
