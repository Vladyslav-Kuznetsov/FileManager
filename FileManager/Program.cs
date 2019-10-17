namespace FileManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Settings.SetupDefaultConsoleSettings();
            Controller engine = new Controller();
            engine.Start();
        }
    }
}
