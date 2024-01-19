using FakeItEasy;
using Scientist.Test.Helpers;
using System;

namespace Scientist.Test
{
    public class Comparisons
    {
        [Test]
        public void OverrideComparison()
        {
            var testMethods = A.Fake<ITestClass<ComplexResult>>();

            var complexResult = A.Fake<ComplexResult>();
            complexResult.Name = "Tester";
            complexResult.Count = 10;
            A.CallTo(() => testMethods.Control()).Returns(complexResult);
            A.CallTo(() => testMethods.Candidate()).Returns(complexResult);

            var result = new Experiment<ComplexResult>(
                    name: nameof(OverrideComparison),
                    control: testMethods.Control
                )
                .ThrowOnMismatch()
                .AddCandidate(testMethods.Candidate)
                .Compare((a, b) => a.Count == b.Count && a.Name == b.Name)
                .Run();

            A.CallTo(() => testMethods.Control()).MustHaveHappened();
            A.CallTo(() => testMethods.Candidate()).MustHaveHappened();
            result.Should().Be(complexResult);
        }

        [Test]
        public void ThrowMismatchOnOverrideComparison()
        {
            var testMethods = A.Fake<ITestClass<ComplexResult>>();

            var complexResult = A.Fake<ComplexResult>();
            complexResult.Name = "Tester";
            complexResult.Count = 10;
            A.CallTo(() => testMethods.Control()).Returns(complexResult);
            A.CallTo(() => testMethods.Candidate()).Returns(complexResult);

            Action act = () => new Experiment<ComplexResult>(
                    name: nameof(ThrowMismatchOnOverrideComparison),
                    control: testMethods.Control
                )
            .ThrowOnMismatch()
                .AddCandidate(testMethods.Candidate)
                .Compare((a, b) => a.Count == 2 && a.Name == b.Name)
                .Run();

            act.Should().Throw<MismatchException<ComplexResult>>()
               .WithMessage($"Experiment '{nameof(testMethods.Candidate)}' observations mismatched");
            A.CallTo(() => testMethods.Control()).MustHaveHappened();
            A.CallTo(() => testMethods.Candidate()).MustHaveHappened();
        }
    }
}
