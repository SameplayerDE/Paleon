using System;

namespace Paleon
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new Engine(1440, 900, "Paleon Game", false))
            {
                game.Run();
            }
        }
    }
#endif
}
