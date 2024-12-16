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
using System.Linq.Expressions;
using System.Media;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using static System.Formats.Asn1.AsnWriter;

namespace KeepYourFocus
{
    public partial class PlayerField : Form
    {
        #region ClassProperties
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
        #endregion

        private int counter_sequences = 1;
        private int counter_levels = 1;
        private int counter_rounds = 0;
        #endregion ClassProperties

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
            #endregion Initialize Components

            #region Startup Game
            // Initialize Stopwatch for gametime
            gameStopwatch = new Stopwatch();

            ////>>>> Start Program <<<<////

            // Welcome MessageBox
            InitializeWelcomeMessageBox();

            // LinkLabels GitHub and Email
            InitializeLinkLabels();

            // Alihning richTextBoxes
            AlignTextButtonBoxesCenter();

            // Display highscore at start
            TextBoxHighscores();
            //SecondTextBoxTopFive();

            // Use initial dictionary for start setup
            InitialDictionaryOfTilesAtStart();

            // Play startup sound
            startupSound.Play();
        }
        #endregion Startup Game

        #region Initialisations and Setups
        // Thank You + some info Spam MessageBox
        private void InitializeWelcomeMessageBox()
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

        /// <summary>
        /// Initializes the game setup when the game starts. This includes setting up the game state, 
        /// updating the UI elements, and triggering the first actions needed for the game.
        /// </summary>
        private void InitializeStartGame()
        {
            // Play the button click sound when the game starts
            buttonClickSound.Play();

            // Log the game start event for debugging purposes
            Debug.WriteLine("gameTime = true");

            // Set the game time flag to true, indicating the game has started
            gameTime = true;

            // Hide the start button and disable it, indicating that the game is in progress
            startButton = false;

            // Enable the computer's turn flag, starting the computer's involvement in the game
            computer = true;

            // Reset the tiles to their initial setup on the board
            InitialDictionaryOfTilesAtStart();

            // Start or reset the stopwatch to track the game time
            InitializeGameStopwatch();

            // Set the UI flags for highscore and visibility of elements
            textBoxHighscore.Visible = false; // Hide the highscore text box

            // Enable the game tiles (picture boxes) for interaction
            pictureBox1.Enabled = true;
            pictureBox2.Enabled = true;
            pictureBox3.Enabled = true;
            pictureBox4.Enabled = true;

            // Hide and disable the start button
            startBTN.Visible = false;
            startBTN.Enabled = false;

            // Hide and disable the retry button since the game is starting afresh
            buttonRetry.Enabled = false;
            buttonRetry.Visible = false;

            // Hide and disable the GitHub and Email links
            linkLabelGitHub.Visible = false;
            linkLabelGitHub.Enabled = false;
            linkLabelEmail.Visible = false;
            linkLabelEmail.Enabled = false;

            // Reset the sequence counter to start from 1
            counter_sequences = 1;

            // Update the sequence, round, and level name to reflect the game's current state
            UpdateSequence();
            UpdateRound();
            UpdateLevelName();

            // Trigger the computer's turn at the beginning of the game
            ComputersTurn();
        }

        /// <summary>
        /// Manages the highlighting of a given PictureBox by adjusting its properties
        /// based on the 'highlight' parameter. It can either apply or remove the highlight effect.
        /// </summary>
        /// <param name="pictureBox">The PictureBox control to apply the highlight effect to.</param>
        /// <param name="highlight">A boolean flag to determine if the PictureBox should be highlighted (true) or not (false).</param>
        private void ManageHighlight(PictureBox pictureBox, bool highlight)
        {
            if (pictureBox.InvokeRequired)
            {
                // If invoking from a different thread, call this method on the UI thread
                pictureBox.Invoke(new Action<PictureBox, bool>(ManageHighlight), pictureBox, highlight);
            }
            else
            {
                if (highlight) // Highlight on
                {
                    // Remove border, add padding, and set background color to white for the highlight effect
                    pictureBox.BorderStyle = BorderStyle.None;
                    pictureBox.Padding = new Padding(5);
                    pictureBox.BackColor = Color.White;
                }
                else // Highlight off
                {
                    // Remove padding and reset background color to transparent
                    pictureBox.Padding = new Padding(0);
                    pictureBox.BackColor = Color.Transparent;
                }
            }
        }

        /// <summary>
        /// Plays a specific sound associated with the given tile color name.
        /// </summary>
        /// <param name="tile">The name of the tile color (e.g., "Red", "Blue", "Orange", etc.).</param>
        private void PlaySound(string tile)
        {
            // Switch statement to select and play the appropriate sound based on the tile color name
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

        /// <summary>
        /// Aligns the text of several UI controls (buttons and textboxes) to the center
        /// and ensures they are properly displayed. It also manages their visibility and 
        /// alignment properties.
        /// </summary>
        private void AlignTextButtonBoxesCenter()
        {
            // Align startBTN (Start button)
            startBTN.Visible = false; // Temporarily hide to modify alignment
            startBTN.TextAlign = ContentAlignment.MiddleCenter; // Align text in the center
            startBTN.Visible = true; // Make the button visible again

            // Align richTextBoxShowLevelNumber
            richTextBoxShowLevelNumber.Visible = false; // Temporarily hide to modify alignment
            richTextBoxShowLevelNumber.SelectAll(); // Select all text to align it
            richTextBoxShowLevelNumber.SelectionAlignment = HorizontalAlignment.Center; // Center align text
            richTextBoxShowLevelNumber.DeselectAll(); // Deselect text after alignment
            richTextBoxShowLevelNumber.Visible = true; // Make the rich text box visible again

            // Align richTextBoxShowLevelName
            richTextBoxShowLevelName.Visible = false; // Temporarily hide to modify alignment
            richTextBoxShowLevelName.SelectAll(); // Select all text to align it
            richTextBoxShowLevelName.SelectionAlignment = HorizontalAlignment.Center; // Center align text
            richTextBoxShowLevelName.DeselectAll(); // Deselect text after alignment
            richTextBoxShowLevelName.Visible = true; // Make the rich text box visible again

            // Align richTextBoxShowNumbersOfSequences
            richTextBoxShowNumbersOfSequences.Visible = false; // Temporarily hide to modify alignment
            richTextBoxShowNumbersOfSequences.SelectAll(); // Select all text to align it
            richTextBoxShowNumbersOfSequences.SelectionAlignment = HorizontalAlignment.Center; // Center align text
            richTextBoxShowNumbersOfSequences.DeselectAll(); // Deselect text after alignment
            richTextBoxShowNumbersOfSequences.Visible = true; // Make the rich text box visible again

            // Align richTextBoxTurn
            richTextBoxTurn.Visible = false; // Temporarily hide to modify alignment
            richTextBoxTurn.SelectAll(); // Select all text to align it
            richTextBoxTurn.SelectionAlignment = HorizontalAlignment.Center; // Center align text
            richTextBoxTurn.DeselectAll(); // Deselect text after alignment
            richTextBoxTurn.Visible = true; // Make the rich text box visible again

            // Align richTextBoxShowRounds
            richTextBoxShowRounds.Visible = false; // Temporarily hide to modify alignment
            richTextBoxShowRounds.SelectAll(); // Select all text to align it
            richTextBoxShowRounds.SelectionAlignment = HorizontalAlignment.Center; // Center align text
            richTextBoxShowRounds.DeselectAll(); // Deselect text after alignment
            richTextBoxShowRounds.Visible = true; // Make the rich text box visible again

            // Align textBoxInputName (Input name textbox)
            textBoxInputName.Visible = false; // Temporarily hide to modify visibility
            textBoxInputName.Enabled = false; // Disable interaction with the input box

            // Align textBoxShowResults (Show results textbox)
            textBoxShowResults.DeselectAll(); // Ensure no text is selected
        }

        /// <summary>
        /// Handles the click event for the Start button to initialize the game setup.
        /// Checks if the start button is enabled before calling the InitializeStartGame method.
        /// </summary>
        private void InitializeButtonStart_Click(object sender, EventArgs e)
        {
            if (!startButton)
                return;

            // Initialize the game by setting up necessary parameters and starting the game
            InitializeStartGame();
        }

        /// <summary>
        /// Handles the click event for the Retry button at the end of the game.
        /// Includes a delay and enables the retry button, along with making certain link labels visible.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">Event arguments.</param>
        private async void InitializeButtonRetry_Click(object sender, EventArgs e)
        {
            // Any additional logic specific to retry
            await Task.Delay(500); // Wait for 500 milliseconds before enabling retry button

            // Enable and make the retry button visible
            buttonRetry.Enabled = true;
            buttonRetry.Visible = true;

            // Set LinkLabels (GitHub and Email) to be visible and enabled
            linkLabelGitHub.Visible = true;
            linkLabelGitHub.Enabled = true;
            linkLabelEmail.Visible = true;
            linkLabelEmail.Enabled = true;

            // Reset the game board tiles to their initial state
            InitialDictionaryOfTilesAtStart();
            InitializeStartGame(); // Re-initialize the game
        }

        /// <summary>
        /// Handles the click event for the Enter button when the player submits their name.
        /// Processes the entered player name and sets the result for further use.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">Event arguments.</param>
        private void InitializeButtonEnter_Click(object sender, EventArgs e)
        {
            // Process and store the player name
            string playerName = ProcessInputName();

            // Mark the Task as completed when the player name is entered
            playerNameTcs.TrySetResult(ProcessInputName());

            // Deselect any selected text in the results textbox
            textBoxShowResults.DeselectAll();

            // Log the player name entry action for debugging
            Debug.WriteLine("playerName entered by buttonEnter");
        }

        /// <summary>
        /// Handles the KeyDown event for the Enter key in the player name input field.
        /// Processes the player name input when Enter is pressed and sets the result accordingly.
        /// </summary>
        private void InitializeKeyDownEnter()
        {
            // Event handler for the Enter key being pressed
            textBoxInputName.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    // Prevents the Enter key from inserting a newline and stops the "ding" sound
                    e.Handled = true;
                    e.SuppressKeyPress = true;

                    // Process and store the player name
                    string playerName = ProcessInputName();

                    // Mark the Task as completed when the player name is entered
                    playerNameTcs.TrySetResult(ProcessInputName());

                    // Deselect any selected text in the results textbox
                    textBoxShowResults.DeselectAll();

                    // Log the player name entry action for debugging
                    Debug.WriteLine("playerName entered by Keys.Enter");
                }
            };
        }

        /// <summary>
        /// Initializes and manages the game stopwatch for tracking game time.
        /// Resets and starts the stopwatch if the game is ongoing, or stops and formats the elapsed time when the game ends.
        /// </summary>
        /// <returns>Formatted string representing the elapsed game time in mm:ss format, or an empty string if no time was recorded.</returns>
        private string InitializeGameStopwatch()
        {
            if (gameTime)
            {
                // Reset and start the stopwatch
                gameStopwatch.Reset();
                gameStopwatch.Start();
                Debug.WriteLine("\n[Start Stopwatch]\n");
                return "";
            }
            else
            {
                // Stop the stopwatch and format the elapsed time
                gameStopwatch.Stop();
                Debug.WriteLine("\n[Stop Stopwatch]\n");

                if (gameStopwatch.Elapsed.TotalMilliseconds > 0)
                {
                    // Format elapsed time to minutes:seconds
                    string elapsedGameTime = gameStopwatch.Elapsed.ToString(@"mm\:ss");
                    Debug.WriteLine($"Elapsed time: {elapsedGameTime}");
                    return elapsedGameTime;
                }
                else
                {
                    // If no time was recorded, log that the elapsed time is empty
                    Debug.WriteLine("elapsedTime is empty. No time recorded!");
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Initializes and returns the root path of the application, including the "KeepYourFocus" directory.
        /// If the directory cannot be found, an error message is displayed.
        /// </summary>
        /// <returns>The root directory path as a string, or an empty string if the directory is not found.</returns>
        static string InitializeRootPath()
        {
            string directoryPath = Environment.CurrentDirectory;

            if (string.IsNullOrEmpty(directoryPath))
            {
                Debug.WriteLine("Error: Unable to determine root path.");
                MessageBox.Show("Error: Unable to determine root path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }

            string[] directorySplitPath = directoryPath.Split(Path.DirectorySeparatorChar);
            int index = Array.IndexOf(directorySplitPath, "KeepYourFocus");

            if (index != -1)
            {
                // Join the parts of the path up to the "KeepYourFocus" directory
                string rootPath = string.Join(Path.DirectorySeparatorChar.ToString(), directorySplitPath.Take(index + 1));

                if (!rootPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    rootPath += Path.DirectorySeparatorChar;
                }
                return rootPath;
            }
            else
            {
                // If the directory is not found, log an error and return an empty string
                Debug.WriteLine("Error: 'KeepYourFocus' directory not found in the path.");
                return string.Empty;
            }
        }

        /// <summary>
        /// Initializes the LinkLabels for GitHub and Email by setting their text and links.
        /// Also enables them for interaction.
        /// </summary>
        private void InitializeLinkLabels()
        {
            // Set text for the GitHub and Email link labels
            linkLabelGitHub.Text = "https://github.com/Peanutsch/KeepYourFocus.git";
            linkLabelEmail.Text = "peanutsch@duck.com";

            // Add the respective link data for the GitHub and Email links
            linkLabelGitHub.Links.Add(0, linkLabelGitHub.Text.Length, "https://github.com/Peanutsch/KeepYourFocus.git");
            linkLabelEmail.Links.Add(0, linkLabelEmail.Text.Length, "mailto:peanutsch@duck.com");
        }

        /// <summary>
        /// Handles the click event for the GitHub LinkLabel. Opens the GitHub link in the default browser.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">Event arguments containing information about the clicked link.</param>
        private void LinkLabelGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Link?.LinkData is string url && !string.IsNullOrWhiteSpace(url))
            {
                // Open the link in the default browser
                OpenLink(url);
            }
            else
            {
                MessageBox.Show("The GitHub link is invalid or missing.");
            }
        }

        /// <summary>
        /// Handles the click event for the Email LinkLabel. Opens the default email client with the specified email address.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">Event arguments containing information about the clicked link.</param>
        private void LinkLabelEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Link?.LinkData is string url && !string.IsNullOrWhiteSpace(url))
            {
                // Open the link in the default email client
                OpenLink(url);
            }
            else
            {
                MessageBox.Show("The email link is invalid or missing.");
            }
        }

        /// <summary>
        /// Opens a given URL in the default browser or email client. 
        /// If unable to open the link, shows an error message.
        /// </summary>
        /// <param name="url">The URL to be opened.</param>
        private void OpenLink(string? url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url!) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open link. Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Initializes the dictionary of PictureBox tiles at the start of the game.
        /// If the dictionary already has items, it refreshes and repositions the PictureBoxes.
        /// Otherwise, it sets up the initial PictureBoxes for each tile type and makes them visible.
        /// </summary>
        private void InitialDictionaryOfTilesAtStart()
        {
            if (pictureBoxDictionary.Count > 0)
            {
                RefreshAndRepositionPictureBoxes();
                return;
            }

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

        /// <summary>
        /// Returns a dictionary of all possible tiles with their corresponding image paths.
        /// The dictionary includes a variety of tiles like Red, Blue, Orange, Green, and others.
        /// </summary>
        /// <returns>A dictionary containing tile names as keys and their respective image paths as values.</returns>
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

        /// <summary>
        /// Initializes a PictureBox with a specified tile type and image path.
        /// Configures the PictureBox properties, attaches an event handler for click actions, and adds it to the dictionary.
        /// </summary>
        /// <param name="pictureBox">The PictureBox to initialize.</param>
        /// <param name="tile">The name of the tile type (e.g., "Red").</param>
        /// <param name="imagePath">The file path to the tile image.</param>
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

        /// <summary>
        /// Randomizes the order of the tiles in the dictionary and repositions the PictureBoxes accordingly.
        /// The Fisher-Yates shuffle algorithm is used to shuffle the keys of the dictionary.
        /// </summary>
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

        /// <summary>
        /// Randomizes the tiles to ensure no more than two of the same tile appear consecutively.
        /// Uses Fisher-Yates shuffle to randomize the tiles and checks if any tile appears three times in a row.
        /// If it does, a swap is performed to maintain randomness and avoid consecutive duplicates.
        /// </summary>
        /// <returns>The name of the newly selected tile after randomization.</returns>
        private string RandomizerTiles()
        {
            // Verify if the dictionary is not empty
            if (pictureBoxDictionary.Count == 0)
            {
                Debug.WriteLine("PictureBoxDictionary is empty. Verify filepaths of tiles in InitialDictionaryOfTilesAtStart()");
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
                    Debug.WriteLine("3x in a row: swapIndex");
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

            Debug.WriteLine($"Selected new tile: {newTile}");

            return newTile;
        }

        /// <summary>
        /// Shuffles the dictionary of all tiles using the Fisher-Yates shuffle algorithm (also known as Knuth shuffle).
        /// The dictionary is first converted into a list of key-value pairs, shuffled, and then converted back into a dictionary.
        /// </summary>
        /// <returns>A shuffled dictionary where the tiles are in random order.</returns>
        private Dictionary<string, string> ShuffleDictOfAllTiles()
        {
            Dictionary<string, string> dictOfAllTiles = DictOfAllTiles();

            // Convert the dictionary to a list of KeyValuePair
            List<KeyValuePair<string, string>> listOfAllTiles = dictOfAllTiles.ToList();

            // Fisher-Yates shuffle algorithm (Knuth shuffle)
            int numberOfItems = listOfAllTiles.Count;
            while (numberOfItems > 1)
            {
                numberOfItems--;
                int randomIndex = rnd.Next(numberOfItems + 1);
                KeyValuePair<string, string> temp = listOfAllTiles[randomIndex];
                listOfAllTiles[randomIndex] = listOfAllTiles[numberOfItems];
                listOfAllTiles[numberOfItems] = temp;
            }

            // Convert the shuffled list back into a dictionary
            Dictionary<string, string> shuffledDictOfAllTiles = listOfAllTiles.ToDictionary(kv => kv.Key, kv => kv.Value);

            return shuffledDictOfAllTiles;
        }

        /// <summary>
        /// Defines fixed positions for the PictureBoxes based on the index.
        /// This method maps an index to a specific position on the screen.
        /// </summary>
        /// <param name="index">The index that determines the position of the PictureBox.</param>
        /// <returns>A Point representing the position of the PictureBox based on the provided index.
        /// Returns Point.Empty if the index is out of the defined range.</returns>
        private static Point SetFixedPositionPictureBoxes(int index)
        {
            // Define fixed positions based on the index
            switch (index)
            {
                case 0:
                    return new Point(13, 12); // Position for index 0
                case 1:
                    return new Point(321, 12); // Position for index 1
                case 2:
                    return new Point(13, 316); // Position for index 2
                case 3:
                    return new Point(321, 316); // Position for index 3
                default:
                    return Point.Empty; // Default position if index is out of range
            }
        }
        #endregion

        #region Difficulties
        /// <summary>
        /// Randomly repositions the PictureBoxes based on the shuffled order of the tiles.
        /// This method shuffles the PictureBoxes and assigns new fixed positions to them.
        /// </summary>
        private void RefreshAndRepositionPictureBoxes()
        {
            // Get the shuffled PictureBoxes from the dictionary
            var shuffledPictureBoxes = pictureBoxDictionary.Values.OrderBy(x => rnd.Next()).ToList();

            // Iterate through the shuffled PictureBoxes and assign fixed positions
            for (int i = 0; i < shuffledPictureBoxes.Count; i++)
            {
                shuffledPictureBoxes[i].Location = SetFixedPositionPictureBoxes(i); // Set position based on index
                shuffledPictureBoxes[i].Visible = true; // Make sure the PictureBox is visible
            }
        }

        /// <summary>
        /// Shuffles the PictureBoxes either before the player's turn or after the player clicks,
        /// based on the level and probability factors. This method handles both the display sequence shuffle 
        /// and the player turn shuffle with respective delays and sound transitions.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task ShufflePictureBoxes()
        {
            // Check for display sequence shuffling condition based on current level and randomness
            if (counter_levels == 2 && rnd.Next(100) <= 100 && isDisplaySequence ||
                counter_levels >= 3 && rnd.Next(100) <= 75 && isDisplaySequence ||
                counter_levels >= 5 && rnd.Next(100) <= 85 && isDisplaySequence)
            {
                Debug.WriteLine($"Shuffle PictureBoxes Case 1: Shuffle before player's turn");

                await Task.Delay(500); // Delay 500 ms for space between colorSound and transitionSound
                transitionSound.Play(); // Play transition sound
                RandomizerShufflePictureBoxes(); // Shuffle the PictureBoxes
                RefreshAndRepositionPictureBoxes(); // Reposition the shuffled PictureBoxes
                await Task.Delay(500); // Wait for another 500 ms
            }

            // Check for player turn shuffling condition based on level and randomness
            if (counter_levels >= 3 && rnd.Next(100) <= 55 && isPlayerTurn ||
                counter_levels >= 4 && rnd.Next(100) <= 75 && isPlayerTurn ||
                counter_levels >= 6 && rnd.Next(100) <= 85 && isPlayerTurn)
            {
                Debug.WriteLine($"Shuffle PictureBoxes Case 2: Shuffle after player click");

                RandomizerShufflePictureBoxes(); // Shuffle the PictureBoxes
                RefreshAndRepositionPictureBoxes(); // Reposition the shuffled PictureBoxes
            }
        }

        /// <summary>
        /// Replaces a tile on the board and/or in the sequence with a new randomly selected tile.
        /// This method checks if the replacement should happen in order or on the board, 
        /// and updates both the board and the sequence accordingly.
        /// </summary>
        /// <returns>
        /// A tuple containing the updated dictionary of PictureBoxes, updated sequence, and a flag indicating if a replacement occurred.
        /// </returns>
        private (Dictionary<string, PictureBox>, List<string>, bool) ReplaceTileOnBoardAndInSequence()
        {
            string newTile = RandomizerTiles(); // Get a new tile randomly
            Dictionary<string, string> dictOfAllTiles = DictOfAllTiles(); // Get the dictionary of all tiles
            List<KeyValuePair<string, string>> listOfAllTiles = dictOfAllTiles.ToList(); // Convert to a list for random selection

            // Conditions to check if replacement should happen in order or on the board
            bool checkReplaceInOrder = (counter_levels >= 5 && correctOrder.Count > 2 && rnd.Next(100) <= 55) ||
                                       (counter_levels >= 6 && correctOrder.Count > 2 && rnd.Next(100) <= 75) ||
                                       (counter_levels >= 8 && correctOrder.Count > 2 && rnd.Next(100) <= 85);

            bool checkReplaceOnBoard = (counter_levels >= 6 && correctOrder.Count > 2 && rnd.Next(100) <= 55) ||
                                       (counter_levels >= 7 && correctOrder.Count > 2 && rnd.Next(100) <= 75) ||
                                       (counter_levels >= 9 && correctOrder.Count > 2 && rnd.Next(100) <= 85);

            bool replacementOccurred = false; // Flag to track if replacement happened

            if (checkReplaceInOrder || checkReplaceOnBoard)
            {
                // Copy the correctOrder to make modifications
                List<string> copyCorrectOrder = new List<string>(correctOrder);

                // Randomize the tile to be deleted from the order
                int randomIndex = rnd.Next(copyCorrectOrder.Count);
                string deleteTile = copyCorrectOrder[randomIndex];

                Debug.WriteLine($"deleteTile: [{deleteTile}]");

                // If replacing in order, ensure the new tile is not the same as the deleted tile
                if (checkReplaceInOrder && newTile != deleteTile && randomIndex != copyCorrectOrder.Count - 1)
                {
                    Debug.WriteLine("\nCorrectOrder = " + string.Join(", ", correctOrder));
                    Debug.WriteLine($"Replacing in order [{deleteTile}] at index [{randomIndex}] with new tile [{newTile}]\n");

                    copyCorrectOrder[randomIndex] = newTile; // Replace in the copy of the order

                    // Update the correctOrder with the modified copyCorrectOrder
                    correctOrder = copyCorrectOrder;
                    replacementOccurred = true;
                }

                // If replacing on board, find the corresponding PictureBox and update it
                if (checkReplaceOnBoard)
                {
                    // Get the PictureBox associated with the deleteTile
                    PictureBox pictureBoxToReplace = pictureBoxDictionary[deleteTile];

                    // Remove the old tile from the board
                    pictureBoxDictionary.Remove(deleteTile);

                    // Randomize a new tile that's not already on the board or in the correct order
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

                    // Replace the deleted tile in the copyCorrectOrder
                    for (int indexItem = 0; indexItem < copyCorrectOrder.Count; indexItem++)
                    {
                        if (copyCorrectOrder[indexItem] == deleteTile)
                        {
                            copyCorrectOrder[indexItem] = pickNewTile;
                        }
                    }

                    // Update correctOrder with the modified copyCorrectOrder
                    correctOrder = copyCorrectOrder;
                    replacementOccurred = true;
                }

                Debug.WriteLine("Updated correctOrder = " + string.Join(", ", correctOrder));
            }
            return (pictureBoxDictionary, correctOrder, replacementOccurred);
        }

        /// <summary>
        /// Replaces all tiles on the board when the player levels up.
        /// This method shuffles the tiles and replaces the PictureBoxes with new tiles based on the shuffled order.
        /// </summary>
        private void ReplaceAllTiles()
        {
            // Conditions for level-up replacement based on current level and randomness
            if (counter_levels >= 4 && levelUp == true && rnd.Next(100) <= 55 ||
                counter_levels >= 5 && levelUp == true && rnd.Next(100) <= 75 ||
                counter_levels >= 7 && levelUp == true && rnd.Next(100) <= 85)
            {
                // Shuffle all the tiles
                Dictionary<string, string> shuffledTiles = ShuffleDictOfAllTiles();

                // Ensure there are enough tiles to assign to the PictureBoxes
                if (shuffledTiles.Count >= 3)
                {
                    // Retrieve the first 4 key-value pairs from the shuffled tiles
                    KeyValuePair<string, string> kvp1 = shuffledTiles.ElementAt(0);
                    KeyValuePair<string, string> kvp2 = shuffledTiles.ElementAt(1);
                    KeyValuePair<string, string> kvp3 = shuffledTiles.ElementAt(2);
                    KeyValuePair<string, string> kvp4 = shuffledTiles.ElementAt(3);

                    try
                    {
                        // Clear the dictionary and reinitialize the PictureBoxes with the shuffled tiles
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

        /// <summary>
        /// Displays a message in the form of a label, indicating either the computer's or the player's turn.
        /// The message content and visibility are randomized based on the current game level and turn (computer or player).
        /// </summary>
        /// <param name="iscomputerTurn">
        /// A boolean value indicating whether it is the computer's turn (true) or the player's turn (false).
        /// </param>
        private async void DisplayLabelMessage(bool iscomputerTurn)
        {
            /*
             * Show labels with text in either computer's or Player's turn
             * E.g. Computer's turn: "Click Here", "Start Here!", "Start With this One!" (45%, 55% or 65% chance depending on level)
             * Player's turn: various messages based on different levels and conditions
             */

            // Determine the chance of showing a message based on the game level
            int chance = counter_levels >= 6 ? 55 : 45;

            // Decide whether to show the message based on the turn and conditions
            bool showMessage = computer
                ? counter_levels >= 2 && rnd.Next(100) <= chance && correctOrder.Count != correctOrder.Count - 1
                : counter_levels >= 2 && rnd.Next(100) <= 65 && playerOrder.Count != correctOrder.Count;

            // If the conditions are met to display a message
            if (showMessage)
            {
                // Define a list of available labels to display the message
                List<Label> labels = new List<Label> { LabelMessage1, LabelMessage2, LabelMessage3, LabelMessage4 };

                // Define possible messages for the labels based on whose turn it is
                List<string> labelText;

                // Messages for the computer's turn
                if (iscomputerTurn)
                {
                    labelText = new List<string> { "Click Here", "Start Here!", "Start With\nthis One!", "This One!", "Over Here!" };
                }
                else // Messages for the player's turn
                {
                    labelText = new List<string>
                                        {
                                         "Click Here", "This Is NOT\nThe Correct tile!", "The computer\nIs Lying!",
                                         "This Is\nThe One!", "Just Kidding!\nClick This One!", "This Is NOT\nThe Right Tile!",
                                         "This Is\nThe Next\nOne!", "Now This One!", "This One!", "Over Here!"
                                        };
                }

                // Randomize which label will show the message (from LabelMessage1 to LabelMessage4)
                int pickLabelIndex = rnd.Next(labels.Count);
                Label randomizedLabelClickHere = labels[pickLabelIndex];

                // Randomize which message will be displayed
                int pickLabelText = rnd.Next(labelText.Count);
                string randomizedText = labelText[pickLabelText];

                // Wait for a brief moment (250ms delay before displaying the message)
                await Task.Delay(250);

                // Set the chosen label's properties
                randomizedLabelClickHere.AutoSize = true;
                randomizedLabelClickHere.Text = randomizedText;
                randomizedLabelClickHere.Visible = true;

                // Wait for 750ms before hiding the label again
                await Task.Delay(750);

                // Hide the label after the message is displayed
                randomizedLabelClickHere.Visible = false;
            }
        }
        #endregion

        #region Game Elements
        /// <summary>
        /// Handles the computer's turn in the game. The computer generates a new random tile sequence,
        /// updates the turn display, and then shows the sequence to the player.
        /// </summary>
        private void ComputersTurn()
        {
            textBoxShowResults.Visible = false;

            isComputerTurn = true;  // Indicate that it's the computer's turn

            computer = true;
            correctOrder.Add(RandomizerTiles());  // Add a new tile to the sequence
            UpdateTurn();  // Update the turn display (computer's turn)
            DisplaySequence();  // Display the sequence to the player

            isComputerTurn = false;  // End the computer's turn
        }

        /// <summary>
        /// Displays the sequence of tiles that the computer has set for the player to follow. 
        /// The sequence is shown with highlights and sounds played for each tile in the sequence.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async void DisplaySequence()
        {
            Dictionary<string, PictureBox> updatedPictureBoxDictionary;
            List<string> updatedCorrectOrder;
            bool replacementOccurred;

            // Verify if squares are replaced during the sequence update
            (updatedPictureBoxDictionary, updatedCorrectOrder, replacementOccurred) = ReplaceTileOnBoardAndInSequence();

            if (replacementOccurred)
            {
                // If sequence replacement occurred, update dictionaries and orders
                pictureBoxDictionary = updatedPictureBoxDictionary;
                correctOrder = updatedCorrectOrder;
            }

            isDisplaySequence = true;
            computer = true;

            Debug.WriteLine($"\nDisplay Sequence: {counter_sequences}");
            Debug.WriteLine("correctOrder = " + string.Join(", ", correctOrder));

            await Task.Delay(500);  // Wait for 500ms before starting to show the sequence

            // Display each tile in the sequence
            foreach (var tile in correctOrder)
            {
                var box = pictureBoxDictionary[tile];
                if (box == null)
                    continue;  // Skip if box is null

                await Task.Delay(500);  // Delay before showing next tile in the sequence

                PlaySound(tile);  // Play the sound for the tile
                ManageHighlight(box, true);  // Highlight the tile
                await Task.Delay(150);  // Keep highlight for 150ms
                ManageHighlight(box, false);  // Remove the highlight
                await Task.Delay(50);  // Short delay before next tile
            }

            // Verify difficulty and other game actions (e.g., level up)
            await ManageActions();

            await Task.Delay(500);  // Delay before switching to player's turn

            computer = false;
            isDisplaySequence = false;

            UpdateTurn();  // Update the display to show it's the player's turn
        }

        /// <summary>
        /// Handles the player's turn in the game. The player selects tiles, and the input is checked 
        /// against the correct sequence. If the player's input is incorrect, the game ends.
        /// </summary>
        /// <param name="sender">The sender object, typically the clicked tile.</param>
        /// <param name="e">The event arguments for the click event.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async void PlayersTurn(object? sender, EventArgs e)
        {
            isPlayerTurn = true;  // Indicate that it's the player's turn

            // Block clicks during the computer's turn or before the start button is clicked
            if (startButton || computer)
                return;

            if (sender is PictureBox clickedBox)
            {
                string tile = clickedBox.Tag?.ToString() ?? "";  // Get the tile identifier

                PlaySound(tile);  // Play sound for the selected tile
                ManageHighlight(clickedBox, true);  // Highlight the clicked tile

                playerOrder.Add(tile);  // Add the selected tile to the player's sequence

                Debug.WriteLine($"Player: [{tile}]");

                // Verify the player's input against the correct sequence
                await ManageActions();

                // Check if each tile in the player's input matches the correct order
                for (int input = 0; input < playerOrder.Count; input++)
                {
                    if (playerOrder[input] != correctOrder[input])
                    {
                        await Task.Delay(100);  // Short delay before providing feedback
                        ManageHighlight(clickedBox, false);  // Remove highlight from the clicked tile
                        await Task.Delay(250);  // Delay to provide feedback before game over
                        TextBoxHighscores();  // Show high score message
                        GameOver();  // End the game

                        isPlayerTurn = false;  // End the player's turn

                        return;
                    }
                }

                // If the player has entered the correct sequence
                if (playerOrder.Count == correctOrder.Count)
                {
                    await Task.Delay(100);  // Short delay before proceeding
                    ManageHighlight(clickedBox, false);  // Remove highlight after correct input
                    await Task.Delay(50);  // Brief pause

                    ManageCountersAndLevels();  // Update counters and levels

                    isPlayerTurn = false;  // End the player's turn
                }
                else
                {
                    // If the sequence is not yet complete, remove the highlight and wait
                    await Task.Delay(100);
                    ManageHighlight(clickedBox, false);
                    await Task.Delay(50);

                    isPlayerTurn = false;  // End the player's turn
                }
            }
            isPlayerTurn = false;  // Reset player turn status
        }


        /// <summary>
        /// Manages the counters and levels after the player correctly replicates the sequence. 
        /// It updates the game state (round, level, counters) and prepares for the next round.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async void ManageCountersAndLevels()
        {
            // If the player's sequence is shorter than the correct sequence, exit the method
            if (playerOrder.Count < correctOrder.Count)
                return;

            // Block player clicks as the game progresses to the next round
            computer = true;

            nextRound = true;  // Indicate that the next round is starting

            // Delay 250ms between the beep sound and the correct sound
            await Task.Delay(250);
            correctSound.Play();  // Play the sound indicating the player's input was correct

            // Update the game counters, sequence, round, level name, and turn
            UpdateCounters();
            UpdateSequence();
            UpdateRound();
            UpdateLevelName();
            UpdateTurn();

            // Wait for 2750ms before starting the next round
            await Task.Delay(2750);

            // Clear the player's order and prepare for the next round
            playerOrder.Clear();
            nextRound = false;  // Indicate that the round is no longer in progress

            // Start the computer's turn for the next round
            ComputersTurn();
        }


        /// <summary>
        /// Updates the counters (sequence, level, and round) based on the player's progress in the game. 
        /// It handles level progression, resets, and increments based on the number of sequences completed per level.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async void UpdateCounters()
        {
            isSetCounters = true;

            switch (counter_sequences)
            {
                // TEST WITH 3 ROUNDS
                // When the player reaches 6 sequences and is below level 8, level up
                case (3) when counter_levels < 8:
                    levelUp = true;  // Indicate that level up is triggered
                    correctOrder.Clear();  // Clear the sequence
                    playerOrder.Clear();  // Clear the player's order
                    counter_sequences = 1;  // Reset sequence to 1
                    counter_levels++;  // Increment the level
                    counter_rounds++;  // Increment the round

                    await ManageActions();  // Perform actions like shuffling tiles
                    UpdateTurn();  // Update the turn display

                    levelUp = false;  // Reset level up flag
                    break;

                // Default case: Increment sequences and rounds, and update turn
                default:
                    if (counter_levels >= 8)
                    {
                        levelUp = true;  // Indicate that level up is triggered

                        counter_sequences++;  // Increment sequence count
                        counter_rounds++;  // Increment round count
                        UpdateTurn();  // Update the turn display

                        isSetCounters = false;  // Set counters flag to false
                    }
                    else
                    {
                        counter_sequences++;  // Increment sequence count
                        counter_rounds++;  // Increment round count
                        UpdateTurn();  // Update the turn display

                        isSetCounters = false;  // Set counters flag to false
                    }
                    break;
            }
            isSetCounters = false;  // Set counters flag to false
        }

        /// <summary>
        /// Verifies and handles actions based on the current turn state. 
        /// It handles different phases like the computer's turn, player's turn, or sequence display.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ManageActions()
        {
            if (isComputerTurn)
            {
                await ShufflePictureBoxes();  // Shuffle tiles during computer's turn
            }
            if (isPlayerTurn)
            {
                Debug.WriteLine("ManageActions> isPlayerTurn = true");
                await ShufflePictureBoxes();  // Shuffle tiles during player's turn
            }
            if (isDisplaySequence)
            {
                Debug.WriteLine("ManageActions> isDisplaySequence = true");
                await ShufflePictureBoxes();  // Shuffle tiles when displaying the sequence
                ReplaceTileOnBoardAndInSequence();  // Replace tiles on board during sequence display
            }
            if (isSetCounters)
            {
                ReplaceAllTiles();  // Replace all tiles when counters are set
            }
        }

        /// <summary>
        /// Updates the rich text box displaying the current sequence number.
        /// </summary>
        private void UpdateSequence()
        {
            richTextBoxShowNumbersOfSequences.BackColor = Color.Yellow;  // Set background color for sequence display
            richTextBoxShowNumbersOfSequences.Text = $"{new string(' ', 3)}Sequence of {counter_sequences}";  // Display the current sequence number
        }

        /// <summary>
        /// Updates the rich text box displaying the current turn, showing whether it's the player's or computer's turn, 
        /// or if it's the next round or level up.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async void UpdateTurn()
        {
            switch (computer, startButton, nextRound)
            {
                // Case for level up or next round
                case (_, _, true):
                    if (levelUp)
                    {
                        richTextBoxTurn.BackColor = Color.LightGreen;  // Set background color for correct turn
                        richTextBoxTurn.Text = $"\nCORRECT";  // Display "CORRECT"

                        await Task.Delay(1500);  // Wait for 1.5 seconds

                        richTextBoxTurn.Text = $"\nLevel  Up";  // Display "Level Up"

                        await Task.Delay(1000);  // Wait for 1 second
                        break;
                    }
                    else
                    {
                        richTextBoxTurn.BackColor = Color.LightGreen;  // Set background color for correct turn
                        richTextBoxTurn.Text = $"\nCORRECT";  // Display "CORRECT"

                        await Task.Delay(1500);  // Wait for 1.5 seconds
                        richTextBoxTurn.Text = $"\nNext Sequence";  // Display "Next Sequence"

                        await Task.Delay(1000);  // Wait for 1 second
                        break;
                    }

                // Case for computer's turn
                case (true, false, _):
                    richTextBoxTurn.BackColor = Color.Salmon;  // Set background color for computer's turn
                    richTextBoxTurn.Text = $"computer's Turn";  // Display "Computer's Turn"
                    richTextBoxTurn.Text = $"Running\n::\nSequence";  // Display sequence message
                    break;

                // Case for player's turn
                case (false, false, _):
                    richTextBoxTurn.BackColor = Color.Green;  // Set background color for player's turn
                    richTextBoxTurn.Text = $"\nPlayer's Turn";  // Display "Player's Turn"
                    break;
            }
        }

        /// <summary>
        /// Updates the rich text box displaying the current round number.
        /// </summary>
        private void UpdateRound()
        {
            richTextBoxShowRounds.BackColor = Color.LightSkyBlue;  // Set background color for round display
            richTextBoxShowRounds.Text = $"{new string(' ', 4)}Completed: {counter_rounds}";  // Display the current round number
        }

        /// <summary>
        /// Updates the rich text boxes displaying the current level name and number, based on the current level.
        /// </summary>
        private void UpdateLevelName()
        {
            switch (counter_levels)
            {
                case (1):
                    richTextBoxShowLevelName.BackColor = Color.LightSkyBlue;  // Set background color for level 1
                    richTextBoxShowLevelNumber.BackColor = Color.LightSkyBlue;  // Set background color for level number display

                    richTextBoxShowLevelNumber.Text = $"{counter_levels}";  // Display the level number
                    richTextBoxShowLevelName.Text = $"EasyPeasy";  // Display the level name
                    break;
                case (2):
                    richTextBoxShowLevelName.BackColor = Color.SkyBlue;  // Set background color for level 2
                    richTextBoxShowLevelNumber.BackColor = Color.SkyBlue;  // Set background color for level number display

                    richTextBoxShowLevelNumber.Text = $"{counter_levels}";  // Display the level number
                    richTextBoxShowLevelName.Text = $"OkiDoki";  // Display the level name
                    break;
                case (3):
                    richTextBoxShowLevelName.BackColor = Color.CornflowerBlue;  // Set background color for level 3
                    richTextBoxShowLevelNumber.BackColor = Color.CornflowerBlue;  // Set background color for level number display

                    richTextBoxShowLevelNumber.Text = $"{counter_levels}";  // Display the level number
                    richTextBoxShowLevelName.Text = $"Please No";  // Display the level name
                    break;
                case (4):
                    richTextBoxShowLevelName.BackColor = Color.RoyalBlue;  // Set background color for level 4
                    richTextBoxShowLevelNumber.BackColor = Color.RoyalBlue;  // Set background color for level number display

                    richTextBoxShowLevelNumber.Text = $"{counter_levels}";  // Display the level number
                    richTextBoxShowLevelName.Text = $"No Way!";  // Display the level name
                    break;
                case (5):
                    richTextBoxShowLevelName.BackColor = Color.DarkKhaki;  // Set background color for level 5
                    richTextBoxShowLevelNumber.BackColor = Color.DarkKhaki;  // Set background color for level number display

                    richTextBoxShowLevelNumber.Text = $"{counter_levels}";  // Display the level number
                    richTextBoxShowLevelName.Text = $"HELL NO";  // Display the level name
                    break;
                case (6):
                    richTextBoxShowLevelName.BackColor = Color.DarkOrange;  // Set background color for level 6
                    richTextBoxShowLevelNumber.BackColor = Color.DarkOrange;  // Set background color for level number display

                    richTextBoxShowLevelNumber.Text = $"{counter_levels}";  // Display the level number
                    richTextBoxShowLevelName.Text = $"NONONONO";  // Display the level name
                    break;
                case (999):
                    richTextBoxShowLevelNumber.BackColor = Color.Red;  // Set background color for game over level
                    richTextBoxShowLevelName.BackColor = Color.Red;  // Set background color for game over level name
                    richTextBoxShowRounds.BackColor = Color.Red;  // Set background color for game over rounds display

                    richTextBoxShowNumbersOfSequences.BackColor = Color.Red;  // Set background color for game over sequence display
                    richTextBoxShowNumbersOfSequences.Text = $"{new string(' ', 5)}GAME OVER";  // Display "Game Over" message

                    textBoxShowResults.Visible = true;  // Show results textbox
                    break;
                default:
                    richTextBoxShowLevelName.BackColor = Color.OrangeRed;  // Set background color for undefined level
                    richTextBoxShowLevelNumber.BackColor = Color.OrangeRed;  // Set background color for undefined level number display

                    richTextBoxShowLevelNumber.Text = $"666";  // Display undefined level number
                    richTextBoxShowLevelName.Text = $"HELLMODE";  // Display undefined level name
                    break;
            }
        }
        #endregion

        #region Processing Game Over, Input playerName, sort and display Highscores
        /// <summary>
        /// Initializes the game setup when the game is over. 
        /// Stops the game, disables controls, plays a sound, saves the score, 
        /// and resets relevant game variables for a new round.
        /// </summary>
        private async void GameOver()
        {
            // Set flags
            computer = false;  // Disable computer's turn
            startButton = true;  // Enable the start button
            gameTime = false;  // End game time
            Debug.WriteLine("gameTime = false");

            // Disable all picture boxes
            pictureBox1.Enabled = false;
            pictureBox2.Enabled = false;
            pictureBox3.Enabled = false;
            pictureBox4.Enabled = false;

            // Make result elements visible
            textBoxHighscore.Visible = true;
            textBoxShowResults.Visible = true;
            linkLabelGitHub.Visible = true;
            linkLabelGitHub.Enabled = true;
            linkLabelEmail.Visible = true;
            linkLabelEmail.Enabled = true;

            // Stop the stopwatch for the game timer
            InitializeGameStopwatch();

            // Play wrong sound on game over
            wrongSound.Play();

            // Save player score to highscore list
            await VerifyPlayerRank(counter_rounds, counter_levels, richTextBoxShowLevelName.Text);

            // Clear player and sequence orders
            correctOrder.Clear();
            playerOrder.Clear();

            // Set level to 999 (indicating game over)
            counter_levels = 999;

            // Update the level name display
            UpdateLevelName();

            // Initialize the dictionary of tiles at start
            InitialDictionaryOfTilesAtStart();

            // Reset counters for rounds and levels
            counter_rounds = 0;
            counter_levels = 1;
        }

        /// <summary>
        /// Processes the player's name input from the textbox. 
        /// If no name is entered, sets a default name.
        /// Updates UI elements and prepares for game start.
        /// </summary>
        /// <returns>The player's name.</returns>
        private string ProcessInputName()
        {
            Debug.WriteLine("\nProcessInputName()");
            // Get the player's name from the textbox, or set default if empty
            string playerName = textBoxInputName.Text.ToUpper().Trim();

            if (string.IsNullOrWhiteSpace(playerName))
            {
                playerName = "PEANUTSCH";  // Default name
            }

            // Hide input fields and disable the enter button
            textBoxInputName.Visible = false;
            buttonEnter.Enabled = false;
            buttonEnter.Visible = false;

            // Display high scores in the textbox
            TextBoxHighscores();

            // Update start button text and enable it
            startBTN.TextAlign = ContentAlignment.MiddleCenter;
            startBTN.Text = "Click to Start";
            startBTN.Enabled = true;
            startButton = true;
            richTextBoxShowRounds.Text = $"Good Luck!";

            Debug.WriteLine($"Input name is {playerName}\n");
            return playerName;
        }

        /// <summary>
        /// Prompts the player to enter their name, waits for input, 
        /// and returns the entered name. If no name is entered, sets a default name.
        /// </summary>
        /// <returns>The player's name.</returns>
        private async Task<string> PlayerName()
        {
            playerNameTcs = new TaskCompletionSource<string>();

            // Clear input field and set placeholder text
            textBoxInputName.Clear();
            textBoxInputName.PlaceholderText = "YourNameHere";
            textBoxInputName.Visible = true;
            textBoxInputName.Enabled = true;
            buttonEnter.Visible = true;
            buttonEnter.Enabled = true;

            // Focus on the textbox to enter name
            textBoxInputName.Focus();

            // Initialize key down event for enter key
            InitializeKeyDownEnter();

            // Wait for the player's name to be entered and return it
            string playerName = await playerNameTcs.Task;

            // If no name is entered, set a default name
            if (string.IsNullOrWhiteSpace(playerName))
            {
                playerName = "PEANUTSCH";
            }

            return playerName;
        }

        /// <summary>
        /// Displays the high scores in the textbox with a formatted list.
        /// The list includes player rank, name, sequences completed, and date of play.
        /// </summary>
        private void TextBoxHighscores()
        {
            // Get top high scores and sort them
            List<(string, int, int, string, string, string)> topHighscores = SortBestScores();
            List<int> listLineNumber = new List<int>();
            List<int> listTotalRounds = new List<int>();

            Debug.WriteLine($"topHighscores count: {topHighscores.Count}");

            // Set textbox properties for proper display
            textBoxHighscore.Clear();  // Clear previous content
            textBoxHighscore.Visible = true;  // Make textbox visible
            textBoxHighscore.Font = new Font("Courier New", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);  // Use fixed-width font for alignment

            // Add header to the highscore list
            textBoxHighscore.Text = "\r\n===HIGHSCORES===\r\n\r\n";
            textBoxHighscore.AppendText(string.Format("{0, -5} {1, -10} {2, -10} {3, -10}\r\n", "Rank", "Player", "Sequences", "Date"));

            // Append the high scores data
            int lineNumber = 1;
            foreach (var score in topHighscores)
            {
                string playerName = score.Item1;
                int totalRounds = score.Item2;
                int levelReached = score.Item3;
                string levelName = score.Item4;
                string isDate = score.Item5;
                string elapsedGameTime = score.Item6;

                // Format each score entry
                textBoxHighscore.AppendText(string.Format("{0, -5} {1, -10} {2, -10} {3, -10}\r\n", lineNumber, playerName, totalRounds, isDate));
                lineNumber++;

                listLineNumber.Add(lineNumber);  // Add line number for tracking
                listTotalRounds.Add(totalRounds);  // Track total rounds
            }
        }

        /// <summary>
        /// Reads the scores from the "setters.txt" file and returns a list of score data.
        /// </summary>
        /// <returns>A list of player scores with rank, name, rounds, date, and elapsed time.</returns>
        private List<(string, int, int, string, string, string)> ReadScoresFromFile()
        {
            // Get file path for the score data
            string file = Path.Combine(InitializeRootPath(), "sounds", "setters.txt");
            List<(string, int, int, string, string, string)> scoresList = new List<(string, int, int, string, string, string)>();

            try
            {
                using (StreamReader getHighscore = new StreamReader(file))
                {
                    string? line;
                    // Read each line of the file
                    while ((line = getHighscore.ReadLine()) != null)
                    {
                        string[] parts = line.Split(',');  // Split each line by commas
                        if (parts.Length >= 6)
                        {
                            // Parse and add valid score entries
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while reading scores: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return scoresList;
        }

        /// <summary>
        /// Sorts the list of scores by player score and elapsed time, and returns the top 8 scores.
        /// </summary>
        /// <returns>A sorted list of the top 8 high scores.</returns>
        private List<(string, int, int, string, string, string)> SortBestScores()
        {
            List<(string, int, int, string, string, string)> bestScores = new List<(string, int, int, string, string, string)>();

            try
            {
                // Get all scores from file
                List<(string, int, int, string, string, string)> scoresList = ReadScoresFromFile();

                // Sort by player score (descending) and then by game time (ascending)
                bestScores = scoresList.OrderByDescending(x => x.Item2)  // Sort by score
                                       .ThenBy(x => TimeSpan.Parse(x.Item6))  // Then sort by elapsed game time
                                       .Take(8)  // Take the top 8 entries
                                       .ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"An error occurred while getting top scores: {e.Message}");
            }

            return bestScores;
        }
        #endregion

        #region Processing Score
        /// <summary>
        /// Initializes a new TaskCompletionSource for handling player name input.
        /// </summary>
        private TaskCompletionSource<string> playerNameTcs = new TaskCompletionSource<string>();

        /// <summary>
        /// Verifies the player's rank by checking if their score qualifies for the top scores.
        /// If the score qualifies, it saves the score and updates the high scores.
        /// </summary>
        /// <param name="totalRounds">Total rounds completed by the player.</param>
        /// <param name="levelReached">The highest level the player reached during the game.</param>
        /// <param name="levelName">The name of the level reached by the player.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task VerifyPlayerRank(int totalRounds, int levelReached, string levelName)
        {
            var highScores = SortBestScores()
                .Select(score => (score.Item1, score.Item2, score.Item3, score.Item4, score.Item5, score.Item6))
                .ToList();

            string elapsedGameTime = InitializeGameStopwatch();
            string currentDate = DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            // Check if the player's score qualifies for the top scores
            if (QualifiesForTopScores(highScores, totalRounds, elapsedGameTime))
            {
                string placeholderText = string.Empty; // Placeholder for player name until validated
                highScores.Add((placeholderText, totalRounds, levelReached, levelName.Trim(), currentDate, elapsedGameTime));

                highScores = highScores
                    .OrderByDescending(score => score.Item2)
                    .ThenBy(score => TimeSpan.Parse(score.Item6))
                    .Take(8)
                    .ToList();

                // Determine the player's rank based on their score
                int playerRank = highScores.FindIndex(score => score.Item2 == totalRounds && score.Item6 == elapsedGameTime) + 1;

                IsHighscoreText(totalRounds, playerRank); // Update the highscore text

                // Wait for the player to input their name
                string playerName = await PlayerName();

                // Replace the placeholder with the player's actual name
                highScores.Clear();
                highScores.Add((playerName, totalRounds, levelReached, levelName.Trim(), currentDate, elapsedGameTime));

                // Save the updated score to the file
                SaveScoreToFile(highScores);
                Debug.WriteLine($"Game data saved: {playerName}, {totalRounds}, {levelReached}, {levelName.Trim()}, {currentDate}, {elapsedGameTime}");

                // Hide the player name input and the enter button, then show the retry button
                textBoxInputName.Visible = false;
                textBoxInputName.Enabled = false;
                buttonEnter.Visible = false;
                buttonEnter.Enabled = false;
                buttonRetry.Enabled = true;
                buttonRetry.Visible = true;
            }
            else
            {
                // Display current high scores in the textbox
                textBoxHighscore.Clear(); // Clear any existing text
                TextBoxHighscores();

                IsNotHighscoreText(totalRounds); // Update the text to indicate the player's score is not a high score

                buttonRetry.Enabled = true;
                buttonRetry.Visible = true;
            }

            // Update the highscore display
            TextBoxHighscores();
        }

        /// <summary>
        /// Checks if the player's score qualifies for the top scores list based on the number of rounds and the elapsed game time.
        /// </summary>
        /// <param name="highScores">A list of high scores.</param>
        /// <param name="totalRounds">The total rounds completed by the player.</param>
        /// <param name="elapsedGameTime">The elapsed game time for the player.</param>
        /// <returns>True if the score qualifies for the top scores list, false otherwise.</returns>
        private bool QualifiesForTopScores(List<(string, int, int, string, string, string)> highScores, int totalRounds, string elapsedGameTime)
        {
            return highScores.Count < 8 && counter_rounds > 0 ||
                    highScores.Any(score => score.Item2 < totalRounds ||
                    (score.Item2 == totalRounds && TimeSpan.Parse(score.Item6) > TimeSpan.Parse(elapsedGameTime)));
        }

        /// <summary>
        /// Sets the display for the text box showing the results if the player's score is in the top scores list.
        /// </summary>
        /// <param name="totalRounds">The total rounds completed by the player.</param>
        /// <param name="playerRank">The rank of the player in the high score list.</param>
        private void IsHighscoreText(int totalRounds, int playerRank)
        {
            TextBoxHighscores();
            textBoxShowResults.Visible = true;
            textBoxShowResults.Text = $"Your score:\r\n{totalRounds} sequences\r\nYour rank:\r\n#{playerRank}";
        }

        /// <summary>
        /// Sets the display for the text box showing the results if the player's score is NOT in the top scores list.
        /// </summary>
        /// <param name="totalRounds">The total rounds completed by the player.</param>
        private void IsNotHighscoreText(int totalRounds)
        {
            TextBoxHighscores();
            textBoxShowResults.Visible = true;
            textBoxShowResults.Text = $"\r\nYour score:\r\n{totalRounds} sequences";
        }

        /// <summary>
        /// Saves the player's score to a file. If the number of scores exceeds 15, the lowest score is replaced with the new score.
        /// </summary>
        /// <param name="highScores">The list of high scores to be saved.</param>
        private void SaveScoreToFile(List<(string, int, int, string, string, string)> highScores)
        {
            string rootPath = InitializeRootPath(); // Construct the file path using RootPath
            string file = Path.Combine(rootPath, "sounds", "setters.txt");
            string existingContent = File.Exists(file) ? File.ReadAllText(file) : string.Empty; // Read the existing content of the file

            List<(string, int, int, string, string, string)> currentScores = existingContent
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Split(','))
                .Select(parts => (parts[0], int.Parse(parts[1]), int.Parse(parts[2]), parts[3], parts[4], parts[5]))
                .ToList();

            if (currentScores.Count < 15)
            {
                currentScores.AddRange(highScores);
                currentScores = currentScores
                    .OrderByDescending(score => score.Item2)
                    .ThenBy(score => TimeSpan.Parse(score.Item6))
                    .Take(15)
                    .ToList();

                // Save updated scores to the file
                using (StreamWriter saveScore = new StreamWriter(file, false))
                {
                    foreach (var element in currentScores)
                    {
                        saveScore.WriteLine($"{element.Item1},{element.Item2},{element.Item3},{element.Item4},{element.Item5},{element.Item6}");
                    }
                    WriteToCopies(); // Save backup copies of the score file
                }
            }
            else
            {
                // Replace the lowest score if there are more than 15 scores
                var allScores = currentScores.Concat(highScores).ToList();
                var lowestScore = allScores
                    .OrderBy(score => score.Item2)
                    .ThenByDescending(score => TimeSpan.Parse(score.Item6))
                    .First();

                if (highScores.Any(newScore => newScore.Item2 > lowestScore.Item2))
                {
                    var updatedScores = currentScores
                        .Where(score => score != lowestScore)
                        .Concat(highScores)
                        .OrderByDescending(score => score.Item2)
                        .ThenBy(score => TimeSpan.Parse(score.Item6))
                        .Take(15)
                        .ToList();

                    // Save the updated scores to the file
                    using (StreamWriter saveScore = new StreamWriter(file, false))
                    {
                        foreach (var element in updatedScores)
                        {
                            saveScore.WriteLine($"{element.Item1},{element.Item2},{element.Item3},{element.Item4},{element.Item5},{element.Item6}");
                        }
                        Debug.WriteLine("line replaced in save file");
                        WriteToCopies(); // Save backup copies of the score file
                    }
                }
            }
        }

        /// <summary>
        /// Writes a copy of the score file to backup directories for redundancy.
        /// </summary>
        public void WriteToCopies()
        {
            string rootPath = InitializeRootPath(); // Construct the file path using RootPath

            string file = Path.Combine(rootPath, "sounds", "setters.txt");

            // Copy the file to another directory
            string copyToDir = Path.Combine(rootPath);
            Directory.CreateDirectory(copyToDir); // Ensure the directory exists
            string copyFile = Path.Combine(copyToDir, "higscores.txt");
            string copyFile2 = Path.Combine(copyToDir, "BackUp", "higscores.txt");

            // Copy the file and overwrite if it exists
            File.Copy(file, copyFile, true);
            File.Copy(file, copyFile2, true);
        }
        #endregion
    }
}