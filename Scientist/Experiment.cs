using System;
using System.Collections.Generic;

namespace Scientist
{
    public class Experiment<T>(string name, Func<T> control, bool throwOnMismatch = false) : IExperiment<T>
    {
        private readonly List<Func<T>> _candidates = [];
        public IExperiment<T> AddCandidate(Func<T> candidate)
        {
            _candidates.Add(candidate);
            return this;
        }

        private Func<T, T, bool> _comparison = DefaultComparison;
        private bool _comparisonOverride = false;
        public IExperiment<T> Compare(Func<T, T, bool> comparison)
        {
            if (_comparisonOverride)
                throw new ArgumentException("Compare has already been set", nameof(comparison));

            _comparisonOverride = true;
            _comparison = comparison;

            return this;
        }
        static readonly Func<T, T, bool> DefaultComparison = (instance, comparand) =>
        {
            return (instance == null && comparand == null)
                || (instance != null && instance.Equals(comparand))
                || (CompareInstances(instance as IEquatable<T>, comparand));
        };
        static bool CompareInstances(IEquatable<T> instance, T comparand) => instance != null && instance.Equals(comparand);


        public T Run()
        {
            var controlResult = control.Invoke();

            CandidateRunner(controlResult);

            return controlResult;
        }

        public T RunIf(Func<bool> runIf)
        {
            var controlResult = control.Invoke();
            if (runIf.Invoke())
            {
                CandidateRunner(controlResult);
            }
            else
            {
                // TODO: Should alert that runIf was false?
            }

            return controlResult;
        }

        private void CandidateRunner(T controlResult)
        {
            foreach (var candidate in _candidates)
            {
                var candidateResult = candidate.Invoke();

                var result = _comparison(controlResult, controlResult);
                if (candidateResult != null && !result && throwOnMismatch)
                {
                    throw new MismatchException<T>(name, candidateResult);
                }
            }
        }
    }
}
