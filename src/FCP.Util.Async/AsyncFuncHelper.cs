using System;
using System.Threading;
using System.Threading.Tasks;

namespace FCP.Util.Async
{
    public static class AsyncFuncHelper
    {
        private static readonly TaskFactory s_myTaskFactory = new TaskFactory(CancellationToken.None,
            TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return s_myTaskFactory.StartNew(func)
                .Unwrap().GetAwaiter().GetResult();
        }

        public static void RunSync(Func<Task> func)
        {
            s_myTaskFactory.StartNew(func)
              .Unwrap().GetAwaiter().GetResult();
        }
    }
}
