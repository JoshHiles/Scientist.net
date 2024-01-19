using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Scientist
{
    public class Experiment<T> 
    {
        public string Name = nameof(T);
        private Dictionary<string, Func<Task<T>>> _candidates = [];
        private bool _asyncExperiment = false;

        public Experiment(string name, Func<T> control)
        {
            Name = name;
            _candidates.Add("control", () => Task.FromResult(control()));
        }
        public Experiment(string name, Func<Task<T>> control)
        {
            Name = name;
            _asyncExperiment = true;
            _candidates.Add("control", control);
        }

        private bool _throwOnMismatch = false;
        public Experiment<T> ThrowOnMismatch()
        {
            _throwOnMismatch = true;
            return this;
        }

        public Experiment<T> AddCandidate(Func<T> candidate, string name = "")
        {
            AddCandidate(
                    name == "" ? nameof(candidate) : name,
                    () => Task.FromResult(candidate())
                 );

            return this;
        }

        public Experiment<T> AddCandidate(Func<Task<T>> candidate, string name = "")
        {
            AddCandidate(name == "" ? nameof(candidate) : name, candidate);

            return this;
        }

        private Experiment<T> AddCandidate(string name, Func<Task<T>> candidate
           )
        {
            if (_candidates.ContainsKey(name))
            {
                throw new InvalidOperationException($"You've already added {name}");
            }
            _candidates.Add(name, candidate);
            return this;
        }

        private Func<T, T, bool> _comparator = (instance, comparand) =>
        {
            return (instance == null && comparand == null)
                || (instance != null && instance.Equals(comparand))
                || CompareInstances(instance as IEquatable<T>, comparand);
        };

        static bool CompareInstances(IEquatable<T> instance, T comparand) => instance != null && instance.Equals(comparand);

        private bool _comparisonOverride = false;
        public Experiment<T> Compare(Func<T, T, bool> comparison)
        {
            if (_comparisonOverride)
                throw new ArgumentException("Compare has already been set", nameof(comparison));

            _comparisonOverride = true;
            _comparator = comparison;

            return this;
        }

        private readonly Dictionary<string, dynamic> _contexts = [];
        public Experiment<T> AddContext(string key, dynamic data)
        {
            _contexts.Add(key, data);
            return this;
        }


        private Func<bool>? _runIf;
        public T RunIf(Func<bool> runIf)
        {
            _runIf = runIf;
            return Run();
        }

        private Results<T>? _results;
        public T Run()
        {
            return Task.Run(CandidateRunner).Result;
        }

        public async Task<T> RunAsync()
        {
            return await CandidateRunner();
        }

        private async Task<T> CandidateRunner()
        {
            var stopwatch = Stopwatch.StartNew();

            var observations = new ConcurrentBag<Result<T>>
            {
                await Test<T>.Perform(control: _candidates.First(candidate => candidate.Key == "control").Value)
            };



           await Parallel.ForEachAsync(_candidates.Where(candidate => candidate.Key != "control"), async(candidate, cancellationToken)=>
            {
                if (_runIf == null || _runIf.Invoke())
                {
                    observations.Add(
                        await Test<T>.Perform(
                            candidateName: candidate.Key,
                            candidate: candidate.Value,
                            comparator: _comparator,
                            controlResult: observations.First(x => x.IsControl).Value,
                            throwOnMismatch: _throwOnMismatch
                        )
                    );
                }
            });
            stopwatch.Stop();

            _results = new Results<T>(
                experimentName: Name,
                overrallDuration: stopwatch.Elapsed,
                results: observations,
                overallMatch: observations.Any(x => x.Mismatched == true)
                ); ;

            return _results.Control.Value;
        }

        public void Publish(IPublisher publisher)
        {
            publisher.Publish(_results);
        }

        public async void PublishAsync(IPublisher publisher)
        {
            await publisher.Publish(_results);
        }
    }
}
