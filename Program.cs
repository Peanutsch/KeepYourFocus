using KeepYourFocus.MonoGame;

namespace KeepYourFocus
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point - runs MonoGame version
        /// To switch back to Windows Forms: comment out MonoGame code and uncomment Windows Forms code below
        /// </summary>
        [STAThread]
        static void Main()
        {
            // === MONOGAME VERSION (Currently Active) ===
            using (var game = new MonoGameGame())
                game.Run();

            // === WINDOWS FORMS VERSION ===
            // Uncomment below and comment out MonoGame code above to use Windows Forms version
            //ApplicationConfiguration.Initialize();
            //Application.Run(new Focus());
        }
    }
}