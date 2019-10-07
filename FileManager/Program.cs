namespace FileManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Settings.SetupDefaultConsoleSettings();
            Window window = new Window();
            window.Explorer();
        }
    }
}
