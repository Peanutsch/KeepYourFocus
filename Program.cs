using KeepYourFocus;
using System.Windows.Forms;

namespace KeepYourFocus
{
    /// <summary>
    /// Main entry point for KeepYourFocus WinForms application.
    /// </summary>
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Initialize WinForms
            ApplicationConfiguration.Initialize();

            // Run the main form
            Application.Run(new Focus());
        }
    }
}