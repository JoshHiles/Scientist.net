namespace Scientist.Test.Helpers
{
    public interface ITestClass<T>
    {
        T Control();
        T Candidate();
    }
}
