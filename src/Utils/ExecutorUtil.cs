using System;
using System.Threading;
using System.Threading.Tasks;

namespace Volte.Utils
{
    public static class ExecutorUtil
    {
        public static async Task ExecuteAfterDelayAsync(TimeSpan delay, Func<Task> action)
            => await Task.Delay(delay).ContinueWith(async _ => await action());

        public static async Task ExecuteAfterDelayAsync(int ms, Func<Task> action)
            => await Task.Delay(ms).ContinueWith(async _ => await action());

        public static async Task ExecuteAsync(Action action)
            => await Task.Run(action);

        public static void ExecuteAfterDelay(TimeSpan delay, Func<Task> action)
            => new Thread(async () =>
            {
                Thread.Sleep(delay);
                await action();
            }).Start();

        public static void ExecuteAfterDelay(int ms, Func<Task> action)
            => new Thread(async () =>
            {
                Thread.Sleep(ms);
                await action();
            }).Start();

        public static void Execute(Func<Task> action)
            => new Thread(async () => await action()).Start();
    }
}