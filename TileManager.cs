using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simon_Says
{
    public class TileManager
    {
        // Lazy-loaded GameSounds object
        private readonly GameSounds sounds = new GameSounds();

        private Random rnd;
        private Dictionary<string, PictureBox> pictureBoxDictionary;
        private List<string> correctOrder;


        public TileManager(Dictionary<string, PictureBox> pictureBoxDict, List<string> initialCorrectOrder, Random random)
        {
            rnd = random;
            pictureBoxDictionary = pictureBoxDict;
            correctOrder = initialCorrectOrder;
        }

        // Shuffle dictionary of all tiles
        public Dictionary<string, string> ShuffleDictOfAllTiles(Dictionary<string, string> dictOfAllTiles)
        {
            List<KeyValuePair<string, string>> listOfAllTiles = dictOfAllTiles.ToList();

            // Fisher-Yates shuffle
            int numberOfItems = listOfAllTiles.Count;
            while (numberOfItems > 1)
            {
                numberOfItems--;
                int randomIndex = rnd.Next(numberOfItems + 1);
                KeyValuePair<string, string> temp = listOfAllTiles[randomIndex];
                listOfAllTiles[randomIndex] = listOfAllTiles[numberOfItems];
                listOfAllTiles[numberOfItems] = temp;
            }

            return listOfAllTiles.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        // Reposition tiles after shuffle
        public void RefreshAndRepositionPictureBoxes()
        {
            var shuffledPictureBoxes = pictureBoxDictionary.Values.OrderBy(x => rnd.Next()).ToList();

            for (int itemIndex = 0; itemIndex < shuffledPictureBoxes.Count; itemIndex++)
            {
                shuffledPictureBoxes[itemIndex].Location = InitialSetup.SetFixedPositionPictureBoxes(itemIndex);
                shuffledPictureBoxes[itemIndex].Visible = true;
            }
        }

        // Shuffle the tiles during gameplay (after display sequence or player click)
        public async Task ShufflePictureBoxes()
        {
            int rndChance = rnd.Next(100);

            switch (true)
            {
                case true when isDisplaySequence && ShouldShuffleDuringDisplaySequence(rndChance):
                    await ShuffleAfterDisplaySequence();
                    break;

                case true when isPlayerTurn && ShouldShuffleAfterPlayerClick(rndChance):
                    ShuffleAfterPlayerClick();
                    break;

                default:
                    Debug.WriteLine("No shuffle this time");
                    break;
            }

            actionTaken = true;
        }

        private bool ShouldShuffleDuringDisplaySequence(int rndChance)
        {
            return (counterLevels == 2 && rndChance <= 55) ||
                   (counterLevels >= 3 && rndChance <= 75) ||
                   (counterLevels >= 5 && rndChance <= 85) ||
                   (isHardLevel && rndChance <= 85);
        }

        private bool ShouldShuffleAfterPlayerClick(int rndChance)
        {
            return (counterLevels >= 3 && rndChance <= 55) ||
                   (counterLevels >= 4 && rndChance <= 75) ||
                   (counterLevels >= 6 && rndChance <= 85);
        }

        private async Task ShuffleAfterDisplaySequence()
        {
            Debug.WriteLine("Shuffle PictureBoxes after display sequence");
            await Task.Delay(500);
            // Play sound, shuffle and reposition
            sounds.PlayTransition();
            RandomizerShufflePictureBoxes();
            RefreshAndRepositionPictureBoxes();
            await Task.Delay(500);
        }

        private void ShuffleAfterPlayerClick()
        {
            Debug.WriteLine("Shuffle PictureBoxes after player click");
            RandomizerShufflePictureBoxes();
            RefreshAndRepositionPictureBoxes();
        }

        // Replace a tile on the board and in the sequence
        public (Dictionary<string, PictureBox>, List<string>, bool) ReplaceTileOnBoardAndInSequence()
        {
            int rndChance = rnd.Next(100);
            string newTile = RandomizerTiles();
            var dictOfAllTiles = DictOfAllTiles();
            var listOfAllTiles = dictOfAllTiles.ToList();
            var copyCorrectOrder = new List<string>(correctOrder);
            bool replacementOccurred = false;

            bool checkReplaceInOrder = ShouldReplaceInOrder(rndChance);
            bool checkReplaceOnBoard = ShouldReplaceOnBoard(rndChance);

            if (!checkReplaceInOrder && !checkReplaceOnBoard)
                return (pictureBoxDictionary, correctOrder, false);

            Debug.WriteLine("ReplaceTileOnBoardAndInSequence");

            int randomIndex = rnd.Next(copyCorrectOrder.Count);
            string deleteTile = copyCorrectOrder[randomIndex];

            if (checkReplaceInOrder && newTile != deleteTile && randomIndex != copyCorrectOrder.Count - 1)
            {
                copyCorrectOrder[randomIndex] = newTile;
                correctOrder = copyCorrectOrder;
                replacementOccurred = true;
            }

            if (checkReplaceOnBoard)
            {
                replacementOccurred = ReplaceTileOnBoard(deleteTile, copyCorrectOrder, listOfAllTiles);
            }

            Debug.WriteLine("Updated correctOrder = " + string.Join(", ", correctOrder));
            actionTaken = true;

            return (pictureBoxDictionary, correctOrder, replacementOccurred);
        }

        private bool ShouldReplaceInOrder(int rndChance)
        {
            return (counterLevels >= 5 && correctOrder.Count > 2 && rndChance <= 55) ||
                   (counterLevels >= 6 && correctOrder.Count > 2 && rndChance <= 75) ||
                   (counterLevels >= 8 && correctOrder.Count > 2 && rndChance <= 85) ||
                   (isHardLevel && rndChance <= 85 && isDisplaySequence);
        }

        private bool ShouldReplaceOnBoard(int rndChance)
        {
            return (counterLevels >= 6 && correctOrder.Count > 2 && rndChance <= 55) ||
                   (counterLevels >= 7 && correctOrder.Count > 2 && rndChance <= 75) ||
                   (counterLevels >= 9 && correctOrder.Count > 2 && rndChance <= 85) ||
                   (isHardLevel && rndChance <= 85 && isDisplaySequence);
        }

        private bool ReplaceTileOnBoard(string deleteTile, List<string> copyCorrectOrder, List<KeyValuePair<string, string>> listOfAllTiles)
        {
            PictureBox pictureBoxToReplace = pictureBoxDictionary[deleteTile];
            pictureBoxDictionary.Remove(deleteTile);

            string pickNewTile;
            do
            {
                pickNewTile = listOfAllTiles[rnd.Next(listOfAllTiles.Count)].Key;
            }
            while (copyCorrectOrder.Contains(pickNewTile) || pictureBoxDictionary.ContainsKey(pickNewTile));

            InitializePictureBox(pictureBoxToReplace, pickNewTile, listOfAllTiles.ToDictionary(kv => kv.Key, kv => kv.Value)[pickNewTile]);
            pictureBoxDictionary[pickNewTile] = pictureBoxToReplace;

            for (int i = 0; i < copyCorrectOrder.Count; i++)
            {
                if (copyCorrectOrder[i] == deleteTile)
                    copyCorrectOrder[i] = pickNewTile;
            }

            correctOrder = copyCorrectOrder;
            return true;
        }

        // Replace all tiles when level up
        public void ReplaceAllTiles()
        {
            int rndChance = rnd.Next(100);

            bool shouldReplace = (counterLevels >= 4 && levelUp && rndChance <= 55) ||
                                 (counterLevels >= 5 && levelUp && rndChance <= 75) ||
                                 (counterLevels >= 7 && levelUp && rndChance <= 85) ||
                                 (isHardLevel && isDisplaySequence && rndChance <= 85);

            if (!shouldReplace)
                return;

            Debug.WriteLine("ReplaceAllTiles");

            var shuffledTiles = ShuffleDictOfAllTiles(DictOfAllTiles());

            if (shuffledTiles.Count < 4)
            {
                Debug.WriteLine("Not enough tiles to initialize picture boxes.");
                MessageBox.Show("Not enough tiles to initialize picture boxes.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                pictureBoxDictionary.Clear();
                var kvps = shuffledTiles.Take(4).ToArray();

                InitializePictureBox(pictureBox1, kvps[0].Key, kvps[0].Value);
                InitializePictureBox(pictureBox2, kvps[1].Key, kvps[1].Value);
                InitializePictureBox(pictureBox3, kvps[2].Key, kvps[2].Value);
                InitializePictureBox(pictureBox4, kvps[3].Key, kvps[3].Value);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            actionTaken = true;
        }

        // Utility method for initializing PictureBox
        private void InitializePictureBox(PictureBox pictureBox, string tile, string imagePath)
        {
            try
            {
                pictureBox.Image = Image.FromFile(imagePath);
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox.BackColor = Color.Transparent;
                pictureBox.Cursor = Cursors.Hand;
                pictureBox.Tag = tile;

                pictureBox.Click -= PlayersTurn; // Remove any previous attachment

                pictureBox.Click += PlayersTurn; // Attach event handler

                // Update the dictionary with the new PictureBox
                pictureBoxDictionary[tile] = pictureBox;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing PictureBox for tile {tile}: {ex.Message}");
                MessageBox.Show($"Error initializing PictureBox for tile {tile}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Utility method for randomizing tiles
        private string RandomizerTiles()
        {
            List<string> shuffledTiles = pictureBoxDictionary.Keys.ToList();
            int numberOfItems = shuffledTiles.Count;
            while (numberOfItems > 1)
            {
                numberOfItems--;
                int randomIndex = rnd.Next(numberOfItems + 1);
                string temp = shuffledTiles[randomIndex];
                shuffledTiles[randomIndex] = shuffledTiles[numberOfItems];
                shuffledTiles[numberOfItems] = temp;
            }

            return shuffledTiles[0];
        }

        private void RandomizerShufflePictureBoxes()
        {
            // Shuffle the keys of the dictionary
            List<string> keys = pictureBoxDictionary.Keys.ToList();
            int lastIndex = keys.Count - 1;

            for (int currentIndex = lastIndex; currentIndex > 0; currentIndex--)
            {
                int randomIndex = rnd.Next(0, currentIndex + 1);
                (keys[randomIndex], keys[currentIndex]) = (keys[currentIndex], keys[randomIndex]);
            }

            // Update the positions of the PictureBoxes based on the shuffled keys
            int index = 0;
            foreach (string key in keys)
            {
                PictureBox pictureBox = pictureBoxDictionary[key];
                pictureBox.Location = InitialSetup.SetFixedPositionPictureBoxes(index);
                index++;
            }
        }
    }

}
