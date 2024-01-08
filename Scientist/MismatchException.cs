using System;

namespace Scientist
{
    public class MismatchException<T> : Exception
    {
        public MismatchException(string name, T result)
            : base($"Experiment '{name}' observations mismatched")
        {
            Name = name;
            Result = result;
        }

        public string Name { get; }

        public T Result { get; }
    }
}
