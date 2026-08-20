using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeepYourFocus.Managers
{
    /// <summary>
    /// Manages difficulty-based actions: board shuffling, tile replacement, and repositioning.
    /// Decouples difficulty logic from UI and core game flow.
    /// </summary>
    internal sealed class DifficultyManager
    {
        private readonly Random rnd = new Random();
        private readonly TileManager tileManager;

        public DifficultyManager(TileManager tileManager)
        {
            this.tileManager = tileManager ?? throw new ArgumentNullException(nameof(tileManager));
        }

        /// <summary>
        /// Fired when UI should reposition tiles after a shuffle.
        /// </summary>
        public event Action? RequestRepositionTiles;

        /// <summary>
        /// Shuffle the board tiles and signal UI to reposition visually.
        /// </summary>
        public void ShuffleBoard(IDictionary<string, PictureBox> registry)
        {
            if (registry == null || registry.Count == 0) return;

            tileManager.ShufflePositions();
            RequestRepositionTiles?.Invoke();
        }

        /// <summary>
        /// Reposition all PictureBoxes on the form based on fixed grid positions.
        /// Call this after shuffle or when board layout changes.
        /// </summary>
        public void RepositionPictureBoxes(IDictionary<string, PictureBox> registry)
        {
            if (registry == null || registry.Count == 0) return;

            var shuffled = tileManager.PictureBoxDictionary.Values.OrderBy(_ => rnd.Next()).ToList();
            for (int i = 0; i < shuffled.Count && i < 4; i++)
            {
                shuffled[i].Location = tileManager.GetFixedPosition(i);
                shuffled[i].Visible = true;
            }
        }

        /// <summary>
        /// Determine if board should shuffle based on game level and turn state.
        /// Higher levels = higher probability of shuffling.
        /// </summary>
        public bool ShouldShuffle(int counterLevels, bool isDisplaySequence, bool isPlayerTurn, bool isComputerTurn)
        {
            if (!isDisplaySequence && !isPlayerTurn && !isComputerTurn) return false;

            int chance = counterLevels switch
            {
                < 2 => 0,
                2 => 40,
                3 => 55,
                4 => 65,
                5 => 75,
                >= 6 => 85
            };

            return rnd.Next(100) < chance;
        }

        /// <summary>
        /// Decide if a tile should be replaced and return old/new tile pair.
        /// Returns (false, "", "") if no replacement should occur.
        /// </summary>
        public (bool shouldReplace, string oldTile, string newTile) DecideReplaceTile(
            List<string> correctOrder,
            IDictionary<string, PictureBox> registry,
            int counterLevels)
        {
            if (correctOrder == null || correctOrder.Count <= 2 || registry == null || registry.Count == 0)
                return (false, "", "");

            int chance = counterLevels switch
            {
                < 5 => 0,
                5 => 40,
                6 => 55,
                7 => 65,
                >= 8 => 80
            };

            if (rnd.Next(100) >= chance) return (false, "", "");

            var allTiles = TileManager.DictOfAllTiles()?.Keys.ToList();
            if (allTiles == null || allTiles.Count == 0) return (false, "", "");

            int randomIdx = rnd.Next(correctOrder.Count);
            string oldTile = correctOrder[randomIdx];

            string newTile = allTiles.FirstOrDefault(t => t != oldTile && !registry.ContainsKey(t)) ?? string.Empty;
            if (string.IsNullOrEmpty(newTile) || newTile == oldTile) return (false, "", "");

            Debug.WriteLine($"Difficulty: Replacing '{oldTile}' with '{newTile}'");
            return (true, oldTile, newTile);
        }

        /// <summary>
        /// Apply tile replacement in the correct order list.
        /// </summary>
        public void ApplyTileReplacement(List<string> correctOrder, string oldTile, string newTile)
        {
            if (correctOrder == null) return;
            for (int i = 0; i < correctOrder.Count; i++)
            {
                if (correctOrder[i] == oldTile)
                    correctOrder[i] = newTile;
            }
        }

        /// <summary>
        /// Decide if all tiles should be replaced (on level-up).
        /// </summary>
        public bool ShouldReplaceAllTiles(int counterLevels, bool levelUp)
        {
            if (!levelUp || counterLevels < 4) return false;

            int chance = counterLevels switch
            {
                4 => 40,
                5 => 55,
                6 => 70,
                >= 7 => 85,
                _ => 0  // Default for any other value (unreachable at runtime)
            };

            return rnd.Next(100) < chance;
        }
    }
}
