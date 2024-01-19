using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scientist
{
    public class FireAndForgetResultPublisher : IPublisher
    {
        private readonly IPublisher _publisher;
        private readonly Action<Exception> _onPublisherException;
        private readonly ConcurrentSet<Task> _publishingTasks = new ConcurrentSet<Task>();

        public FireAndForgetResultPublisher(IPublisher publisher) : this(publisher, e => { })
        {
        }

        public FireAndForgetResultPublisher(IPublisher publisher, Action<Exception> onPublisherException)
        {
            _publisher = publisher;
            _onPublisherException = onPublisherException;
        }

        public Task Publish<T>(Results<T> result)
        {

            // Disable the warning, because the task is being tracked on the set, 
            // and immediately removed upon completion.
#pragma warning disable CS4014
            var subTask = Task.Run(async () =>
            {
                try
                {
                    await _publisher.Publish(result);
                }
                catch (Exception ex)
                {
                    _onPublisherException(ex);
                }
            });
            _publishingTasks.TryAdd(subTask);
            subTask.ContinueWith(_ =>
            {
                _publishingTasks.TryRemove(subTask);
            });
#pragma warning restore CS4014
            return Task.FromResult((object)null);
        }

        public Task WhenPublished() =>
            Task.WhenAll(_publishingTasks.Items);
    }
}
