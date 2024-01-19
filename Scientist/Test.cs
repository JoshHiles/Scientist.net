using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Scientist
{
    internal static class Test<T>
    {
        public static async Task<Result<T>> Perform(Func<Task<T>> control)
        {
            return await Run("Control", control);
        }
        public static async Task<Result<T>> Perform(string candidateName, Func<Task<T>> candidate, Func<T, T, bool> comparator, T controlResult, bool throwOnMismatch)
        {
            var result = await Run(candidateName, candidate);
            var valuesAreEqual = comparator(controlResult, result.Value);
            if (result.Value != null && !valuesAreEqual)
            {
                result.Mismatched = true;

                if (throwOnMismatch)
                    throw new MismatchException<T>(nameof(candidate), result.Value);
            }
            result.Mismatched = false;

            return result;
        }

        private static async Task<Result<T>> Run(string name, Func<Task<T>> function)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = await Task.Run(function);
            stopwatch.Stop();

            return new Result<T>(name: name, duration: stopwatch.Elapsed, value: result, isControl: true);
        }
    }
}
