using FakeItEasy;
using Scientist.Test.Helpers;
using System;

namespace Scientist.Test
{
    public class RunIf
    {

        [Test]
        public void ItDoesntRunCandidate()
        {
            int actualResult = 42;

            var testMethods = A.Fake<ITestClass<int>>();
            var runIf = A.Fake<Func<bool>>();
            A.CallTo(() => testMethods.Control()).Returns(actualResult);
            A.CallTo(() => runIf()).Returns(false);

            var result = new Experiment<int>(
                name: nameof(ItDoesntRunCandidate),
                control: testMethods.Control)
                .ThrowOnMismatch()
                .AddCandidate(testMethods.Candidate)
                .RunIf(runIf);

            A.CallTo(() => testMethods.Control()).MustHaveHappened();
            A.CallTo(() => runIf()).MustHaveHappened();
            A.CallTo(() => testMethods.Candidate()).MustNotHaveHappened();
            result.Should().Be(actualResult);
        }

        [Test]
        public void ItRunsCandidate()
        {
            int actualResult = 42;

            var testMethods = A.Fake<ITestClass<int>>();
            var runIf = A.Fake<Func<bool>>();
            A.CallTo(() => testMethods.Control()).Returns(actualResult);
            A.CallTo(() => testMethods.Candidate()).Returns(actualResult);
            A.CallTo(() => runIf()).Returns(true);

            var result = new Experiment<int>(
                name: nameof(ItRunsCandidate),
                control: testMethods.Control)
                .ThrowOnMismatch()
                .AddCandidate(testMethods.Candidate)
                .RunIf(runIf);

            A.CallTo(() => testMethods.Control()).MustHaveHappened();
            A.CallTo(() => runIf()).MustHaveHappened();
            A.CallTo(() => testMethods.Candidate()).MustHaveHappened();
            result.Should().Be(actualResult);
        }
    }
}
