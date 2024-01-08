using System;

namespace Scientist
{
    public interface IExperiment<T>
    {
        IExperiment<T> AddCandidate(Func<T> candidate);
        IExperiment<T> Compare(Func<T,T, bool> comparison);
        T Run();
        T RunIf(Func<bool> func);
    }
}
