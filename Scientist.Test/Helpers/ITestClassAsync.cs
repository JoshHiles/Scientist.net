using System.Threading.Tasks;

namespace Scientist.Test.Helpers
{
    public interface ITestClassAsync<T>
    {
        Task<T> Control();
        Task<T> Candidate();
    }
}
