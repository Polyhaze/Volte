namespace BrackeysBot
{
    public class Program
    {
        public static void Main(string[] args)
            => new BrackeysBot().RunAsync().GetAwaiter().GetResult();
    }
}
