using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scientist
{
    public interface IPublisher
    {
        Task Publish<T>(Results<T> results);
    }
}
