using System.Diagnostics;

namespace KeepYourFocus
{
    public class ScoreManager
    {
        public static readonly Dictionary<string, int> DifficultyPriorities = new()
        {
            { "Hard", 1 },
            { "Default", 2 },
            { "Easy", 3 }
        };

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

        public List<(string, int, int, string, string, string, int)> SortBestScores()
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

        public static bool QualifiesForTopScores(List<(string, int, int, string, string, string, int)> highScores, int totalRounds, string elapsedGameTime, int difficultyLevel)
        {
            return highScores.Count < 8 ||
                   highScores.Any(score => score.Item2 < totalRounds ||
                   (score.Item2 == totalRounds && TimeSpan.Parse(score.Item6) > TimeSpan.Parse(elapsedGameTime)) ||
                   (score.Item2 == totalRounds && TimeSpan.Parse(score.Item6) == TimeSpan.Parse(elapsedGameTime) && score.Item7 > difficultyLevel));
        }

        public static void SaveScoreToFile(List<(string, int, int, string, string, string, int)> highScores)
        {
            Debug.WriteLine("SaveScoreToFile started");

            string rootPath = PathHelper.GetRootPath();
            string file = Path.Combine(rootPath, "sounds", "setters.txt");
            string existingContent = File.Exists(file) ? File.ReadAllText(file) : string.Empty;

            List<(string, int, int, string, string, string, int)> currentScores = existingContent
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Split(','))
                .Select(parts => (parts[0], int.Parse(parts[1]), int.Parse(parts[2]), parts[3], parts[4], parts[5], int.Parse(parts[6])))
                .ToList();

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
                        Debug.WriteLine("line replaced in save file");
                    }
                    WriteToCopies();
                }
            }
            Debug.WriteLine("SaveScoreToFile ended");
        }

        public static void WriteToCopies()
        {
            Debug.WriteLine("WriteToCopies started");

            string rootPath = PathHelper.GetRootPath();
            string file = Path.Combine(rootPath, "sounds", "setters.txt");

            string copyToDir = Path.Combine(rootPath);
            Directory.CreateDirectory(copyToDir);
            string copyFile = Path.Combine(copyToDir, "higscores.txt");
            File.Copy(file, copyFile, true);
            Debug.WriteLine("Copied to file 1");

            string backupDir = Path.Combine(copyToDir, "BackUp");
            Directory.CreateDirectory(backupDir);
            string copyFile2 = Path.Combine(copyToDir, "BackUp", "higscoresBackUp.txt");
            File.Copy(file, copyFile2, true);
            Debug.WriteLine("Copied to file 2");

            Debug.WriteLine("WriteToCopies ended");
        }
    }
}
