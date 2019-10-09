namespace FileManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Settings.SetupDefaultConsoleSettings();
            Engine engine = new Engine();
            engine.Start();
        }
    }
}
