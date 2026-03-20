using Simon_Says.Managers;
using System.Diagnostics;
using System.Globalization;

namespace KeepYourFocus
{
    /// <summary>
    /// Main form and game orchestrator for the KeepYourFocus Simon Says-style memory game.
    /// Coordinates game flow between the computer's sequence display and the player's input,
    /// delegating tile, sound, and score management to dedicated manager classes.
    /// </summary>
    public partial class Focus : Form
    {
        #region === Manager instances ===
        private readonly SoundManager soundManager;
        private readonly TileManager tileManager;
        private readonly ScoreManager scoreManager;
        #endregion

        #region === Game state: ordered sequences for computer and player ===
        public List<string> correctOrder = new List<string>();
        public readonly List<string> playerOrder = new List<string>();
        public readonly List<string> previousTiles = new List<string>();

        public readonly Random rnd = new Random();
        public readonly Stopwatch gameStopwatch = new Stopwatch();
        #endregion 

        #region === GameVariables_Properties === 
        public bool computer = false;       // True when the computer is playing (blocks player input)
        public bool startButton = true;     // True when the start button can be clicked
        public bool nextRound = false;      // True during the transition to the next round
        public bool levelUp = false;        // True when a level-up is in progress
        public bool gameTime = false;       // True while the game stopwatch should be running

        bool isComputerTurn = false;        // Flag for computer's turn phase
        bool isPlayerTurn = false;          // Flag for player's turn phase
        bool isSetCounters = false;         // Flag for counter-update phase
        bool isDisplaySequence = false;     // Flag for sequence-display phase
        bool isHardLevel = false;           // Flag for hard difficulty mode
        bool actionTaken = false;           // Prevents duplicate difficulty actions per turn
        #endregion

        #region === Counters === 
        public int counterSequences = 1;    // Current sequence number within the round
        public int counterLevels = 1;       // Current difficulty level (1-based)
        public int counterRounds = 0;       // Total completed rounds (lifetime)
        public int setSequences = 6;        // Number of sequences per round (set by difficulty)
        #endregion

        /// <summary>Async completion source for awaiting the player's name input after game over.</summary>
        public TaskCompletionSource<string> playerNameTcs = new TaskCompletionSource<string>();

        /// <summary>Convenience property returning all four game PictureBoxes as an array.</summary>
        private PictureBox[] PictureBoxes => new[] { pictureBox1, pictureBox2, pictureBox3, pictureBox4 };

        #region === Constructor === 
        /// <summary>
        /// Initializes the form, creates manager instances, sets up the UI layout,
        /// displays the welcome message, and plays the startup sound.
        /// </summary>
        public Focus()
        {
            InitializeComponent();

            // Initialize managers
            tileManager = new TileManager(PictureBoxes);
            soundManager = new SoundManager();
            scoreManager = new ScoreManager();

            // Initialize Stopwatch for gametime
            gameStopwatch = new Stopwatch();

            // Welcome MessageBox
            InitializeWelcomeMessageBox();

            // LinkLabels GitHub and Email
            InitializeLinkLabels();

            // Align richTextBoxes
            AlignTextButtonBoxesCenter();

            // Display highscore at start
            TextBoxHighscores();

            // Use initial dictionary for start setup
            tileManager.InitialDictionaryOfTilesAtStart(PictureBoxes, PlayersTurn);

            // Register KeyDown handler once (prevents handler stacking)
            textBoxInputName.KeyDown += TextBoxInputName_KeyDown;

            // Play startup sound
            soundManager.PlayStartup();
        }
        #endregion

        // Methods for Initializations: WelcomeMessageBox, StartGame, Stopwatch, LinkLabels and Alignments
        #region === Initialisations === 
        /// <summary>
        /// Displays the welcome message box with game instructions and contact information.
        /// </summary>
        public static void InitializeWelcomeMessageBox()
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

        /// <summary>
        /// Resets the game state, hides non-game UI elements, enables tile interaction,
        /// starts the stopwatch, and begins the first computer turn.
        /// </summary>
        public void InitializeStartGame()
        {
            soundManager.PlayButtonClick();

            gameTime = true;
            startButton = false;
            computer = true;

            // Back to start setup tiles
            tileManager.InitialDictionaryOfTilesAtStart(PictureBoxes, PlayersTurn);

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

        /// <summary>
        /// Starts or stops the game stopwatch based on the <see cref="gameTime"/> flag.
        /// When stopping, returns the elapsed time formatted as mm:ss.
        /// </summary>
        /// <returns>The formatted elapsed time when stopping, or an empty string when starting.</returns>
        public string InitializeGameStopwatch()
        {
            if (gameTime)
            {
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

        /// <summary>
        /// Configures the GitHub and email link labels with their URLs and mailto links.
        /// </summary>
        public void InitializeLinkLabels()
        {
            linkLabelGitHub.Text = "https://github.com/Peanutsch/KeepYourFocus.git";
            linkLabelEmail.Text = "peanutsch@duck.com";

            linkLabelGitHub.Links.Add(0, linkLabelGitHub.Text.Length, "https://github.com/Peanutsch/KeepYourFocus.git");
            linkLabelEmail.Links.Add(0, linkLabelEmail.Text.Length, "mailto:peanutsch@duck.com");
        }

        /// <summary>
        /// Centers text alignment on all RichTextBox and Button controls,
        /// and hides the name input field at startup.
        /// </summary>
        public void AlignTextButtonBoxesCenter()
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

        /// <summary>
        /// Reads the selected difficulty from the CheckedListBox and returns
        /// the corresponding number of sequences per round.
        /// </summary>
        /// <returns>The number of sequences per round (4, 6, or 10). Defaults to 6.</returns>
        public int GetSelectedSequences()
        {
            int setSequences = 6;

            if (checkedListBoxDifficulty.CheckedItems.Count > 0)
            {
                string? selectedDifficulty = checkedListBoxDifficulty.CheckedItems[0]?.ToString();
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
                    default:
                        setSequences = 6;
                        break;
                }
            }

            return setSequences;
        }
        #endregion

        // Click Handlers for start, retry, enter buttons, linklabels and difficulty settings
        #region === Click and Key Handlers === 
        /// <summary>
        /// Handles the Start button click. Starts a new game if the start button is active.
        /// </summary>
        public void InitializeButtonStart_Click(object sender, EventArgs e)
        {
            if (!startButton)
                return;

            InitializeStartGame();
        }

        /// <summary>
        /// Handles the Retry button click. Resets the board tiles and starts a new game
        /// after a short delay.
        /// </summary>
        public async void InitializeButtonRetry_Click(object sender, EventArgs e)
        {
            await Task.Delay(500);

            buttonRetry.Enabled = true;
            buttonRetry.Visible = true;

            linkLabelGitHub.Visible = true;
            linkLabelGitHub.Enabled = true;
            linkLabelEmail.Visible = true;
            linkLabelEmail.Enabled = true;

            tileManager.InitialDictionaryOfTilesAtStart(PictureBoxes, PlayersTurn);
            InitializeStartGame();
        }

        /// <summary>
        /// Handles the Enter button click to submit the player's name after game over.
        /// </summary>
        public void InitializeButtonEnter_Click(object sender, EventArgs e)
        {
            string playerName = ProcessInputName();
            playerNameTcs.TrySetResult(playerName);
            textBoxShowResults.DeselectAll();
            Debug.WriteLine("playerName entered by buttonEnter");
        }

        /// <summary>
        /// Handles KeyDown on the name input text box so pressing Enter
        /// submits the player's name (same as clicking the Enter button).
        /// Registered once in the constructor to prevent handler stacking.
        /// </summary>
        private void TextBoxInputName_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                string playerName = ProcessInputName();
                playerNameTcs.TrySetResult(playerName);
                textBoxShowResults.DeselectAll();
                Debug.WriteLine("playerName entered by Keys.Enter");
            }
        }

        /// <summary>
        /// Opens the GitHub repository URL when the GitHub link label is clicked.
        /// </summary>
        public void LinkLabelGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs? e)
        {
            if (e?.Link?.LinkData != null)
            {
                string? url = e.Link.LinkData.ToString();
                if (!string.IsNullOrEmpty(url))
                {
                    OpenLink(url);
                    return;
                }
            }

            Debug.WriteLine("Warning: LinkData is null or event args missing.");
            MessageBox.Show("Error: Link data is missing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Opens the default email client when the email link label is clicked.
        /// </summary>
        public void LinkLabelEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e?.Link?.LinkData != null)
            {
                string? url = e.Link.LinkData.ToString();
                if (!string.IsNullOrEmpty(url))
                {
                    OpenLink(url);
                    return;
                }
            }

            Debug.WriteLine("Warning: LinkData is null or event args missing.");
            MessageBox.Show("Error: Link data is missing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Handles difficulty selection changes from the combo box control.
        /// Maps each index to a sequences-per-round value.
        /// </summary>
        public void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            if (comboBox.SelectedIndex == 0)
            {
                return;
            }

            int setSequences;
            switch (comboBox.SelectedIndex)
            {
                case 1:
                    setSequences = 6;
                    break;
                case 2:
                    setSequences = 4;
                    break;
                case 3:
                    setSequences = 10;
                    break;
                case 4:
                    setSequences = int.MaxValue;
                    break;
                default:
                    setSequences = 6;
                    break;
            }

            Debug.WriteLine($"Selected difficulty sequences per round: {setSequences}");
        }

        /// <summary>
        /// Handles difficulty selection changes from the checked list box.
        /// Reads the checked item text to determine sequences per round.
        /// </summary>
        public void checkedListBoxDifficulty_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckedListBox checkedListBox = (CheckedListBox)sender;

            int setSequences = 6;

            if (checkedListBox.CheckedItems.Count > 0)
            {
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
                        default:
                            setSequences = 6;
                            break;
                    }
                }
            }
            Debug.WriteLine($"Selected difficulty sequences per round: {setSequences}");
        }

        /// <summary>
        /// Ensures only one difficulty option can be checked at a time
        /// by unchecking all other items when a new item is checked.
        /// </summary>
        public void checkedListBoxDifficulty_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            CheckedListBox checkedListBox = (CheckedListBox)sender;

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

        /// <summary>
        /// Opens the specified URL in the system's default browser or email client.
        /// </summary>
        /// <param name="url">The URL or mailto link to open.</param>
        public static void OpenLink(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("Error: The URL is null or empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open link. Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        // Methods for managing difficulties and shuffling
        #region === Difficulties === 
        /// <summary>
        /// Shuffles the on-screen positions of tile PictureBoxes as a difficulty challenge.
        /// Applies different shuffle rules depending on whether it is the computer's display
        /// phase or the player's turn, with level-based probability.
        /// </summary>
        public async Task ShufflePictureBoxes()
        {
            // Case 1: Shuffle during sequence display (with transition sound and delay)
            if (counterLevels == 1 && rnd.Next(100) <= 100 && isDisplaySequence ||
                counterLevels >= 3 && rnd.Next(100) <= 75 && isDisplaySequence ||
                counterLevels >= 5 && rnd.Next(100) <= 85 && isDisplaySequence ||
                isHardLevel && rnd.Next(100) <= 85 && isDisplaySequence)
            {
                Debug.WriteLine($"Shuffle PictureBoxes Case 1: Shuffle after display sequence");

                await Task.Delay(500);
                soundManager.PlayTransition();
                tileManager.ShufflePositions();
                tileManager.RefreshAndRepositionPictureBoxes();
                await Task.Delay(500);
            }
            // Case 2: Shuffle during player's turn (instant, no sound)
            if (counterLevels >= 1 && rnd.Next(100) <= 100 && isPlayerTurn ||
                counterLevels >= 4 && rnd.Next(100) <= 75 && isPlayerTurn ||
                counterLevels >= 6 && rnd.Next(100) <= 85 && isPlayerTurn ||
                isHardLevel && rnd.Next(100) <= 85 && isDisplaySequence && isDisplaySequence)
            {
                Debug.WriteLine($"Shuffle PictureBoxes Case 2: Shuffle after player click");

                tileManager.ShufflePositions();
                tileManager.RefreshAndRepositionPictureBoxes();
            }
            actionTaken = true;
        }

        /// <summary>
        /// Displays a random misleading or encouraging label message near the tiles
        /// as a distraction challenge. The message content differs between the computer's
        /// turn and the player's turn, with level-based probability.
        /// </summary>
        /// <param name="iscomputerTurn">True for computer-turn messages, false for player-turn messages.</param>
        public async void DisplayLabelMessage(bool iscomputerTurn)
        {
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
                else
                {
                    labelText = new List<string>
                                                    {
                                                     "Click Here", "This Is NOT\nThe Correct tile!", "The computer\nIs Lying!",
                                                     "This Is\nThe One!", "Just Kidding!\nClick This One!", "This Is NOT\nThe Right Tile!",
                                                     "This Is\nThe Next\nOne!", "Now This One!", "This One!", "Over Here!"
                                                    };
                }

                int pickLabelIndex = rnd.Next(labels.Count);
                Label randomizedLabelClickHere = labels[pickLabelIndex];

                int pickLabelText = rnd.Next(labelText.Count);
                string randomizedText = labelText[pickLabelText];

                await Task.Delay(250);

                randomizedLabelClickHere.AutoSize = true;
                randomizedLabelClickHere.Text = randomizedText;
                randomizedLabelClickHere.Visible = true;
                await Task.Delay(750);
                randomizedLabelClickHere.Visible = false;
            }
        }
        #endregion

        // Methods for managing game flow: computer's turn, player's turn, display sequence, manage counters and levels, and verify actions
        #region === Game Flow Handlers === 
        /// <summary>
        /// Initiates the computer's turn: adds a random tile to the correct sequence
        /// and triggers the display of the full sequence.
        /// </summary>
        public void ComputersTurn()
        {
            textBoxShowResults.Visible = false;

            isComputerTurn = true;

            computer = true;
            correctOrder.Add(tileManager.ManageRandomizerTiles());
            UpdateTurn();
            DisplaySequence();

            isComputerTurn = false;
        }

        /// <summary>
        /// Visually plays back the correct sequence by highlighting each tile in order,
        /// with sound effects and timing delays. Checks for tile replacements and
        /// applies difficulty actions after the sequence is shown.
        /// </summary>
        public async void DisplaySequence()
        {
            actionTaken = false;

            // Check if any tiles should be replaced on the board before displaying
            (List<string> updatedCorrectOrder, bool replacementOccurred) = tileManager.ReplaceTileOnBoardAndInSequence(
                correctOrder, counterLevels, isHardLevel, isDisplaySequence, PlayersTurn);

            if (replacementOccurred)
            {
                correctOrder = updatedCorrectOrder;
            }

            isDisplaySequence = true;
            computer = true;

            Debug.WriteLine($"\nDisplay Sequence: {counterSequences}");
            Debug.WriteLine("correctOrder = " + string.Join(", ", correctOrder));

            await Task.Delay(500);

            foreach (string tile in correctOrder)
            {
                if (!tileManager.PictureBoxDictionary.TryGetValue(tile, out PictureBox? box) || box == null)
                    continue;

                await Task.Delay(500);

                soundManager.PlayTileSound(tile);

                tileManager.ManageHighlight(box, true);
                await Task.Delay(150);
                tileManager.ManageHighlight(box, false);
                await Task.Delay(50);
            }
            // Verify difficulty
            await ManageActions();

            await Task.Delay(500);

            computer = false;
            isDisplaySequence = false;

            UpdateTurn();
        }

        /// <summary>
        /// Handles the player's tile click. Validates each click against the correct
        /// sequence in order. Triggers game over on mismatch, or advances the round
        /// when the full sequence is matched.
        /// </summary>
        public async void PlayersTurn(object? sender, EventArgs e)
        {
            actionTaken = false;

            isPlayerTurn = true;

            // Block Player's clicks in computer's turn AND at start game before StartButton is clicked
            if (startButton || computer)
                return;

            if (sender is PictureBox clickedBox)
            {
                string tile = clickedBox.Tag?.ToString() ?? "";

                soundManager.PlayTileSound(tile);
                tileManager.ManageHighlight(clickedBox, true);

                playerOrder.Add(tile);

                Debug.WriteLine($"Player: [{tile}]");

                // Verify difficulty
                await ManageActions();

                // Verify each item with correctOrder
                for (int itemIndex = 0; itemIndex < playerOrder.Count; itemIndex++)
                {
                    // If any item doesn't match, trigger game over
                    if (playerOrder[itemIndex] != correctOrder[itemIndex])
                    {
                        await Task.Delay(100);
                        tileManager.ManageHighlight(clickedBox, false);
                        await Task.Delay(250);
                        TextBoxHighscores();
                        GameOver();

                        isPlayerTurn = false;

                        return;
                    }
                }
                if (playerOrder.Count == correctOrder.Count)
                {
                    // Player completed the sequence correctly, prepare for next round
                    await Task.Delay(100);
                    tileManager.ManageHighlight(clickedBox, false);
                    await Task.Delay(50);

                    ManageCountersAndLevels();

                    isPlayerTurn = false;
                }
                else
                {
                    // Player is correct so far but hasn't completed the sequence, just unhighlight the tile
                    await Task.Delay(100);
                    tileManager.ManageHighlight(clickedBox, false);
                    await Task.Delay(50);

                    isPlayerTurn = false;
                }
            }
            isPlayerTurn = false;
        }

        /// <summary>
        /// Advances counters and levels after the player completes a correct sequence.
        /// Plays the correct sound, updates UI, and starts the next computer turn.
        /// </summary>
        public async void ManageCountersAndLevels()
        {
            if (playerOrder.Count < correctOrder.Count)
                return;

            nextRound = true;

            // Block player's clicks
            computer = true;

            // Delay 250 ms between beepSound and correctSound
            await Task.Delay(250);
            soundManager.PlayCorrect();

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

        /// <summary>
        /// Updates sequence, level, and round counters. Triggers a level-up when the
        /// sequence limit is reached (below level 8), including clearing the sequence
        /// and applying difficulty actions.
        /// </summary>
        public async Task UpdateCounters()
        {
            setSequences = GetSelectedSequences();
            isSetCounters = true;

            // Level-up: reset sequence counter and advance to next level
            if (counterLevels < 8 && counterSequences == setSequences)
            {
                levelUp = true;
                correctOrder.Clear();
                playerOrder.Clear();
                counterSequences = 1;
                counterLevels++;
                counterRounds++;

                await ManageActions();
                UpdateTurn();

                levelUp = false;
            }
            else
            {
                // Level 8+: continuous play (HELLMODE), keep incrementing
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

        /// <summary>
        /// Dispatches difficulty-based actions (shuffling, tile replacement) depending on
        /// the current game phase. Ensures each phase only triggers one action per turn
        /// via the <see cref="actionTaken"/> flag.
        /// </summary>
        public async Task ManageActions()
        {
            Debug.WriteLine("[RUNNING ManageActions()]");

            setSequences = GetSelectedSequences();

            // Shuffle during computer's turn
            if (isComputerTurn && !actionTaken)
            {
                await ShufflePictureBoxes();
                actionTaken = true;
            }
            // Shuffle during player's turn
            if (isPlayerTurn && !actionTaken)
            {
                Debug.WriteLine("ManageActions> isPlayerTurn = true");
                await ShufflePictureBoxes();
                actionTaken = true;
            }
            // Shuffle during sequence display
            if (isDisplaySequence && !actionTaken)
            {
                Debug.WriteLine("ManageActions> isDisplaySequence = true");
                await ShufflePictureBoxes();
                actionTaken = true;
            }
            // Replace all tiles on level-up
            if (isSetCounters && !actionTaken)
            {
                tileManager.ReplaceAllTiles(PictureBoxes, counterLevels, levelUp, isHardLevel, isDisplaySequence, PlayersTurn);
                actionTaken = true;
            }
            // Hard mode: apply all difficulty challenges simultaneously
            if (setSequences == int.MaxValue && !actionTaken)
            {
                isHardLevel = true;
                await ShufflePictureBoxes();
                tileManager.ReplaceTileOnBoardAndInSequence(correctOrder, counterLevels, isHardLevel, isDisplaySequence, PlayersTurn);
                tileManager.ReplaceAllTiles(PictureBoxes, counterLevels, levelUp, isHardLevel, isDisplaySequence, PlayersTurn);
                actionTaken = true;
            }
        }
        #endregion

        // UI update methods
        #region === UI Updates ===
        /// <summary>
        /// Updates the sequence counter display with the current sequence number.
        /// </summary>
        public void UpdateSequence()
        {
            richTextBoxShowNumbersOfSequences.BackColor = Color.Yellow;
            richTextBoxShowNumbersOfSequences.Text = $"{new string(' ', 3)}Sequence of {counterSequences}";
        }

        /// <summary>
        /// Updates the turn indicator display based on the current game state.
        /// Shows "CORRECT" / "Level Up" / "Next Sequence" on round completion,
        /// "Running Sequence" during computer's turn, or "Player's Turn" otherwise.
        /// </summary>
        public async void UpdateTurn()
        {
            switch (computer, startButton, nextRound)
            {
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
                case (true, false, _):
                    richTextBoxTurn.BackColor = Color.Salmon;
                    richTextBoxTurn.Text = $"Running\n::\nSequence";
                    break;
                case (false, false, _):
                    richTextBoxTurn.BackColor = Color.Green;
                    richTextBoxTurn.Text = $"\nPlayer's Turn";
                    break;
            }
        }

        /// <summary>
        /// Updates the completed rounds counter display.
        /// </summary>
        public void UpdateRound()
        {
            richTextBoxShowRounds.BackColor = Color.LightSkyBlue;
            richTextBoxShowRounds.Text = $"{new string(' ', 4)}Completed: {counterRounds}";
        }

        /// <summary>
        /// Updates the level number, name, and color scheme based on the current level.
        /// Level names range from "EasyPeasy" (1) through "HELLMODE" (7+),
        /// with level 999 indicating game over.
        /// </summary>
        public void UpdateLevelName()
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

        // Methods for managing game over, processing player name, displaying highscores
        #region === Processing Game Over === 
        /// <summary>
        /// Handles the game-over state: disables tiles, stops the stopwatch,
        /// plays the wrong sound, saves the score, resets counters,
        /// and restores the start-screen layout.
        /// </summary>
        public async void GameOver()
        {
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

            soundManager.PlayWrong();

            // Save score
            await VerifyPlayerRank(counterRounds, counterLevels, richTextBoxShowLevelName.Text);

            // Reset UI to start screen
            correctOrder.Clear();

            // Clear player sequence and reset counters to initial state for new game
            playerOrder.Clear();

            // Set level to 999 to trigger game over display, then reset to 1 for new game
            counterLevels = 999;

            // Update UI to show game over state
            UpdateLevelName();

            // Delay before resetting to start screen
            tileManager.InitialDictionaryOfTilesAtStart(PictureBoxes, PlayersTurn);

            // Reset counters rounds and levels
            counterRounds = 0;
            counterLevels = 1;
        }

        /// <summary>
        /// Reads and validates the player's name from the input text box.
        /// Defaults to "ANONYMOUS" if the input is empty. Restores the start-screen
        /// UI elements after name submission.
        /// </summary>
        /// <returns>The sanitized, uppercased player name.</returns>
        public string ProcessInputName()
        {
            Debug.WriteLine("\nProcessInputName() start");
            string playerName = textBoxInputName.Text.ToUpper().Trim();

            if (string.IsNullOrWhiteSpace(playerName))
            {
                playerName = "ANONYMOUS";
                Debug.WriteLine($"Input playerName is empty. Forced playerName is {playerName}\n");
            }

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

        /// <summary>
        /// Displays the name input UI and asynchronously waits for the player
        /// to submit their name via the Enter button or Enter key.
        /// </summary>
        /// <returns>The player's submitted name.</returns>
        public async Task<string> PlayerName()
        {
            playerNameTcs = new TaskCompletionSource<string>();

            textBoxInputName.Clear();
            textBoxInputName.PlaceholderText = "YourNameHere";
            textBoxInputName.Visible = true;
            textBoxInputName.Enabled = true;
            buttonEnter.Visible = true;
            buttonEnter.Enabled = true;

            textBoxInputName.Focus();

            string playerName = await playerNameTcs.Task;

            return playerName;
        }

        /// <summary>
        /// Populates the highscore text box with a formatted leaderboard table
        /// showing rank, player name, completed sequences, and difficulty level.
        /// </summary>
        public void TextBoxHighscores()
        {
            List<(string, int, int, string, string, string, int)> topHighscores = scoreManager.SortBestScores();

            textBoxHighscore.Clear();
            textBoxHighscore.Visible = true;

            textBoxHighscore.Font = new Font("Courier New", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);

            textBoxHighscore.Text = "\r\n===HIGHSCORES===\r\n\r\n";
            textBoxHighscore.AppendText(string.Format("{0, -5} {1, -10} {2, -10} {3, -10}\r\n", "Rank", "Player", "Sequences", "Difficulty"));

            int lineNumber = 1;
            foreach (var score in topHighscores)
            {
                string playerName = score.Item1;
                int totalRounds = score.Item2;
                int difficultyLevelValue = score.Item7;

                string difficultyLevel = ScoreManager.DifficultyPriorities
                    .FirstOrDefault(x => x.Value == difficultyLevelValue).Key ?? "Unknown";

                textBoxHighscore.AppendText(string.Format("{0, -5} {1, -10} {2, -10} {3, -10}\r\n", lineNumber, playerName, totalRounds, difficultyLevel));
                lineNumber++;
            }
        }
        #endregion

        // Methods for managing score verification, saving scores to file, and displaying results based on player's rank
        #region === Processing Score === 
        /// <summary>
        /// Determines whether the player's score qualifies for the top 8 leaderboard.
        /// If qualified, prompts for the player's name and saves the score.
        /// If not, displays the score without saving.
        /// </summary>
        /// <param name="totalRounds">Total completed rounds this game.</param>
        /// <param name="levelReached">The highest level reached.</param>
        /// <param name="levelName">The display name of the highest level reached.</param>
        public async Task VerifyPlayerRank(int totalRounds, int levelReached, string levelName)
        {
            var highScores = scoreManager.SortBestScores()
                .Select(score => (score.Item1, score.Item2, score.Item3, score.Item4, score.Item5, score.Item6, score.Item7))
                .ToList();

            string elapsedGameTime = InitializeGameStopwatch();
            string currentDate = DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            if (this.checkedListBoxDifficulty.CheckedItems.Count > 0)
            {
                // Extract difficulty name (before the colon) and look up its priority value
                string? difficulty = this.checkedListBoxDifficulty.CheckedItems[0]?.ToString()?.Split(':')[0].Trim();
                if (ScoreManager.DifficultyPriorities.TryGetValue(difficulty!, out int difficultyLevel))
                {
                    if (ScoreManager.QualifiesForTopScores(highScores, totalRounds, elapsedGameTime, difficultyLevel))
                    {
                        // Temporarily add a placeholder entry to determine the player's rank
                        string placeholderText = string.Empty;
                        highScores.Add((placeholderText, totalRounds, levelReached, levelName.Trim(), currentDate, elapsedGameTime, difficultyLevel));

                        highScores = highScores
                            .OrderByDescending(score => score.Item2)
                            .ThenBy(score => TimeSpan.Parse(score.Item6))
                            .ThenBy(score => score.Item7)
                            .Take(8)
                            .ToList();

                        // Calculate the player's rank position in the sorted leaderboard
                        int playerRank = highScores.FindIndex(score => score.Item2 == totalRounds && score.Item6 == elapsedGameTime && score.Item7 == difficultyLevel) + 1;

                        IsHighscoreText(totalRounds, playerRank);

                        // Replace placeholder with actual player name and persist
                        string playerName = await PlayerName();
                        highScores.RemoveAll(score => score.Item1 == placeholderText);
                        highScores.Add((playerName, totalRounds, levelReached, levelName.Trim(), currentDate, elapsedGameTime, difficultyLevel));

                        ScoreManager.SaveScoreToFile(highScores);
                        Debug.WriteLine($"Game data saved: {playerName}, {totalRounds}, {levelReached}, {levelName.Trim()}, {currentDate}, {elapsedGameTime}, {difficultyLevel}");

                        textBoxInputName.Visible = false;
                        textBoxInputName.Enabled = false;
                        buttonEnter.Visible = false;
                        buttonEnter.Enabled = false;
                        buttonRetry.Enabled = true;
                        buttonRetry.Visible = true;
                    }
                    else
                    {
                        textBoxHighscore.Clear();
                        TextBoxHighscores();

                        IsNotHighscoreText(totalRounds);

                        buttonRetry.Enabled = true;
                        buttonRetry.Visible = true;
                    }
                    TextBoxHighscores();
                }
                else
                {
                    MessageBox.Show("Selected difficulty level is not recognized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No difficulty level selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Displays the player's score and rank when they qualify for the leaderboard.
        /// </summary>
        /// <param name="totalRounds">Total completed rounds.</param>
        /// <param name="playerRank">The player's rank position on the leaderboard.</param>
        public void IsHighscoreText(int totalRounds, int playerRank)
        {
            TextBoxHighscores();
            textBoxShowResults.Visible = true;
            textBoxShowResults.Text = $"Your score:\r\n{totalRounds} sequences\r\nYour rank:\r\n#{playerRank}";
        }

        /// <summary>
        /// Displays the player's score when they do not qualify for the leaderboard.
        /// </summary>
        /// <param name="totalRounds">Total completed rounds.</param>
        public void IsNotHighscoreText(int totalRounds)
        {
            TextBoxHighscores();
            textBoxShowResults.Visible = true;
            textBoxShowResults.Text = $"\r\nYour score:\r\n{totalRounds} sequences";
        }
        #endregion
    }
}