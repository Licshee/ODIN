namespace ODIN
{
    public abstract class OptimizedHandlerBase<TInput, TArguments, TOutput>
    {
        public abstract TArguments ExtractArguments(TInput input);

        protected virtual bool IsCacheable(TArguments args)
            => true;

        protected virtual TOutput RetriveOptimized(TArguments args, TInput input)
            => default(TOutput);

        protected abstract TOutput Process(TArguments args);

        public TOutput Execute(TInput input)
        {
            var args = ExtractArguments(input);

            if (IsCacheable(args))
            {
                var result = RetriveOptimized(args, input);
                if (result != null)
                    return result;
            }

            return Process(args);
        }
    }
}
