using System;
using System.Threading;
using System.Threading.Tasks;

namespace Volte.Utils
{
    public static class ExecutorUtil
    {
        public static async Task ExecuteAfterDelayAsync(TimeSpan delay, Func<Task> func)
            => await Task.Delay(delay).ContinueWith(async _ => await func());

        public static async Task ExecuteAfterDelayAsync(int ms, Func<Task> func)
            => await Task.Delay(ms).ContinueWith(async _ => await func());

        public static async Task ExecuteAsync(Func<Task> func)
            => await Task.Run(async () => await func());

        public static void ExecuteAfterDelay(TimeSpan delay, Action action)
            => new Thread(() =>
            {
                Thread.Sleep(delay);
                action();
            }).Start();

        public static void ExecuteAfterDelay(int ms, Action action)
            => new Thread(() =>
            {
                Thread.Sleep(ms);
                action();
            }).Start();

        public static void Execute(Action action)
            => new Thread(action.Invoke).Start();
    }
}