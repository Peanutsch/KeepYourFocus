using KeepYourFocus.Helpers;
using System.Diagnostics;

namespace KeepYourFocus.Managers
{
    /// <summary>
    /// Handles score persistence: reading, sorting, qualifying, saving, and backing up
    /// high-score data stored in a CSV file (temp).
    /// </summary>
    public static class ScoreManager
    {
        #region === Dictionaries ===
        /// <summary>
        /// Maps difficulty names to their priority values.
        /// Used for sorting and comparing scores across difficulty levels.
        /// </summary>
        public static readonly Dictionary<string, int> DifficultyPriorities = new()
        {
            { "Hard", 1 },
            { "Default", 2 },
            { "Easy", 3 }
        };
        #endregion

        #region === Read Scores ===
        /// <summary>
        /// Reads all score entries from the CSV score file (sounds/setters.txt).
        /// Each line contains: playerName, score, levelReached, levelName, date, elapsedTime, difficultyLevel.
        /// </summary>
        /// <returns>A list of score tuples parsed from the file.</returns>
        public static List<(string, int, int, string, string, string, int)> ReadScoresFromFile()
        {
            string file = Path.Combine(PathHelper.GetRootPath(), "sounds", "setters.txt");
            List<(string, int, int, string, string, string, int)> scoresList = new List<(string, int, int, string, string, string, int)>();

            try
            {
                using (StreamReader getHighscore = new StreamReader(file))
                {
                    string? line;
                    while ((line = getHighscore.ReadLine()) != null)
                    {
                        string[] parts = line.Split(',');
                        // Validate that the line has all 7 expected fields
                        if (parts.Length >= 7)
                        {
                            if (int.TryParse(parts[1], out int playerScore) && int.TryParse(parts[2], out int levelReached) && int.TryParse(parts[6], out int difficultyLevel))
                            {
                                string playerName = parts[0].Trim();
                                string levelName = parts[3].Trim();
                                string isDate = parts[4].Trim();
                                string elapsedGameTime = parts[5].Trim();
                                scoresList.Add((playerName, playerScore, levelReached, levelName, isDate, elapsedGameTime, difficultyLevel));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred while reading scores: {ex.Message}");
                MessageBox.Show($"An error occurred while reading scores: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return scoresList;
        }
        #endregion

        #region === Sort Scores ===
        /// <summary>
        /// Reads all scores from file and returns the top 8, sorted by highest score,
        /// then fastest time, then hardest difficulty.
        /// </summary>
        /// <returns>A sorted list of the top 8 high scores.</returns>
        public static List<(string, int, int, string, string, string, int)> SortBestScores()
        {
            List<(string, int, int, string, string, string, int)> bestScores = new List<(string, int, int, string, string, string, int)>();

            try
            {
                List<(string, int, int, string, string, string, int)> scoresList = ReadScoresFromFile();

                bestScores = scoresList.OrderByDescending(x => x.Item2)
                                       .ThenBy(x => TimeSpan.Parse(x.Item6))
                                       .ThenBy(x => x.Item7)
                                       .Take(8)
                                       .ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"An error occurred while getting top scores: {e.Message}");
                MessageBox.Show($"An error occurred while getting top scores: {e.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return bestScores;
        }
        #endregion

        #region === Qualify Score ===
        /// <summary>
        /// Determines whether a new score qualifies for the top 8 leaderboard
        /// by comparing rounds, elapsed time, and difficulty level.
        /// </summary>
        /// <param name="highScores">The current leaderboard entries.</param>
        /// <param name="totalRounds">The player's completed rounds.</param>
        /// <param name="elapsedGameTime">The player's elapsed game time (mm:ss format).</param>
        /// <param name="difficultyLevel">The player's difficulty priority value.</param>
        /// <returns>True if the score qualifies for the top 8.</returns>
        public static bool QualifiesForTopScores(List<(string, int, int, string, string, string, int)> highScores, int totalRounds, string elapsedGameTime, int difficultyLevel)
        {
            return highScores.Count < 8 ||
                   highScores.Any(score => score.Item2 < totalRounds ||
                   (score.Item2 == totalRounds && TimeSpan.Parse(score.Item6) > TimeSpan.Parse(elapsedGameTime)) ||
                   (score.Item2 == totalRounds && TimeSpan.Parse(score.Item6) == TimeSpan.Parse(elapsedGameTime) && score.Item7 > difficultyLevel));
        }
        #endregion

        #region === Save Score ===
        /// <summary>
        /// Persists the updated high-score list to the CSV file.
        /// Manages a maximum of 15 stored entries, replacing the lowest score when full.
        /// Triggers a backup copy after each save.
        /// </summary>
        /// <param name="highScores">The new high-score entries to merge and save.</param>
        public static void SaveScoreToFile(List<(string, int, int, string, string, string, int)> highScores)
        {
            string rootPath = PathHelper.GetRootPath();
            string file = Path.Combine(rootPath, "sounds", "setters.txt");

            // Reuse existing parsing logic instead of duplicating it
            List<(string, int, int, string, string, string, int)> currentScores = ReadScoresFromFile();

            // Merge and keep the top 15 scores if under capacity
            if (currentScores.Count < 15)
            {
                currentScores.AddRange(highScores);
                currentScores = currentScores
                    .OrderByDescending(score => score.Item2)
                    .ThenBy(score => TimeSpan.Parse(score.Item6))
                    .ThenBy(score => score.Item7)
                    .Take(15)
                    .ToList();

                using (StreamWriter saveScore = new StreamWriter(file, false))
                {
                    foreach (var element in currentScores)
                    {
                        saveScore.WriteLine($"{element.Item1},{element.Item2},{element.Item3},{element.Item4},{element.Item5},{element.Item6},{element.Item7}");
                    }
                }
                WriteToCopies();
            }
            else
            {
                // At capacity: find the lowest score and replace it if the new score is better
                var allScores = currentScores.Concat(highScores).ToList();
                var lowestScore = allScores
                    .OrderBy(score => score.Item2)
                    .ThenByDescending(score => TimeSpan.Parse(score.Item6))
                    .ThenByDescending(score => score.Item7)
                    .First();

                if (highScores.Any(newScore => newScore.Item2 > lowestScore.Item2 ||
                                                (newScore.Item2 == lowestScore.Item2 && TimeSpan.Parse(newScore.Item6) < TimeSpan.Parse(lowestScore.Item6))))
                {
                    var updatedScores = currentScores
                        .Where(score => score != lowestScore)
                        .Concat(highScores)
                        .OrderByDescending(score => score.Item2)
                        .ThenBy(score => TimeSpan.Parse(score.Item6))
                        .ThenBy(score => score.Item7)
                        .Take(15)
                        .ToList();

                    using (StreamWriter saveScore = new StreamWriter(file, false))
                    {
                        foreach (var element in updatedScores)
                        {
                            saveScore.WriteLine($"{element.Item1},{element.Item2},{element.Item3},{element.Item4},{element.Item5},{element.Item6},{element.Item7}");
                        }
                        Debug.WriteLine("[ScoreManager.SaveScoreToFile] Line replaced in save file");
                    }
                    WriteToCopies();
                }
            }
        }

        /// <summary>
        /// Creates backup copies of the score file to the project root (higscores.txt)
        /// and a BackUp subdirectory (higscoresBackUp.txt).
        /// </summary>
        public static void WriteToCopies()
        {
            Debug.WriteLine("WriteToCopies started");

            string rootPath = PathHelper.GetRootPath();
            string file = Path.Combine(rootPath, "sounds", "setters.txt");

            string copyToDir = Path.Combine(rootPath);
            Directory.CreateDirectory(copyToDir);
            string copyFile = Path.Combine(copyToDir, "higscores.txt");
            File.Copy(file, copyFile, true);
            Debug.WriteLine("[ScoreManager.WriteToCopies] Copied to file 1");

            string backupDir = Path.Combine(copyToDir, "BackUp");
            Directory.CreateDirectory(backupDir);
            string copyFile2 = Path.Combine(copyToDir, "BackUp", "higscoresBackUp.txt");
            File.Copy(file, copyFile2, true);
            Debug.WriteLine("[ScoreManager.WriteToCopies] Copied to file 2");

            Debug.WriteLine("WriteToCopies ended");
        }
        #endregion
    }
}
