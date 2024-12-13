using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Media;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using static System.Formats.Asn1.AsnWriter;

namespace KeepYourFocus
{
    public partial class PlayerField : Form
    #region ClassProperties
    {
        private Dictionary<string, PictureBox> pictureBoxDictionary = new Dictionary<string, PictureBox>();
        private List<string> correctOrder = new List<string>();
        private List<string> playerOrder = new List<string>();
        private List<string> previousTiles = new List<string>();

        private readonly Random rnd = new Random();
        private Stopwatch gameStopwatch = new Stopwatch();

        #region GameSound_Properties
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
        #endregion

        #region GameVariables_Properties
        private bool computer = false;
        private bool startButton = true;
        private bool nextRound = false;
        private bool levelUp = false;
        private bool gameTime = false;

        bool isComputerTurn = false;
        bool isPlayerTurn = false;
        bool isSetCounters = false;
        bool isDisplaySequence = false;
        bool isHardLevel = false;
        bool actionTaken = false;
        #endregion

        private int counterSequences = 1;
        private int counterLevels = 1;
        private int counterRounds = 0;
        private int setSequences = 6;
        #endregion

        public PlayerField()
        #region Initialize Components
        {
            InitializeComponent();

            // Load soundfiles. For now 1 beep sound for all colors
            string soundPathBeepALL = Path.Combine(InitializeRootPath(), @"sounds\beep.wav");

            /* ***Pre-made soundPath for all colors*** *\
            string soundPathBeepRed = Path.Combine(InitializeRootPath(), @"sounds\redSound.wav");
            string soundPathBeepBlue = Path.Combine(InitializeRootPath(), @"sounds\blueSound.wav");
            string soundPathBeepOrange = Path.Combine(InitializeRootPath(), @"sounds\orangeSound.wav");
            string soundPathBeepGreen = Path.Combine(InitializeRootPath(), @"sounds\greenSound.wav");
            string soundPathBeepCaribBlue = Path.Combine(InitializeRootPath(), @"sounds\caribBlueSound.wav");
            string soundPathBeepGrey = Path.Combine(InitializeRootPath(), @"sounds\greySound.wav");
            string soundPathBeepIndigo = Path.Combine(InitializeRootPath(), @"sounds\indigoSound.wav");
            string soundPathBeepMaroon = Path.Combine(InitializeRootPath(), @"sounds\maroonSound.wav");
            string soundPathBeepOlive = Path.Combine(InitializeRootPath(), @"sounds\oliveSound.wav");
            string soundPathBeepPink = Path.Combine(InitializeRootPath(), @"sounds\pinkSound.wav");
            */

            string soundPathTransition = Path.Combine(InitializeRootPath(), @"sounds\transistion.wav");
            string soundPathButtonClick = Path.Combine(InitializeRootPath(), @"sounds\buttonclick.wav");
            string soundPathWrong = Path.Combine(InitializeRootPath(), @"sounds\wrong.wav");
            string soundPathCorrect = Path.Combine(InitializeRootPath(), @"sounds\correct.wav");
            string soundPathStartupSound = Path.Combine(InitializeRootPath(), @"sounds\startupSound.wav");

            // Initiaize SoundPlayers
            redSound = new SoundPlayer(soundPathBeepALL);        // redSound = new SoundPlayer(soundPathBeepRed); 
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
            #endregion

            #region Startup Game
            // Initialize Stopwatch for gametime
            gameStopwatch = new Stopwatch();

            ////>>>> Start Program <<<<////

            // Welcome MessageBox
            InitializeWelcomeMessageBox();

            // LinkLabels GitHub and Email
            InitializeLinkLabels();

            // Align richTextBoxes
            AlignTextButtonBoxesCenter();

            // Display highscore at start
            TextBoxHighscores();

            // Use initial dictionary for start setup
            InitialDictionaryOfTilesAtStart();

            // Play startup sound
            startupSound.Play();
        }
        #endregion

        #region Initialisations and Setups
        // Thank You + some info Spam MessageBox
        private void InitializeWelcomeMessageBox()
        {
            MessageBox.Show(
                            "   Thank you for testing the heck out of my very first try-out\r\n" +
                            "   in C# coding!\r\n\r\n" +
                            " * This is a Simon Says-like game with some level based\r\n" +
                            "   challenges\r\n" +
                            " * Each level has 6 sequences. After 6 succesful sequences:\r\n" +
                            " * Level++; Add 1 challenge; Clear correctOrder and\r\n" +
                            "   playerOrder and start with new sequence = 1\r\n" +
                            " * From Level >= 7: no Clear correctOrder; sequences++\r\n" +
                            "   untill game over\r\n" +
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

        // Initialization to both startButton and retryButton to start or retry game
        private void InitializeStartGame()
        {
            buttonClickSound.Play();

            gameTime = true;
            startButton = false;
            computer = true;

            // Back to start setup tiles
            InitialDictionaryOfTilesAtStart();

            // Start Stopwatch
            InitializeGameStopwatch();

            //Set Flags
            textBoxHighscore.Visible = false;
            checkedListBoxDifficulty.Visible = false;

            pictureBox1.Enabled = true;
            pictureBox2.Enabled = true;
            pictureBox3.Enabled = true;
            pictureBox4.Enabled = true;

            startBTN.Visible = false;
            startBTN.Enabled = false;

            buttonRetry.Enabled = false;
            buttonRetry.Visible = false;

            linkLabelGitHub.Visible = false;
            linkLabelGitHub.Enabled = false;
            linkLabelEmail.Visible = false;
            linkLabelEmail.Enabled = false;

            // (re)Set counterSequences
            counterSequences = 1;

            UpdateSequence();
            UpdateRound();
            UpdateLevelName();
            ComputersTurn();
        }

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
            // Align startBTN
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

            // Align textBoxInputName
            textBoxInputName.Visible = false;
            textBoxInputName.Enabled = false;

            // Align textBoxShowResults
            textBoxShowResults.DeselectAll();
        }

        // Click Event for Start Button at start
        private void InitializeButtonStart_Click(object sender, EventArgs e)
        {
            if (!startButton)
                return;

            InitializeStartGame();
        }

        // Click Event for buttonRetry at Game Over
        private async void InitializeButtonRetry_Click(object sender, EventArgs e)
        {
            // Any additional logic specific to retry
            await Task.Delay(500);

            buttonRetry.Enabled = true;
            buttonRetry.Visible = true;

            //Set LinkLabels invisible
            linkLabelGitHub.Visible = true;
            linkLabelGitHub.Enabled = true;
            linkLabelEmail.Visible = true;
            linkLabelEmail.Enabled = true;

            InitialDictionaryOfTilesAtStart();
            InitializeStartGame();
        }

        // Initialize Enter button for input playerName
        private void InitializeButtonEnter_Click(object sender, EventArgs e)
        {
            string playerName = ProcessInputName();
            playerNameTcs.TrySetResult(ProcessInputName()); // TrySetResult() -> Mark Task as completed (Task will be set completed in PlayerName()
            textBoxShowResults.DeselectAll();
            Debug.WriteLine("playerName entered by buttonEnter");
        }

        // Initialize Keys.Enter for input playerName
        private void InitializeKeyEnter()
        {
            textBoxInputName.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true; // Prevents the Enter key from inserting a newline
                    e.SuppressKeyPress = true; // Stops the "ding" sound
                    string playerName = ProcessInputName();
                    playerNameTcs.TrySetResult(ProcessInputName()); // TrySetResult() -> Mark Task as completed (Task will be set completed in PlayerName()
                    textBoxShowResults.DeselectAll();
                    Debug.WriteLine("playerName entered by Keys.Enter");
                }
            };
        }

        // Stopwatch for recording gametime
        private string InitializeGameStopwatch()
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
                    Debug.WriteLine("ElapsedTime is empty. No time recorded!");
                    MessageBox.Show("ElapsedTime is empty.No time recorded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return string.Empty;
                }
            }
        }

        #region ComboBox Choose our Level
        // Set game level combobox dropdown list
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            // Skip handling if the selected item is the placeholder
            if (comboBox.SelectedIndex == 0)
            {
                return;
            }

            int setSequences;
            switch (comboBox.SelectedIndex)
            {
                case 1:
                    setSequences = 6; // Normal
                    break;
                case 2:
                    setSequences = 4; // Easy
                    break;
                case 3:
                    setSequences = 10; // Hard
                    break;
                case 4:
                    setSequences = int.MaxValue; // Hell: Endless seq
                    break;
                default:
                    setSequences = 6; // Default
                    break;
            }

            // Do something with setSequences
            Debug.WriteLine($"Selected difficulty sequences per round: {setSequences}");
        }

        // Return Level and setSequences
        private string ReturnLevelandSetSequences(out int setSequences)
        {
            if (checkedListBoxDifficulty.SelectedIndex == 0) // Placeholder index
            {
                setSequences = 0;
                return "Please select a level.";
            }

            switch (checkedListBoxDifficulty.SelectedIndex)
            {
                case 1:
                    setSequences = 6; // Default
                    return "NORMAL";
                case 2:
                    setSequences = 4; // Easy
                    return "EASY";
                case 3:
                    setSequences = 10; // Hard
                    return "HARD";
                case 4:
                    setSequences = int.MaxValue; // Hell: Endless seq
                    return "HELL";
                default:
                    setSequences = 6; // Default
                    return "NORMAL";
            }
        }
        #endregion

        #region CheckListBox Choose your Level
        // Set game level checkbox
        private void checkedListBoxDifficulty_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckedListBox checkedListBox = (CheckedListBox)sender;

            // Initialize setSequences with a default value
            int setSequences = 6;

            if (checkedListBox.CheckedItems.Count > 0)
            {
                // Process each selected item
                foreach (string selectedDifficulty in checkedListBox.CheckedItems)
                {
                    switch (selectedDifficulty)
                    {
                        case "Default: 6 seq/round":
                            setSequences = 6;
                            break;
                        case "Easy: 4 seq/round":
                            setSequences = 4;
                            break;
                        case "Hard: 10 seq/round":
                            setSequences = 10;
                            break;
                        /*
                        case "Hell: Endless seq":
                            setSequences = int.MaxValue;
                            break;
                        */
                        default:
                            // Handle unexpected cases if needed
                            setSequences = 6; // Default
                            break;
                    }
                }
            }
            Debug.WriteLine($"Selected difficulty sequences per round: {setSequences}");
        }



        // Only 1 box can be checked
        private void checkedListBoxDifficulty_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            CheckedListBox checkedListBox = (CheckedListBox)sender;

            // Uncheck all items except the one that is being checked
            if (e.NewValue == CheckState.Checked)
            {
                for (int i = 0; i < checkedListBox.Items.Count; i++)
                {
                    if (i != e.Index)
                    {
                        checkedListBox.SetItemChecked(i, false);
                    }
                }
            }
        }

        private int GetSelectedSequences()
        {
            int setSequences = 6;

            if (checkedListBoxDifficulty.CheckedItems.Count > 0)
            {
                string selectedDifficulty = checkedListBoxDifficulty.CheckedItems[0].ToString();
                switch (selectedDifficulty)
                {
                    case "Default: 6 seq/round":
                        setSequences = 6;
                        break;
                    case "Easy: 4 seq/round":
                        setSequences = 4;
                        break;
                    case "Hard: 10 seq/round":
                        setSequences = 10;
                        break;
                    /*
                    case "Hell: Endless seq":
                        setSequences = int.MaxValue;
                        break;
                    */
                    default:
                        setSequences = 6; // Default
                        break;
                }
            }

            return setSequences;
        }
        #endregion

        // REVISED METHOD FOR INSTALLATION IN localAPPData! Initialize and return root path including directory \KeepYourFocus\
        static string REVISEDInitializeRootPath() // InitializeRootPath() / REVISEDInitializeRootPath
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
        static string InitializeRootPath() // InitializeRootPath / OriginalInitializeRootPath()
        {
            // string directoryPath = Environment.CurrentDirectory;
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory;

            if (string.IsNullOrEmpty(directoryPath))
            {
                Debug.WriteLine("Error: Unable to determine root path.");
                MessageBox.Show("Error: Unable to determine root path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                Debug.WriteLine("Error: 'KeepYourFocus' directory not found in path.");
                MessageBox.Show("Error: 'KeepYourFocus' directory not found in path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty; // Return an empty string
            }
        }

        // Initialize Labels with links to github and email @duck.com
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
            if (e.Link.LinkData != null)
            {
                string url = e.Link.LinkData.ToString();
                OpenLink(url);
            }
            else
            {
                Debug.WriteLine("Warning: LinkData is null.");
                MessageBox.Show("Error: Link data is missing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Click Event for LinkLabel Email
        private void LinkLabelEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Link.LinkData != null)
            {
                string url = e.Link.LinkData.ToString();
                OpenLink(url);
            }
            else
            {
                Debug.WriteLine("Warning: LinkData is null.");
                MessageBox.Show("Error: Link data is missing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Null Check for open links in default browser and email client
        private void OpenLink(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                // Log een waarschuwing of toon een foutmelding als de URL null of leeg is
                MessageBox.Show("Error: The URL is null or empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Probeer de link te openen in de standaard browser
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                // Vang eventuele uitzonderingen op die optreden bij het openen van de link
                MessageBox.Show("Unable to open link. Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Dictionary for start positions tiles
        private void InitialDictionaryOfTilesAtStart()
        {
            pictureBox1.Visible = false;
            pictureBox2.Visible = false;
            pictureBox3.Visible = false;
            pictureBox4.Visible = false;

            InitializePictureBox(pictureBox1, "Red", Path.Combine(InitializeRootPath(), @"png\red_tile512.png"));
            InitializePictureBox(pictureBox2, "Blue", Path.Combine(InitializeRootPath(), @"png\blue_tile512.png"));
            InitializePictureBox(pictureBox3, "Orange", Path.Combine(InitializeRootPath(), @"png\orange_tile512.png"));
            InitializePictureBox(pictureBox4, "Green", Path.Combine(InitializeRootPath(), @"png\green_tile512.png"));

            pictureBox1.Visible = true;
            pictureBox2.Visible = true;
            pictureBox3.Visible = true;
            pictureBox4.Visible = true;
        }

        // Returns a dictionary of all possible tiles
        static Dictionary<string, string> DictOfAllTiles()
        {
            string redTile = Path.Combine(InitializeRootPath(), "png", "red_tile512.png");
            string blueTile = Path.Combine(InitializeRootPath(), "png", "blue_tile512.png");
            string orangeTile = Path.Combine(InitializeRootPath(), "png", "orange_tile512.png");
            string greenTile = Path.Combine(InitializeRootPath(), "png", "green_tile512.png");
            string caribBlueTile = Path.Combine(InitializeRootPath(), "png", "caribBlue_tile512.png");
            string greyTile = Path.Combine(InitializeRootPath(), "png", "grey_tile512.png");
            string indigoTile = Path.Combine(InitializeRootPath(), "png", "indigo_tile512.png");
            string maroonTile = Path.Combine(InitializeRootPath(), "png", "maroon_tile512.png");
            string oliveTile = Path.Combine(InitializeRootPath(), "png", "olive_tile512.png");
            string pinkTile = Path.Combine(InitializeRootPath(), "png", "pink_tile512.png");

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
                MessageBox.Show($"Error initializing PictureBox for tile {tile}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show($"PictureBoxDictionary is empty. Verify filepaths of tiles in InitialDictionaryOfTilesAtStart()", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new InvalidOperationException("PictureBoxDictionary is empty. Verify filepaths of tiles");
            }

            // Shuffle the keys of pictureBoxDictionary using Fisher-Yates Shuffle
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

            // Ensure no tile appears more than twice in a row
            for (int itemIndex = 2; itemIndex < shuffledTiles.Count; itemIndex++)
            {
                if (shuffledTiles[itemIndex] == shuffledTiles[itemIndex - 1] && shuffledTiles[itemIndex] == shuffledTiles[itemIndex - 2])
                {
                    // Find a new position for shuffledTiles[itemIndex]
                    for (int swapIndex = itemIndex + 1; swapIndex < shuffledTiles.Count; swapIndex++)
                    {
                        if (shuffledTiles[swapIndex] != shuffledTiles[itemIndex - 1])
                        {
                            // Swap positions
                            string temp = shuffledTiles[itemIndex];
                            shuffledTiles[itemIndex] = shuffledTiles[swapIndex];
                            shuffledTiles[swapIndex] = temp;
                            break;
                        }
                    }
                }
            }

            // Select a new tile
            string newTile = shuffledTiles[0]; // Take the first tile from the shuffled list

            return newTile;
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
        #endregion

        #region Difficulties
        // Returns shuffled dictionary of all tiles (Fisher-Yates shuffle algoritme / Knuth shuffle)
        private Dictionary<string, string> ShuffleDictOfAllTiles()
        {
            Dictionary<string, string> dictOfAllTiles = DictOfAllTiles();

            List<KeyValuePair<string, string>> listOfAllTiles = dictOfAllTiles.ToList();

            // Fisher-Yates shuffle algoritme / Knuth shuffle
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

        // Randomizer reposition tiles
        private void RefreshAndRepositionPictureBoxes()
        {
            // Get the shuffled PictureBoxes
            var shuffledPictureBoxes = pictureBoxDictionary.Values.OrderBy(x => rnd.Next()).ToList();

            // Iterate through the shuffled PictureBoxes and assign fixed positions
            for (int itemIndex = 0; itemIndex < shuffledPictureBoxes.Count; itemIndex++)
            {
                shuffledPictureBoxes[itemIndex].Location = SetFixedPositionPictureBoxes(itemIndex);
                shuffledPictureBoxes[itemIndex].Visible = true;
            }
        }

        // Shuffle current tile setup after display sequence and/or after player's click
        private async Task ShufflePictureBoxes()
        {
            // isDisplaySequence
            if (counterLevels == 2 && rnd.Next(100) <= 55 && isDisplaySequence ||
                counterLevels >= 3 && rnd.Next(100) <= 75 && isDisplaySequence ||
                counterLevels >= 5 && rnd.Next(100) <= 85 && isDisplaySequence ||
                isHardLevel && rnd.Next(100) <= 85 && isDisplaySequence)
            {
                Debug.WriteLine($"Shuffle PictureBoxes Case 1: Shuffle after display sequence");

                await Task.Delay(500); // Delay 250 ms for space between colorSound and transitionSound
                transitionSound.Play();
                RandomizerShufflePictureBoxes();
                RefreshAndRepositionPictureBoxes();
                await Task.Delay(500);
            }
            // isPlayerTurn
            if (counterLevels >= 3 && rnd.Next(100) <= 55 && isPlayerTurn ||
                counterLevels >= 4 && rnd.Next(100) <= 75 && isPlayerTurn ||
                counterLevels >= 6 && rnd.Next(100) <= 85 && isPlayerTurn ||
                isHardLevel && rnd.Next(100) <= 85 && isDisplaySequence && isDisplaySequence)
            {
                Debug.WriteLine($"Shuffle PictureBoxes Case 2: Shuffle after player click");

                RandomizerShufflePictureBoxes();
                RefreshAndRepositionPictureBoxes();
            }
            actionTaken = true;
        }

        // Randomizer for replacing tile on board and/or in sequence. Returns (Dict pictureBoxDictionary, List correctOrder, bool replacementOccurred)
        private (Dictionary<string, PictureBox>, List<string>, bool) ReplaceTileOnBoardAndInSequence()
        {
            string newTile = RandomizerTiles();
            Dictionary<string, string> dictOfAllTiles = DictOfAllTiles();
            List<KeyValuePair<string, string>> listOfAllTiles = dictOfAllTiles.ToList();

            bool checkReplaceInOrder = (counterLevels >= 5 && correctOrder.Count > 2 && rnd.Next(100) <= 55) ||
                                       (counterLevels >= 6 && correctOrder.Count > 2 && rnd.Next(100) <= 75) ||
                                       (counterLevels >= 8 && correctOrder.Count > 2 && rnd.Next(100) <= 85) ||
                                       (isHardLevel && rnd.Next(100) <= 85 && isDisplaySequence);

            bool checkReplaceOnBoard = (counterLevels >= 6 && correctOrder.Count > 2 && rnd.Next(100) <= 55) ||
                                       (counterLevels >= 7 && correctOrder.Count > 2 && rnd.Next(100) <= 75) ||
                                       (counterLevels >= 9 && correctOrder.Count > 2 && rnd.Next(100) <= 85) ||
                                       (isHardLevel && rnd.Next(100) <= 85 && isDisplaySequence);

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
                //Debug.WriteLine("Updated pictureBoxDictionary = " + string.Join(", ", pictureBoxDictionary.Keys));
            }
            actionTaken = true;
            return (pictureBoxDictionary, correctOrder, replacementOccurred);
        }

        // Replace and switch all tiles when level up
        private void ReplaceAllTiles()
        {
            if (counterLevels >= 4 && levelUp == true && rnd.Next(100) <= 55 ||
                counterLevels >= 5 && levelUp == true && rnd.Next(100) <= 75 ||
                counterLevels >= 7 && levelUp == true && rnd.Next(100) <= 85 ||
                isHardLevel && rnd.Next(100) <= 85 && isDisplaySequence)
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
            actionTaken = true;
        }

        private async void DisplayLabelMessage(bool iscomputerTurn)
        {
            /*
             * Show labels with text in either computer's or Player's turn
             * E.g. Computer's turn: "Click Here", "Start Here!", "Start With this One!" (45%, 55% or 65% chance depending on level)
             * Player's turn: various messages based on different levels and conditions
             */

            int chance = counterLevels >= 6 ? 55 : 45;
            bool showMessage = computer
                ? counterLevels >= 2 && rnd.Next(100) <= chance && correctOrder.Count != correctOrder.Count - 1
                : counterLevels >= 2 && rnd.Next(100) <= 65 && playerOrder.Count != correctOrder.Count;

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
        #endregion

        #region Game Elements
        private void ComputersTurn()
        {
            textBoxShowResults.Visible = false;

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
                // Use updated pictureBoxDictionary and correctOrder
                pictureBoxDictionary = updatedPictureBoxDictionary;
                correctOrder = updatedCorrectOrder;
            }

            isDisplaySequence = true;
            computer = true;

            Debug.WriteLine($"\nDisplay Sequence: {counterSequences}");
            Debug.WriteLine("correctOrder = " + string.Join(", ", correctOrder));

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
            await ManageActions();

            await Task.Delay(500); // Delay 500 ms before calling PlayersTurn()

            computer = false;
            isDisplaySequence = false;

            UpdateTurn(); // case Player's Turn
        }

        private async void PlayersTurn(object? sender, EventArgs e)
        {
            isPlayerTurn = true;

            // Block Player's clicks in computer's turn AND at start game before StartButton is clicked
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
                await ManageActions();

                // Verify each item with correctOrder
                for (int itemIndex = 0; itemIndex < playerOrder.Count; itemIndex++)
                {
                    if (playerOrder[itemIndex] != correctOrder[itemIndex])
                    {
                        await Task.Delay(100);
                        ManageHighlight(clickedBox, false);
                        await Task.Delay(250); // Delay to provide feedback before game over
                        TextBoxHighscores();
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

            nextRound = true;

            // Block player's clicks
            computer = true;

            // Delay 250 ms between beepSound and correctSound
            await Task.Delay(250);
            correctSound.Play();

            await UpdateCounters();
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
        private async Task UpdateCounters()
        {
            setSequences = GetSelectedSequences();
            isSetCounters = true;

            if (counterLevels < 8 && counterSequences == setSequences)
            {
                levelUp = true;
                correctOrder.Clear();
                playerOrder.Clear();
                counterSequences = 1; // Reset sequence to 1
                counterLevels++;
                counterRounds++;

                await ManageActions();
                UpdateTurn();

                levelUp = false;
            }
            else
            {
                if (counterLevels >= 8)
                {
                    levelUp = true;
                }

                counterSequences++;
                counterRounds++;
                UpdateTurn();
            }

            isSetCounters = false;
        }

        // Verify turn actions
        private async Task ManageActions()
        {
            setSequences = GetSelectedSequences();
            if (isComputerTurn && !actionTaken)
            {
                // Debug.WriteLine("ManageActions> isComputerTurn = true");
                // DisplayLabelMessage(true);
                ShufflePictureBoxes();
                actionTaken = true;

            }
            if (isPlayerTurn && !actionTaken)
            {
                Debug.WriteLine("ManageActions> isPlayerTurn = true");
                // DisplayLabelMessage(false);
                await ShufflePictureBoxes();
                actionTaken = true;
            }
            if (isDisplaySequence && !actionTaken)
            {
                Debug.WriteLine("ManageActions> isDisplaySequence = true");
                await ShufflePictureBoxes();
                ReplaceTileOnBoardAndInSequence();
                actionTaken = true;
            }
            if (isSetCounters && !actionTaken)
            {
                ReplaceAllTiles();
                actionTaken = true;
            }
            if (setSequences == int.MaxValue && !actionTaken)
            {
                isHardLevel = true;
                await ShufflePictureBoxes();
                ReplaceTileOnBoardAndInSequence();
                ReplaceAllTiles();
                actionTaken = true;
            }
        }

        // Update richtextbox ShowNumbersOfSequences
        private void UpdateSequence()
        {
            richTextBoxShowNumbersOfSequences.BackColor = Color.Yellow;
            richTextBoxShowNumbersOfSequences.Text = $"{new string(' ', 3)}Sequence of {counterSequences}";
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
                        richTextBoxTurn.BackColor = Color.LightGreen;
                        richTextBoxTurn.Text = $"\nCORRECT";

                        await Task.Delay(1500);

                        richTextBoxTurn.Text = $"\nLevel  Up";

                        await Task.Delay(1000);
                        break;
                    }
                    else
                    {
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
            richTextBoxShowRounds.Text = $"{new string(' ', 4)}Completed: {counterRounds}";
        }

        // Update richtextbox ShowLevel
        private void UpdateLevelName()
        {
            switch (counterLevels)
            {
                case (1):
                    richTextBoxShowLevelName.BackColor = Color.LightSkyBlue;
                    richTextBoxShowLevelNumber.BackColor = Color.LightSkyBlue;

                    richTextBoxShowLevelNumber.Text = $"{counterLevels}";
                    richTextBoxShowLevelName.Text = $"EasyPeasy";
                    break;
                case (2):
                    richTextBoxShowLevelName.BackColor = Color.SkyBlue;
                    richTextBoxShowLevelNumber.BackColor = Color.SkyBlue;

                    richTextBoxShowLevelNumber.Text = $"{counterLevels}";
                    richTextBoxShowLevelName.Text = $"OkiDoki";
                    break;
                case (3):
                    richTextBoxShowLevelName.BackColor = Color.CornflowerBlue;
                    richTextBoxShowLevelNumber.BackColor = Color.CornflowerBlue;

                    richTextBoxShowLevelNumber.Text = $"{counterLevels}";
                    richTextBoxShowLevelName.Text = $"Please No";
                    break;
                case (4):
                    richTextBoxShowLevelName.BackColor = Color.RoyalBlue;
                    richTextBoxShowLevelNumber.BackColor = Color.RoyalBlue;

                    richTextBoxShowLevelNumber.Text = $"{counterLevels}";
                    richTextBoxShowLevelName.Text = $"No Way!";
                    break;
                case (5):
                    richTextBoxShowLevelName.BackColor = Color.DarkKhaki;
                    richTextBoxShowLevelNumber.BackColor = Color.DarkKhaki;

                    richTextBoxShowLevelNumber.Text = $"{counterLevels}";
                    richTextBoxShowLevelName.Text = $"HELL NO";
                    break;
                case (6):
                    richTextBoxShowLevelName.BackColor = Color.DarkOrange;
                    richTextBoxShowLevelNumber.BackColor = Color.DarkOrange;

                    richTextBoxShowLevelNumber.Text = $"{counterLevels}";
                    richTextBoxShowLevelName.Text = $"NONONONO";
                    break;
                case (999):
                    richTextBoxShowLevelNumber.BackColor = Color.Red;
                    richTextBoxShowLevelName.BackColor = Color.Red;
                    richTextBoxShowRounds.BackColor = Color.Red;

                    richTextBoxShowNumbersOfSequences.BackColor = Color.Red;
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
        #endregion

        #region Processing Game Over, Input playerName, sort and display Highscores
        // Initialize setup when Game Over
        private async void GameOver()
        {
            // Set flags
            computer = false;
            startButton = true;
            gameTime = false;

            pictureBox1.Enabled = false;
            pictureBox2.Enabled = false;
            pictureBox3.Enabled = false;
            pictureBox4.Enabled = false;

            textBoxHighscore.Visible = true;
            textBoxShowResults.Visible = true;

            linkLabelGitHub.Visible = true;
            linkLabelGitHub.Enabled = true;
            linkLabelEmail.Visible = true;
            linkLabelEmail.Enabled = true;

            // Stop Stopwatch
            InitializeGameStopwatch();

            wrongSound.Play();

            // Save score
            await VerifyPlayerRank(counterRounds, counterLevels, richTextBoxShowLevelName.Text);

            correctOrder.Clear();
            playerOrder.Clear();

            counterLevels = 999;

            UpdateLevelName();

            InitialDictionaryOfTilesAtStart();

            // Reset counters rounds and levels
            counterRounds = 0;
            counterLevels = 1;
        }

        // Return playerName and set flags (rich)textboxes
        private string ProcessInputName()
        {
            Debug.WriteLine("\nProcessInputName() start");
            string playerName = textBoxInputName.Text.ToUpper().Trim();

            if (string.IsNullOrWhiteSpace(playerName))
            {
                playerName = "ANONYMOUS";
                Debug.WriteLine($"Input playerName is empty. Forced playerName is {playerName}\n");
            }

            //textBoxInputName.Enabled = false;
            textBoxInputName.Visible = false;
            buttonEnter.Enabled = false;
            buttonEnter.Visible = false;

            TextBoxHighscores();

            checkedListBoxDifficulty.Visible = true;

            startBTN.TextAlign = ContentAlignment.MiddleCenter;
            startBTN.Text = "Click to Start";
            startBTN.Enabled = true;
            startButton = true;
            richTextBoxShowRounds.Text = $"Good Luck!";

            Debug.WriteLine($"Input name is {playerName}\n");
            Debug.WriteLine("\nProcessInputName() end");
            return playerName;
        }

        // Get playerName via TextBoxInputName
        private async Task<string> PlayerName()
        {
            playerNameTcs = new TaskCompletionSource<string>();

            textBoxInputName.Clear();
            textBoxInputName.PlaceholderText = "YourNameHere";
            textBoxInputName.Visible = true;
            textBoxInputName.Enabled = true;
            buttonEnter.Visible = true;
            buttonEnter.Enabled = true;

            textBoxInputName.Focus();

            InitializeKeyEnter();

            string playerName = await playerNameTcs.Task; // return input as playerName and complete task

            return playerName;
        }

        // Display Highscores in TextBoxHighscore
        private void TextBoxHighscores()
        {
            List<(string, int, int, string, string, string, int)> topHighscores = SortBestScores();

            // Set textboxHighscore properties
            textBoxHighscore.Clear(); // Clear any existing text
            textBoxHighscore.Visible = true;

            // Use a fixed-width font for proper alignment
            textBoxHighscore.Font = new Font("Courier New", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);

            // Add the header
            textBoxHighscore.Text = "\r\n===HIGHSCORES===\r\n\r\n";
            textBoxHighscore.AppendText(string.Format("{0, -5} {1, -10} {2, -10} {3, -10}\r\n", "Rank", "Player", "Sequences", "Difficulty"));

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
                int difficultyLevelValue = score.Item7;

                // Safely get the difficulty description using the updated keys
                string difficultyLevel = difficultyPriorities
                    .FirstOrDefault(x => x.Value == difficultyLevelValue).Key ?? "Unknown";


                textBoxHighscore.AppendText(string.Format("{0, -5} {1, -10} {2, -10} {3, -10}\r\n", lineNumber, playerName, totalRounds, difficultyLevel));
                lineNumber++;
            }
        }

        // Returns list with all data in setters.txt
        private List<(string, int, int, string, string, string, int)> ReadScoresFromFile()
        {
            string file = Path.Combine(InitializeRootPath(), "sounds", "setters.txt");
            List<(string, int, int, string, string, string, int)> scoresList = new List<(string, int, int, string, string, string, int)>();

            try
            {
                using (StreamReader getHighscore = new StreamReader(file))
                {
                    string? line;
                    while ((line = getHighscore.ReadLine()) != null)
                    {
                        // Split the line into parts
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


        // Returns decreasing sorted list of higscores.txt
        private List<(string, int, int, string, string, string, int)> SortBestScores()
        {
            List<(string, int, int, string, string, string, int)> bestScores = new List<(string, int, int, string, string, string, int)>();

            try
            {
                // Get scores from file
                List<(string, int, int, string, string, string, int)> scoresList = ReadScoresFromFile();

                // Sort by playerScore, then by gameTime, then by difficulty level
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

        #region Processing Score, Save Score

        // Initialize difficulties from checkListBoxDifficulty
        private Dictionary<string, int> difficultyPriorities = new Dictionary<string, int>
                                                               {
                                                                    { "Hard", 1 },
                                                                    { "Default", 2 },
                                                                    { "Easy", 3 }
                                                               };

        // Initialize new task as private field
        private TaskCompletionSource<string> playerNameTcs = new TaskCompletionSource<string>();

        // If score in top scores, verify player's rank and save score
        private async Task VerifyPlayerRank(int totalRounds, int levelReached, string levelName)
        {
            var highScores = SortBestScores()
                .Select(score => (score.Item1, score.Item2, score.Item3, score.Item4, score.Item5, score.Item6, score.Item7))
                .ToList();

            string elapsedGameTime = InitializeGameStopwatch();
            string currentDate = DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            // Ensure a checked item exists
            if (this.checkedListBoxDifficulty.CheckedItems.Count > 0)
            {
                string difficulty = this.checkedListBoxDifficulty.CheckedItems[0].ToString().Split(':')[0].Trim(); // Extract the difficulty key
                if (difficultyPriorities.TryGetValue(difficulty, out int difficultyLevel))
                {
                    if (QualifiesForTopScores(highScores, totalRounds, elapsedGameTime, difficultyLevel))
                    {
                        string placeholderText = string.Empty; // Use placeholderText instead of playerName and create and adjust new list
                        highScores.Add((placeholderText, totalRounds, levelReached, levelName.Trim(), currentDate, elapsedGameTime, difficultyLevel));

                        highScores = highScores
                            .OrderByDescending(score => score.Item2)
                            .ThenBy(score => TimeSpan.Parse(score.Item6))
                            .ThenBy(score => score.Item7)
                            .Take(8)
                            .ToList();

                        // Determine the rank of the current score as playerRank
                        int playerRank = highScores.FindIndex(score => score.Item2 == totalRounds && score.Item6 == elapsedGameTime && score.Item7 == difficultyLevel) + 1;

                        IsHighscoreText(totalRounds, playerRank);

                        // Wait for valid input playerName
                        string playerName = await PlayerName();
                        // Discard placeholderText and update list highScores with playerName
                        highScores.RemoveAll(score => score.Item1 == placeholderText);
                        highScores.Add((playerName, totalRounds, levelReached, levelName.Trim(), currentDate, elapsedGameTime, difficultyLevel));

                        SaveScoreToFile(highScores);
                        Debug.WriteLine($"Game data saved: {playerName}, {totalRounds}, {levelReached}, {levelName.Trim()}, {currentDate}, {elapsedGameTime}, {difficultyLevel}");

                        // De-activate and hide textBoxInputName and buttonEnter
                        textBoxInputName.Visible = false;
                        textBoxInputName.Enabled = false;
                        buttonEnter.Visible = false;
                        buttonEnter.Enabled = false;
                        // Activate and show buttonRetry
                        buttonRetry.Enabled = true;
                        buttonRetry.Visible = true;
                    }
                    else
                    {
                        // Set textboxHighscore properties for proper display
                        textBoxHighscore.Clear(); // Clear any existing text
                        TextBoxHighscores();

                        IsNotHighscoreText(totalRounds);

                        buttonRetry.Enabled = true;
                        buttonRetry.Visible = true;
                    }
                    TextBoxHighscores();
                }
                else
                {
                    // Handle the case where the difficulty level is not found in the dictionary
                    MessageBox.Show("Selected difficulty level is not recognized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Handle the case where no difficulty is selected
                MessageBox.Show("No difficulty level selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Verify if score qualifies for top scores
        private bool QualifiesForTopScores(List<(string, int, int, string, string, string, int)> highScores, int totalRounds, string elapsedGameTime, int difficultyLevel)
        {
            return highScores.Count < 8 ||
                   highScores.Any(score => score.Item2 < totalRounds ||
                   (score.Item2 == totalRounds && TimeSpan.Parse(score.Item6) > TimeSpan.Parse(elapsedGameTime)) ||
                   (score.Item2 == totalRounds && TimeSpan.Parse(score.Item6) == TimeSpan.Parse(elapsedGameTime) && score.Item7 > difficultyLevel));
        }


        // Setup textBoxShowResults if score in top scores
        private void IsHighscoreText(int totalRounds, int playerRank)
        {
            TextBoxHighscores();
            textBoxShowResults.Visible = true;
            textBoxShowResults.Text = $"Your score:\r\n{totalRounds} sequences\r\nYour rank:\r\n#{playerRank}";
        }

        // Setup textBoxShowResults if score NOT in top scores
        private void IsNotHighscoreText(int totalRounds)
        {
            TextBoxHighscores();
            textBoxShowResults.Visible = true;
            textBoxShowResults.Text = $"\r\nYour score:\r\n{totalRounds} sequences";
        }


        // Save score to file. Max save scores set at 15. If currentScore == 15, replace lowest score with new score
        private void SaveScoreToFile(List<(string, int, int, string, string, string, int)> highScores)
        {
            Debug.WriteLine("SaveScoreToFile started");

            string rootPath = InitializeRootPath(); // Construct the file path using RootPath
            string file = Path.Combine(rootPath, "sounds", "setters.txt");
            string existingContent = File.Exists(file) ? File.ReadAllText(file) : string.Empty; // Read the existing content of the file

            List<(string, int, int, string, string, string, int)> currentScores = existingContent
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Split(','))
                .Select(parts => (parts[0], int.Parse(parts[1]), int.Parse(parts[2]), parts[3], parts[4], parts[5], int.Parse(parts[6])))
                .ToList();

            if (currentScores.Count < 15)
            {
                currentScores.AddRange(highScores);
                currentScores = currentScores
                    .OrderByDescending(score => score.Item2) // Sort by totalRounds
                    .ThenBy(score => TimeSpan.Parse(score.Item6)) // Sort by elapsedGameTime
                    .ThenBy(score => score.Item7) // Sort by difficultyLevel
                    .Take(15) // Keep top 15 scores
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
                // Replace lowest score with new score if applicable
                var allScores = currentScores.Concat(highScores).ToList();
                var lowestScore = allScores
                    .OrderBy(score => score.Item2) // Sort by totalRounds ascending
                    .ThenByDescending(score => TimeSpan.Parse(score.Item6)) // Sort by elapsedGameTime descending
                    .ThenByDescending(score => score.Item7) // Sort by difficultyLevel descending
                    .First();

                if (highScores.Any(newScore => newScore.Item2 > lowestScore.Item2))
                {
                    var updatedScores = currentScores
                        .Where(score => score != lowestScore)
                        .Concat(highScores)
                        .OrderByDescending(score => score.Item2) // Sort by totalRounds
                        .ThenBy(score => TimeSpan.Parse(score.Item6)) // Sort by elapsedGameTime
                        .ThenBy(score => score.Item7) // Sort by difficultyLevel
                        .Take(15) // Keep top 15 scores
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


        /*
        // Save score to file. Max save scores set at 15. If currentScore == 15, replace lowest score with new score
        private void SaveScoreToFile(List<(string, int, int, string, string, string, int)> highScores)
        {
            Debug.WriteLine("SaveScoreToFile started");

            string rootPath = InitializeRootPath(); // Construct the file path using RootPath
            string file = Path.Combine(rootPath, "sounds", "setters.txt");

            // Read existing content from the file
            List<(string, int, int, string, string, string, int)> currentScores = new List<(string, int, int, string, string, string, int)>();

            if (File.Exists(file))
            {
                try
                {
                    string existingContent = File.ReadAllText(file);
                    currentScores = existingContent
                        .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(line => line.Split(','))
                        .Select(parts => (parts[0].Trim(), int.Parse(parts[1].Trim()), int.Parse(parts[2].Trim()), parts[3].Trim(), parts[4].Trim(), parts[5].Trim(), int.Parse(parts[6].Trim())))
                        .ToList();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"An error occurred while reading existing scores: {ex.Message}");
                    MessageBox.Show($"An error occurred while reading existing scores: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Combine the existing scores with the new high scores
            List<(string, int, int, string, string, string, int)> updatedScores;

            if (currentScores.Count < 15)
            {
                // If there are fewer than 15 scores, just add the new scores
                updatedScores = currentScores
                    .Concat(highScores)
                    .OrderByDescending(score => score.Item2)
                    .ThenBy(score => TimeSpan.Parse(score.Item6))
                    .Take(15)
                    .ToList();
            }
            else
            {
                // Replace the lowest score if necessary
                var allScores = currentScores.Concat(highScores).ToList();
                var lowestScore = allScores
                    .OrderBy(score => score.Item2)
                    .ThenByDescending(score => TimeSpan.Parse(score.Item6))
                    .First();

                if (highScores.Any(newScore => newScore.Item2 > lowestScore.Item2 ||
                                                (newScore.Item2 == lowestScore.Item2 && TimeSpan.Parse(newScore.Item6) < TimeSpan.Parse(lowestScore.Item6))))
                {
                    updatedScores = allScores
                        .Where(score => score != lowestScore)
                        .OrderByDescending(score => score.Item2)
                        .ThenBy(score => TimeSpan.Parse(score.Item6))
                        .Take(15)
                        .ToList();
                }
                else
                {
                    // If no new scores are better than the lowest, retain current scores
                    updatedScores = currentScores;
                }
            }

            // Clear the file and write updated scores
            try
            {
                using (StreamWriter saveScore = new StreamWriter(file, false))
                {
                    foreach (var element in updatedScores)
                    {
                        saveScore.WriteLine($"{element.Item1},{element.Item2},{element.Item3},{element.Item4},{element.Item5},{element.Item6},{element.Item7}");
                    }
                }
                Debug.WriteLine("Scores saved successfully.");
                WriteToCopies();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred while saving scores: {ex.Message}");
                MessageBox.Show($"An error occurred while saving scores: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Debug.WriteLine("SaveScoreToFile ended");
        }
        */





        public void WriteToCopies()
        {
            Debug.WriteLine("WriteToCopies started");

            string rootPath = InitializeRootPath(); // Construct the file path using RootPath

            string file = Path.Combine(rootPath, "sounds", "setters.txt");

            // Copy the file to highscores.txt
            string copyToDir = Path.Combine(rootPath);
            Directory.CreateDirectory(copyToDir); // Ensure the directory exists
            string copyFile = Path.Combine(copyToDir, "higscores.txt");
            File.Copy(file, copyFile, true); // Copy the file and overwrite if exists
            Debug.WriteLine("Copied to file 1");

            // Copy the file to highscoresBackUp
            string backupDir = Path.Combine(copyToDir, "BackUp");
            Directory.CreateDirectory(backupDir); // Ensure the backup directory exists
            string copyFile2 = Path.Combine(copyToDir, "BackUp", "higscoresBackUp.txt");
            File.Copy(file, copyFile2, true); // Copy the file and overwrite if exists
            Debug.WriteLine("Copied to file 2");

            Debug.WriteLine("WriteToCopies ended");
        }
        #endregion

        #region Revised_example_code
        /*
        private async Task SaveScore_revised(int totalRounds, int levelReached, string levelName)
        {
            List<HighscoreItem> getHighScores = SortBestScores_revised();

            string elapsedGameTime = InitializeGameStopwatch(); // Get elapsed game time
            string rootPath = InitializeRootPath(); // Construct the file path using RootPath
            DateTime isToday = DateTime.Today; // Get current date
            string isDate = isToday.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture); // Set format date

            // Null check
            if (string.IsNullOrEmpty(rootPath))
            {
                MessageBox.Show("Error: Unable to determine root path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string file = Path.Combine(rootPath, "sounds", "setters.txt");
            HighscoreItem item = getHighScores[0];
            Debug.WriteLine($" {item.PlayerName} ; {item.Round} ");
            try
            {
                // TODO: fill in processing bits from non-revised function.
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            throw new NotImplementedException();
        }
        
        private List<HighscoreItem> SortBestScores_revised()
        {
            List<(string, int, int, string, string, string)> bestScores = new List<(string, int, int, string, string, string)>();
            List<HighscoreItem> bestScoresItems = new List<HighscoreItem>();
            try
            {
                // Get scores from file
                List<(string, int, int, string, string, string)> scoresList = ReadScoresFromFile();

                // Sort by playerScore and then by gameTime
                bestScores = scoresList.OrderByDescending(x => x.Item2)
                                       .ThenBy(x => TimeSpan.Parse(x.Item6))
                                       .Take(8)
                                       .ToList();

                foreach (var item in bestScores)
                {
                    string isName = item.Item1;
                    int isRounds = item.Item2;
                    int isLevelReached = item.Item3;
                    string isLevelName = item.Item4;
                    string dateToday = item.Item5;
                    string gameTime = item.Item6;

                    bestScoresItems.Add(new HighscoreItem() { Round = isRounds, LevelReached = isLevelReached, LevelName = isLevelName, DateToday = dateToday, GameTime = gameTime });
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine($"An error occurred while getting top scores: {e.Message}");
            }
            return bestScoresItems;
        }
        */
        #endregion
    }
}