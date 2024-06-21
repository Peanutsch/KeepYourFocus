/*
 * Simon Says-like game with some level based enhancements difficulties
 * Each level has 6 sequences. After 6 succesful sequences:
 * Level++; Add 1 game enhancement; Clear correctOrder and playerOrder and start with new sequence = 1
 * From Level >= 6: no Clear correctOrder; sequences++ untill game over
 * More enhancements in progress
 */

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Media;

namespace KeepYourFocus
{
    public partial class PlayerField : Form
    {
        private readonly string pathRoot = Path.GetPathRoot(Application.ExecutablePath) ?? "";
        private readonly SoundPlayer redSound;
        private readonly SoundPlayer blueSound;
        private readonly SoundPlayer orangeSound;
        private readonly SoundPlayer greenSound;
        private readonly SoundPlayer transitionSound;
        private readonly SoundPlayer buttonClickSound;
        private readonly SoundPlayer wrongSound;
        private readonly SoundPlayer correctSound;
        private readonly SoundPlayer startupSound;
        
        private Label labelClickHere1 = new Label();
        private Label labelClickHere2 = new Label();
        private Label labelClickHere3 = new Label();
        private Label labelClickHere4 = new Label();

        private Random rnd = new Random();

        private bool Computer = false;
        private bool startButton = true;
        private bool nextRound = false;
        private bool levelUp = false;

        private int consecutiveCount = 0;
        private int counter_sequences = 1;
        private int counter_levels = 1;
        private int counter_rounds = 1;

        private Dictionary<string, PictureBox> pictureBoxDictionary = new Dictionary<string, PictureBox>();
        private List<string> correctOrder = new List<string>();
        private List<string> playerOrder = new List<string>();

        private string[] previousColors = new string[2];

        public PlayerField()
        {
            InitializeComponent();

            // Load soundfiles. For now 1 beep sound for all colors
            string soundPathbeep = Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\beep.wav");
            string soundPathTransition = Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\transistion.wav");
            string soundPathButtonClick = Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\buttonclick.wav");
            string soundPathWrong = Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\wrong.wav");
            string soundPathCorrect = Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\correct.wav");
            string soundPathStartupSound = Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\startsound.wav");

            // Initiaize SoundPlayers
            redSound = new SoundPlayer(soundPathbeep);      // Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\redSound.wav");
            blueSound = new SoundPlayer(soundPathbeep);     // Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\blueSound.wav");
            orangeSound = new SoundPlayer(soundPathbeep);   // Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\orangeSound.wav");
            greenSound = new SoundPlayer(soundPathbeep);    // Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\greenSound.wav");
            transitionSound = new SoundPlayer(soundPathTransition);
            buttonClickSound = new SoundPlayer(soundPathButtonClick);
            wrongSound = new SoundPlayer(soundPathWrong);
            correctSound = new SoundPlayer(soundPathCorrect);
            startupSound = new SoundPlayer(soundPathStartupSound);

            // Play startup sound
            startupSound.Play();

            SetupSquares();
        }

        // Click Event for Start Button at start
        private void BTN_Start_Click(object sender, EventArgs e)
        {
            if (!startButton)
                return;

            buttonClickSound.Play();

            startButton = false;
            Computer = true;
            counter_sequences = 1;

            UpdateSequence();
            UpdateRound();
            UpdateLevelName();

            ComputersTurn();
            SetStartButtonInvisible();
        }

        private void SetStartButtonGameOver()
        {
            BTN_Start.Visible = true;
            BTN_Start.Enabled = true;
            BTN_Start.BackColor = Color.DarkRed;
            BTN_Start.Cursor = Cursors.Hand;
            BTN_Start.Text = $"{new string(' ', 1)}Wrong\n Sequence";
            BTN_Start.FlatStyle = FlatStyle.Popup;
        }

        private void SetStartButtonInvisible()
        {
            BTN_Start.Visible = false;
            BTN_Start.Enabled = false;
        }

        private void SetupSquares()
        {
            if (pictureBoxDictionary.Count > 0)
            {
                RefreshAndRepositionPictureBoxes();
                return;
            }

            InitializePictureBox(pictureBoxRed, "Red", Path.Combine(pathRoot, @"C#\KeepYourFocus\png\red_square512.png"));
            InitializePictureBox(pictureBoxBlue, "Blue", Path.Combine(pathRoot, @"C#\KeepYourFocus\png\blue_square512.png"));
            InitializePictureBox(pictureBoxOrange, "Orange", Path.Combine(pathRoot, @"C#\KeepYourFocus\png\orange_square512.png"));
            InitializePictureBox(pictureBoxGreen, "Green", Path.Combine(pathRoot, @"C#\KeepYourFocus\png\green_square512.png"));
        }

        private void InitializePictureBox(PictureBox pictureBox, string color, string imagePath)
        {
            pictureBox.Image = Image.FromFile(imagePath);
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.BackColor = Color.Transparent;
            pictureBox.Cursor = Cursors.Hand;
            pictureBox.Tag = color;
            pictureBox.Click += PlayersTurn;
            pictureBoxDictionary.Add(color, pictureBox);
        }

        private void RandomizePictureBoxes()
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
                pictureBox.Location = GetFixedPosition(index);
                index++;
            }
        }

        // Define fixed positions for PictureBoxes
        private static Point GetFixedPosition(int index)
        {
            // Define fixed positions based on the index
            switch (index)
            {
                case 0:
                    return new Point(14, 14);
                case 1:
                    return new Point(321, 14);
                case 2:
                    return new Point(14, 316);
                case 3:
                    return new Point(321, 316);
                default:
                    return Point.Empty; // Default position if index is out of range
            }
        }

        private void RefreshAndRepositionPictureBoxes()
        {
            var fixedPositions = new List<Point>{
                                                    new Point(14, 14), new Point(321, 14),
                                                    new Point(14, 316), new Point(321, 316)
                                                };

            var shuffledPictureBoxes = pictureBoxDictionary.Values.OrderBy(x => rnd.Next()).ToList();

            for (int i = 0; i < fixedPositions.Count; i++)
            {
                shuffledPictureBoxes[i].Location = fixedPositions[i];
                shuffledPictureBoxes[i].Visible = true;
            }
        }

        private async void ComputersTurn()
        {
            correctOrder.Add(Randomizer());
            UpdateTurn(); // case Computer's Turn


            Debug.WriteLine($"counter_levels: {counter_levels} counter_rounds: {counter_rounds} counter_sequences: {counter_sequences}");

            await Task.Delay(1000); // Delay 1000 ms before display Computer's Sequence

            DisplaySequence();
        }

        private async void DisplaySequence()
        {
            Debug.WriteLine("\nComputer's Sequence");
            Computer = true;

            foreach (var color in correctOrder)
            {
                var box = pictureBoxDictionary[color];
                if (box == null)
                    continue;

                await Task.Delay(500); // Delay 500 ms before start highlights and beepSound

                PlaySound(color);

                ApplyHighlight(box);
                await Task.Delay(100);
                RemoveHighlight(box);
                await Task.Delay(50);

                Debug.WriteLine($"Color: [{color}]");
            }

            // Check difficulty
            Debug.WriteLine("Check Enhancements Computer's Turn");
            Enhancements();

            await Task.Delay(1000); // Delay 1000 ms before calling PlayersTurn()

            Computer = false;
            UpdateTurn(); // case Player's Turn
        }

        private async void PlayersTurn(object? sender, EventArgs e)
        {
            if (startButton || Computer)
                return;

            if (sender is PictureBox clickedBox)
            {
                string color = clickedBox.Tag?.ToString() ?? "";

                // Debug.WriteLine($"Player: {color}");

                PlaySound(color);
                ApplyHighlight(clickedBox);

                playerOrder.Add(color);

                // Check difficulty
                Debug.WriteLine("Check Enhancements Player's Turn");
                Enhancements();

                // Verify each player's click against the correctOrder
                for (int i = 0; i < playerOrder.Count; i++)
                {
                    if (playerOrder[i] != correctOrder[i])
                    {
                        await Task.Delay(100);
                        RemoveHighlight(clickedBox);
                        await Task.Delay(250); // Delay to provide feedback before game over
                        GameOver();
                        return;
                    }
                }

                if (playerOrder.Count == correctOrder.Count)
                {
                    await Task.Delay(100);
                    RemoveHighlight(clickedBox);
                    await Task.Delay(50);
                    VerifyAndManageCountersAndLevels();
                }
                else
                {
                    await Task.Delay(100);
                    RemoveHighlight(clickedBox);
                    await Task.Delay(50);
                }
            }
        }

        private async void VerifyAndManageCountersAndLevels()
        {
            if (playerOrder.Count < correctOrder.Count)
                return;

            // block player's clicks
            Computer = true;
            nextRound = true;

            // Delay 250 ms between beepSound and correctSound
            await Task.Delay(250); 

            correctSound.Play();

            if (counter_sequences == 6 && counter_levels < 6) 
            {
                levelUp = true;
                
                correctOrder.Clear();
                playerOrder.Clear();
                counter_sequences = 1; // Reset sequence to 1
                counter_levels++;
                counter_rounds++;

                UpdateTurn();

                levelUp = false;
            }
            else if (counter_levels >= 6) 
            {
                Debug.WriteLine("Level >= 6: no correctOrder.Clear(). Every sequence +1");

                levelUp = true;
                counter_sequences++;
                counter_rounds++;
                UpdateTurn();
                levelUp = false;
            }
            else 
            {
                counter_sequences++;
                counter_rounds++;
            }

            UpdateTurn();
            UpdateSequence();
            UpdateRound();
            UpdateLevelName();

            await Task.Delay(3000);

            playerOrder.Clear();

            nextRound = false;

            ComputersTurn();
        }

        // Method to set difficulties. 2 cases: Computers turn and Players turn
        private void Enhancements()
        {
            switch (Computer)
            {
                // Computers Turn //
                case (true):
                    ComputersLabel();
                    ShuffleAfterDisplaySequence();
                    break;

                // Players Turn //
                case (false):
                    PlayersLabel();
                    ShuffleAfterPlayersClick();
                    break;
            }
        }

        private async void ComputersLabel()
        {
            /*
             * Show label with text in Computer's turn, like 'Click Here', 'This is not color X', 'Just kidding. Click this one!' at random
             * All code in switch/case or all enhancements in own methods and call them in switch/case?
             */

            if (counter_levels >= 1 && rnd.Next(100) <= 100 && correctOrder.Count != correctOrder.Count - 1)
            {
                List<Label> labels = new List<Label>
                                            { LabelClickHere1, LabelClickHere2,
                                              LabelClickHere3, LabelClickHere4 };

                List<string> labelText = new List<string>
                                                { "Click Here", "Start Here!", "Start With\nthis One!" };
                // Randomizer Label
                int pickLabelIndex = rnd.Next(labels.Count);

                Label randomizedLabelClickHere = labels[pickLabelIndex];

                Debug.WriteLine($"Randomized label: index {pickLabelIndex} > {randomizedLabelClickHere}");

                // Randomizer labelText
                int pickLabelText = rnd.Next(labelText.Count);
                string randomizedText = labelText[pickLabelText];

                Debug.WriteLine($"Randomized labelText: {randomizedText}");

                await Task.Delay(250);

                randomizedLabelClickHere.Location = new Point(
                                        pictureBoxRed.Width / 2 - randomizedLabelClickHere.Width / 2,
                                        pictureBoxRed.Height / 2 - randomizedLabelClickHere.Height / 2);
                randomizedLabelClickHere.Text = randomizedText;
                randomizedLabelClickHere.AutoSize = true;
                randomizedLabelClickHere.Visible = true;
                await Task.Delay(750);
                randomizedLabelClickHere.Visible = false;
            }
        }

        private async void ShuffleAfterDisplaySequence()
        {
            if (counter_levels == 2 && rnd.Next(100) <= 55 ||
                counter_levels >= 3 && rnd.Next(100) <= 75 ||
                counter_levels >= 6)
            {
                Debug.WriteLine($"Enhancements case 1: Shuffle after Display Sequence");

                await Task.Delay(250); // Delay 250 ms for space between colorSound and transitionSound
                transitionSound.Play();
                RandomizePictureBoxes();
                RefreshAndRepositionPictureBoxes();
                await Task.Delay(1000);
            }
        }

        private async void PlayersLabel()
        {
            /*
             * Show label with text in Player's's turn, like 'Click Here', 'This is not color X', 'Computer is lying! Click this one!' at random
             * All code in switch/case or all enhancements in own methods and call them in switch/case?
             */
            if (counter_levels >= 1 && rnd.Next(100) <= 100 && playerOrder.Count != correctOrder.Count)
            {
                List<Label> labels = new List<Label>
                                            { LabelClickHere1, LabelClickHere2,
                                              LabelClickHere3, LabelClickHere4 };
                List<string> labelText = new List<string>
                                                { "Click Here", "This Is NOT\nThe Correct color!", "The Computer\nIs Lying!",
                                                  "This Is\nThe One!", "Just Kidding!\nClick This One!", "This Is NOT\nThe Right Color!",
                                                  "This Is\nThe Next\nOne!", "Now This One!" };

                // Randomizer Label
                int pickLabelIndex = rnd.Next(labels.Count);

                Label randomizedLabelClickHere = labels[pickLabelIndex];

                Debug.WriteLine($"Randomized label: index {pickLabelIndex} > {randomizedLabelClickHere}");

                // Randomizer labelText
                int pickLabelText = rnd.Next(labelText.Count);
                string randomizedText = labelText[pickLabelText];

                Debug.WriteLine($"Randomized labelText: {randomizedText}");

                await Task.Delay(250);
                randomizedLabelClickHere.Location = new Point(
                                        pictureBoxRed.Width / 2 - randomizedLabelClickHere.Width / 2,
                                        pictureBoxRed.Height / 2 - randomizedLabelClickHere.Height / 2);
                randomizedLabelClickHere.AutoSize = true;
                randomizedLabelClickHere.Text = randomizedText;
                randomizedLabelClickHere.Visible = true;
                await Task.Delay(750);
                randomizedLabelClickHere.Visible = false;
            }

        }

        private void ShuffleAfterPlayersClick()
        {
            if (counter_levels >= 3 && rnd.Next(100) <= 55 ||
                counter_levels >= 4 && rnd.Next(100) <= 75 ||
                counter_levels >= 6)
            {
                Debug.WriteLine($"Enhancements Case 2: Shuffle after player click");

                RandomizePictureBoxes();
                RefreshAndRepositionPictureBoxes();
            }
        }


        private void UpdatePictureBoxImage(PictureBox pictureBox, string imagePath)
        {
            if (File.Exists(imagePath))
            {
                pictureBox.Image = Image.FromFile(imagePath);
            }
            else
            {
                // Handle case where image file does not exist
                MessageBox.Show($"Image file '{imagePath}' not found.");
            }
        }

        private string GetImagePathForColor(string color)
        {
            switch (color)
            {
                case "CaribBlue":
                    return Path.Combine(pathRoot, @"C#\KeepYourFocus\png\caribBlue_square512.png");
                case "Grey":
                    return Path.Combine(pathRoot, @"C#\KeepYourFocus\png\grey_square512.png");
                case "Indigo":
                    return Path.Combine(pathRoot, @"C#\KeepYourFocus\png\indigo_square512.png");
                case "Maroon":
                    return Path.Combine(pathRoot, @"C#\KeepYourFocus\png\maroon_square512.png");
                case "Olive":
                    return Path.Combine(pathRoot, @"C#\KeepYourFocus\png\olive_square-512.png");
                default:
                    return ""; // Handle default case appropriately
            }
        }

        private void ApplyHighlight(PictureBox pictureBox)
        {
            if (pictureBox.InvokeRequired)
            {
                pictureBox.Invoke(new Action<PictureBox>(ApplyHighlight), pictureBox);
            }
            else
            {
                pictureBox.BorderStyle = BorderStyle.None;
                pictureBox.Padding = new Padding(2);
                pictureBox.BackColor = Color.White;
            }
        }

        private void RemoveHighlight(PictureBox pictureBox)
        {
            if (pictureBox.InvokeRequired)
            {
                pictureBox.Invoke(new Action<PictureBox>(RemoveHighlight), pictureBox);
            }
            else
            {
                pictureBox.Padding = new Padding(0);
                pictureBox.BackColor = Color.Transparent;
            }
        }

        private string Randomizer()
        {
            string newColor;

            if (consecutiveCount <= 2)
            {
                do
                {
                    newColor = pictureBoxDictionary.Keys.ElementAt(rnd.Next(pictureBoxDictionary.Count));
                }
                while (previousColors.Contains(newColor));

                previousColors[0] = previousColors[1];
                previousColors[1] = newColor;
                consecutiveCount++;
            }
            else
            {
                previousColors = new string[2];
                newColor = pictureBoxDictionary.Keys.ElementAt(rnd.Next(pictureBoxDictionary.Count));
                consecutiveCount = 1;
            }

            return newColor;
        }

        private void PlaySound(string color)
        {
            switch (color)
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
            switch (Computer, startButton, nextRound)
            {
                // Next Round or Level Up
                case (_, _, true):
                    if (levelUp)
                    {
                        Debug.WriteLine("UpdateTurn LevelUp");

                        richTextBoxTurn.BackColor = Color.LightGreen;
                        richTextBoxTurn.Text = $"\n{new string(' ', 7)}CORRECT";

                        await Task.Delay(1500);

                        richTextBoxTurn.Text = $"\n{new string(' ', 8)}Level  Up";
                        await Task.Delay(1000);
                        break;
                    }
                    else
                    {
                        Debug.WriteLine("UpdateTurn Correct");

                        richTextBoxTurn.BackColor = Color.LightGreen;
                        richTextBoxTurn.Text = $"\n{new string(' ', 7)}CORRECT";

                        await Task.Delay(1500);
                        richTextBoxTurn.Text = $"\n{new string(' ', 3)}Next Sequence";

                        await Task.Delay(1000);
                        break;
                    }
                // Computer's turn
                case (true, false, _):
                    richTextBoxTurn.BackColor = Color.Salmon;
                    richTextBoxTurn.Text = $"{new string(' ', 4)}Computer's Turn";
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
                    richTextBoxShowLevelName.Text = $"{new string(' ', 1)}HELL NO";
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

        private void GameOver()
        {
            Computer = false;
            startButton = true;
            correctOrder.Clear();
            playerOrder.Clear();

            wrongSound.Play();

            counter_levels = 999;

            UpdateLevelName();

            SetStartButtonGameOver();

            counter_rounds = 1;
            counter_levels = 1;
        }
    }
}