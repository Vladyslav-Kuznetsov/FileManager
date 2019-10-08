namespace FileManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Settings.SetupDefaultConsoleSettings();
            Engine window = new Engine();
            window.Explorer();
        }
    }
}
