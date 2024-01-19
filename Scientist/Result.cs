
using System;

namespace Scientist
{
    public class Result<T>(string name, TimeSpan duration, T value, bool isControl = false)
    {
        public string Name = name;
        public TimeSpan Duration = duration;
        public bool? Mismatched;
        public bool IsControl = isControl;
        public T Value = value;
    }
}
