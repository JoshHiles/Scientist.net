using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Scientist
{
    public class Results<T>(
            string experimentName,
            TimeSpan overrallDuration,
            ConcurrentBag<Result<T>> results,
            bool overallMatch
        )
    {
        public string ExperimentName = experimentName;
        public TimeSpan OverrallDuration = overrallDuration;
        public bool OverrallMatch = overallMatch;
        public Result<T> Control = results.First(result => result.IsControl);
        public ConcurrentBag<Result<T>> Candidates = new(results.Where(x => !x.IsControl));
    }
}
