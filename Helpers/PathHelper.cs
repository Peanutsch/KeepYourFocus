using System.Diagnostics;

namespace Simon_Says.Helpers
{
    /// <summary>
    /// Static utility class for resolving application file paths.
    /// </summary>
    public static class PathHelper
    {
        private static string? _cachedRootPath;

        /// <summary>
        /// Resolves the root path of the KeepYourFocus project by searching for the
        /// "KeepYourFocus" directory segment in the application base directory.
        /// The result is cached after the first call.
        /// </summary>
        /// <returns>The absolute root path ending with a directory separator, or <see cref="string.Empty"/> on failure.</returns>
        public static string GetRootPath()
        {
            if (_cachedRootPath != null)
                return _cachedRootPath;
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory;

            if (string.IsNullOrEmpty(directoryPath))
            {
                Debug.WriteLine("Error: Unable to determine root path.");
                MessageBox.Show("Error: Unable to determine root path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }

            // Split the base directory into segments and locate the project folder
            string[] directorySplitPath = directoryPath.Split(Path.DirectorySeparatorChar);
            int index = Array.IndexOf(directorySplitPath, "KeepYourFocus");

            if (index != -1)
            {
                // Reconstruct the path up to and including the project folder
                string rootPath = string.Join(Path.DirectorySeparatorChar.ToString(), directorySplitPath.Take(index + 1));

                if (!rootPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    rootPath += Path.DirectorySeparatorChar;
                }
                _cachedRootPath = rootPath;
                return rootPath;
            }
            else
            {
                Debug.WriteLine("Error: 'KeepYourFocus' directory not found in path.");
                MessageBox.Show("Error: 'KeepYourFocus' directory not found in path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns the path to the application's LocalAppData directory, creating it if it does not exist.
        /// </summary>
        /// <returns>The absolute LocalAppData path ending with a directory separator, or <see cref="string.Empty"/> on failure.</returns>
        public static string GetLocalAppDataPath()
        {
            string localAppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KeepYourFocus");

            if (string.IsNullOrEmpty(localAppDataPath))
            {
                Debug.WriteLine("Error: Application path is not valid.");
                return string.Empty;
            }

            try
            {
                Directory.CreateDirectory(localAppDataPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: Unable to create application directory. {ex.Message}");
                return string.Empty;
            }

            if (!localAppDataPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                localAppDataPath += Path.DirectorySeparatorChar;
            }

            return localAppDataPath;
        }
    }
}
