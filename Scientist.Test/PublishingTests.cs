using FakeItEasy;
using NUnit.Framework.Internal;
using Scientist.Test.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Scientist.Test
{
    public class Publishing
    {
        [Test]
        public void WithPublisher()
        {
            int actualResult = 42;

            var testMethods = A.Fake<ITestClass<int>>();
            A.CallTo(() => testMethods.Control()).Returns(actualResult);
            A.CallTo(() => testMethods.Candidate()).Returns(actualResult);

            var testPublisher = A.Fake<IPublisher>();
            A.CallTo(() => testPublisher.Publish(A<Results<int>>.Ignored));

            var experiment = new Experiment<int>(name: nameof(WithPublisher), control: testMethods.Control)
                .ThrowOnMismatch();
            var result = experiment.Run();
            experiment.Publish(testPublisher);

            A.CallTo(() => testMethods.Control()).MustHaveHappened();
            result.Should().Be(actualResult);
            A.CallTo(() => testPublisher.Publish(A<Results<int>>.Ignored)).MustHaveHappened();
        }

        [Test]
        public void WithAsyncPublisher()
        {
            int actualResult = 42;

            var testMethods = A.Fake<ITestClass<int>>();
            A.CallTo(() => testMethods.Control()).Returns(actualResult);
            A.CallTo(() => testMethods.Candidate()).Returns(actualResult);

            var testPublisher = A.Fake<IPublisher>();
            A.CallTo(() => testPublisher.Publish(A<Results<int>>.Ignored));

            var experiment = new Experiment<int>(name: nameof(WithAsyncPublisher), control: testMethods.Control)
                .ThrowOnMismatch();
            var result = experiment.Run();
            experiment.PublishAsync(testPublisher);

            A.CallTo(() => testMethods.Control()).MustHaveHappened();
            result.Should().Be(actualResult);
            A.CallTo(() => testPublisher.Publish(A<Results<int>>.Ignored)).MustHaveHappened();
        }

        [Test]
        public async Task FireAndForgetAsync()
        {
            int actualResult = 42;

            var pendingPublishTask = new TaskCompletionSource<object>();

            var testPublisher = A.Fake<IPublisher>();
            A.CallTo(() => testPublisher.Publish(A<Results<int>>.Ignored)).Returns(pendingPublishTask.Task);

            var testMethods = A.Fake<ITestClass<int>>();
            A.CallTo(() => testMethods.Control()).Returns(actualResult);
            A.CallTo(() => testMethods.Candidate()).Returns(actualResult);

            var fireAndForgetPublisher = new FireAndForgetResultPublisher(testPublisher, ex => { });


            const int count = 10;
            Parallel.ForEach(
                Enumerable.Repeat(0, count),
                src =>
                {
                    var experiment = new Experiment<int>(
                            name: $"{nameof(WithAsyncPublisher)}-{count}",
                            control: testMethods.Control
                        )
                            .ThrowOnMismatch()
                            .AddCandidate(testMethods.Candidate);

                    var result = experiment.Run();
                    experiment.Publish(fireAndForgetPublisher);

                    result.Should().Be(actualResult);
                });

            Task whenPublished = fireAndForgetPublisher.WhenPublished();
            whenPublished.Should().NotBeNull();

            A.CallTo(() => testMethods.Control()).MustHaveHappened(count, Times.Exactly);
            A.CallTo(() => testMethods.Candidate()).MustHaveHappened(count, Times.Exactly);

            whenPublished.IsCompleted.Should().BeFalse("When Published Task completed early.");

            pendingPublishTask.SetResult(null);

            await whenPublished;
            whenPublished.IsCompleted.Should().BeTrue("When Published Task isn't complete.");
        }

        [Test]
        public void WithContext()
        {
            int actualResult = 42;

            var testMethods = A.Fake<ITestClass<int>>();
            A.CallTo(() => testMethods.Control()).Returns(actualResult);
            A.CallTo(() => testMethods.Candidate()).Returns(actualResult);

            var testPublisher = A.Fake<IPublisher>();
            A.CallTo(() => testPublisher.Publish(A<Results<int>>.Ignored));

            var experiment = new Experiment<int>(name: nameof(WithPublisher), control: testMethods.Control)
                .ThrowOnMismatch();
            var result = experiment.Run();
            experiment.Publish(testPublisher);

            A.CallTo(() => testMethods.Control()).MustHaveHappened();
            result.Should().Be(actualResult);
            A.CallTo(() => testPublisher.Publish(A<Results<int>>.Ignored)).MustHaveHappened();

            
            var publishResults = TestHelper.Results<int>(experimentName).First();

            Assert.Equal(42, result);
            Assert.Equal(1, publishResults.Contexts.Count);

            var context = publishResults.Contexts.First();
            Assert.Equal("test", context.Key);
            Assert.Equal("data", context.Value);
        }
    }
}
