/*
 * Simon Says-like game with some level based challenges
 * Each level has 6 sequences. After 6 succesful sequences:
 * Level++; Add 1 challenge; Clear correctOrder and playerOrder and start with new sequence = 1
 * From Level >= 7: no Clear correctOrder; sequences++ untill game over
 * More challenges in progress
 * 
 * === Levels ===
 * Level 1 [EasyPeasy]: standard
 * level 2 [OkiDoki] and onward: some misleading text in pictureboxes, plus:
 *                               Shuffle Pictureboxes before start player's turn with 55% chance; level 3 75% chance; >= level 5 85% chance
 * level 3 [Please No]: Shuffle Pictureboxes per player click with 55% chance; level 4 75% chance; >= level 6 85% chance
 * Level 4 [No Way!]: When level up, replace all tile squares on board with 55%; level 5 75%; >= level 7 85%
 * level 5 [HELL NO]: In each sequence, replace one tile in running order with 55% chance; level 6 75% chance; level 8 85%
 * level 6 [NONONONO]: Replace 1 tile with other tile on board and in running order in the running order with 55% chance; level 7 75% chance; >= level 9 85%
 * level 7 [...]: 
 * level 8 [...]: 
 * level 9 [...]: 
 * level 666 [HELLMODE]: No Clear correctOrder; sequences++ untill game over
 */

using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Media;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Xml;
using System.Windows.Forms;

namespace KeepYourFocus
{
    public partial class PlayerField : Form
    {
        private Dictionary<string, PictureBox> pictureBoxDictionary = new Dictionary<string, PictureBox>();
        private List<string> correctOrder = new List<string>();
        private List<string> playerOrder = new List<string>();
        private List<string> previousTiles = new List<string>();

        private readonly SoundPlayer redSound;
        private readonly SoundPlayer blueSound;
        private readonly SoundPlayer orangeSound;
        private readonly SoundPlayer greenSound;
        private readonly SoundPlayer caribBlueSound;
        private readonly SoundPlayer greySound;
        private readonly SoundPlayer indigoSound;
        private readonly SoundPlayer maroonSound;
        private readonly SoundPlayer oliveSound;
        private readonly SoundPlayer pinkSound;

        private readonly SoundPlayer transitionSound;
        private readonly SoundPlayer buttonClickSound;
        private readonly SoundPlayer wrongSound;
        private readonly SoundPlayer correctSound;
        private readonly SoundPlayer startupSound;

        private readonly Random rnd = new Random();

        private bool computer = false;
        private bool startButton = true;
        private bool nextRound = false;
        private bool levelUp = false;

        private int counter_sequences = 1;
        private int counter_levels = 1;
        private int counter_rounds = 1;

        public PlayerField()
        {
            InitializeComponent();

            // Load soundfiles. For now 1 beep sound for all colors
            string soundPathBeepALL = Path.Combine(RootPath(), @"sounds\beep.wav");

            /* Pre-made soundPath for all colors *\
            string soundPathBeepRed = Path.Combine(RootPath(), @"sounds\redSound.wav");
            string soundPathBeepBlue = Path.Combine(RootPath(), @"sounds\blueSound.wav");
            string soundPathBeepOrange = Path.Combine(RootPath(), @"sounds\orangeSound.wav");
            string soundPathBeepGreen = Path.Combine(RootPath(), @"sounds\greenSound.wav");
            string soundPathBeepCaribBlue = Path.Combine(RootPath(), @"sounds\caribBlueSound.wav");
            string soundPathBeepGrey = Path.Combine(RootPath(), @"\sounds\greySound.wav");
            string soundPathBeepIndigo = Path.Combine(RootPath(), @"sounds\indigoSound.wav");
            string soundPathBeepMaroon = Path.Combine(RootPath(), @\sounds\maroonSound.wav");
            string soundPathBeepOlive = Path.Combine(RootPath(), @\sounds\oliveSound.wav");
            string soundPathBeepPink = Path.Combine(RootPath(), @\sounds\pinkSound.wav");
            */

            string soundPathTransition = Path.Combine(RootPath(), @"sounds\transistion.wav");
            string soundPathButtonClick = Path.Combine(RootPath(), @"sounds\buttonclick.wav");
            string soundPathWrong = Path.Combine(RootPath(), @"sounds\wrong.wav");
            string soundPathCorrect = Path.Combine(RootPath(), @"sounds\correct.wav");
            string soundPathStartupSound = Path.Combine(RootPath(), @"sounds\startupSound.wav");

            // Initiaize SoundPlayers
            redSound = new SoundPlayer(soundPathBeepALL);
            blueSound = new SoundPlayer(soundPathBeepALL);
            orangeSound = new SoundPlayer(soundPathBeepALL);
            greenSound = new SoundPlayer(soundPathBeepALL);
            caribBlueSound = new SoundPlayer(soundPathBeepALL);
            greySound = new SoundPlayer(soundPathBeepALL);
            indigoSound = new SoundPlayer(soundPathBeepALL);
            maroonSound = new SoundPlayer(soundPathBeepALL);
            oliveSound = new SoundPlayer(soundPathBeepALL);
            pinkSound = new SoundPlayer(soundPathBeepALL);

            transitionSound = new SoundPlayer(soundPathTransition);
            buttonClickSound = new SoundPlayer(soundPathButtonClick);
            wrongSound = new SoundPlayer(soundPathWrong);
            correctSound = new SoundPlayer(soundPathCorrect);
            startupSound = new SoundPlayer(soundPathStartupSound);

            ////>>>> Start Program <<<<////

            // Play startup sound
            startupSound.Play();

            // Display highscore at start
            TextBoxTopFive();

            // Use initial dictionary for start setup
            InitialDictionaryOfTilesAtStart();
        }

        // Initialize and return root path including directory \KeepYourFocus\
        static string RootPath()
        {
            string directoryPath = Environment.CurrentDirectory;

            if (string.IsNullOrEmpty(directoryPath))
            {
                Debug.WriteLine("Error: Application executable path is not valid.");
                return string.Empty; // Return an empty string
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
                Debug.WriteLine("Error: 'KeepYourFocus' directory not found in the path.");
                return string.Empty; // Return an empty string
            }
        }

        // Click Event for Start Button at start
        private void StartButtonClick(object sender, EventArgs e)
        {
            if (!startButton) return;

            buttonClickSound.Play();

            startButton = false;
            computer = true;

            // Set visibility text- and pictureboxes
            textBoxTopFive.Visible = false;
            textBoxMessage.Visible = false;
            // Enable all pictureboxes
            pictureBox1.Enabled = true;
            pictureBox2.Enabled = true;
            pictureBox3.Enabled = true;
            pictureBox4.Enabled = true;
            // Set startBTN invisible
            startBTN.Visible = false;
            startBTN.Enabled = false;

            counter_sequences = 1;

            UpdateSequence();
            UpdateRound();
            UpdateLevelName();
            ComputersTurn();
        }

        // Initialize Game Over Button
        private async void GameOverButton()
        {
            startBTN.Visible = true;
            startBTN.Enabled = true;
            startBTN.BackColor = Color.DarkRed;
            startBTN.Cursor = Cursors.Hand;
            // startBTN.Text = $"{new string(' ', 1)}Wrong\n Sequence";
            startBTN.Text = $"{new string(' ', 1)}GAME\nOVER";
            startBTN.FlatStyle = FlatStyle.Popup;

            await Task.Delay(500);

            textBoxTopFive.Visible = false;
            textBoxMessage.Visible = false;

            ReplaceAllTiles();
        }

        // Startup tiles dictionary 
        private void InitialDictionaryOfTilesAtStart()
        {
            if (pictureBoxDictionary.Count > 0)
            {
                RefreshAndRepositionPictureBoxes();
                return;
            }

            InitializePictureBox(pictureBox1, "Red", Path.Combine(RootPath(), @"png\red_tile512.png"));
            InitializePictureBox(pictureBox2, "Blue", Path.Combine(RootPath(), @"png\blue_tile512.png"));
            InitializePictureBox(pictureBox3, "Orange", Path.Combine(RootPath(), @"png\orange_tile512.png"));
            InitializePictureBox(pictureBox4, "Green", Path.Combine(RootPath(), @"png\green_tile512.png"));
        }

        // Returns a dictionary of all possible tiles
        static Dictionary<string, string> DictOfAllTiles()
        {
            string redTile = Path.Combine(RootPath(), "png", "red_tile512.png");
            string blueTile = Path.Combine(RootPath(), "png", "blue_tile512.png");
            string orangeTile = Path.Combine(RootPath(), "png", "orange_tile512.png");
            string greenTile = Path.Combine(RootPath(), "png", "green_tile512.png");
            string caribBlueTile = Path.Combine(RootPath(), "png", "caribBlue_tile512.png");
            string greyTile = Path.Combine(RootPath(), "png", "grey_tile512.png");
            string indigoTile = Path.Combine(RootPath(), "png", "indigo_tile512.png");
            string maroonTile = Path.Combine(RootPath(), "png", "maroon_tile512.png");
            string oliveTile = Path.Combine(RootPath(), "png", "olive_tile512.png");
            string pinkTile = Path.Combine(RootPath(), "png", "pink_tile512.png");

            Dictionary<string, string> dictOfAllTiles = new Dictionary<string, string>()
                                                                {
                                                                    {"Red", redTile},
                                                                    {"Blue", blueTile},
                                                                    {"Orange", orangeTile},
                                                                    {"Green", greenTile},
                                                                    {"CaribBlue", caribBlueTile},
                                                                    {"Grey", greyTile},
                                                                    {"Indigo", indigoTile},
                                                                    {"Maroon", maroonTile },
                                                                    {"Olive", oliveTile},
                                                                    {"Pink", pinkTile}
                                                                };
            return dictOfAllTiles;
        }

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
            }
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
                pictureBox.Location = SetFixedPositionPictureBoxes(index);
                index++;
            }
        }

        // Randomize tiles. No more then 2 of the same tiles in a row
        private string RandomizerTiles()
        {
            // Verify if the dictionary is not empty
            if (pictureBoxDictionary.Count == 0)
            {
                Debug.WriteLine("PictureBoxDictionary is empty. Verify filepaths of tiles in InitialDictionaryOfTilesAtStart()");
                throw new InvalidOperationException("PictureBoxDictionary is empty. Verify filepaths of tiles");
            }

            string newTile;
            bool isValid;

            do
            {
                // Select a new tile
                newTile = pictureBoxDictionary.Keys.ElementAt(rnd.Next(pictureBoxDictionary.Count));
                Debug.WriteLine($"\nGenerated newTile: {newTile}");

                // Validate the new tile
                if (previousTiles.Count < 2)
                {
                    // If the list has fewer than 2 items, any new tile is valid as long as it differs from the last one
                    isValid = previousTiles.Count < 1 || newTile != previousTiles[^1];
                }
                else
                {
                    // If the list has 2 or more items, ensure the new tile is different from the last two
                    isValid = newTile != previousTiles[^1] || newTile != previousTiles[^2];
                }

                Debug.WriteLine($"Is newTile valid? {isValid}");

            } while (!isValid);

            // Add the new tile to the list
            previousTiles.Add(newTile);

            // Keep only the last three tiles in the list
            if (previousTiles.Count > 3)
            {
                Debug.WriteLine($"Removed: {previousTiles[0]}");
                previousTiles.RemoveAt(0);
            }

            Debug.WriteLine($"Updated previousTiles: " + string.Join(", ", previousTiles));

            return newTile;
        }

        // Shuffles dictionary of all tiles
        private Dictionary<string, string> ShuffleDictOfAllTiles()
        {
            Dictionary<string, string> dictOfAllTiles = DictOfAllTiles();

            List<KeyValuePair<string, string>> listOfAllTiles = dictOfAllTiles.ToList();

            int numberOfItems = listOfAllTiles.Count;
            while (numberOfItems > 1)
            {
                numberOfItems--;
                int randomIndex = rnd.Next(numberOfItems + 1);
                KeyValuePair<string, string> temp = listOfAllTiles[randomIndex];
                listOfAllTiles[randomIndex] = listOfAllTiles[numberOfItems];
                listOfAllTiles[numberOfItems] = temp;
            }

            Dictionary<string, string> shuffledDictOfAllTiles = listOfAllTiles.ToDictionary(kv => kv.Key, kv => kv.Value);

            return shuffledDictOfAllTiles;
        }

        // Define fixed positions for PictureBoxes
        private static Point SetFixedPositionPictureBoxes(int index)
        {
            // Define fixed positions based on the index
            switch (index)
            {
                case 0:
                    return new Point(13, 12);
                case 1:
                    return new Point(321, 12);
                case 2:
                    return new Point(13, 316);
                case 3:
                    return new Point(321, 316);
                default:
                    return Point.Empty; // Default position if index is out of range
            }
        }

        private void RefreshAndRepositionPictureBoxes()
        {
            // Get the shuffled PictureBoxes
            var shuffledPictureBoxes = pictureBoxDictionary.Values.OrderBy(x => rnd.Next()).ToList();

            // Iterate through the shuffled PictureBoxes and assign fixed positions
            for (int i = 0; i < shuffledPictureBoxes.Count; i++)
            {
                shuffledPictureBoxes[i].Location = SetFixedPositionPictureBoxes(i);
                shuffledPictureBoxes[i].Visible = true;
            }
        }

        private void ComputersTurn()
        {
            computer = true;
            correctOrder.Add(RandomizerTiles());
            UpdateTurn(); // case computer's Turn
            DisplaySequence();
        }

        private async void DisplaySequence()
        {
            Dictionary<string, PictureBox> updatedPictureBoxDictionary;
            List<string> updatedCorrectOrder;
            bool replacementOccurred;

            // Verify if squares are replaced
            (updatedPictureBoxDictionary, updatedCorrectOrder, replacementOccurred) = ReplaceTileOnBoardAndInSequence();

            if (replacementOccurred)
            {
                // Use updated dictionaries and orders
                pictureBoxDictionary = updatedPictureBoxDictionary;
                correctOrder = updatedCorrectOrder;
            }

            computer = true;

            Debug.WriteLine($"\nDisplay Sequence: {counter_sequences}");
            Debug.WriteLine("correctOrder = " + string.Join(", ", correctOrder));
            Debug.WriteLine("pictureBoxDictionary = " + string.Join(", ", pictureBoxDictionary.Keys));

            await Task.Delay(500);

            foreach (var tile in correctOrder)
            {
                var box = pictureBoxDictionary[tile];
                if (box == null)
                    continue;

                await Task.Delay(500); // Delay 500 ms before start highlights and beepSound

                PlaySound(tile);

                SetHighlight(box, true);
                await Task.Delay(150);
                SetHighlight(box, false);
                await Task.Delay(50);
            }
            // Verify difficulty
            SetTurnActions();

            await Task.Delay(500); // Delay 500 ms before calling PlayersTurn()

            computer = false;

            UpdateTurn(); // case Player's Turn
        }

        private async void PlayersTurn(object? sender, EventArgs e)
        {
            // Block Player's clicks in computer's turn AND before StartButton is clicked
            if (startButton || computer) return;

            if (sender is PictureBox clickedBox)
            {
                string tile = clickedBox.Tag?.ToString() ?? "";

                PlaySound(tile);
                SetHighlight(clickedBox, true);

                playerOrder.Add(tile);

                // Verify difficulty
                SetTurnActions();

                // Verify each input with correctOrder
                for (int input = 0; input < playerOrder.Count; input++)
                {
                    if (playerOrder[input] != correctOrder[input])
                    {
                        await Task.Delay(100);
                        SetHighlight(clickedBox, false);
                        await Task.Delay(250); // Delay to provide feedback before game over
                        GameOver();
                        TextBoxTopFive();
                        return;
                    }
                }
                if (playerOrder.Count == correctOrder.Count)
                {
                    await Task.Delay(100);
                    SetHighlight(clickedBox, false);
                    await Task.Delay(50);

                    VerifyAndManageCountersAndLevels();
                }
                else
                {
                    await Task.Delay(100);
                    SetHighlight(clickedBox, false);
                    await Task.Delay(50);
                }
            }
        }

        private async void VerifyAndManageCountersAndLevels()
        {
            if (playerOrder.Count < correctOrder.Count)
                return;

            // Block player's clicks
            computer = true;

            nextRound = true;

            // Delay 250 ms between beepSound and correctSound
            await Task.Delay(250);
            correctSound.Play();

            SetCounters();
            UpdateSequence();
            UpdateRound();
            UpdateLevelName();
            UpdateTurn();

            await Task.Delay(2750);

            playerOrder.Clear();
            nextRound = false;
            ComputersTurn();
        }

        // TESTING WITH 6 SEQUENCES PER LEVEL
        private void SetCounters()
        {
            switch (counter_sequences)
            {
                case (6) when counter_levels < 7:
                    levelUp = true;
                    correctOrder.Clear();
                    playerOrder.Clear();
                    counter_sequences = 1; // Reset sequence to 1
                    counter_levels++;
                    counter_rounds++;

                    ReplaceAllTiles();

                    UpdateTurn();
                    levelUp = false;
                    break;
                default:
                    if (counter_levels >= 7)
                    {
                        levelUp = true;
                        counter_sequences++;
                        counter_rounds++;
                        UpdateTurn();
                    }
                    else
                    {
                        counter_sequences++;
                        counter_rounds++;
                        UpdateTurn();
                    }
                    break;
            }
        }

        // Method to verify difficulties. 2 cases: computers turn and Players turn
        private void SetTurnActions()
        {

            switch (computer)
            {
                // Computers Turn //
                case true:
                    DisplayLabelMessage(true);
                    ShufflePictureBoxes();
                    break;
                // Players Turn //
                case false:
                    DisplayLabelMessage(false);
                    ShufflePictureBoxes();
                    break;
            }
        }


        ////>>>> DIFFICULTIES <<<<////


        // Shuffle currect tile setup before player's turn and/or after player's click
        private async void ShufflePictureBoxes() // Called in SetTurnActions()
        {
            switch (computer)
            {
                case (true):
                    if (counter_levels == 2 && rnd.Next(100) <= 55 ||
                        counter_levels >= 3 && rnd.Next(100) <= 75 ||
                        counter_levels >= 5 && rnd.Next(100) <= 85)
                    {
                        Debug.WriteLine($"Shuffle PictureBoxes Case 1: Shuffle before player's turn");

                        await Task.Delay(250); // Delay 250 ms for space between colorSound and transitionSound
                        transitionSound.Play();
                        RandomizerShufflePictureBoxes();
                        RefreshAndRepositionPictureBoxes();
                        await Task.Delay(1000);
                    }
                    break;
                case (false):
                    if (counter_levels >= 3 && rnd.Next(100) <= 55 ||
                        counter_levels >= 4 && rnd.Next(100) <= 75 ||
                        counter_levels >= 6 && rnd.Next(100) <= 85)
                    {
                        Debug.WriteLine($"Shuffle PictureBoxes Case 2: Shuffle after player click");

                        RandomizerShufflePictureBoxes();
                        RefreshAndRepositionPictureBoxes();
                    }
                    break;
            }
        }

        // Replace and switch all tiles when level up
        private void ReplaceAllTiles() // Called in SetCounters()
        {
            if (counter_levels >= 4 && levelUp == true && rnd.Next(100) <= 55 ||
                counter_levels >= 5 && levelUp == true && rnd.Next(100) <= 75 ||
                counter_levels >= 7 && levelUp == true && rnd.Next(100) <= 85)
            {

                Dictionary<string, string> shuffledTiles = ShuffleDictOfAllTiles();

                // Ensure that we have enough colors to assign
                if (shuffledTiles.Count >= 3)
                {
                    // Retrieve the first 4 key-value pairs from shuffledTiles
                    KeyValuePair<string, string> kvp1 = shuffledTiles.ElementAt(0);
                    KeyValuePair<string, string> kvp2 = shuffledTiles.ElementAt(1);
                    KeyValuePair<string, string> kvp3 = shuffledTiles.ElementAt(2);
                    KeyValuePair<string, string> kvp4 = shuffledTiles.ElementAt(3);

                    try
                    {
                        // Clear the dictionary and add the new tiles
                        pictureBoxDictionary.Clear();

                        InitializePictureBox(pictureBox1, kvp1.Key, kvp1.Value);
                        InitializePictureBox(pictureBox2, kvp2.Key, kvp2.Value);
                        InitializePictureBox(pictureBox3, kvp3.Key, kvp3.Value);
                        InitializePictureBox(pictureBox4, kvp4.Key, kvp4.Value);
                    }
                    catch (ArgumentException ex)
                    {
                        Debug.WriteLine($"An item with the same key has already been added: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
                else
                {
                    Debug.WriteLine("Not enough tiles in shuffledTiles to initialize picture boxes.");
                }
            }
        }

        // Method to replace 1 tile in running sequence and/or on board. Returns (Dict pictureBoxDictionary, List correctOrder, bool replacementOccurred)
        private (Dictionary<string, PictureBox>, List<string>, bool) ReplaceTileOnBoardAndInSequence() // Called in DisplaySequence()
        {
            string newTile = RandomizerTiles();
            Dictionary<string, string> dictOfAllTiles = DictOfAllTiles();
            List<KeyValuePair<string, string>> listOfAllTiles = dictOfAllTiles.ToList();

            bool checkReplaceInOrder = (counter_levels >= 5 && correctOrder.Count > 2 && rnd.Next(100) <= 55) ||
                                       (counter_levels >= 6 && correctOrder.Count > 2 && rnd.Next(100) <= 75) ||
                                       (counter_levels >= 8 && correctOrder.Count > 2 && rnd.Next(100) <= 85);

            bool checkReplaceOnBoard = (counter_levels >= 6 && correctOrder.Count > 2 && rnd.Next(100) <= 55) ||
                                       (counter_levels >= 7 && correctOrder.Count > 2 && rnd.Next(100) <= 75) ||
                                       (counter_levels >= 9 && correctOrder.Count > 2 && rnd.Next(100) <= 85);

            bool replacementOccurred = false; // Flag to indicate if any replacement happened

            if (checkReplaceInOrder || checkReplaceOnBoard)
            {
                // Make copy of correctOrder as copyCorrectOrder
                List<string> copyCorrectOrder = new List<string>(correctOrder);

                // Randomize tile to delete from copyCorrectOrder
                int randomIndex = rnd.Next(copyCorrectOrder.Count);
                string deleteTile = copyCorrectOrder[randomIndex];

                Debug.WriteLine($"deleteTile: [{deleteTile}]");

                if (checkReplaceInOrder && newTile != deleteTile && randomIndex != copyCorrectOrder.Count - 1)
                {
                    Debug.WriteLine("\nCorrectOrder = " + string.Join(", ", correctOrder));
                    Debug.WriteLine($"Replacing in order [{deleteTile}] at index [{randomIndex}] with new tile [{newTile}]\n");

                    copyCorrectOrder[randomIndex] = newTile;

                    // Update correctOrder with the new copyCorrectOrder
                    correctOrder = copyCorrectOrder;
                    replacementOccurred = true;
                }
                if (checkReplaceOnBoard)
                {
                    // Get the PictureBox associated with the deleteTile
                    PictureBox pictureBoxToReplace = pictureBoxDictionary[deleteTile];

                    // Remove the old tile from the board
                    pictureBoxDictionary.Remove(deleteTile);

                    // Randomize new tile that's not in the remaining colors on the board
                    string pickNewTile;
                    do
                    {
                        pickNewTile = listOfAllTiles[rnd.Next(listOfAllTiles.Count)].Key;
                        Debug.WriteLine($"pickNewTile: [{pickNewTile}]");
                    } while (copyCorrectOrder.Contains(pickNewTile) || pictureBoxDictionary.ContainsKey(pickNewTile));

                    Debug.WriteLine("\nCorrectOrder = " + string.Join(", ", correctOrder));
                    Debug.WriteLine($"Replaced on board and in order [{deleteTile}] with [{pickNewTile}]");

                    // Initialize the PictureBox with the new tile
                    InitializePictureBox(pictureBoxToReplace, pickNewTile, dictOfAllTiles[pickNewTile]);

                    // Add the new tile to the pictureBoxDictionary
                    pictureBoxDictionary[pickNewTile] = pictureBoxToReplace;

                    // Iter through copyCorrectOrder and replace all deleteTile with pickNewcolor at their index
                    for (int indexItem = 0; indexItem < copyCorrectOrder.Count; indexItem++)
                    {
                        if (copyCorrectOrder[indexItem] == deleteTile)
                        {
                            copyCorrectOrder[indexItem] = pickNewTile;
                        }
                    }

                    // Update correctOrder with the new copyCorrectOrder
                    correctOrder = copyCorrectOrder;
                    replacementOccurred = true;
                }
                Debug.WriteLine("Updated correctOrder = " + string.Join(", ", correctOrder));
                Debug.WriteLine("Updated pictureBoxDictionary = " + string.Join(", ", pictureBoxDictionary.Keys));
            }
            return (pictureBoxDictionary, correctOrder, replacementOccurred);
        }

        private async void DisplayLabelMessage(bool iscomputerTurn)
        {
            /*
             * Show labels with text in either computer's or Player's turn
             * E.g. Computer's turn: "Click Here", "Start Here!", "Start With this One!" (45%, 55% or 65% chance depending on level)
             * Player's turn: various messages based on different levels and conditions
             */

            int chance = counter_levels >= 6 ? 55 : 45;
            bool showMessage = iscomputerTurn
                ? counter_levels >= 2 && rnd.Next(100) <= chance && correctOrder.Count != correctOrder.Count - 1
                : counter_levels >= 2 && rnd.Next(100) <= 65 && playerOrder.Count != correctOrder.Count;

            if (showMessage)
            {
                List<Label> labels = new List<Label> { LabelMessage1, LabelMessage2, LabelMessage3, LabelMessage4 };
                List<string> labelText;

                if (iscomputerTurn)
                {
                    labelText = new List<string> { "Click Here", "Start Here!", "Start With\nthis One!", "This One!", "Over Here!" };
                }
                else // is Player's turn
                {
                    labelText = new List<string>
                                                {
                                                 "Click Here", "This Is NOT\nThe Correct tile!", "The computer\nIs Lying!",
                                                 "This Is\nThe One!", "Just Kidding!\nClick This One!", "This Is NOT\nThe Right Tile!",
                                                 "This Is\nThe Next\nOne!", "Now This One!", "This One!", "Over Here!"
                                                };
                }

                // Randomize Label 1 - label 4
                int pickLabelIndex = rnd.Next(labels.Count);
                Label randomizedLabelClickHere = labels[pickLabelIndex];

                // Randomize labelText
                int pickLabelText = rnd.Next(labelText.Count);
                string randomizedText = labelText[pickLabelText];

                await Task.Delay(250);

                // Initialize label
                randomizedLabelClickHere.AutoSize = true;
                randomizedLabelClickHere.Text = randomizedText;
                randomizedLabelClickHere.Visible = true;
                await Task.Delay(750);
                randomizedLabelClickHere.Visible = false;
            }
        }

        ////>>>> INITIALIZE HIGHLIGHTS, SOUND, TEXTBOXES AND GAME OVER <<<<////

        private void SetHighlight(PictureBox pictureBox, bool highlight)
        {
            if (pictureBox.InvokeRequired)
            {
                pictureBox.Invoke(new Action<PictureBox, bool>(SetHighlight), pictureBox, highlight);
            }
            else
            {
                if (highlight) // Higlight on
                {
                    pictureBox.BorderStyle = BorderStyle.None;
                    pictureBox.Padding = new Padding(5);
                    pictureBox.BackColor = Color.White;
                }
                else // Highlight off
                {
                    pictureBox.Padding = new Padding(0);
                    pictureBox.BackColor = Color.Transparent;
                }
            }
        }

        private void PlaySound(string tile)
        {
            switch (tile)
            {
                case "Red":
                    redSound.Play();
                    break;
                case "Blue":
                    blueSound.Play();
                    break;
                case "Orange":
                    orangeSound.Play();
                    break;
                case "Green":
                    greenSound.Play();
                    break;
                case "CaribBlue":
                    caribBlueSound.Play();
                    break;
                case "Grey":
                    greySound.Play();
                    break;
                case "Indigo":
                    indigoSound.Play();
                    break;
                case "Maroon":
                    maroonSound.Play();
                    break;
                case "Olive":
                    oliveSound.Play();
                    break;
                case "Pink":
                    pinkSound.Play();
                    break;
            }
        }

        // Update richtextbox ShowNumbersOfSequences
        private void UpdateSequence()
        {
            richTextBoxShowNumbersOfSequences.BackColor = Color.Yellow;
            richTextBoxShowNumbersOfSequences.Text = $"{new string(' ', 4)}Sequence of {counter_sequences}";
        }

        // Update richtextbox Turn
        private async void UpdateTurn()
        {
            switch (computer, startButton, nextRound)
            {
                // Next Round or Level Up
                case (_, _, true):
                    if (levelUp)
                    {
                        // Debug.WriteLine("UpdateTurn LevelUp");

                        richTextBoxTurn.BackColor = Color.LightGreen;
                        richTextBoxTurn.Text = $"\n{new string(' ', 7)}CORRECT";

                        await Task.Delay(1500);

                        richTextBoxTurn.Text = $"\n{new string(' ', 8)}Level  Up";

                        // ReplaceAllTiles();

                        await Task.Delay(1000);
                        break;
                    }
                    else
                    {
                        // Debug.WriteLine("UpdateTurn Correct");

                        richTextBoxTurn.BackColor = Color.LightGreen;
                        richTextBoxTurn.Text = $"\n{new string(' ', 7)}CORRECT";

                        await Task.Delay(1500);
                        richTextBoxTurn.Text = $"\n{new string(' ', 3)}Next Sequence";

                        await Task.Delay(1000);
                        break;
                    }
                // computer's turn
                case (true, false, _):
                    richTextBoxTurn.BackColor = Color.Salmon;
                    richTextBoxTurn.Text = $"{new string(' ', 4)}computer's Turn";
                    richTextBoxTurn.Text = $"{new string(' ', 9)}Running\n{new string(' ', 14)}::\n{new string(' ', 8)}Sequence";
                    break;
                // Player's turn
                case (false, false, _):
                    richTextBoxTurn.BackColor = Color.Green;
                    richTextBoxTurn.Text = $"\n{new string(' ', 5)}Player's Turn";
                    break;
            }
        }

        // Update richtextbox ShowRounds
        private void UpdateRound()
        {
            richTextBoxShowRounds.BackColor = Color.LightSkyBlue;
            richTextBoxShowRounds.Text = $"{new string(' ', 1)}Total Rounds: {new string(' ', 0)}{counter_rounds}";
            richTextBoxShowRounds.SelectAll();
            richTextBoxShowRounds.SelectionAlignment = HorizontalAlignment.Center;
        }

        // Update richtextbox ShowLevel
        private void UpdateLevelName()
        {
            switch (counter_levels)
            {
                case (1):
                    richTextBoxShowLevelName.BackColor = Color.LightSkyBlue;
                    richTextBoxShowLevelNumber.BackColor = Color.LightSkyBlue;

                    richTextBoxShowLevelNumber.Text = $"{new string(' ', 3)}{counter_levels}";
                    richTextBoxShowLevelName.Text = $"{new string(' ', 1)}EasyPeasy";
                    break;
                case (2):
                    richTextBoxShowLevelName.BackColor = Color.SkyBlue;
                    richTextBoxShowLevelNumber.BackColor = Color.SkyBlue;

                    richTextBoxShowLevelNumber.Text = $"{new string(' ', 3)}{counter_levels}";
                    richTextBoxShowLevelName.Text = $"{new string(' ', 4)}OkiDoki";
                    break;
                case (3):
                    richTextBoxShowLevelName.BackColor = Color.CornflowerBlue;
                    richTextBoxShowLevelNumber.BackColor = Color.CornflowerBlue;

                    richTextBoxShowLevelNumber.Text = $"{new string(' ', 3)}{counter_levels}";
                    richTextBoxShowLevelName.Text = $"{new string(' ', 2)}Please No";
                    break;
                case (4):
                    richTextBoxShowLevelName.BackColor = Color.RoyalBlue;
                    richTextBoxShowLevelNumber.BackColor = Color.RoyalBlue;

                    richTextBoxShowLevelNumber.Text = $"{new string(' ', 3)}{counter_levels}";
                    richTextBoxShowLevelName.Text = $"{new string(' ', 4)}No Way!";
                    break;
                case (5):
                    richTextBoxShowLevelName.BackColor = Color.DarkKhaki;
                    richTextBoxShowLevelNumber.BackColor = Color.DarkKhaki;

                    richTextBoxShowLevelNumber.Text = $"{new string(' ', 3)}{counter_levels}";
                    richTextBoxShowLevelName.Text = $"{new string(' ', 1)}HELL NO";
                    break;
                case (6):
                    richTextBoxShowLevelName.BackColor = Color.DarkOrange;
                    richTextBoxShowLevelNumber.BackColor = Color.DarkOrange;

                    richTextBoxShowLevelNumber.Text = $"{new string(' ', 3)}{counter_levels}";
                    richTextBoxShowLevelName.Text = $"{new string(' ', 0)}NONONONO";
                    break;
                case (999):
                    richTextBoxShowLevelNumber.BackColor = Color.Red;
                    richTextBoxShowLevelName.BackColor = Color.Red;
                    richTextBoxShowRounds.BackColor = Color.Red;

                    richTextBoxShowNumbersOfSequences.Text = $"{new string(' ', 1)}WRONGWRONG";
                    break;
                default:
                    richTextBoxShowLevelName.BackColor = Color.OrangeRed;
                    richTextBoxShowLevelNumber.BackColor = Color.OrangeRed;

                    richTextBoxShowLevelNumber.Text = $"{new string(' ', 1)}666";
                    richTextBoxShowLevelName.Text = $"{new string(' ', 0)}HELLMODE";
                    break;
            }
        }

        private void PlayerInfo(object sender, EventArgs e)
        {
            // empty textbox Click action for input playerName
        }

        private void TextBoxMessage()
        {
            // empty textbox Click action for input playerName
        }

        // Shows Top 5 of best scores in TextBoxTopFive
        private void TextBoxTopFive()
        {
            Dictionary<int, (int, string)> allScores = OrderBestScores();

            // Sort and take the top five scores
            var topFiveScores = allScores.OrderByDescending(kvp => kvp.Key).Take(5);

            // Set textbox visible
            textBoxTopFive.Visible = true;

            // Clear any existing text
            textBoxTopFive.Clear();

            // Use a fixed-width font for proper alignment
            textBoxTopFive.Font = new Font("Courier New", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);

            // Add the header
            // Format "{0, -8} {1, -8} {2, -8} {3}\r\n" specifies the column widths, where -8 indicates left alignment with a width of 8 characters.
            textBoxTopFive.Text = "===========\r\n===BestScores===\r\n===========\r\n\n\n";
            textBoxTopFive.AppendText(string.Format("{0, -8} {1, -8} {2, -8} {3}\r\n\n", "Place", "Score", "Level", "Name"));

            // Append the top five scores
            int lineNumber = 1;
            foreach (var kvp in topFiveScores)
            {
                int score = kvp.Key;
                int levelReached = kvp.Value.Item1;
                string levelName = kvp.Value.Item2;

                textBoxTopFive.AppendText(string.Format("{0, -8} {1, -8} {2, -8} {3}\r\n", lineNumber, score, levelReached, levelName));
                lineNumber++;
            }
        }

        // When Game Over, saves score in Highscore.txt on new line
        private void SaveScore(int playerScore, int levelReached, string levelName)
        {
            string file = Path.Combine(RootPath(), "Highscore.txt");

            try
            {
                // Read the existing content of the file
                string existingContent = File.Exists(file) ? File.ReadAllText(file) : string.Empty;

                // Write the new score followed by the existing content
                using (StreamWriter saveScore = new StreamWriter(file, false))
                {
                    saveScore.WriteLine($"{playerScore},{levelReached},{levelName.Trim()}");
                    saveScore.Write(existingContent);
                }

                MessageBox.Show("Game data saved successfully.", "Save Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GetScoresFromFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving game data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Method returns dictionary from all data in Highscore.txt
        private Dictionary<int, (int, string)> GetScoresFromFile()
        {
            string file = Path.Combine(RootPath(), "Highscore.txt");
            Dictionary<int, (int, string)> dictScores = new Dictionary<int, (int, string)>();

            try
            {
                using (StreamReader getHighscore = new StreamReader(file))
                {
                    string line;
                    int counter_lines = 0;

                    while ((line = getHighscore.ReadLine()) != null)
                    {
                        // Split the line into parts if needed
                        string[] parts = line.Split(',');
                        if (parts.Length >= 3)
                        {
                            if (int.TryParse(parts[0], out int playerScore) && int.TryParse(parts[1], out int levelReached))
                            {
                                string levelName = parts[2].Trim();
                                dictScores[playerScore] = (levelReached, levelName);

                                // Process the parsed values as needed
                                Debug.WriteLine($"Player Score: {playerScore}, Level Reached: {levelReached}, Level Name: {levelName}");
                            }
                        }
                        counter_lines++;
                    }
                }
                MessageBox.Show("Scores read successfully.", "Read Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while reading scores: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dictScores;
        }

        // Returns decreasing ordered dictionary of higscores.txt
        private Dictionary<int, (int, string)> OrderBestScores()
        {
            Dictionary<int, (int, string)> bestScores = new Dictionary<int, (int, string)>();

            try
            {
                // Get scores from file
                Dictionary<int, (int, string)> orderDictScores = GetScoresFromFile();

                // Order by playerScore (Key) and take the top five scores
                bestScores = orderDictScores.OrderByDescending(x => x.Key)
                                              .Take(orderDictScores.Count)
                                              .ToDictionary(x => x.Key, x => x.Value);

                // Example output to Debug (you can adjust how you use bestScores)
                foreach (var score in bestScores)
                {
                    Debug.WriteLine($"Player Score: {score.Key}, Level Reached: {score.Value.Item1}, Level Name: {score.Value.Item2}");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"An error occurred while getting top ten scores: {e.Message}");
                // Optionally handle or log the exception
            }
            return bestScores;
        }

        private void GameOver()
        {
            computer = false;
            startButton = true;

            SaveScore(counter_rounds, counter_levels, richTextBoxShowLevelName.Text);

            correctOrder.Clear();
            playerOrder.Clear();

            wrongSound.Play();

            counter_levels = 999;

            UpdateLevelName();
            GameOverButton();
            InitialDictionaryOfTilesAtStart();

            counter_rounds = 1;
            counter_levels = 1;
        }
    }
}