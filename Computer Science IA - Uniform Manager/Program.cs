namespace Computer_Science_IA___Uniform_Manager
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application
        ///  The API should be running before starting application
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new LoginForm());
        }
    }
}