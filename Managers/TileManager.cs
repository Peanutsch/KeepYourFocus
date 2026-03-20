using Simon_Says.Helpers;
using System.Diagnostics;

namespace Simon_Says.Managers
{
    /// <summary>
    /// Manages tile (PictureBox) initialization, layout, shuffling, highlighting,
    /// and replacement logic for the game board.
    /// </summary>
    public class TileManager
    {
        /// <summary>Maps tile color names to their corresponding PictureBox controls on the board.</summary>
        public Dictionary<string, PictureBox> PictureBoxDictionary { get; set; } = new();

        /// <summary>The DPI-scaled size captured at startup to keep PictureBoxes consistent.</summary>
        private readonly Size pictureBoxFixedSize;
        /// <summary>The DPI-scaled positions captured at startup for each PictureBox slot.</summary>
        private readonly Point[] pictureBoxFixedPositions;
        private readonly Random rnd = new();

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
            string rootPath = PathHelper.GetRootPath();
            return new Dictionary<string, string>
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
        /// Randomly replaces a tile in the correct sequence and/or on the board as a
        /// difficulty challenge. The probability increases with level progression.
        /// </summary>
        /// <param name="correctOrder">The current correct tile sequence.</param>
        /// <param name="counterLevels">The current difficulty level.</param>
        /// <param name="isHardLevel">Whether the hard difficulty mode is active.</param>
        /// <param name="isDisplaySequence">Whether the sequence is currently being displayed.</param>
        /// <param name="clickHandler">The click event handler for newly created PictureBoxes.</param>
        /// <returns>A tuple containing the (possibly updated) correct order and whether a replacement occurred.</returns>
        public (List<string> correctOrder, bool replacementOccurred) ReplaceTileOnBoardAndInSequence(
            List<string> correctOrder, int counterLevels, bool isHardLevel, bool isDisplaySequence, EventHandler clickHandler)
        {
            Debug.WriteLine("[TileManager.ReplaceTileOnBoardAndInSequence] Replace tile on board and/or in sequence... Testing lvl 1 100% chance ");

            string newTile = ManageRandomizerTiles();
            Dictionary<string, string> dictOfAllTiles = DictOfAllTiles();
            List<KeyValuePair<string, string>> listOfAllTiles = dictOfAllTiles.ToList();

            // Determine replacement probability based on current level
            bool checkReplaceInOrder = (counterLevels >= 1 && correctOrder.Count > 2 && rnd.Next(100) <= 100) ||
                                       (counterLevels >= 6 && correctOrder.Count > 2 && rnd.Next(100) <= 75) ||
                                       (counterLevels >= 8 && correctOrder.Count > 2 && rnd.Next(100) <= 85) ||
                                       (isHardLevel && rnd.Next(100) <= 85 && isDisplaySequence);

            bool checkReplaceOnBoard = (counterLevels >= 1 && correctOrder.Count > 2 && rnd.Next(100) <= 100) ||
                                       (counterLevels >= 7 && correctOrder.Count > 2 && rnd.Next(100) <= 75) ||
                                       (counterLevels >= 9 && correctOrder.Count > 2 && rnd.Next(100) <= 85) ||
                                       (isHardLevel && rnd.Next(100) <= 85 && isDisplaySequence);

            bool replacementOccurred = false;

            if (checkReplaceInOrder || checkReplaceOnBoard)
            {
                List<string> copyCorrectOrder = new List<string>(correctOrder);
                int randomIndex = rnd.Next(copyCorrectOrder.Count);
                string deleteTile = copyCorrectOrder[randomIndex];

                Debug.WriteLine($"deleteTile: [{deleteTile}]");

                if (checkReplaceInOrder && newTile != deleteTile && randomIndex != copyCorrectOrder.Count - 1)
                {
                    Debug.WriteLine("\nCorrectOrder = " + string.Join(", ", correctOrder));
                    Debug.WriteLine($"Replacing in order [{deleteTile}] at index [{randomIndex}] with new tile [{newTile}]. Testing lvl 1 chance 100%\n");

                    copyCorrectOrder[randomIndex] = newTile;
                    correctOrder = new List<string>(copyCorrectOrder);
                    replacementOccurred = true;
                }
                if (checkReplaceOnBoard)
                {
                    if (!PictureBoxDictionary.TryGetValue(deleteTile, out PictureBox? pictureBoxToReplace) || pictureBoxToReplace == null)
                    {
                        Debug.WriteLine($"Warning: deleteTile '{deleteTile}' not found on board. Skipping board replacement.");
                    }
                    else
                    {
                        PictureBoxDictionary.Remove(deleteTile);

                        string pickNewTile;
                        do
                        {
                            pickNewTile = listOfAllTiles[rnd.Next(listOfAllTiles.Count)].Key;
                            Debug.WriteLine($"pickNewTile: [{pickNewTile}]");
                        } while (copyCorrectOrder.Contains(pickNewTile) || PictureBoxDictionary.ContainsKey(pickNewTile));

                        Debug.WriteLine("\nCorrectOrder = " + string.Join(", ", correctOrder));
                        Debug.WriteLine($"Replaced on board and in order [{deleteTile}] with [{pickNewTile}]");

                        InitializePictureBox(pictureBoxToReplace, pickNewTile, dictOfAllTiles[pickNewTile], clickHandler);
                        PictureBoxDictionary[pickNewTile] = pictureBoxToReplace;

                        for (int indexItem = 0; indexItem < copyCorrectOrder.Count; indexItem++)
                        {
                            if (copyCorrectOrder[indexItem] == deleteTile)
                            {
                                copyCorrectOrder[indexItem] = pickNewTile;
                            }
                        }

                        correctOrder = new List<string>(copyCorrectOrder);
                        replacementOccurred = true;
                    }
                }
                Debug.WriteLine("Updated correctOrder = " + string.Join(", ", correctOrder));
            }
            return (correctOrder, replacementOccurred);
        }

        /// <summary>
        /// Replaces all four board tiles with a new random set from the full tile pool.
        /// Triggered on level-up or in hard mode, with probability based on level.
        /// </summary>
        /// <param name="pictureBoxes">The four PictureBox controls to reinitialize.</param>
        /// <param name="counterLevels">The current difficulty level.</param>
        /// <param name="levelUp">Whether a level-up just occurred.</param>
        /// <param name="isHardLevel">Whether the hard difficulty mode is active.</param>
        /// <param name="isDisplaySequence">Whether the sequence is currently being displayed.</param>
        /// <param name="clickHandler">The click event handler for the new PictureBoxes.</param>
        public void ReplaceAllTiles(PictureBox[] pictureBoxes, int counterLevels, bool levelUp, bool isHardLevel, bool isDisplaySequence, EventHandler clickHandler)
        {
            Debug.WriteLine("[TileManager.ReplaceAllTiles] Replace and switch all tiles when level up... Testing lvl 1 Chance 100%");

            if (counterLevels >= 1 && levelUp == true && rnd.Next(100) <= 100 ||
                counterLevels >= 5 && levelUp == true && rnd.Next(100) <= 75 ||
                counterLevels >= 7 && levelUp == true && rnd.Next(100) <= 85 ||
                isHardLevel && rnd.Next(100) <= 85 && isDisplaySequence)
            {
                Dictionary<string, string> shuffledTiles = ShuffleDictOfAllTiles();

                if (shuffledTiles.Count >= 4)
                {
                    KeyValuePair<string, string> kvp1 = shuffledTiles.ElementAt(0);
                    KeyValuePair<string, string> kvp2 = shuffledTiles.ElementAt(1);
                    KeyValuePair<string, string> kvp3 = shuffledTiles.ElementAt(2);
                    KeyValuePair<string, string> kvp4 = shuffledTiles.ElementAt(3);

                    try
                    {
                        PictureBoxDictionary.Clear();
                        InitializePictureBox(pictureBoxes[0], kvp1.Key, kvp1.Value, clickHandler);
                        InitializePictureBox(pictureBoxes[1], kvp2.Key, kvp2.Value, clickHandler);
                        InitializePictureBox(pictureBoxes[2], kvp3.Key, kvp3.Value, clickHandler);
                        InitializePictureBox(pictureBoxes[3], kvp4.Key, kvp4.Value, clickHandler);
                    }
                    catch (ArgumentException ex)
                    {
                        Debug.WriteLine($"An item with the same key has already been added: {ex.Message}");
                        MessageBox.Show($"An item with the same key has already been added: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        MessageBox.Show($"{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    Debug.WriteLine("Not enough tiles in shuffledTiles to initialize picture boxes.");
                    MessageBox.Show($"Not enough tiles in shuffledTiles to initialize picture boxes.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
