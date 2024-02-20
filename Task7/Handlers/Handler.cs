using Microsoft.Extensions.Logging;
using Task7.Contracts;
using Task7.Models;

namespace Task7.Handlers
{
    public class Handler : IHandler
    {
        private readonly IClient _client1;
        private readonly IClient _client2;
        private readonly ILogger<Handler> _logger;
        private readonly TimeSpan _maxWorkDuration = TimeSpan.FromSeconds(15);

        public Handler(
          IClient service1,
          IClient service2,
          ILogger<Handler> logger)
        {
            _client1 = service1;
            _client2 = service2;
            _logger = logger;
        }

        public Task<IApplicationStatus> GetApplicationStatus(string id)
        {            
            var firstClientCancelTokenSource = new CancellationTokenSource();
            var firstClientCancelationToken = firstClientCancelTokenSource.Token;
            
            var secondClientCancelTokenSource = new CancellationTokenSource();
            var secondClientCancelationToken = secondClientCancelTokenSource.Token;


            var taskResult1 = _client1.GetApplicationStatus(id, firstClientCancelationToken);

            var taskResult2 = _client2.GetApplicationStatus(id, secondClientCancelationToken);

            var taskIndex = Task.WaitAny(new Task[] { taskResult1, taskResult2 }, _maxWorkDuration);

            if(taskIndex == -1)
            {
                return Task.FromResult(new FailureStatus(DateTime.Now, 0) as IApplicationStatus);
            }

            ResponseInfo responseInfo = null; 
            if (taskIndex == 0)
            {
                responseInfo = InnerHandleTask(taskResult1, _client1, id, firstClientCancelTokenSource, secondClientCancelTokenSource);
            }
            if (taskIndex == 1)
            {
                responseInfo = InnerHandleTask(taskResult2, _client2, id, secondClientCancelTokenSource, firstClientCancelTokenSource);
            }

            IApplicationStatus status = null;

            if(responseInfo.Response is SuccessResponse)
            {
                var successResponse = (SuccessResponse)responseInfo.Response;
                status = new SuccessStatus(id, successResponse.Status);
            }

            if (responseInfo.Response is FailureResponse)
            {
                var successResponse = (FailureResponse)responseInfo.Response;
                status = new FailureStatus(DateTime.Now, responseInfo.RetriesCount);
            }

            return Task.FromResult(status);
        }

        private ResponseInfo InnerHandleTask(Task<IResponse> task, IClient client, string id, CancellationTokenSource own, CancellationTokenSource other)
        {
            var response = task.Result;

            var retryResponse = response as RetryResponse;
            if (retryResponse != null)
            {
                Task.Delay(retryResponse.Delay).ContinueWith(t => { response = client.GetApplicationStatus(id, own.Token).Result; });
                return new ResponseInfo { Response = response, RetriesCount = 1 };
            }

            other.Cancel();
            own.Dispose();
            other.Dispose();

            return new ResponseInfo { Response = response, RetriesCount = 0 };
        }
    }
}
