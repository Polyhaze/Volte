using System;
using System.Threading.Tasks;

namespace Volte.Core.Utils
{
    public static class ExecutorUtil
    {
        public static async Task ExecuteAfterDelayAsync(TimeSpan delay, Action action)
        {
            await Task.Delay(delay).ContinueWith(_ => action());
        }

        public static async Task ExecuteAfterDelayAsync(int ms, Action action)
        {
            await Task.Delay(ms).ContinueWith(_ => action());
        }

        public static async Task ExecuteAsync(Action action)
        {
            await Task.Run(action);
        }

        public static void ExecuteAfterDelay(TimeSpan delay, Action action)
        {
            Task.Delay(delay).ContinueWith(_ => action());
        }

        public static void ExecuteAfterDelay(int ms, Action action)
        {
            Task.Delay(ms).ContinueWith(_ => action());
        }

        public static void Execute(Action action)
        {
            Task.Run(action);
        }
    }
}
