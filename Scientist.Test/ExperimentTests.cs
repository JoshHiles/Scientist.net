using FakeItEasy;
using Scientist.Test.Helpers;
using System;

namespace Scientist.Test
{
    public class Experiments
    {
        [Test]
        public void WithoutACandidate()
        {
            int actualResult = 42;

            var testMethods = A.Fake<ITestClass<int>>();
            A.CallTo(() => testMethods.Control()).Returns(actualResult);
            A.CallTo(() => testMethods.Candidate()).Returns(actualResult);

            var result = new Experiment<int>(name: nameof(WithoutACandidate), control: testMethods.Control)
                .ThrowOnMismatch()
                .Run();

            A.CallTo(() => testMethods.Control()).MustHaveHappened();
            result.Should().Be(actualResult);
        }

        [Test]
        public void WithACandidate()
        {
            int actualResult = 42;

            var testMethods = A.Fake<ITestClass<int>>();
            A.CallTo(() => testMethods.Control()).Returns(actualResult);
            A.CallTo(() => testMethods.Candidate()).Returns(actualResult);

            var result = new Experiment<int>(name: nameof(WithACandidate), control: testMethods.Control)
                .ThrowOnMismatch()
                .AddCandidate(testMethods.Candidate)
                .Run();

            A.CallTo(() => testMethods.Control()).MustHaveHappened();
            A.CallTo(() => testMethods.Candidate()).MustHaveHappened();
            result.Should().Be(actualResult);
        }

        [Test]
        public void WithMultipleCandidates()
        {
            int actualResult = 42;

            var testMethods = A.Fake<ITestClass<int>>();
            var fakeCandidate = A.Fake<Func<int>>();
            A.CallTo(() => testMethods.Control()).Returns(actualResult);
            A.CallTo(() => testMethods.Candidate()).Returns(actualResult);
            A.CallTo(() => fakeCandidate()).Returns(actualResult);

            var result = new Experiment<int>(
                name: nameof(WithMultipleCandidates),
                control: testMethods.Control)
                 .ThrowOnMismatch()
                .AddCandidate(testMethods.Candidate)
                .AddCandidate(name: "Candidate 2", candidate: fakeCandidate)
                .Run();

            A.CallTo(() => testMethods.Control()).MustHaveHappened();
            A.CallTo(() => testMethods.Candidate()).MustHaveHappened();
            A.CallTo(() => fakeCandidate()).MustHaveHappened();
            result.Should().Be(actualResult);
        }
    }
}
