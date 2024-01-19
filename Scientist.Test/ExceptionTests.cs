using FakeItEasy;
using Scientist.Test.Helpers;
using System;

namespace Scientist.Test
{
    public class Exceptions
    {
       
        [Test]
        public void ThrowOnMismatch()
        {
            int actualResult = 42;

            var testMethods = A.Fake<ITestClass<int>>();
            A.CallTo(() => testMethods.Control()).Returns(actualResult);
            A.CallTo(() => testMethods.Candidate()).Returns(33);

            Action act = () => new Experiment<int>(
                name: nameof(ThrowOnMismatch),
                control: testMethods.Control)
                .ThrowOnMismatch()
                .AddCandidate(testMethods.Candidate)
                .Run();

            act.Should().Throw<MismatchException<int>>()
                .WithMessage($"Experiment '{nameof(testMethods.Candidate)}' observations mismatched");
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
