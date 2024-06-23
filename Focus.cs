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
        private readonly SoundPlayer caribBlueSound;
        private readonly SoundPlayer greySound;
        private readonly SoundPlayer indigoSound;
        private readonly SoundPlayer maroonSound;

        private readonly SoundPlayer transitionSound;
        private readonly SoundPlayer buttonClickSound;
        private readonly SoundPlayer wrongSound;
        private readonly SoundPlayer correctSound;
        private readonly SoundPlayer startupSound;
        
        private readonly Random rnd = new Random();

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
            string soundPathBeepALL = Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\beep.wav");
            /*
            string soundPathBeepRed = Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\redSound.wav");
            string soundPathBeepBlue = Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\blueSound.wav");
            string soundPathBeepOrange = Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\orangeSound.wav");
            string soundPathBeepGreen = Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\greenSound.wav");
            string soundPathBeepCaribBlue = Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\caribBlueSound.wav");
            string soundPathBeepGrey = Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\greySound.wav");
            string soundPathBeepIndigo = Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\indigoSound.wav");
            string soundPathBeepMaroon = Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\maroonSound.wav");
            */

            string soundPathTransition = Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\transistion.wav");
            string soundPathButtonClick = Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\buttonclick.wav");
            string soundPathWrong = Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\wrong.wav");
            string soundPathCorrect = Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\correct.wav");
            string soundPathStartupSound = Path.Combine(pathRoot, @"C#\KeepYourFocus\sounds\startsound.wav");

            // Initiaize SoundPlayers
            redSound = new SoundPlayer(soundPathBeepALL);
            blueSound = new SoundPlayer(soundPathBeepALL);
            orangeSound = new SoundPlayer(soundPathBeepALL);
            greenSound = new SoundPlayer(soundPathBeepALL);
            caribBlueSound = new SoundPlayer(soundPathBeepALL);
            greySound = new SoundPlayer(soundPathBeepALL);
            indigoSound = new SoundPlayer(soundPathBeepALL);
            maroonSound = new SoundPlayer(soundPathBeepALL);
            transitionSound = new SoundPlayer(soundPathTransition);
            buttonClickSound = new SoundPlayer(soundPathButtonClick);
            wrongSound = new SoundPlayer(soundPathWrong);
            correctSound = new SoundPlayer(soundPathCorrect);
            startupSound = new SoundPlayer(soundPathStartupSound);

            // Play startup sound
            startupSound.Play();
            FieldSetupAtStart();
        }

        // Click Event for Start Button at start
        private void startButtonClick(object sender, EventArgs e)
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

        private void SetGameOverButton()
        {
            startBTN.Visible = true;
            startBTN.Enabled = true;
            startBTN.BackColor = Color.DarkRed;
            startBTN.Cursor = Cursors.Hand;
            startBTN.Text = $"{new string(' ', 1)}Wrong\n Sequence";
            startBTN.FlatStyle = FlatStyle.Popup;
        }

        private void SetStartButtonInvisible()
        {
            startBTN.Visible = false;
            startBTN.Enabled = false;
        }

        private void FieldSetupAtStart()
        {
            if (pictureBoxDictionary.Count > 0)
            {
                RefreshAndRepositionPictureBoxes();
                return;
            }

            InitializePictureBox(pictureBox1, "Red", Path.Combine(pathRoot, @"C#\KeepYourFocus\png\red_square512.png"));
            InitializePictureBox(pictureBox2, "Blue", Path.Combine(pathRoot, @"C#\KeepYourFocus\png\blue_square512.png"));
            InitializePictureBox(pictureBox3, "Orange", Path.Combine(pathRoot, @"C#\KeepYourFocus\png\orange_square512.png"));
            InitializePictureBox(pictureBox4, "Green", Path.Combine(pathRoot, @"C#\KeepYourFocus\png\green_square512.png"));
        }

        private void InitializePictureBox(PictureBox pictureBox, string color, string imagePath)
        {
            try
            {
                pictureBox.Image = Image.FromFile(imagePath);
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox.BackColor = Color.Transparent;
                pictureBox.Cursor = Cursors.Hand;
                pictureBox.Tag = color;

                Debug.WriteLine("Remove previous attached event handler");
                pictureBox.Click -= PlayersTurn; // Remove any previous attachment

                Debug.WriteLine("Attach event handler");
                pictureBox.Click += PlayersTurn; // Attach event handler

                // Update the dictionary with the new PictureBox
                pictureBoxDictionary[color] = pictureBox;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing PictureBox for color {color}: {ex.Message}");
            }
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
            var fixedPositions = new List<Point>
                {
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
            Computer = true;
            correctOrder.Add(RandomizerNoMoreColorsTwiceInARow());
            UpdateTurn(); // case Computer's Turn

            await Task.Delay(1000); // Delay 1000 ms before display Computer's Sequence

            DisplaySequence();
        }

        private async void DisplaySequence()
        {
            Debug.WriteLine("\nSequence: ");
            Computer = true;

            foreach (var color in correctOrder)
            {
                var box = pictureBoxDictionary[color];
                if (box == null)
                    continue;

                await Task.Delay(500); // Delay 500 ms before start highlights and beepSound

                PlaySound(color);

                ApplyHighlight(box);
                await Task.Delay(150);
                RemoveHighlight(box);
                await Task.Delay(50);
                Debug.WriteLine($"Color: [{color}]");
            }
            // Check difficulty
            SetTurnActions();

            await Task.Delay(1000); // Delay 1000 ms before calling PlayersTurn()

            Computer = false;
            UpdateTurn(); // case Player's Turn
        }

        private async void PlayersTurn(object? sender, EventArgs e)
        {
            // Block Player's clicks in Computer's turn AND before BTN_Start is clicked
            if (startButton || Computer)
                return;

            if (sender is PictureBox clickedBox)
            {
                string color = clickedBox.Tag?.ToString() ?? "";

                PlaySound(color);
                ApplyHighlight(clickedBox);

                playerOrder.Add(color);

                // Check difficulty
                SetTurnActions();

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

            // Block player's clicks
            Computer = true;

            nextRound = true;

            // Delay 250 ms between beepSound and correctSound
            await Task.Delay(250); 
            correctSound.Play();

            SetCounters();
            UpdateSequence();
            UpdateRound();
            UpdateLevelName();
            UpdateTurn();

            await Task.Delay(3000);

            playerOrder.Clear();
            nextRound = false;
            ComputersTurn();
        }

        private void SetCounters()
        {
            switch (counter_sequences)
            {
                case (6) when counter_levels < 6:
                    levelUp = true;
                    correctOrder.Clear();
                    playerOrder.Clear();
                    counter_sequences = 1; // Reset sequence to 1
                    counter_levels++;
                    counter_rounds++;
                    UpdateTurn();
                    levelUp = false;
                    break;
                default:
                    if (counter_levels >= 6)
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

        // Method to set difficulties. 2 cases: Computers turn and Players turn
        private void SetTurnActions()
        {
            
            switch (Computer)
            {
                // Computers Turn //
                case true:
                    DisplayLabelMessage(true);
                    ShufflePictureBoxes();
                    //ShuffleAfterDisplaySequence();
                    break;

                // Players Turn //
                case false:
                    DisplayLabelMessage(false);
                    ShufflePictureBoxes();
                    //ShuffleAfterPlayersClick();
                    break;
            }
        }

        private async void ShufflePictureBoxes()
        {
            switch (Computer)
            {
                case (true):
                    if (counter_levels == 2 && rnd.Next(100) <= 55 ||
                        counter_levels >= 3 && rnd.Next(100) <= 85 ||
                        counter_levels >= 6)
                    {
                        Debug.WriteLine($"ShufflePictureBoxes Case 1: Shuffle after display sequence");

                        await Task.Delay(250); // Delay 250 ms for space between colorSound and transitionSound
                        transitionSound.Play();
                        RandomizePictureBoxes();
                        RefreshAndRepositionPictureBoxes();
                        await Task.Delay(1000);
                    }
                    break;
                case (false):
                    if (counter_levels >= 3 && rnd.Next(100) <= 55 ||
                        counter_levels >= 4 && rnd.Next(100) <= 85 ||
                        counter_levels >= 6)
                    {
                        Debug.WriteLine($"ShufflePictureBoxes Case 2: Shuffle after player click");

                        RandomizePictureBoxes();
                        RefreshAndRepositionPictureBoxes();
                    }
                    break;
            }
        }

        private Dictionary<string, string> ShuffleAllColorSquares()
        {
            string redSquare = Path.Combine(pathRoot, @"C#\KeepYourFocus\png\red_square512.png");
            string blueSquare = Path.Combine(pathRoot, @"C#\KeepYourFocus\png\blue_square512.png");
            string orangeSquare = Path.Combine(pathRoot, @"C#\KeepYourFocus\png\orange_square512.png");
            string greenSquare = Path.Combine(pathRoot, @"C#\KeepYourFocus\png\green_square512.png");
            string caribBlueSquare = Path.Combine(pathRoot, @"C#\KeepYourFocus\png\caribBlue_square512.png");
            string greySquare = Path.Combine(pathRoot, @"C#\KeepYourFocus\png\grey_square512.png");
            string indigoSquare = Path.Combine(pathRoot, @"C#\KeepYourFocus\png\indigo_square512.png");
            string maroonSquare = Path.Combine(pathRoot, @"C#\KeepYourFocus\png\maroon_square512.png");

            Dictionary<string, string> dictOfAllColorSquares = new Dictionary<string, string>()
            {
                {"Red", redSquare},
                {"Blue", blueSquare},
                {"Orange", orangeSquare},
                {"Green", greenSquare},
                {"CaribBlue", caribBlueSquare},
                {"Grey", greySquare},
                {"Indigo", indigoSquare},
                {"Maroon", maroonSquare }
            };

            List<KeyValuePair<string, string>> listToShuffle = dictOfAllColorSquares.ToList();

            int numberOfItems = listToShuffle.Count;
            while (numberOfItems > 1)
            {
                numberOfItems--;
                int randomIndex = rnd.Next(numberOfItems + 1);
                KeyValuePair<string, string> temp = listToShuffle[randomIndex];
                listToShuffle[randomIndex] = listToShuffle[numberOfItems];
                listToShuffle[numberOfItems] = temp;
            }

            Dictionary<string, string> shuffleAllColorSquares = listToShuffle.ToDictionary(kv => kv.Key, kv => kv.Value);

            return shuffleAllColorSquares;
        }

        private async void CheckChangeColors()
        {
            if (counter_levels == 2 && rnd.Next(100) <= 100 ||
                counter_levels >= 3 && rnd.Next(100) <= 100 ||
                counter_levels >= 6)
            {

                Dictionary<string, string> shuffledColourSquares = ShuffleAllColorSquares();

                // Ensure that we have enough colors to assign
                if (shuffledColourSquares.Count >= 4)
                {
                    // Retrieve the first 4 key-value pairs from shuffledColourSquares
                    KeyValuePair<string, string> kvp1 = shuffledColourSquares.ElementAt(0);
                    KeyValuePair<string, string> kvp2 = shuffledColourSquares.ElementAt(1);
                    KeyValuePair<string, string> kvp3 = shuffledColourSquares.ElementAt(2);
                    KeyValuePair<string, string> kvp4 = shuffledColourSquares.ElementAt(3);

                    try
                    {
                        // Clear the dictionary and add the new colors
                        pictureBoxDictionary.Clear();

                        InitializePictureBox(pictureBox1, kvp1.Key, kvp1.Value);
                        InitializePictureBox(pictureBox2, kvp2.Key, kvp2.Value);
                        InitializePictureBox(pictureBox3, kvp3.Key, kvp3.Value);
                        InitializePictureBox(pictureBox4, kvp4.Key, kvp4.Value);

                        await Task.Delay(250); // Delay 250 ms for space between colorSound and transitionSound
                        transitionSound.Play();

                        //RefreshAndRepositionPictureBoxes();
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
                    Debug.WriteLine("Not enough colors in shuffledColourSquares to initialize picture boxes.");
                }
            }
        }

        private void SwapOneSquareInLiveSequence()
        {
            Dictionary<string, string> getOneNewSquare = ShuffleAllColorSquares();

            // Retrieve the first 4 key-value pairs from shuffledColourSquares
            KeyValuePair<string, string> kvpA = getOneNewSquare.ElementAt(0);
            KeyValuePair<string, string> kvpB = getOneNewSquare.ElementAt(1);
            KeyValuePair<string, string> kvpC = getOneNewSquare.ElementAt(2);
            KeyValuePair<string, string> kvpD = getOneNewSquare.ElementAt(3);

            InitializePictureBox(pictureBox1, kvpA.Key, kvpA.Value);
            InitializePictureBox(pictureBox2, kvpB.Key, kvpB.Value);
            InitializePictureBox(pictureBox3, kvpC.Key, kvpC.Value);
            InitializePictureBox(pictureBox4, kvpD.Key, kvpD.Value);
        }


        private async void DisplayLabelMessage(bool isComputerTurn)
        {
            /*
             * Show label with text in either Computer's or Player's turn
             * Computer's turn: "Click Here", "Start Here!", "Start With this One!" (45% or 75% chance depending on level)
             * Player's turn: various messages based on different levels and conditions
             */

            int chance = counter_levels >= 6 ? 75 : 45;
            bool showMessage = isComputerTurn
                ? counter_levels >= 1 && rnd.Next(100) <= chance && correctOrder.Count != correctOrder.Count - 1
                : counter_levels >= 1 && rnd.Next(100) <= 65 && playerOrder.Count != correctOrder.Count;

            if (showMessage)
            {
                List<Label> labels = new List<Label> { LabelMessage1, LabelMessage2, LabelMessage3, LabelMessage4 };
                List<string> labelText;

                if (isComputerTurn)
                {
                    labelText = new List<string> { "Click Here", "Start Here!", "Start With\nthis One!", "This One!", "Over Here!" };
                }
                else
                {
                    labelText = new List<string>
                        {
                         "Click Here", "This Is NOT\nThe Correct color!", "The Computer\nIs Lying!",
                         "This Is\nThe One!", "Just Kidding!\nClick This One!", "This Is NOT\nThe Right Color!",
                         "This Is\nThe Next\nOne!", "Now This One!", "This One!", "Over Here!"
                        };
                }

                // RandomizerNoMoreColorsTwiceInARow() Label
                int pickLabelIndex = rnd.Next(labels.Count);
                Label randomizedLabelClickHere = labels[pickLabelIndex];
                
                // Debug.WriteLine($"Randomized label: index {pickLabelIndex}");

                // RandomizerNoMoreColorsTwiceInARow() labelText
                int pickLabelText = rnd.Next(labelText.Count);
                string randomizedText = labelText[pickLabelText];
                
                // Debug.WriteLine($"Randomized labelText: {randomizedText}");

                await Task.Delay(250);

                randomizedLabelClickHere.AutoSize = true;
                randomizedLabelClickHere.Text = randomizedText;
                randomizedLabelClickHere.Visible = true;
                await Task.Delay(750);
                randomizedLabelClickHere.Visible = false;
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

        private string RandomizerNoMoreColorsTwiceInARow()
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
                        // Debug.WriteLine("UpdateTurn LevelUp");

                        richTextBoxTurn.BackColor = Color.LightGreen;
                        richTextBoxTurn.Text = $"\n{new string(' ', 7)}CORRECT";

                        await Task.Delay(1500);

                        richTextBoxTurn.Text = $"\n{new string(' ', 8)}Level  Up";
                        
                        CheckChangeColors();

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

            SetGameOverButton();
            FieldSetupAtStart();

            counter_rounds = 1;
            counter_levels = 1;
        }
    }
}