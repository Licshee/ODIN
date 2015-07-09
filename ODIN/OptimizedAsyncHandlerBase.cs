using System;
using System.Threading.Tasks;

namespace ODIN
{
#if NET45
    using TaskEx = Task;
#endif

    public abstract class OptimizedAsyncHandlerBase<TInput, TArguments, TOutput>
        : OptimizedHandlerBase<TInput, TArguments, TOutput>
    {
        protected readonly static Task<TOutput> DefaultResultTask;

        static OptimizedAsyncHandlerBase()
        {
#if LegacyTask
            var defaultTCS = new TaskCompletionSource<TOutput>();
            defaultTCS.SetResult(default(TOutput));
            DefaultResultTask = defaultTCS.Task;
#else
            DefaultResultTask = TaskEx.FromResult(default(TOutput));
#endif
        }

#if LegacyTask
        public Task<TOutput> ExecuteAsync(TInput input)
        {
            return ExtractArgumentsAsync(input)
                .ContinueWith(argsTask =>
                {
                    var args = argsTask.Result;

                    if (!IsCacheable(args))
                        return ProcessAsync(args);

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
#else
        public async Task<TOutput> ExecuteAsync(TInput input)
        {
            var args = await ExtractArgumentsAsync(input);

            if (IsCacheable(args))
            {
                var result = await RetriveOptimizedAsync(args, input);
                if (result != null)
                    return result;
            }

            return await ProcessAsync(args);
        }
#endif

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
#if LegacyTask
            return Task.Factory.StartNew(() => ExtractArgumentsAsync(input)).Unwrap().Result;
#else
            return TaskEx.Run(() => ExtractArgumentsAsync(input)).Result;
#endif
        }
        public virtual Task<TArguments> ExtractArgumentsAsync(TInput input)
        {
            _IsAsyncExtractArgumentsOverridden = false;
            EnsureExtractArgumentsOverridden();

#if LegacyTask
            return Task.Factory.StartNew(() => ExtractArguments(input));
#else
            return TaskEx.Run(() => ExtractArguments(input));
#endif
        }

        protected override TOutput RetriveOptimized(TArguments args, TInput input)
#if LegacyTask
            => Task.Factory.StartNew(() => RetriveOptimizedAsync(args, input)).Unwrap().Result;
#else
            => TaskEx.Run(() => RetriveOptimizedAsync(args, input)).Result;
#endif
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

#if LegacyTask
            return Task.Factory.StartNew(() => ProcessAsync(args)).Unwrap().Result;
#else
            return TaskEx.Run(() => ProcessAsync(args)).Result;
#endif
        }
        protected virtual Task<TOutput> ProcessAsync(TArguments args)
        {
            _IsAsyncProcessOverridden = false;
            EnsureProcessOverridden();

#if LegacyTask
            return Task.Factory.StartNew(() => Process(args));
#else
            return TaskEx.Run(() => Process(args));
#endif
        }
    }
}
