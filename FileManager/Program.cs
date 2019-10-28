namespace FileManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Settings.SetupDefaultConsoleSettings();
            Controller conroller = new Controller();
            conroller.Start();
        }
    }
}
