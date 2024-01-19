using FakeItEasy;
using Scientist.Test.Helpers;
using System.Threading.Tasks;

namespace Scientist.Test
{
    public class AsyncExperiments
    {
        [Test]
        public async Task AsynchronousExperimentAsync()
        {
            int actualResult = 42;

            var testMethods = A.Fake<ITestClassAsync<int>>();
            A.CallTo(() => testMethods.Control()).Returns(Task.FromResult(actualResult));
            A.CallTo(() => testMethods.Candidate()).Returns(Task.FromResult(actualResult));

            var result = await new Experiment<int>(
                name: nameof(AsynchronousExperimentAsync),
                control: testMethods.Control)
                .AddCandidate(testMethods.Candidate)
                .ThrowOnMismatch()
                .RunAsync();

            A.CallTo(() => testMethods.Control()).MustHaveHappened();
            result.Should().Be(actualResult);
        }
    }
}
