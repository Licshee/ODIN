namespace ODIN
{
    public abstract class OptimizedHandlerBase<TInput, TArguments, TOutput>
        : IOptimizedHandler<TInput, TOutput>
    {
        public TOutput Execute(TInput input)
        {
            var args = ExtractArguments(input);

            var result = RetriveOptimized(args, input);
            if (result != null)
                return result;

            return Process(args);
        }

        public abstract TArguments ExtractArguments(TInput input);

        protected virtual TOutput RetriveOptimized(TArguments args, TInput input)
            => default(TOutput);

        protected abstract TOutput Process(TArguments args);
    }
}
