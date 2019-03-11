using System;
using System.Threading;
using System.Threading.Tasks;

namespace Volte.Utils
{
    public static class ExecutorUtil
    {
        public static async Task ExecuteAfterDelayAsync(TimeSpan delay, Action action)
            => await Task.Delay(delay).ContinueWith(_ => action());

        public static async Task ExecuteAfterDelayAsync(int ms, Action action)
            => await Task.Delay(ms).ContinueWith(_ => action());

        public static async Task ExecuteAsync(Action action)
            => await Task.Run(action);

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