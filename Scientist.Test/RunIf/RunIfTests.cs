using FakeItEasy;
using Scientist.Test.Helpers;
using System;

namespace Scientist.Test.RunIf
{
    public class RunIfTests
    {
     
        [Test]
        public void RunIfFalse()
        {
            int actualResult = 42;

            var testMethods = A.Fake<ITestClass<int>>();
            var runIf = A.Fake<Func<bool>>();
            A.CallTo(() => testMethods.Control()).Returns(actualResult);
            A.CallTo(() => runIf()).Returns(false);

            var result = new Experiment<int>(
                name: nameof(RunIfFalse),
                control: testMethods.Control)
                .AddCandidate(testMethods.Candidate)
                .RunIf(runIf);

            A.CallTo(() => testMethods.Control()).MustHaveHappened();
            A.CallTo(() => runIf()).MustHaveHappened();
            A.CallTo(() => testMethods.Candidate()).MustNotHaveHappened();
            result.Should().Be(actualResult);
        }

        [Test]
        public void RunIfTrue()
        {
            int actualResult = 42;

            var testMethods = A.Fake<ITestClass<int>>();
            var runIf = A.Fake<Func<bool>>();
            A.CallTo(() => testMethods.Control()).Returns(actualResult);
            A.CallTo(() => runIf()).Returns(true);

            var result = new Experiment<int>(
                name: nameof(RunIfTrue),
                control: testMethods.Control)
                .AddCandidate(testMethods.Candidate)
                .RunIf(runIf);

            A.CallTo(() => testMethods.Control()).MustHaveHappened();
            A.CallTo(() => runIf()).MustHaveHappened();
            A.CallTo(() => testMethods.Candidate()).MustHaveHappened();
            result.Should().Be(actualResult);
        }
    }
}
