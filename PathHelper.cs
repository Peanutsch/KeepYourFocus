using System.Diagnostics;

namespace KeepYourFocus
{
    public static class PathHelper
    {
        public static string GetRootPath()
        {
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory;

            if (string.IsNullOrEmpty(directoryPath))
            {
                Debug.WriteLine("Error: Unable to determine root path.");
                MessageBox.Show("Error: Unable to determine root path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }

            string[] directorySplitPath = directoryPath.Split(Path.DirectorySeparatorChar);
            int index = Array.IndexOf(directorySplitPath, "KeepYourFocus");

            if (index != -1)
            {
                string rootPath = string.Join(Path.DirectorySeparatorChar.ToString(), directorySplitPath.Take(index + 1));

                if (!rootPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    rootPath += Path.DirectorySeparatorChar;
                }
                return rootPath;
            }
            else
            {
                Debug.WriteLine("Error: 'KeepYourFocus' directory not found in path.");
                MessageBox.Show("Error: 'KeepYourFocus' directory not found in path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }
        }

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
