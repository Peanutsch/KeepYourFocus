using KeepYourFocus.Helpers;
using System.Diagnostics;

namespace KeepYourFocus.Managers
{
    /// <summary>
    /// Manages tile (PictureBox) initialization, layout, shuffling, highlighting,
    /// and replacement logic for the game board.
    /// </summary>
    public class TileManager
    {
        #region === Fields and Properties ===
        /// <summary>Maps tile color names to their corresponding PictureBox controls on the board.</summary>
        public Dictionary<string, PictureBox> PictureBoxDictionary { get; set; } = new();

        /// <summary>The DPI-scaled size captured at startup to keep PictureBoxes consistent.</summary>
        private readonly Size pictureBoxFixedSize;

        /// <summary>The DPI-scaled positions captured at startup for each PictureBox slot.</summary>
        private readonly Point[] pictureBoxFixedPositions;
        private readonly Random rnd = new();
        private static Dictionary<string, string>? _cachedAllTiles;
        #endregion

        #region === Constructor ===
        /// <summary>
        /// Captures the runtime-scaled size and positions from the initial PictureBox layout.
        /// </summary>
        /// <param name="pictureBoxes">The four PictureBox controls from the form.</param>
        public TileManager(PictureBox[] pictureBoxes)
        {
            // Capture the DPI-scaled size from the first PictureBox
            pictureBoxFixedSize = pictureBoxes[0].Size;
            // Store each PictureBox's initial position for consistent repositioning
            pictureBoxFixedPositions = pictureBoxes.Select(pb => pb.Location).ToArray();
        }
        #endregion

        #region === Initializations ===
        /// <summary>
        /// Configures a single PictureBox with a tile image, tag, event handler, and consistent sizing.
        /// </summary>
        /// <param name="pictureBox">The PictureBox control to configure.</param>
        /// <param name="tile">The tile color name used as dictionary key and Tag.</param>
        /// <param name="imagePath">Absolute path to the tile image file.</param>
        /// <param name="clickHandler">The click event handler (PlayersTurn) to attach.</param>
        public void InitializePictureBox(PictureBox pictureBox, string tile, string imagePath, EventHandler clickHandler)
        {
            try
            {
                // Dispose the previous image to prevent GDI+ handle leaks
                pictureBox.Image?.Dispose();
                pictureBox.Image = Image.FromFile(imagePath);
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox.BackColor = Color.Transparent;
                pictureBox.Padding = new Padding(0);
                pictureBox.Size = pictureBoxFixedSize;
                pictureBox.Cursor = Cursors.Hand;
                pictureBox.Tag = tile;

                pictureBox.Click -= clickHandler;
                pictureBox.Click += clickHandler;

                PictureBoxDictionary[tile] = pictureBox;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing PictureBox for tile {tile}: {ex.Message}");
                MessageBox.Show($"Error initializing PictureBox for tile {tile}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region === Dictionary Management ===
        /// <summary>
        /// Sets up the initial four-tile board layout (Red, Blue, Orange, Green)
        /// by initializing each PictureBox and registering it in the dictionary.
        /// </summary>
        /// <param name="pictureBoxes">The four PictureBox controls from the form.</param>
        /// <param name="clickHandler">The click event handler to attach to each tile.</param>
        public void InitialDictionaryOfTilesAtStart(PictureBox[] pictureBoxes, EventHandler clickHandler)
        {
            string rootPath = PathHelper.GetRootPath();

            // Clear stale entries from previous games (e.g. replaced tiles like "CaribBlue", "Olive")
            PictureBoxDictionary.Clear();

            foreach (PictureBox? pb in pictureBoxes)
                pb.Visible = false;

            InitializePictureBox(pictureBoxes[0], "Red", Path.Combine(rootPath, @"png\red_tile512.png"), clickHandler);
            InitializePictureBox(pictureBoxes[1], "Blue", Path.Combine(rootPath, @"png\blue_tile512.png"), clickHandler);
            InitializePictureBox(pictureBoxes[2], "Orange", Path.Combine(rootPath, @"png\orange_tile512.png"), clickHandler);
            InitializePictureBox(pictureBoxes[3], "Green", Path.Combine(rootPath, @"png\green_tile512.png"), clickHandler);

            // Reset each PictureBox to its original fixed position (undoes any shuffle)
            for (int i = 0; i < pictureBoxes.Length; i++)
            {
                pictureBoxes[i].Location = GetFixedPosition(i);
                pictureBoxes[i].Visible = true;
            }
        }

        /// <summary>
        /// Returns a dictionary of all 10 available tile colors mapped to their image file paths.
        /// </summary>
        /// <returns>A dictionary with tile color names as keys and absolute image paths as values.</returns>
        public static Dictionary<string, string> DictOfAllTiles()
        {
            if (_cachedAllTiles != null)
                return _cachedAllTiles;

            string rootPath = PathHelper.GetRootPath();
            _cachedAllTiles = new Dictionary<string, string>
            {
                { "Red", Path.Combine(rootPath, "png", "red_tile512.png") },
                { "Blue", Path.Combine(rootPath, "png", "blue_tile512.png") },
                { "Orange", Path.Combine(rootPath, "png", "orange_tile512.png") },
                { "Green", Path.Combine(rootPath, "png", "green_tile512.png") },
                { "CaribBlue", Path.Combine(rootPath, "png", "caribBlue_tile512.png") },
                { "Grey", Path.Combine(rootPath, "png", "grey_tile512.png") },
                { "Indigo", Path.Combine(rootPath, "png", "indigo_tile512.png") },
                { "Maroon", Path.Combine(rootPath, "png", "maroon_tile512.png") },
                { "Olive", Path.Combine(rootPath, "png", "olive_tile512.png") },
                { "Pink", Path.Combine(rootPath, "png", "pink_tile512.png") }
            };
            return _cachedAllTiles;
        }

        /// <summary>
        /// Returns a shuffled copy of all available tiles using the Fisher-Yates algorithm.
        /// </summary>
        /// <returns>A randomly ordered dictionary of tile names and image paths.</returns>
        public Dictionary<string, string> ShuffleDictOfAllTiles()
        {
            List<KeyValuePair<string, string>> listOfAllTiles = DictOfAllTiles().ToList();

            int numberOfItems = listOfAllTiles.Count;
            while (numberOfItems > 1)
            {
                numberOfItems--;
                int randomIndex = rnd.Next(numberOfItems + 1);
                (listOfAllTiles[randomIndex], listOfAllTiles[numberOfItems]) = (listOfAllTiles[numberOfItems], listOfAllTiles[randomIndex]);
            }

            return listOfAllTiles.ToDictionary(kv => kv.Key, kv => kv.Value);
        }
        #endregion

        #region === Highlight Management ===
        /// <summary>
        /// Toggles the visual highlight on a PictureBox. When highlighted, a white border
        /// padding is applied; when unhighlighted, padding and size are reset to defaults.
        /// Thread-safe via <see cref="Control.Invoke"/>.
        /// </summary>
        /// <param name="pictureBox">The PictureBox to highlight or unhighlight.</param>
        /// <param name="highlight">True to apply highlight, false to remove it.</param>
        public void ManageHighlight(PictureBox pictureBox, bool highlight)
        {
            if (pictureBox.InvokeRequired)
            {
                // Invoke on the UI thread to safely update the PictureBox properties
                pictureBox.Invoke(new Action<PictureBox, bool>(ManageHighlight), pictureBox, highlight);
            }
            else
            {
                if (highlight)
                {
                    pictureBox.BorderStyle = BorderStyle.None;
                    pictureBox.Padding = new Padding(5);
                    pictureBox.BackColor = Color.White;
                }
                else
                {
                    pictureBox.Padding = new Padding(0);
                    pictureBox.BackColor = Color.Transparent;
                    pictureBox.Size = pictureBoxFixedSize;
                }
            }
        }
        #endregion

        #region === Position & Layout Management ===
        /// <summary>
        /// Returns the DPI-scaled fixed position for the given slot index.
        /// </summary>
        /// <param name="index">Zero-based slot index (0–3).</param>
        /// <returns>The fixed position point, or <see cref="Point.Empty"/> if the index is out of range.</returns>
        public Point GetFixedPosition(int index)
        {
            if (index >= 0 && index < pictureBoxFixedPositions.Length)
            {
                return pictureBoxFixedPositions[index];
            }
            return Point.Empty;
        }

        /// <summary>
        /// Randomly shuffles the on-screen positions of the current board tiles
        /// using the Fisher-Yates algorithm, resetting padding and size.
        /// </summary>
        public void ShufflePositions()
        {
            List<string> keys = PictureBoxDictionary.Keys.ToList();
            int lastIndex = keys.Count - 1;

            // Fisher-Yates shuffle on tile keys
            for (int currentIndex = lastIndex; currentIndex > 0; currentIndex--)
            {
                int randomIndex = rnd.Next(0, currentIndex + 1);
                (keys[randomIndex], keys[currentIndex]) = (keys[currentIndex], keys[randomIndex]);
            }

            int index = 0;
            foreach (string key in keys)
            {
                PictureBox pictureBox = PictureBoxDictionary[key];
                pictureBox.Location = GetFixedPosition(index);
                pictureBox.Size = pictureBoxFixedSize;
                pictureBox.Padding = new Padding(0);
                pictureBox.BackColor = Color.Transparent;
                index++;
            }
        }

        /// <summary>
        /// Randomizes the visual order of PictureBoxes and repositions them
        /// to the fixed layout positions, resetting size, padding, and visibility.
        /// </summary>
        public void RefreshAndRepositionPictureBoxes()
        {
            Debug.WriteLine("[TileManager.RefreshAndRepositionPictureBoxes] Repositioning PictureBoxes...");

            List<PictureBox> shuffledPictureBoxes = PictureBoxDictionary.Values.OrderBy(x => rnd.Next()).ToList();

            for (int itemIndex = 0; itemIndex < shuffledPictureBoxes.Count; itemIndex++)
            {
                shuffledPictureBoxes[itemIndex].Location = GetFixedPosition(itemIndex);
                shuffledPictureBoxes[itemIndex].Size = pictureBoxFixedSize;
                shuffledPictureBoxes[itemIndex].Padding = new Padding(0);
                shuffledPictureBoxes[itemIndex].BackColor = Color.Transparent;
                shuffledPictureBoxes[itemIndex].Visible = true;
            }
        }
        #endregion

        #region === Tile Randomization & Selection ===
        /// <summary>
        /// Selects a random tile from the current board by shuffling the dictionary keys
        /// and preventing three identical tiles in a row.
        /// </summary>
        /// <returns>The color name of the randomly selected tile.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no tiles are on the board.</exception>
        public string ManageRandomizerTiles()
        {
            if (PictureBoxDictionary.Count == 0)
            {
                Debug.WriteLine("[TileManager.ManageRandomizerTiles] PictureBoxDictionary is empty. Verify filepaths of tiles in InitialDictionaryOfTilesAtStart()");
                MessageBox.Show($"PictureBoxDictionary is empty. Verify filepaths of tiles in InitialDictionaryOfTilesAtStart()", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new InvalidOperationException("PictureBoxDictionary is empty. Verify filepaths of tiles");
            }

            // Fisher-Yates shuffle on current board tile keys
            List<string> shuffledTiles = PictureBoxDictionary.Keys.ToList();
            int numberOfItems = shuffledTiles.Count;
            while (numberOfItems > 1)
            {
                numberOfItems--;
                int randomIndex = rnd.Next(numberOfItems + 1);
                string temp = shuffledTiles[randomIndex];
                shuffledTiles[randomIndex] = shuffledTiles[numberOfItems];
                shuffledTiles[numberOfItems] = temp;
        }

            // Prevent three identical consecutive tiles by swapping with the next different tile
            for (int itemIndex = 2; itemIndex < shuffledTiles.Count; itemIndex++)
            {
                if (shuffledTiles[itemIndex] == shuffledTiles[itemIndex - 1] && shuffledTiles[itemIndex] == shuffledTiles[itemIndex - 2])
                {
                    for (int swapIndex = itemIndex + 1; swapIndex < shuffledTiles.Count; swapIndex++)
                    {
                        if (shuffledTiles[swapIndex] != shuffledTiles[itemIndex - 1])
                        {
                            string temp = shuffledTiles[itemIndex];
                            shuffledTiles[itemIndex] = shuffledTiles[swapIndex];
                            shuffledTiles[swapIndex] = temp;
                            break;
                        }
                    }
                }
            }

            return shuffledTiles[0];
            }
        #endregion

        #region === Tile Replacement ===
        /// <summary>
        /// Randomly replaces a tile in the correct sequence and/or on the board as a
        /// difficulty challenge. Orchestrates the replacement workflow by delegating to specialized methods.
        /// </summary>
        /// <param name="correctOrder">The current correct tile sequence.</param>
        /// <param name="counterLevels">The current difficulty level (1–9+).</param>
        /// <param name="isHardLevel">Whether the hard difficulty mode (endless) is active.</param>
        /// <param name="isDisplaySequence">Whether the sequence is currently being displayed to the player.</param>
        /// <param name="clickHandler">The click event handler to attach to newly created PictureBoxes.</param>
        /// <returns>A tuple containing the (possibly updated) correct order and whether a replacement occurred.</returns>
        public (List<string> correctOrder, bool replacementOccurred) ReplaceTileOnBoardAndInSequence(
            List<string> correctOrder, int counterLevels, bool isHardLevel, bool isDisplaySequence, EventHandler clickHandler)
        {
            Debug.WriteLine("[TileManager.ReplaceTileOnBoardAndInSequence] Orchestrating tile replacement...");

            // Step 1: Prepare replacement data (new tile, all tiles, working copy)
            var (newTile, allTiles, copyCorrectOrder) = PrepareReplacementData(correctOrder);

            // Step 2: Determine whether to replace in sequence and/or on board based on level
            var (shouldReplaceInOrder, shouldReplaceOnBoard) = DetermineReplacementActions(
                counterLevels, correctOrder.Count, isHardLevel, isDisplaySequence);

            // Early exit if no replacements are needed
            if (!shouldReplaceInOrder && !shouldReplaceOnBoard)
            {
                Debug.WriteLine("[TileManager.ReplaceTileOnBoardAndInSequence] No replacement conditions met.");
                return (correctOrder, false);
            }

            // Step 3: Select the tile to be replaced
            int randomIndex = rnd.Next(copyCorrectOrder.Count);
            string deleteTile = copyCorrectOrder[randomIndex];
            Debug.WriteLine($"[TileManager.ReplaceTileOnBoardAndInSequence] Target deletion: [{deleteTile}] at index [{randomIndex}]");

            bool replacementOccurred = false;

            // Step 4: Execute replacement in correct order (sequence) if conditions met
            if (shouldReplaceInOrder)
                (copyCorrectOrder, replacementOccurred) = ReplaceInCorrectOrder(
                    copyCorrectOrder, newTile, deleteTile, randomIndex, replacementOccurred);

            // Step 5: Execute replacement on board (visual) if conditions met
            if (shouldReplaceOnBoard)
                (copyCorrectOrder, replacementOccurred) = ReplaceOneTile(
                    copyCorrectOrder, deleteTile, allTiles, clickHandler, replacementOccurred);

            Debug.WriteLine($"[TileManager.ReplaceTileOnBoardAndInSequence] Replacement complete. Occurred: {replacementOccurred}");
            return (copyCorrectOrder, replacementOccurred);
        }

        /// <summary>
        /// Prepares tile data required for replacement operations: selects a new random tile,
        /// retrieves the full tile dictionary, and creates a working copy of the correct order.
        /// </summary>
        /// <param name="correctOrder">The current correct tile sequence.</param>
        /// <returns>A tuple of (newTile, allTilesList, workingCopy).</returns>
        private (string newTile, List<KeyValuePair<string, string>> allTiles, List<string> copyOrder) PrepareReplacementData(List<string> correctOrder)
        {
            // Select a new random tile from the board
            string newTile = ManageRandomizerTiles();
            Debug.WriteLine($"[PrepareReplacementData] Selected new tile: [{newTile}]");

            // Retrieve all available tiles (cached)
            var allTiles = DictOfAllTiles().ToList();

            // Create a working copy of the correct order to avoid unintended mutations
            var copyOrder = new List<string>(correctOrder);

            return (newTile, allTiles, copyOrder);
        }

        /// <summary>
        /// Determines whether to replace a tile in the sequence and/or on the board
        /// based on the current level, sequence length, hard mode status, and display state.
        /// Probability thresholds increase with difficulty progression.
        /// </summary>
        /// <param name="counterLevels">The current difficulty level.</param>
        /// <param name="correctOrderCount">The number of tiles in the current sequence.</param>
        /// <param name="isHardLevel">Whether hard mode is active.</param>
        /// <param name="isDisplaySequence">Whether the sequence is being displayed.</param>
        /// <returns>A tuple of (replaceInOrder, replaceOnBoard) decision flags.</returns>
        private (bool replaceInOrder, bool replaceOnBoard) DetermineReplacementActions(
            int counterLevels, int correctOrderCount, bool isHardLevel, bool isDisplaySequence)
        {
            // Determine replacement probability for the correct order (sequence) based on level
            // Levels 1+: 100%, Level 6+: 75%, Level 8+: 85%, Hard mode: 85% if displaying
            bool shouldReplaceInOrder =
                (counterLevels >= 1 && correctOrderCount > 2 && rnd.Next(100) <= 100) ||
                (counterLevels >= 6 && correctOrderCount > 2 && rnd.Next(100) <= 75) ||
                (counterLevels >= 8 && correctOrderCount > 2 && rnd.Next(100) <= 85) ||
                (isHardLevel && rnd.Next(100) <= 85 && isDisplaySequence);

            // Determine replacement probability for the board (visual) based on level
            // Levels 1+: 100%, Level 7+: 75%, Level 9+: 85%, Hard mode: 85% if displaying
            bool shouldReplaceOnBoard =
                (counterLevels >= 1 && correctOrderCount > 2 && rnd.Next(100) <= 100) ||
                (counterLevels >= 7 && correctOrderCount > 2 && rnd.Next(100) <= 75) ||
                (counterLevels >= 9 && correctOrderCount > 2 && rnd.Next(100) <= 85) ||
                (isHardLevel && rnd.Next(100) <= 85 && isDisplaySequence);

            Debug.WriteLine($"[DetermineReplacementActions] InOrder: {shouldReplaceInOrder}, OnBoard: {shouldReplaceOnBoard}");
            return (shouldReplaceInOrder, shouldReplaceOnBoard);
        }

        /// <summary>
        /// Replaces a tile within the correct order sequence at the specified index,
        /// provided the new tile is different and the index is not the last position.
        /// </summary>
        /// <param name="copyCorrectOrder">The working copy of the correct order.</param>
        /// <param name="newTile">The new tile to insert.</param>
        /// <param name="deleteTile">The tile being replaced (for validation).</param>
        /// <param name="randomIndex">The position to replace.</param>
        /// <param name="occurred">Whether a replacement has already occurred.</param>
        /// <returns>Updated (correctOrder, replacementOccurred) tuple.</returns>
        private static (List<string> correctOrder, bool occurred) ReplaceInCorrectOrder(
            List<string> copyCorrectOrder, string newTile, string deleteTile, int randomIndex, bool occurred)
        {
            // Validate replacement conditions: new tile must differ and not be at the end of sequence
            if (newTile == deleteTile || randomIndex == copyCorrectOrder.Count - 1)
            {
                Debug.WriteLine($"[ReplaceInCorrectOrder] Skipped: newTile matches deleteTile or at end of sequence");
                return (copyCorrectOrder, occurred);
            }

            // Replace the tile in the sequence
            Debug.WriteLine($"[ReplaceInCorrectOrder] Replacing [{deleteTile}] at index [{randomIndex}] with [{newTile}]");
            copyCorrectOrder[randomIndex] = newTile;

            return (copyCorrectOrder, true);
        }

        /// <summary>
        /// Replaces a tile on the board (visual) by removing the old tile's PictureBox mapping,
        /// selecting an unused tile, initializing the PictureBox with the new tile, and updating
        /// all references in the correct order sequence.
        /// </summary>
        /// <param name="copyCorrectOrder">The working copy of the correct order.</param>
        /// <param name="deleteTile">The tile color to remove from the board.</param>
        /// <param name="allTiles">List of all available tiles.</param>
        /// <param name="clickHandler">The click event handler for the new PictureBox.</param>
        /// <param name="occurred">Whether a replacement has already occurred.</param>
        /// <returns>Updated (correctOrder, replacementOccurred) tuple.</returns>
        private (List<string> correctOrder, bool occurred) ReplaceOneTile(
            List<string> copyCorrectOrder, string deleteTile,
            List<KeyValuePair<string, string>> allTiles, EventHandler clickHandler, bool occurred)
        {
            // Attempt to retrieve the PictureBox for the tile to be replaced
            if (!PictureBoxDictionary.TryGetValue(deleteTile, out PictureBox? pictureBox) || pictureBox == null)
            {
                Debug.WriteLine($"[ReplaceOneTile] Warning: tile '{deleteTile}' not found on board. Skipping.");
                return (copyCorrectOrder, occurred);
            }

            // Select a new tile color that is not already in use
            string newTile = SelectUnusedTile(allTiles, copyCorrectOrder);
            Debug.WriteLine($"[ReplaceOneTile] Selected replacement tile: [{newTile}] for [{deleteTile}]");

            // Remove the old tile mapping from the dictionary
            PictureBoxDictionary.Remove(deleteTile);

            // Get the full tile dictionary for path lookup
            var tileDict = DictOfAllTiles();

            // Configure the PictureBox with the new tile
            InitializePictureBox(pictureBox, newTile, tileDict[newTile], clickHandler);

            // Register the new tile in the dictionary
            PictureBoxDictionary[newTile] = pictureBox;

            // Update all references in the correct order sequence from old tile to new tile
            for (int i = 0; i < copyCorrectOrder.Count; i++)
            {
                if (copyCorrectOrder[i] == deleteTile)
                {
                    Debug.WriteLine($"[ReplaceOneTile] Updated sequence at [{i}]: [{deleteTile}] → [{newTile}]");
                    copyCorrectOrder[i] = newTile;
                }
            }

            Debug.WriteLine($"[ReplaceOneTile] Replacement complete: [{deleteTile}] → [{newTile}]");
            return (copyCorrectOrder, true);
        }

        /// <summary>
        /// Selects a tile color that has not been used yet on the board or in the correct order.
        /// Performs repeated random selection until an unused tile is found.
        /// </summary>
        /// <param name="allTiles">List of all available tiles (key-value pairs).</param>
        /// <param name="excludeTiles">Tiles currently in use (e.g., correct order sequence).</param>
        /// <returns>The name of an unused tile color.</returns>
        private string SelectUnusedTile(List<KeyValuePair<string, string>> allTiles, List<string> excludeTiles)
        {
            string selected;

            // Repeatedly select random tiles until one is found that is not in use
            do
            {
                selected = allTiles[rnd.Next(allTiles.Count)].Key;
                Debug.WriteLine($"[SelectUnusedTile] Candidate: [{selected}]");
            } while (excludeTiles.Contains(selected) || PictureBoxDictionary.ContainsKey(selected));

            Debug.WriteLine($"[SelectUnusedTile] Final selection: [{selected}]");
            return selected;
        }

        /// <summary>
        /// Replaces all four board tiles with a new random set from the full tile pool.
        /// Triggered on level-up or in hard mode, with probability based on level.
        /// Orchestrates the full board replacement workflow by delegating to specialized methods.
        /// </summary>
        /// <param name="pictureBoxes">The four PictureBox controls to reinitialize.</param>
        /// <param name="counterLevels">The current difficulty level (1–9+).</param>
        /// <param name="levelUp">Whether a level-up just occurred.</param>
        /// <param name="isHardLevel">Whether the hard difficulty mode (endless) is active.</param>
        /// <param name="isDisplaySequence">Whether the sequence is currently being displayed.</param>
        /// <param name="clickHandler">The click event handler for the new PictureBoxes.</param>
        public void ReplaceAllTiles(PictureBox[] pictureBoxes, int counterLevels, bool levelUp, bool isHardLevel, bool isDisplaySequence, EventHandler clickHandler)
        {
            Debug.WriteLine("[TileManager.ReplaceAllTiles] Replace and switch all tiles when level up... Testing lvl 1 Chance 100%");

            // Determine whether a full board replacement should occur based on level progression
            if (!ShouldReplaceAllTiles(counterLevels, levelUp, isHardLevel, isDisplaySequence))
            {
                Debug.WriteLine("[TileManager.ReplaceAllTiles] Replacement conditions not met.");
                return;
            }

            // Execute the full board replacement with new tiles
            ExecuteReplacementAllTiles(pictureBoxes, clickHandler);

            Debug.WriteLine("[TileManager.ReplaceAllTiles] Full board replacement complete.");
        }

        /// <summary>
        /// Determines whether all tiles on the board should be replaced based on level progression,
        /// level-up status, hard mode activation, and display state.
        /// Probability thresholds increase with difficulty progression.
        /// </summary>
        /// <param name="counterLevels">The current difficulty level.</param>
        /// <param name="levelUp">Whether a level-up just occurred.</param>
        /// <param name="isHardLevel">Whether the hard difficulty mode (endless) is active.</param>
        /// <param name="isDisplaySequence">Whether the sequence is currently being displayed.</param>
        /// <returns>True if all board tiles should be replaced; otherwise false.</returns>
        private bool ShouldReplaceAllTiles(int counterLevels, bool levelUp, bool isHardLevel, bool isDisplaySequence)
        {
            // Determine replacement probability based on level and conditions:
            // Level 1+ with level-up: 100% chance
            // Level 5+ with level-up: 75% chance
            // Level 7+ with level-up: 85% chance
            // Hard mode with display: 85% chance
            bool shouldReplace =
                (counterLevels >= 1 && levelUp && rnd.Next(100) <= 100) ||
                (counterLevels >= 5 && levelUp && rnd.Next(100) <= 75) ||
                (counterLevels >= 7 && levelUp && rnd.Next(100) <= 85) ||
                (isHardLevel && rnd.Next(100) <= 85 && isDisplaySequence);

            Debug.WriteLine($"[TileManager.ShouldReplaceAllTiles] Level: {counterLevels}, LevelUp: {levelUp}, HardMode: {isHardLevel}, Decision: {shouldReplace}");
            return shouldReplace;
        }

        /// <summary>
        /// Replaces all four tiles on the board with a new random set from the full tile pool.
        /// Clears the picture box dictionary, shuffles all available tiles, validates the shuffled set,
        /// and reinitializes all four board slots with fresh tiles.
        /// </summary>
        /// <param name="pictureBoxes">The four PictureBox controls to reinitialize.</param>
        /// <param name="clickHandler">The click event handler to attach to the new PictureBoxes.</param>
        private void ExecuteReplacementAllTiles(PictureBox[] pictureBoxes, EventHandler clickHandler)
        {
            Debug.WriteLine("[TileManager.ExecuteReplacementAllTiles] Replacing all board tiles with new random set...");

            try
            {
                // Get a shuffled set of all available tiles using Fisher-Yates algorithm
                Dictionary<string, string> shuffledTiles = ShuffleDictOfAllTiles();
                Debug.WriteLine($"[TileManager.ExecuteReplacementAllTiles] Shuffled tile set generated with {shuffledTiles.Count} tiles");

                // Validate that we have at least 4 tiles available for the 4 board slots
                if (shuffledTiles.Count < 4)
                {
                    Debug.WriteLine("[TileManager.ExecuteReplacementAllTiles] Error: Not enough tiles in shuffled set.");
                    MessageBox.Show(
                        "Error: Not enough tiles in shuffled set to initialize picture boxes.",
                        "Tile Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Clear old tile mappings from the dictionary
                PictureBoxDictionary.Clear();
                Debug.WriteLine("[TileManager.ExecuteReplacementAllTiles] Cleared old tile mappings from dictionary.");

                // Initialize the four board slots with fresh tiles
                InitializeNewTileSet(pictureBoxes, shuffledTiles, clickHandler);

                Debug.WriteLine("[TileManager.ExecuteReplacementAllTiles] All board tiles replaced successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TileManager.ExecuteReplacementAllTiles] Unexpected error: {ex.GetType().Name}: {ex.Message}");
                MessageBox.Show(
                    $"Unexpected error during board replacement: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Initializes the four board PictureBox slots with a new set of tiles from the shuffled tile dictionary.
        /// Iterates through the first four shuffled tiles and configures each PictureBox accordingly.
        /// </summary>
        /// <param name="pictureBoxes">The four PictureBox controls to initialize (expects array of length 4).</param>
        /// <param name="shuffledTiles">Dictionary of shuffled tiles (key: color name, value: image file path).</param>
        /// <param name="clickHandler">The click event handler to attach to each PictureBox.</param>
        private void InitializeNewTileSet(PictureBox[] pictureBoxes, Dictionary<string, string> shuffledTiles, EventHandler clickHandler)
        {
            try
            {
                // Convert dictionary to list for indexed access
                var tilesList = shuffledTiles.ToList();
                Debug.WriteLine($"[TileManager.InitializeNewTileSet] Starting initialization of {Math.Min(4, tilesList.Count)} board slots");

                // Initialize each of the four board positions with a new tile from the shuffled set
                for (int i = 0; i < 4 && i < tilesList.Count; i++)
                {
                    var kvp = tilesList[i];

                    // Configure the PictureBox with the tile color and image
                    Debug.WriteLine($"[TileManager.InitializeNewTileSet] Initializing slot {i} with tile [{kvp.Key}]");
                    InitializePictureBox(pictureBoxes[i], kvp.Key, kvp.Value, clickHandler);
                }

                Debug.WriteLine("[TileManager.InitializeNewTileSet] All four board slots initialized successfully.");
            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine($"[TileManager.InitializeNewTileSet] Argument exception during initialization: {ex.Message}");
                MessageBox.Show(
                    $"Error initializing tile set: {ex.Message}",
                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TileManager.InitializeNewTileSet] Unexpected error: {ex.GetType().Name}: {ex.Message}");
                MessageBox.Show(
                    $"Unexpected error during initialization: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }
        #endregion
    }
}
