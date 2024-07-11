using System;
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
using System.Windows.Forms;

namespace KeepYourFocus
{
    public partial class PlayerField : Form
    {
        private Dictionary<string, PictureBox> pictureBoxDictionary = new Dictionary<string, PictureBox>();
        private List<string> correctOrder = new List<string>();
        private List<string> playerOrder = new List<string>();
        private List<string> previousTiles = new List<string>();
        private List<string> storePlayerName = new List<string> { "PEANUTSCH" };

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
        private bool gameTime = false;

        bool isComputerTurn = false;
        bool isPlayerTurn = false;
        bool isSetCounters = false;
        bool isDisplaySequence = false;
        bool gameOver = false;

        private Stopwatch gameStopwatch = new Stopwatch();

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
            string soundPathBeepGrey = Path.Combine(RootPath(), @"sounds\greySound.wav");
            string soundPathBeepIndigo = Path.Combine(RootPath(), @"sounds\indigoSound.wav");
            string soundPathBeepMaroon = Path.Combine(RootPath(), @"sounds\maroonSound.wav");
            string soundPathBeepOlive = Path.Combine(RootPath(), @"sounds\oliveSound.wav");
            string soundPathBeepPink = Path.Combine(RootPath(), @"sounds\pinkSound.wav");
            */

            string soundPathTransition = Path.Combine(RootPath(), @"sounds\transistion.wav");
            string soundPathButtonClick = Path.Combine(RootPath(), @"sounds\buttonclick.wav");
            string soundPathWrong = Path.Combine(RootPath(), @"sounds\wrong.wav");
            string soundPathCorrect = Path.Combine(RootPath(), @"sounds\correct.wav");
            string soundPathStartupSound = Path.Combine(RootPath(), @"sounds\startupSound.wav");

            // Initiaize SoundPlayers
            redSound = new SoundPlayer(soundPathBeepALL);       // redSound = new SoundPlayer(soundPathBeepRed); 
            blueSound = new SoundPlayer(soundPathBeepALL);      // blueSound = new SoundPlayer(soundPathBeepBlue); 
            orangeSound = new SoundPlayer(soundPathBeepALL);    // orangeSound = new SoundPlayer(soundPathBeepOrange); 
            greenSound = new SoundPlayer(soundPathBeepALL);     // greenSound = new SoundPlayer(soundPathBeepGreen);
            caribBlueSound = new SoundPlayer(soundPathBeepALL); // caribBlueSound = new SoundPlayer(soundPathBeepCaribBlue);
            greySound = new SoundPlayer(soundPathBeepALL);      // greySound = new SoundPlayer(soundPathBeepGrey); 
            indigoSound = new SoundPlayer(soundPathBeepALL);    // indigoSound = new SoundPlayer(soundPathBeepIndigo);
            maroonSound = new SoundPlayer(soundPathBeepALL);    // maroonSound = new SoundPlayer(soundPathBeepMaroon);
            oliveSound = new SoundPlayer(soundPathBeepALL);     // oliveSound = new SoundPlayer(soundPathBeepOlive);
            pinkSound = new SoundPlayer(soundPathBeepALL);      // pinkSound = new SoundPlayer(soundPathBeepPink);

            transitionSound = new SoundPlayer(soundPathTransition);
            buttonClickSound = new SoundPlayer(soundPathButtonClick);
            wrongSound = new SoundPlayer(soundPathWrong);
            correctSound = new SoundPlayer(soundPathCorrect);
            startupSound = new SoundPlayer(soundPathStartupSound);

            // Initialize Stopwatch for gametime
            gameStopwatch = new Stopwatch();

            ////>>>> Start Program <<<<////

            // Welcome MessageBox
            WelcomeMessageBox();

            // LinkLabels GitHub and Email
            InitializeLinkLabels();

            // Alihning richTextBoxes
            AlignTextButtonBoxesCenter();

            // Display highscore at start
            TextBoxHighscore();
            //SecondTextBoxTopFive();

            // Use initial dictionary for start setup
            InitialDictionaryOfTilesAtStart();

            // Play startup sound
            startupSound.Play();

            // Display textbox for input
            textBoxInputName.Visible = true;

            // Ask for Player's name
            PlayerName();
        }

        // Thank You + some info Spam MessageBox
        private void WelcomeMessageBox()
        {
            MessageBox.Show(
                            "   Thank you for testing the heck out of my very first try-out\r\n" +
                            "   in C# coding!\r\n\r\n" +
                            " * Simon Says-like game with some level based challenges\r\n" +
                            " * Each level has 6 sequences. After 6 succesful sequences:\r\n" +
                            " * Level++; Add 1 challenge; Clear correctOrder and\r\n" +
                            " * playerOrder and start with new sequence = 1\r\n" +
                            " * From Level >= 7: no Clear correctOrder; sequences++\r\n" +
                            " * untill game over\r\n" +
                            " * More challenges, tiles and sounds in progress\r\n" +
                            " * Next update: file encryption!\r\n" +
                            " * You can find all the chaos at:\r\n" +
                            "   https://github.com/Peanutsch/KeepYourFocus.git\r\n" +
                            " * Please feel free to review my code and give feedback!\r\n\r\n" +
                            "   Michiel / Peanutsch\r\n" +
                            "   peanutsch@duck.com\r\n" +
                            "   (preRookie wannabeeCodeDev)\r\n",
                            "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information
                            );
        }

        // Get playerName via TextBoxInputName
        private void PlayerName()
        {
            // Display textbox for input
            textBoxInputName.Visible = true;
            textBoxInputName.Enabled = true;

            // Enter input by buttonEnter and Key.Enter
            textBoxInputName.Focus();
            KeyDownEnter();
        }

        // Stopwatch for recording gametime
        private string GameStopwatch()
        {
            if (gameTime)
            {
                // Reset stopwatch before Start
                gameStopwatch.Reset();
                gameStopwatch.Start();
                Debug.WriteLine("\n[Start Stopwatch]\n");
                return "";
            }
            else
            {
                gameStopwatch.Stop();
                Debug.WriteLine("\n[Stop Stopwatch]\n");

                if (gameStopwatch.Elapsed.TotalMilliseconds > 0)
                {
                    // Format the elapsed time minutes:seconds
                    string elapsedGameTime = gameStopwatch.Elapsed.ToString(@"mm\:ss");
                    Debug.WriteLine($"Elapsed time: {elapsedGameTime}");
                    return elapsedGameTime;
                }
                else
                {
                    Debug.WriteLine("elapsedTime is empty. No time recorded!");
                    return string.Empty;
                }
            }
        }

        // TESTING --> Initialize and return root path including directory \KeepYourFocus\
        static string TESTRootPath()
        {
            // Use the local application data path and the app name to construct the root path
            string localAppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KeepYourFocus");

            if (string.IsNullOrEmpty(localAppDataPath))
            {
                Debug.WriteLine("Error: Application path is not valid.");
                return string.Empty; // Return an empty string
            }

            // Ensure the directory exists, create if it doesn't
            try
            {
                Directory.CreateDirectory(localAppDataPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: Unable to create application directory. {ex.Message}");
                return string.Empty; // Return an empty string
            }

            // Ensure the path ends with a directory separator
            if (!localAppDataPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                localAppDataPath += Path.DirectorySeparatorChar;
            }

            return localAppDataPath;
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
        private void ButtonStart_Click(object sender, EventArgs e)
        {
            if (!startButton)
                return;

            buttonClickSound.Play();

            Debug.WriteLine("gameTime = true");
            gameTime = true;
            startButton = false;
            computer = true;

            // Start Stopwatch
            GameStopwatch();

            // Set Highscores invisible
            textBoxHighscore.Visible = false;
            // Enable all pictureboxes
            pictureBox1.Enabled = true;
            pictureBox2.Enabled = true;
            pictureBox3.Enabled = true;
            pictureBox4.Enabled = true;
            // Set startBTN invisible
            startBTN.Visible = false;
            startBTN.Enabled = false;
            //Set LinkLabels invisible
            linkLabelGitHub.Visible = false;
            linkLabelGitHub.Enabled = false;
            linkLabelEmail.Visible = false;
            linkLabelEmail.Enabled = false;

            counter_sequences = 1;

            UpdateSequence();
            UpdateRound();
            UpdateLevelName();
            ComputersTurn();
        }

        // Initialize Game Over Button
        private async void ButtonGameOver_Click()
        {
            textBoxHighscore.Visible = true;

            startBTN.Visible = true;
            startBTN.Enabled = true;
            startBTN.BackColor = Color.DarkRed;
            startBTN.Cursor = Cursors.Hand;

            //startBTN.Text = $"{new string(' ', 1)}GAME\nOVER";
            startBTN.Text = $"Click to\nRETRY";
            startBTN.FlatStyle = FlatStyle.Popup;

            //Set LinkLabels invisible
            linkLabelGitHub.Visible = true;
            linkLabelGitHub.Enabled = true;
            linkLabelEmail.Visible = true;
            linkLabelEmail.Enabled = true;

            await Task.Delay(500);

            InitialDictionaryOfTilesAtStart();
        }

        // InitializeKeyDownENTER
        private void KeyDownEnter()
        {
            string playerName = textBoxInputName.Text.Trim().ToUpper();

            // Event handler for KeyDown within this method
            textBoxInputName.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true; // Prevents the Enter key from inserting a newline
                    e.SuppressKeyPress = true; // Stops the "ding" sound
                    playerName = textBoxInputName.Text.Trim().ToUpper();

                    if (!string.IsNullOrWhiteSpace(playerName) && playerName != "YOURNAME" && playerName != "YOUR NAME")
                    {
                        storePlayerName.Clear();
                        storePlayerName.Add(playerName);

                        // After valid input: disable textBoxInputName and buttonEnter, enable startBTN
                        textBoxInputName.Visible = false;
                        textBoxInputName.Enabled = false;
                        buttonEnter.Enabled = false;
                        buttonEnter.Visible = false;

                        startBTN.TextAlign = ContentAlignment.MiddleCenter;
                        startBTN.Text = "Click to Start";
                        startBTN.Enabled = true;
                        startButton = true;
                        // Fill richtextboxes
                        richTextBoxShowRounds.Text = $"Succes {playerName}";

                        Debug.WriteLine($"Input name is {playerName}");
                    }
                    else
                    {
                        playerName = storePlayerName[0];
                        Debug.WriteLine($"Forced input name is {playerName}");

                        // Continue without text in richTextBoxShowLevelName and richTextBoxShowRounds
                        textBoxInputName.Visible = false;
                        textBoxInputName.Enabled = false;

                        buttonEnter.Enabled = false;
                        buttonEnter.Visible = false;

                        startBTN.TextAlign = ContentAlignment.MiddleCenter;
                        startBTN.Text = "Click to Start";
                        startBTN.Enabled = true;
                        startButton = true;
                    }
                }
            };
        }

        // Initialize OK button for input playerName
        private void ButtonEnter_Click(object sender, EventArgs e)
        {
            string playerName = textBoxInputName.Text.Trim().ToUpper();

            if (!string.IsNullOrWhiteSpace(playerName) && playerName != "YOURNAME" && playerName != "YOUR NAME")
            {
                storePlayerName.Clear();
                storePlayerName.Add(playerName);

                // After valid input: disable textBoxInputName, enable startBTN
                textBoxInputName.Visible = false;
                textBoxInputName.Enabled = false;
                startBTN.Enabled = true;
                startButton = true;

                Debug.WriteLine($"Input name is {playerName}");

                // Fill richtextboxes
                richTextBoxShowRounds.Text = $"Success {playerName}";

                startBTN.TextAlign = ContentAlignment.MiddleCenter;
                startBTN.Text = "Click to Start";

                buttonEnter.Enabled = false;
                buttonEnter.Visible = false;
            }
            else
            {
                if (storePlayerName.Count > 0)
                {
                    playerName = storePlayerName[0];
                    Debug.WriteLine($"Forced input name is {playerName}");
                }

                textBoxInputName.Visible = false;
                textBoxInputName.Enabled = false;
                buttonEnter.Enabled = false;
                buttonEnter.Visible = false;

                startBTN.TextAlign = ContentAlignment.MiddleCenter;
                startBTN.Text = "Click to Start";
                startBTN.Enabled = true;
                startButton = true;
            }
        }

        private void InitializeLinkLabels()
        {
            // Setup LinkLabels text
            linkLabelGitHub.Text = "https://github.com/Peanutsch/KeepYourFocus.git";
            linkLabelEmail.Text = "peanutsch@duck.com";

            // Add link data
            linkLabelGitHub.Links.Add(0, linkLabelGitHub.Text.Length, "https://github.com/Peanutsch/KeepYourFocus.git");
            linkLabelEmail.Links.Add(0, linkLabelEmail.Text.Length, "mailto:peanutsch@duck.com");
        }

        // Click Event for linklabel GitHub
        private void LinkLabelGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenLink(e.Link.LinkData.ToString());
        }

        // Click Event for LinkLabel Email
        private void LinkLabelEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenLink(e.Link.LinkData.ToString());
        }

        // Null Check for open links in default browser and email client
        private void OpenLink(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open link. Error: " + ex.Message);
            }
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

        // Randomize tiles. No more then 2 of the same tiles in a row (is the idea)
        private string RandomizerTiles()
        {
            // Verify if the dictionary is not empty
            if (pictureBoxDictionary.Count == 0)
            {
                Debug.WriteLine("PictureBoxDictionary is empty. Verify filepaths of tiles in InitialDictionaryOfTilesAtStart()");
                throw new InvalidOperationException("PictureBoxDictionary is empty. Verify filepaths of tiles");
            }

            string newTile;
            Dictionary<string, int> tileCount = correctOrder
                .GroupBy(tile => tile)
                .ToDictionary(group => group.Key, group => group.Count());

            do
            {
                // Select a new tile
                newTile = pictureBoxDictionary.Keys.ElementAt(rnd.Next(pictureBoxDictionary.Count));

                // Check if newTile appears less than three times in correctOrder
            } while (tileCount.ContainsKey(newTile) && tileCount[newTile] >= 3);

            // Add the new tile to previousTiles
            previousTiles.Add(newTile);
            Debug.WriteLine($"Added: {newTile}");

            // Keep only the last three tiles in the list
            if (previousTiles.Count > 3)
            {
                previousTiles.RemoveAt(0);
                Debug.WriteLine($"Removed: {previousTiles[0]}");
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
            isComputerTurn = true;

            computer = true;
            correctOrder.Add(RandomizerTiles());
            UpdateTurn(); // case computer's Turn
            DisplaySequence();

            isComputerTurn = false;
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

            isDisplaySequence = true;
            computer = true;

            Debug.WriteLine($"\nDisplay Sequence: {counter_sequences}");
            Debug.WriteLine("correctOrder = " + string.Join(", ", correctOrder));
            // Debug.WriteLine("pictureBoxDictionary = " + string.Join(", ", pictureBoxDictionary.Keys));

            await Task.Delay(500);

            foreach (var tile in correctOrder)
            {
                var box = pictureBoxDictionary[tile];
                if (box == null)
                    continue;

                await Task.Delay(500); // Delay 500 ms before start highlights and beepSound

                PlaySound(tile);

                ManageHighlight(box, true);
                await Task.Delay(150);
                ManageHighlight(box, false);
                await Task.Delay(50);
            }
            // Verify difficulty
            ManageActions();


            await Task.Delay(500); // Delay 500 ms before calling PlayersTurn()

            computer = false;
            isDisplaySequence = false;

            UpdateTurn(); // case Player's Turn
        }

        private async void PlayersTurn(object? sender, EventArgs e)
        {
            isPlayerTurn = true;

            // Block Player's clicks in computer's turn AND before StartButton is clicked
            if (startButton || computer)
                return;


            if (sender is PictureBox clickedBox)
            {
                string tile = clickedBox.Tag?.ToString() ?? "";

                PlaySound(tile);
                ManageHighlight(clickedBox, true);

                playerOrder.Add(tile);

                Debug.WriteLine($"Player: [{tile}]");

                // Verify difficulty
                //SetTurnActions();
                ManageActions();

                // Verify each input with correctOrder
                for (int input = 0; input < playerOrder.Count; input++)
                {
                    if (playerOrder[input] != correctOrder[input])
                    {
                        await Task.Delay(100);
                        ManageHighlight(clickedBox, false);
                        await Task.Delay(250); // Delay to provide feedback before game over
                        TextBoxHighscore();
                        GameOver();

                        isPlayerTurn = false;

                        return;
                    }
                }
                if (playerOrder.Count == correctOrder.Count)
                {
                    await Task.Delay(100);
                    ManageHighlight(clickedBox, false);
                    await Task.Delay(50);

                    ManageCountersAndLevels();

                    isPlayerTurn = false;
                }
                else
                {
                    await Task.Delay(100);
                    ManageHighlight(clickedBox, false);
                    await Task.Delay(50);

                    isPlayerTurn = false;
                }
            }
            isPlayerTurn = false;
        }

        private async void ManageCountersAndLevels()
        {
            if (playerOrder.Count < correctOrder.Count)
                return;

            // Block player's clicks
            computer = true;

            nextRound = true;

            // Delay 250 ms between beepSound and correctSound
            await Task.Delay(250);
            correctSound.Play();

            UpdateCounters();
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
        private void UpdateCounters()
        {
            isSetCounters = true;

            switch (counter_sequences)
            {
                case (6) when counter_levels < 8:
                    levelUp = true;
                    correctOrder.Clear();
                    playerOrder.Clear();
                    counter_sequences = 1; // Reset sequence to 1
                    counter_levels++;
                    counter_rounds++;

                    ManageActions();
                    UpdateTurn();

                    levelUp = false;
                    break;
                default:
                    if (counter_levels >= 8)
                    {
                        levelUp = true;

                        counter_sequences++;
                        counter_rounds++;
                        UpdateTurn();

                        isSetCounters = false;
                    }
                    else
                    {
                        counter_sequences++;
                        counter_rounds++;
                        UpdateTurn();

                        isSetCounters = false;
                    }
                    break;
            }
            isSetCounters = false;
        }

        // Verify turn actions
        private void ManageActions()
        {
            if (isComputerTurn)
            {
                // Debug.WriteLine("ManageActions> isComputerTurn = true");
                // DisplayLabelMessage(true);
                // ShufflePictureBoxes();
                
            }
            if (isPlayerTurn)
            {
                Debug.WriteLine("ManageActions> isPlayerTurn = true");
                // DisplayLabelMessage(false);
                ShufflePictureBoxes();
            }
            if (isDisplaySequence)
            {
                Debug.WriteLine("ManageActions> isDisplaySequence = true");
                ShufflePictureBoxes();
                ReplaceTileOnBoardAndInSequence();
            }
            if (isSetCounters)
            {
                ReplaceAllTiles();
            }
        }


        ////>>>> DIFFICULTIES <<<<////


        // Shuffle currect tile setup before player's turn and/or after player's click
        private async void ShufflePictureBoxes()
        {
            // isDisplaySequence
            if (counter_levels == 2 && rnd.Next(100) <= 55 && isDisplaySequence ||
                counter_levels >= 3 && rnd.Next(100) <= 75 && isDisplaySequence ||
                counter_levels >= 5 && rnd.Next(100) <= 85 && isDisplaySequence)
            {
                Debug.WriteLine($"Shuffle PictureBoxes Case 1: Shuffle before player's turn");

                await Task.Delay(250); // Delay 250 ms for space between colorSound and transitionSound
                transitionSound.Play();
                RandomizerShufflePictureBoxes();
                RefreshAndRepositionPictureBoxes();
                await Task.Delay(1000);
            }
            // isPlayerTurn
            if (counter_levels >= 3 && rnd.Next(100) <= 55 && isPlayerTurn ||
                counter_levels >= 4 && rnd.Next(100) <= 75 && isPlayerTurn ||
                counter_levels >= 6 && rnd.Next(100) <= 85 && isPlayerTurn)
            {
                Debug.WriteLine($"Shuffle PictureBoxes Case 2: Shuffle after player click");

                RandomizerShufflePictureBoxes();
                RefreshAndRepositionPictureBoxes();
            }
        }

        private (Dictionary<string, PictureBox>, List<string>, bool) ReplaceTileOnBoardAndInSequence()
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

        // Replace and switch all tiles when level up
        private void ReplaceAllTiles()
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

        // Replace 1 tile in running sequence and/or on board. Returns (Dict pictureBoxDictionary, List correctOrder, bool replacementOccurred)

        private async void DisplayLabelMessage(bool iscomputerTurn)
        {
            /*
             * Show labels with text in either computer's or Player's turn
             * E.g. Computer's turn: "Click Here", "Start Here!", "Start With this One!" (45%, 55% or 65% chance depending on level)
             * Player's turn: various messages based on different levels and conditions
             */

            int chance = counter_levels >= 6 ? 55 : 45;
            bool showMessage = computer
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

        private void ManageHighlight(PictureBox pictureBox, bool highlight)
        {
            if (pictureBox.InvokeRequired)
            {
                pictureBox.Invoke(new Action<PictureBox, bool>(ManageHighlight), pictureBox, highlight);
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

        private void AlignTextButtonBoxesCenter()
        {
            // Align Buttons
            startBTN.Visible = false;
            startBTN.TextAlign = ContentAlignment.MiddleCenter;
            startBTN.Visible = true;

            // Align richTextBoxShowLevelNumber
            richTextBoxShowLevelNumber.Visible = false;
            richTextBoxShowLevelNumber.SelectAll();
            richTextBoxShowLevelNumber.SelectionAlignment = HorizontalAlignment.Center;
            richTextBoxShowLevelNumber.DeselectAll();
            richTextBoxShowLevelNumber.Visible = true;

            // Align richTextBoxShowLevelName
            richTextBoxShowLevelName.Visible = false;
            richTextBoxShowLevelName.SelectAll();
            richTextBoxShowLevelName.SelectionAlignment = HorizontalAlignment.Center;
            richTextBoxShowLevelName.DeselectAll();
            richTextBoxShowLevelName.Visible = true;

            // Align richTextBoxShowNumbersOfSequences
            richTextBoxShowNumbersOfSequences.Visible = false;
            richTextBoxShowNumbersOfSequences.SelectAll();
            richTextBoxShowNumbersOfSequences.SelectionAlignment = HorizontalAlignment.Center;
            richTextBoxShowNumbersOfSequences.DeselectAll();
            richTextBoxShowNumbersOfSequences.Visible = true;

            // Align richTextBoxTurn
            richTextBoxTurn.Visible = false;
            richTextBoxTurn.SelectAll();
            richTextBoxTurn.SelectionAlignment = HorizontalAlignment.Center;
            richTextBoxTurn.DeselectAll();
            richTextBoxTurn.Visible = true;

            // Align richTextBoxShowRounds
            richTextBoxShowRounds.Visible = false;
            richTextBoxShowRounds.SelectAll();
            richTextBoxShowRounds.SelectionAlignment = HorizontalAlignment.Center;
            richTextBoxShowRounds.DeselectAll();
            richTextBoxShowRounds.Visible = true;

        }


        // Update richtextbox ShowNumbersOfSequences
        private void UpdateSequence()
        {
            richTextBoxShowNumbersOfSequences.BackColor = Color.Yellow;
            richTextBoxShowNumbersOfSequences.Text = $"   Sequence of {counter_sequences}";
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
                        richTextBoxTurn.Text = $"\nCORRECT";

                        await Task.Delay(1500);

                        richTextBoxTurn.Text = $"\nLevel  Up";

                        await Task.Delay(1000);
                        break;
                    }
                    else
                    {
                        // Debug.WriteLine("UpdateTurn Correct");

                        richTextBoxTurn.BackColor = Color.LightGreen;
                        richTextBoxTurn.Text = $"\nCORRECT";

                        await Task.Delay(1500);
                        richTextBoxTurn.Text = $"\nNext Sequence";

                        await Task.Delay(1000);
                        break;
                    }
                // computer's turn
                case (true, false, _):
                    richTextBoxTurn.BackColor = Color.Salmon;
                    richTextBoxTurn.Text = $"computer's Turn";
                    richTextBoxTurn.Text = $"Running\n::\nSequence";
                    break;
                // Player's turn
                case (false, false, _):
                    richTextBoxTurn.BackColor = Color.Green;
                    richTextBoxTurn.Text = $"\nPlayer's Turn";
                    break;
            }
        }

        // Update richtextbox ShowRounds
        private void UpdateRound()
        {
            richTextBoxShowRounds.BackColor = Color.LightSkyBlue;
            richTextBoxShowRounds.Text = $"Total Rounds: {counter_rounds}";
        }

        // Update richtextbox ShowLevel
        private void UpdateLevelName()
        {
            switch (counter_levels)
            {
                case (1):
                    richTextBoxShowLevelName.BackColor = Color.LightSkyBlue;
                    richTextBoxShowLevelNumber.BackColor = Color.LightSkyBlue;

                    richTextBoxShowLevelNumber.Text = $"{counter_levels}";
                    richTextBoxShowLevelName.Text = $"EasyPeasy";
                    break;
                case (2):
                    richTextBoxShowLevelName.BackColor = Color.SkyBlue;
                    richTextBoxShowLevelNumber.BackColor = Color.SkyBlue;

                    richTextBoxShowLevelNumber.Text = $"{counter_levels}";
                    richTextBoxShowLevelName.Text = $"OkiDoki";
                    break;
                case (3):
                    richTextBoxShowLevelName.BackColor = Color.CornflowerBlue;
                    richTextBoxShowLevelNumber.BackColor = Color.CornflowerBlue;

                    richTextBoxShowLevelNumber.Text = $"{counter_levels}";
                    richTextBoxShowLevelName.Text = $"Please No";
                    break;
                case (4):
                    richTextBoxShowLevelName.BackColor = Color.RoyalBlue;
                    richTextBoxShowLevelNumber.BackColor = Color.RoyalBlue;

                    richTextBoxShowLevelNumber.Text = $"{counter_levels}";
                    richTextBoxShowLevelName.Text = $"No Way!";
                    break;
                case (5):
                    richTextBoxShowLevelName.BackColor = Color.DarkKhaki;
                    richTextBoxShowLevelNumber.BackColor = Color.DarkKhaki;

                    richTextBoxShowLevelNumber.Text = $"{counter_levels}";
                    richTextBoxShowLevelName.Text = $"HELL NO";
                    break;
                case (6):
                    richTextBoxShowLevelName.BackColor = Color.DarkOrange;
                    richTextBoxShowLevelNumber.BackColor = Color.DarkOrange;

                    richTextBoxShowLevelNumber.Text = $"{counter_levels}";
                    richTextBoxShowLevelName.Text = $"NONONONO";
                    break;
                case (999):
                    richTextBoxShowLevelNumber.BackColor = Color.Red;
                    richTextBoxShowLevelName.BackColor = Color.Red;
                    richTextBoxShowRounds.BackColor = Color.Red;

                    richTextBoxShowNumbersOfSequences.Text = $"{new string(' ', 5)}GAME OVER";

                    textBoxShowResults.Visible = true;
                    break;
                default:
                    richTextBoxShowLevelName.BackColor = Color.OrangeRed;
                    richTextBoxShowLevelNumber.BackColor = Color.OrangeRed;

                    richTextBoxShowLevelNumber.Text = $"666";
                    richTextBoxShowLevelName.Text = $"HELLMODE";
                    break;
            }
        }

        // Shows Top 8 Highscores in TextBoxHighscore
        private void TextBoxHighscore()
        {
            List<(string, int, int, string, string, string)> topHighscores = SortBestScores();
            List<int> listLineNumber = new List<int>();
            List<int> listTotalRounds = new List<int>();

            Debug.WriteLine($"topHighscores count: {topHighscores.Count}");

            // Set textbox visible
            textBoxHighscore.Visible = true;

            // Clear any existing text
            textBoxHighscore.Clear();

            // Use a fixed-width font for proper alignment
            textBoxHighscore.Font = new Font("Courier New", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);

            // Add the header
            textBoxHighscore.Text = "\r\n===HIGHSCORES===\r\n\r\n";
            // textBoxHighscore.AppendText(string.Format("{0, -8} {1, -8} {2, -8} {3, -8} {4, -10} {5, 7}\r\n", "Place", "Player", "Rounds", "Level", "lvlName", "Time"));
            textBoxHighscore.AppendText(string.Format("{0, -9} {1, -9} {2, -9} {3, -9} {4, -10}\r\n", "Place", "Player", "Sequences", "Level", "Date"));

            // Append the highscores in textbox
            int lineNumber = 1;
            foreach (var score in topHighscores)
            {
                string playerName = score.Item1;
                int totalRounds = score.Item2;
                int levelReached = score.Item3;
                string levelName = score.Item4;
                string isDate = score.Item5;
                string elapsedGameTime = score.Item6;

                // textBoxHighscore.AppendText(string.Format("{0, -8} {1, -8} {2, -8} {3, -8} {4, -10} {5, 7}\r\n", lineNumber, playerName, totalRounds, levelReached, levelName, elapsedGameTime));
                textBoxHighscore.AppendText(string.Format("{0, -9} {1, -9} {2, -9} {3, -9} {4, -10}\r\n", lineNumber, playerName, totalRounds, levelReached, isDate));
                lineNumber++;

                listLineNumber.Add(lineNumber);
                listTotalRounds.Add(totalRounds);
            }
        }

        // TESTING --> When Game Over, saves score on new line
        private void SaveScore(int totalRounds, int levelReached, string levelName)
        {
            // ("playerName", totalRounds, levelReached, levelName, isDate, elapsedGameTime)
            List<(string, int, int, string, string, string)> topHighScores = SortBestScores();

            // Get playerName from storePlayerName
            string playerName = storePlayerName[0];

            // Get elapsed game time
            string elapsedGameTime = GameStopwatch();

            // Get current date
            DateTime isToday = DateTime.Today;
            string isDate = isToday.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            // Construct the file path using RootPath
            string rootPath = RootPath();

            // Null check
            if (string.IsNullOrEmpty(rootPath))
            {
                MessageBox.Show("Error: Unable to determine root path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string file = Path.Combine(rootPath, "sounds", "setters.txt");

            try
            {
                // Check if the new score qualifies for the top scores list
                bool qualifiesForTopScores = topHighScores.Count < 10 || topHighScores.Any(score => score.Item2 < totalRounds || (score.Item2 == totalRounds && TimeSpan.Parse(score.Item6) > TimeSpan.Parse(elapsedGameTime)));

                if (qualifiesForTopScores)
                {
                    // Add the new score
                    topHighScores.Add((playerName.Trim(), totalRounds, levelReached, levelName.Trim(), isDate, elapsedGameTime));

                    // Sort and take the top 10 scores
                    topHighScores = topHighScores.OrderByDescending(x => x.Item2)
                                                 .ThenBy(x => TimeSpan.Parse(x.Item6))
                                                 .Take(10)
                                                 .ToList();

                    // Determine the rank of the current score
                    int ranking = topHighScores.FindIndex(score => score.Item1 == playerName.Trim() && score.Item2 == totalRounds && score.Item6 == elapsedGameTime) + 1;

                    // Write the updated top scores back to the file
                    using (StreamWriter saveScore = new StreamWriter(file, false))
                    {
                        foreach (var score in topHighScores)
                        {
                            saveScore.WriteLine($"{score.Item1},{score.Item2},{score.Item3},{score.Item4},{score.Item5},{score.Item6}");
                        }
                    }

                    // Refresh and show textBoxHighscores and textBoxShowResults
                    TextBoxHighscore();
                    textBoxShowResults.Visible = true;
                    textBoxShowResults.Text = $"Your score:\r\n{totalRounds} sequences\r\nYou ranked place:\r\n#{ranking}";

                    // Log the saved data
                    Debug.WriteLine($"Game data saved: {playerName}, {totalRounds}, {levelReached}, {levelName.Trim()}, {isDate}, {elapsedGameTime}");
                    ReadScoresFromFile();
                }
                else // Not in top 10; show only totalRounds
                {
                    // Refresh and show textBoxHighscores and textBoxShowResults
                    TextBoxHighscore();
                    textBoxShowResults.Visible = true;
                    textBoxShowResults.Text = $"Your score:\r\n{totalRounds} sequences";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving game data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // When Game Over, saves score on new line
        private void TESTSaveScore(int playerScore, int levelReached, string levelName)
        {
            // Get playerName from storePlayerName
            string playerName = storePlayerName[0];

            // Get elapsed game time
            string elapsedGameTime = GameStopwatch();

            // Construct the file path in the user's local application data directory
            string localAppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KeepYourFocus");
            Directory.CreateDirectory(localAppDataPath); // Ensure the directory exists
            string file = Path.Combine(localAppDataPath, "setters.txt");

            try
            {
                // Read the existing content of the file
                string existingContent = File.Exists(file) ? File.ReadAllText(file) : string.Empty;

                // Write the new score followed by the existing content
                using (StreamWriter saveScore = new StreamWriter(file, false))
                {
                    saveScore.WriteLine($"{playerName},{playerScore},{levelReached},{levelName.Trim()},{elapsedGameTime}");
                    saveScore.Write(existingContent);
                }

                // Log the saved data
                Debug.WriteLine($"Game data saved: {playerName}, {playerScore}, {levelReached}, {levelName.Trim()}, {elapsedGameTime}");
                ReadScoresFromFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving game data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Returns list with all data in setters.txt
        private List<(string, int, int, string, string, string)> ReadScoresFromFile()
        {
            string file = Path.Combine(RootPath(), "sounds", "setters.txt");
            List<(string, int, int, string, string, string)> scoresList = new List<(string, int, int, string, string, string)>();

            try
            {
                using (StreamReader getHighscore = new StreamReader(file))
                {
                    string? line;
                    while ((line = getHighscore.ReadLine()) != null)
                    {
                        // Split the line into parts
                        string[] parts = line.Split(',');
                        if (parts.Length >= 6)
                        {
                            if (int.TryParse(parts[1], out int playerScore) && int.TryParse(parts[2], out int levelReached))
                            {
                                string playerName = parts[0].Trim();
                                string levelName = parts[3].Trim();
                                string isDate = parts[4].Trim();
                                string elapsedGameTime = parts[5].Trim();
                                scoresList.Add((playerName, playerScore, levelReached, levelName, isDate, elapsedGameTime));
                            }
                        }
                    }
                }
                // Debug.WriteLine("Scores read successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while reading scores: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // Debug.WriteLine($"scoresList count: {scoresList.Count}");
            return scoresList;
        }


        // Returns decreasing sorted list of higscores.txt
        private List<(string, int, int, string, string, string)> SortBestScores()
        {
            List<(string, int, int, string, string, string)> bestScores = new List<(string, int, int, string, string, string)>();

            try
            {
                // Get scores from file
                List<(string, int, int, string, string, string)> scoresList = ReadScoresFromFile();

                // Sort by playerScore and then by gameTime
                bestScores = scoresList.OrderByDescending(x => x.Item2)
                                       .ThenBy(x => TimeSpan.Parse(x.Item6))
                                       .Take(10)
                                       .ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"An error occurred while getting top scores: {e.Message}");
            }
            return bestScores;
        }

        private void GameOver()
        {

            gameOver = true;
            // Disable textBoxHighscore and all pictureboxes
            textBoxHighscore.Enabled = false;
            pictureBox1.Enabled = false;
            pictureBox2.Enabled = false;
            pictureBox3.Enabled = false;
            pictureBox4.Enabled = false;
            textBoxShowResults.Visible = true;

            // Set flags
            computer = false;
            startButton = true;
            gameTime = false;

            //Stop Stopwatch
            GameStopwatch();

            // Save the score
            SaveScore(counter_rounds, counter_levels, richTextBoxShowLevelName.Text);

            correctOrder.Clear();
            playerOrder.Clear();

            wrongSound.Play();

            counter_levels = 999;

            UpdateLevelName();
            ButtonGameOver_Click();
            InitialDictionaryOfTilesAtStart();

            counter_rounds = 1;
            counter_levels = 1;
        }
    }
}