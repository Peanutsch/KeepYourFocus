using System.Diagnostics;

namespace KeepYourFocus
{
    public class TileManager
    {
        public Dictionary<string, PictureBox> PictureBoxDictionary { get; set; } = new();

        private readonly Size pictureBoxFixedSize;
        private readonly Point[] pictureBoxFixedPositions;
        private readonly Random rnd = new();

        public TileManager(PictureBox[] pictureBoxes)
        {
            pictureBoxFixedSize = pictureBoxes[0].Size;
            pictureBoxFixedPositions = pictureBoxes.Select(pb => pb.Location).ToArray();
        }

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

        public void InitialDictionaryOfTilesAtStart(PictureBox[] pictureBoxes, EventHandler clickHandler)
        {
            string rootPath = PathHelper.GetRootPath();

            foreach (var pb in pictureBoxes)
                pb.Visible = false;

            InitializePictureBox(pictureBoxes[0], "Red", Path.Combine(rootPath, @"png\red_tile512.png"), clickHandler);
            InitializePictureBox(pictureBoxes[1], "Blue", Path.Combine(rootPath, @"png\blue_tile512.png"), clickHandler);
            InitializePictureBox(pictureBoxes[2], "Orange", Path.Combine(rootPath, @"png\orange_tile512.png"), clickHandler);
            InitializePictureBox(pictureBoxes[3], "Green", Path.Combine(rootPath, @"png\green_tile512.png"), clickHandler);

            foreach (var pb in pictureBoxes)
                pb.Visible = true;
        }

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

        public void ManageHighlight(PictureBox pictureBox, bool highlight)
        {
            if (pictureBox.InvokeRequired)
            {
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

        public void ShufflePositions()
        {
            List<string> keys = PictureBoxDictionary.Keys.ToList();
            int lastIndex = keys.Count - 1;

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

        public void RefreshAndRepositionPictureBoxes()
        {
            Debug.WriteLine("[RefreshAndRepositionPictureBoxes] Repositioning PictureBoxes...");

            var shuffledPictureBoxes = PictureBoxDictionary.Values.OrderBy(x => rnd.Next()).ToList();

            for (int itemIndex = 0; itemIndex < shuffledPictureBoxes.Count; itemIndex++)
            {
                shuffledPictureBoxes[itemIndex].Location = GetFixedPosition(itemIndex);
                shuffledPictureBoxes[itemIndex].Size = pictureBoxFixedSize;
                shuffledPictureBoxes[itemIndex].Padding = new Padding(0);
                shuffledPictureBoxes[itemIndex].BackColor = Color.Transparent;
                shuffledPictureBoxes[itemIndex].Visible = true;
            }
        }

        public string ManageRandomizerTiles()
        {
            if (PictureBoxDictionary.Count == 0)
            {
                Debug.WriteLine("PictureBoxDictionary is empty. Verify filepaths of tiles in InitialDictionaryOfTilesAtStart()");
                MessageBox.Show($"PictureBoxDictionary is empty. Verify filepaths of tiles in InitialDictionaryOfTilesAtStart()", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new InvalidOperationException("PictureBoxDictionary is empty. Verify filepaths of tiles");
            }

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

        public Point GetFixedPosition(int index)
        {
            if (index >= 0 && index < pictureBoxFixedPositions.Length)
            {
                return pictureBoxFixedPositions[index];
            }
            return Point.Empty;
        }

        public (List<string> correctOrder, bool replacementOccurred) ReplaceTileOnBoardAndInSequence(
            List<string> correctOrder, int counterLevels, bool isHardLevel, bool isDisplaySequence, EventHandler clickHandler)
        {
            Debug.WriteLine("Replace tile on board and/or in sequence...");

            string newTile = ManageRandomizerTiles();
            Dictionary<string, string> dictOfAllTiles = DictOfAllTiles();
            List<KeyValuePair<string, string>> listOfAllTiles = dictOfAllTiles.ToList();

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
                    Debug.WriteLine($"Replacing in order [{deleteTile}] at index [{randomIndex}] with new tile [{newTile}]\n");

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

        public void ReplaceAllTiles(PictureBox[] pictureBoxes, int counterLevels, bool levelUp, bool isHardLevel, bool isDisplaySequence, EventHandler clickHandler)
        {
            Debug.WriteLine("Replace and switch all tiles when level up...");

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
