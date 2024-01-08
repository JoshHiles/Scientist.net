using FakeItEasy;
using Scientist.Test.Helpers;
using System;

namespace Scientist.Test.Experiment
{
    public class ExperimentTests
    {
        [Test]
        public void WithoutACandidate()
        {
            int actualResult = 42;

            var testMethods = A.Fake<ITestClass<int>>();
            A.CallTo(() => testMethods.Control()).Returns(actualResult);
            A.CallTo(() => testMethods.Candidate()).Returns(actualResult);

            var result = new Experiment<int>(name: nameof(WithoutACandidate), control: testMethods.Control, throwOnMismatch: true).Run();

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

            var result = new Experiment<int>(name: nameof(WithACandidate), control: testMethods.Control, throwOnMismatch: true).AddCandidate(testMethods.Candidate)
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
                control: testMethods.Control, throwOnMismatch: true)
                .AddCandidate(testMethods.Candidate)
                .AddCandidate(fakeCandidate)
                .Run();

            A.CallTo(() => testMethods.Control()).MustHaveHappened();
            A.CallTo(() => testMethods.Candidate()).MustHaveHappened();
            A.CallTo(() => fakeCandidate()).MustHaveHappened();
            result.Should().Be(actualResult);
        }

        [Test]
        public void ThrowOnMismatch()
        {
            int actualResult = 42;

            var testMethods = A.Fake<ITestClass<int>>();
            var fakeCandidate = A.Fake<Func<int>>();
            A.CallTo(() => testMethods.Control()).Returns(actualResult);
            A.CallTo(() => testMethods.Candidate()).Returns(33);
            A.CallTo(() => fakeCandidate()).Returns(21);

            Action act = () => new Experiment<int>(
                name: nameof(ThrowOnMismatch),
                control: testMethods.Control, throwOnMismatch: true)
                .AddCandidate(testMethods.Candidate)
                .AddCandidate(testMethods.Candidate)
                .Run();

            act.Should().Throw<MismatchException<int>>()
                .WithMessage($"Experiment '{nameof(ThrowOnMismatch)}' observations mismatched");
            A.CallTo(() => testMethods.Control()).MustHaveHappened();
            A.CallTo(() => testMethods.Candidate()).MustHaveHappened();
        }


        [Test]
        public void ThrowOnMismatchTurnedOffDoesntThrow()
        {
            int actualResult = 42;

            var testMethods = A.Fake<ITestClass<int>>();
            var fakeCandidate = A.Fake<Func<int>>();
            A.CallTo(() => testMethods.Control()).Returns(actualResult);
            A.CallTo(() => testMethods.Candidate()).Returns(33);

            Action act = () => new Experiment<int>(
                name: nameof(ThrowOnMismatchTurnedOffDoesntThrow),
                control: testMethods.Control)
                .AddCandidate(testMethods.Candidate)
                .Run();

            act.Should().NotThrow<MismatchException<int>>();
            A.CallTo(() => testMethods.Control()).MustHaveHappened();
            A.CallTo(() => testMethods.Candidate()).MustHaveHappened();
        }
    }
}
