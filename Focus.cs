/*
 * Simon Says-like game with some level based challenges
 * Each level has 6 sequences. After 6 succesful sequences:
 * Level++; Add 1 challenge; Clear correctOrder and playerOrder and start with new sequence = 1
 * From Level >= 6: no Clear correctOrder; sequences++ untill game over
 * More challenges in progress
 * 
 * === Levels ===
 * Level 1 [EasyPeasy]: standard
 * level 2 [OkiDoki] and onward: some misleading text in pictureboxes, plus:
 *                               Shuffle Pictureboxes before start player's turn with 55% chance; level 3 85% chance; >= level 5 95% chance
 * level 3 [Please No]: Shuffle Pictureboxes per player click with 55% chance; level 4 85% chance; >= level 6 95% chance
 * Level 4 [No Way!]: When level up, replace all color squares on board with 55%; level 5 85%; >= level 7 95%
 * level 5 [HELL NO]: In each sequence, replace one color in running order with 55% chance; level 6 85% chance; level 8 95%
 * level 6 [NONONONO]: Replace 1 color with other color on board and in running order in the running order with 55% chance; level 7 85% chance; >= level 9 95%
 * level 7 [...]: 
 * level 8 [...]: 
 * level 9 [...]: 
 * level 666 [HELLMODE]: No Clear correctOrder; sequences++ untill game over
 */

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace KeepYourFocus
{
    public partial class PlayerField : Form
    {
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
        private bool replaceColor = false;

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
            string soundPathBeepALL = Path.Combine(SetRootPath(), @"sounds\beep.wav");

            /* Pre-made soundPath for all colors *\
            string soundPathBeepRed = Path.Combine(SetRootPath(), @"sounds\redSound.wav");
            string soundPathBeepBlue = Path.Combine(SetRootPath(), @"sounds\blueSound.wav");
            string soundPathBeepOrange = Path.Combine(SetRootPath(), @"sounds\orangeSound.wav");
            string soundPathBeepGreen = Path.Combine(SetRootPath(), @"sounds\greenSound.wav");
            string soundPathBeepCaribBlue = Path.Combine(SetRootPath(), @"sounds\caribBlueSound.wav");
            string soundPathBeepGrey = Path.Combine(SetRootPath(), @"\sounds\greySound.wav");
            string soundPathBeepIndigo = Path.Combine(SetRootPath(), @"sounds\indigoSound.wav");
            string soundPathBeepMaroon = Path.Combine(SetRootPath(), @\sounds\maroonSound.wav");
            */

            string soundPathTransition = Path.Combine(SetRootPath(), @"sounds\transistion.wav");
            string soundPathButtonClick = Path.Combine(SetRootPath(), @"sounds\buttonclick.wav");
            string soundPathWrong = Path.Combine(SetRootPath(), @"sounds\wrong.wav");
            string soundPathCorrect = Path.Combine(SetRootPath(), @"sounds\correct.wav");
            string soundPathStartupSound = Path.Combine(SetRootPath(), @"sounds\startupSound.wav");

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

            SetupFieldAtStart();
        }

        // Initialize and return root path including directory \KeepYourFocus\
        static string SetRootPath()
        {
            // string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
            string directoryPath = Environment.CurrentDirectory; // kan ook

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

        private void SetupFieldAtStart()
        {
            if (pictureBoxDictionary.Count > 0)
            {
                RefreshAndRepositionPictureBoxes();
                return;
            }

            InitializePictureBox(pictureBox1, "Red", Path.Combine(SetRootPath(), @"png\red_square512.png"));
            InitializePictureBox(pictureBox2, "Blue", Path.Combine(SetRootPath(), @"png\blue_square512.png"));
            InitializePictureBox(pictureBox3, "Orange", Path.Combine(SetRootPath(), @"png\orange_square512.png"));
            InitializePictureBox(pictureBox4, "Green", Path.Combine(SetRootPath(), @"png\green_square512.png"));
        }

        // Returns a dictionary of all possible squares
        static Dictionary<string, string> DictionaryOfAllSquares()
        {
            string redSquare = Path.Combine(SetRootPath(), "png", "red_square512.png");
            string blueSquare = Path.Combine(SetRootPath(), "png", "blue_square512.png");
            string orangeSquare = Path.Combine(SetRootPath(), "png", "orange_square512.png");
            string greenSquare = Path.Combine(SetRootPath(), "png", "green_square512.png");
            string caribBlueSquare = Path.Combine(SetRootPath(), "png", "caribBlue_square512.png");
            string greySquare = Path.Combine(SetRootPath(), "png", "grey_square512.png");
            string indigoSquare = Path.Combine(SetRootPath(), "png", "indigo_square512.png");
            string maroonSquare = Path.Combine(SetRootPath(), "png", "maroon_square512.png");

            Dictionary<string, string> dictOfAllSquares = new Dictionary<string, string>()
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

            return dictOfAllSquares;
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

                pictureBox.Click -= PlayersTurn; // Remove any previous attachment

                pictureBox.Click += PlayersTurn; // Attach event handler

                // Update the dictionary with the new PictureBox
                pictureBoxDictionary[color] = pictureBox;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing PictureBox for color {color}: {ex.Message}");
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
                pictureBox.Location = GetFixedPosition(index);
                index++;
            }
        }

        // Randomize colors for computer sequence. There should be not more then 2x same color in a row
        private string RandomizerColors()
        {
            string newColor;
            bool isValid;

            do
            {
                newColor = pictureBoxDictionary.Keys.ElementAt(rnd.Next(pictureBoxDictionary.Count));

                if (consecutiveCount < 2)
                {
                    // If the last color is not the same as the new color, it's valid
                    isValid = newColor != previousColors[1];
                }
                else
                {
                    // If the last two colors are the same, ensure the new color is different
                    isValid = newColor != previousColors[0] && newColor != previousColors[1];
                }

            } while (!isValid);

            // Shift the colors in the previousColors array
            previousColors[0] = previousColors[1];
            previousColors[1] = newColor;

            // Update the consecutive count
            if (previousColors[0] == newColor)
            {
                consecutiveCount++;
            }
            else
            {
                consecutiveCount = 1;
            }

            return newColor;
        }

        // Shuffles dictionary of all colors
        private Dictionary<string, string> ShuffleDictOfAllColorSquares()
        {
            Dictionary<string, string> dictOfAllSquares = DictionaryOfAllSquares();

            List<KeyValuePair<string, string>> listOfAllSquares = dictOfAllSquares.ToList();

            int numberOfItems = listOfAllSquares.Count;
            while (numberOfItems > 1)
            {
                numberOfItems--;
                int randomIndex = rnd.Next(numberOfItems + 1);
                KeyValuePair<string, string> temp = listOfAllSquares[randomIndex];
                listOfAllSquares[randomIndex] = listOfAllSquares[numberOfItems];
                listOfAllSquares[numberOfItems] = temp;
            }

            Dictionary<string, string> shuffledDictOfAllColorSquares = listOfAllSquares.ToDictionary(kv => kv.Key, kv => kv.Value);

            return shuffledDictOfAllColorSquares;
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
            // Get the shuffled PictureBoxes
            var shuffledPictureBoxes = pictureBoxDictionary.Values.OrderBy(x => rnd.Next()).ToList();

            // Iterate through the shuffled PictureBoxes and assign fixed positions
            for (int i = 0; i < shuffledPictureBoxes.Count; i++)
            {
                shuffledPictureBoxes[i].Location = GetFixedPosition(i);
                shuffledPictureBoxes[i].Visible = true;
            }
        }


        private async void ComputersTurn()
        {
            Computer = true;
            correctOrder.Add(RandomizerColors());
            UpdateTurn(); // case Computer's Turn

            // Debug.WriteLine("Verify ReplaceOneColorInOrder()");
            // ReplaceOneColorInOrder();

            await Task.Delay(1000); // Delay 1000 ms before display Computer's Sequence

            DisplaySequence();
        }

        private async void DisplaySequence()
        {
            Dictionary<string, PictureBox> updatedPictureBoxDictionary;
            Dictionary<string, PictureBox> usePictureBoxDictionary;
            List<string> updatedCorrectOrder;
            List<string> useOrder;

            (updatedPictureBoxDictionary, updatedCorrectOrder) = ReplaceColorOnBoardAndInOrder();

            if (replaceColor)
            {
                usePictureBoxDictionary = updatedPictureBoxDictionary;
                useOrder = updatedCorrectOrder;
            }
            else
            {
                usePictureBoxDictionary = pictureBoxDictionary;
                useOrder = correctOrder;
            }


            Computer = true;

            Debug.WriteLine($"\nDisplay Sequence: {counter_sequences}");
            Debug.WriteLine("useOrder = " + string.Join(", ", useOrder));
            Debug.WriteLine("usePictureBoxDictionary = " + string.Join(", ", usePictureBoxDictionary.Keys));

            await Task.Delay(1000);

            foreach (var color in useOrder)
            {
                var box = usePictureBoxDictionary[color];
                if (box == null)
                    continue;

                await Task.Delay(500); // Delay 500 ms before start highlights and beepSound

                PlaySound(color);

                SetHighlight(box, true);
                await Task.Delay(150);
                SetHighlight(box, false);
                await Task.Delay(50);
                // Debug.WriteLine($"Color: [{color}]");
            }
            // Check difficulty
            SetTurnActions();

            await Task.Delay(1000); // Delay 1000 ms before calling PlayersTurn()

            Computer = false;
            replaceColor = false;

            UpdateTurn(); // case Player's Turn
        }

        private async void PlayersTurn(object? sender, EventArgs e)
        {
            // Block Player's clicks in Computer's turn AND before StartButton is clicked
            if (startButton || Computer)
                return;

            if (sender is PictureBox clickedBox)
            {
                string color = clickedBox.Tag?.ToString() ?? "";

                PlaySound(color);
                SetHighlight(clickedBox, true);

                playerOrder.Add(color);

                // Check difficulty
                SetTurnActions();

                // Verify each player's click with correctOrder
                for (int i = 0; i < playerOrder.Count; i++)
                {
                    if (playerOrder[i] != correctOrder[i])
                    {
                        await Task.Delay(100);
                        SetHighlight(clickedBox, false);
                        await Task.Delay(250); // Delay to provide feedback before game over
                        GameOver();
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

                    ReplaceAllColorSquares();

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

        // Method to verify difficulties. 2 cases: Computers turn and Players turn
        private void SetTurnActions()
        {

            switch (Computer)
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

        private async void ShufflePictureBoxes() // Called in SetTurnActions()
        {
            switch (Computer)
            {
                case (true):
                    if (counter_levels == 2 && rnd.Next(100) <= 55 ||
                        counter_levels >= 3 && rnd.Next(100) <= 85 ||
                        counter_levels >= 5 && rnd.Next(100) <= 95)
                    {
                        Debug.WriteLine($"ShufflePictureBoxes Case 1: Shuffle after display sequence");

                        await Task.Delay(250); // Delay 250 ms for space between colorSound and transitionSound
                        transitionSound.Play();
                        RandomizerShufflePictureBoxes();
                        RefreshAndRepositionPictureBoxes();
                        await Task.Delay(1000);
                    }
                    break;
                case (false):
                    if (counter_levels >= 3 && rnd.Next(100) <= 55 ||
                        counter_levels >= 4 && rnd.Next(100) <= 85 ||
                        counter_levels >= 6 && rnd.Next(100) <= 95)
                    {
                        Debug.WriteLine($"ShufflePictureBoxes Case 2: Shuffle after player click");

                        RandomizerShufflePictureBoxes();
                        RefreshAndRepositionPictureBoxes();
                    }
                    break;
            }
        }

        private void ReplaceAllColorSquares() // Called in SetCounters()
        {
            if (counter_levels >= 4 && levelUp == true && rnd.Next(100) <= 55 ||
                counter_levels >= 5 && levelUp == true && rnd.Next(100) <= 85 ||
                counter_levels >= 7 && levelUp == true && rnd.Next(100) <= 95)
            {

                Dictionary<string, string> shuffledColourSquares = ShuffleDictOfAllColorSquares();

                // Ensure that we have enough colors to assign
                if (shuffledColourSquares.Count >= 3)
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

                        Debug.WriteLine("Activate ReplaceAllColorSquares()");
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
        
        // Method to replace 1 color in running sequence and/or on board
        private (Dictionary<string, PictureBox>, List<string>) ReplaceColorOnBoardAndInOrder() // Called in DisplaySequence()
        {
            string newColor = RandomizerColors();
            Dictionary<string, string> dictOfAllSquares = DictionaryOfAllSquares();
            List<KeyValuePair<string, string>> listOfAllSquares = dictOfAllSquares.ToList();

            bool shouldReplaceInOrder = (counter_levels >= 5 && correctOrder.Count > 2 && rnd.Next(100) <= 55) ||
                                        (counter_levels >= 6 && correctOrder.Count > 2 && rnd.Next(100) <= 85) ||
                                        (counter_levels >= 8 && correctOrder.Count > 2 && rnd.Next(100) <= 95); 

            bool shouldReplaceOnBoard = (counter_levels >= 6 && correctOrder.Count > 2 && rnd.Next(100) <= 55) ||
                                        (counter_levels >= 7 && correctOrder.Count > 2 && rnd.Next(100) <= 85) ||
                                        (counter_levels >= 9 && correctOrder.Count > 2 && rnd.Next(100) <= 95);

            if (shouldReplaceInOrder || shouldReplaceOnBoard)
            {
                replaceColor = true;

                // Make copy of correctOrder as copyCorrectOrder
                List<string> copyCorrectOrder = new List<string>(correctOrder);

                // Randomize color to delete from copyCorrectOrder
                int randomIndex = rnd.Next(copyCorrectOrder.Count);
                string deleteColor = copyCorrectOrder[randomIndex];

                Debug.WriteLine($"deleteColor: [{deleteColor}]");

                if (shouldReplaceInOrder && newColor != deleteColor && randomIndex != copyCorrectOrder.Count - 1)
                {
                    Debug.WriteLine("\nCorrectOrder = " + string.Join(", ", correctOrder));
                    Debug.WriteLine($"Replacing in order [{deleteColor}] at index [{randomIndex}] with new color [{newColor}]\n");
                    
                    copyCorrectOrder[randomIndex] = newColor;

                    // Update correctOrder with the new copyCorrectOrder
                    correctOrder = copyCorrectOrder;
                }
                if (shouldReplaceOnBoard)
                {
                    replaceColor = true;

                    // Get the PictureBox associated with the deleteColor
                    PictureBox pictureBoxToReplace = pictureBoxDictionary[deleteColor];

                    // Remove the old color square from the board
                    pictureBoxDictionary.Remove(deleteColor);

                    // Randomize new color that's not in the remaining colors on the board
                    string pickNewColor;
                    do
                    {
                        pickNewColor = listOfAllSquares[rnd.Next(listOfAllSquares.Count)].Key;
                        Debug.WriteLine($"pickNewColor: [{pickNewColor}]");
                    } while (copyCorrectOrder.Contains(pickNewColor) || pictureBoxDictionary.ContainsKey(pickNewColor));

                    Debug.WriteLine("\nCorrectOrder = " + string.Join(", ", correctOrder));
                    Debug.WriteLine($"Replaced on board and in order [{deleteColor}] with [{pickNewColor}]");

                    // Initialize the PictureBox with the new color
                    InitializePictureBox(pictureBoxToReplace, pickNewColor, dictOfAllSquares[pickNewColor]);

                    // Add the new color to the pictureBoxDictionary
                    pictureBoxDictionary[pickNewColor] = pictureBoxToReplace;

                    // Iter through copyCorrectOrder and replace deleteColor with pickNewcolor at the same index
                    for (int indexItem = 0; indexItem < copyCorrectOrder.Count; indexItem++)
                    {
                        if (copyCorrectOrder[indexItem] == deleteColor)
                        {
                            copyCorrectOrder[indexItem] = pickNewColor;
                        }
                    }

                    // Update correctOrder with the new copyCorrectOrder
                    correctOrder = copyCorrectOrder;

                }
                Debug.WriteLine("Updated correctOrder = " + string.Join(", ", correctOrder));
                Debug.WriteLine("Updated pictureBoxDictionary = " + string.Join(", ", pictureBoxDictionary.Keys));
            }
            return (pictureBoxDictionary, correctOrder);
        }


        private async void DisplayLabelMessage(bool isComputerTurn)
        {
            /*
             * Show label with text in either Computer's or Player's turn
             * Computer's turn: "Click Here", "Start Here!", "Start With this One!" (45% or 75% chance depending on level)
             * Player's turn: various messages based on different levels and conditions
             */

            int chance = counter_levels >= 6 ? 55 : 45;
            bool showMessage = isComputerTurn
                ? counter_levels >= 2 && rnd.Next(100) <= chance && correctOrder.Count != correctOrder.Count - 1
                : counter_levels >= 2 && rnd.Next(100) <= 65 && playerOrder.Count != correctOrder.Count;

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
                    pictureBox.Padding = new Padding(2);
                    pictureBox.BackColor = Color.White;
                }
                else // Highlight off
                {
                    pictureBox.Padding = new Padding(0);
                    pictureBox.BackColor = Color.Transparent;
                }
            }
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

                        // ReplaceAllColorSquares();

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
            SetupFieldAtStart();

            counter_rounds = 1;
            counter_levels = 1;
        }
    }
}