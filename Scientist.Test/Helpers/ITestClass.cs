using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scientist.Test.Helpers
{
    public interface ITestClass<T>
    {
        T Control();
        T Candidate();
    }
}
