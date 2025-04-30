using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simon_Says
{
    class RoundManager
    {
        private List<string> correctOrder;
        private List<string> playerOrder;

        private int counterLevels;
        private int counterRounds;
        private int counterSequences;

        private bool computer;
        private bool isPlayerTurn;
        private bool isComputerTurn;
        private bool nextRound;

        // Constructor
        public RoundManager()
        {
            counterLevels = 1;
            counterRounds = 1;
            counterSequences = 1;
            correctOrder = new List<string>();
            playerOrder = new List<string>();
        }

        private static void ComputersTurn()
        {
            textBoxShowResults.Visible = false;

            isComputerTurn = true;
            computer = true;

            actionTaken = false;

            correctOrder.Add(RandomizerTiles());
            UpdateTurn(); // Case: computer's turn

            DisplaySequence();
        }


        private async void DisplaySequence()
        {
            isDisplaySequence = true;

            var (updatedPictureBoxDictionary, updatedCorrectOrder, replacementOccurred) = ReplaceTileOnBoardAndInSequence();

            if (replacementOccurred)
            {
                pictureBoxDictionary = updatedPictureBoxDictionary;
                correctOrder = updatedCorrectOrder;
            }

            Debug.WriteLine($"\nLevel: {counterLevels} Display Sequence: {counterSequences}");
            Debug.WriteLine("correctOrder = " + string.Join(", ", correctOrder));

            await Task.Delay(500);

            foreach (var tile in correctOrder)
            {
                if (pictureBoxDictionary.TryGetValue(tile, out var box) && box != null)
                {
                    await Task.Delay(500);
                    PlaySound(tile);
                    ManageHighlight(box, true);
                    await Task.Delay(150);
                    ManageHighlight(box, false);
                    await Task.Delay(50);
                }
            }

            await ManageActions();
            await Task.Delay(500);

            isComputerTurn = false;
            computer = false;
            isDisplaySequence = false;

            UpdateTurn(); // Case: player's turn
        }

        private async void PlayersTurn(object? sender, EventArgs e)
        {
            if (startButton || computer || sender is not PictureBox clickedBox)
                return;

            isPlayerTurn = true;
            actionTaken = false;

            string tile = clickedBox.Tag?.ToString() ?? "";

            PlaySound(tile);
            ManageHighlight(clickedBox, true);
            playerOrder.Add(tile);

            Debug.WriteLine($"Player: [{tile}]");

            await ManageActions();

            // Check if current tile is correct
            int lastIndex = playerOrder.Count - 1;
            if (playerOrder[lastIndex] != correctOrder[lastIndex])
            {
                await Task.Delay(100);
                ManageHighlight(clickedBox, false);
                await Task.Delay(250);
                TextBoxHighscores();
                GameOver();
                isPlayerTurn = false;
                return;
            }

            await Task.Delay(100);
            ManageHighlight(clickedBox, false);
            await Task.Delay(50);

            if (playerOrder.Count == correctOrder.Count)
                ManageCountersAndLevels();

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
            sounds.PlayCorrect();

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

            if (actionTaken)
                return;

            switch (true)
            {
                case true when isComputerTurn:
                    Debug.WriteLine("ManageActions > isComputerTurn = true");
                    await ShufflePictureBoxes();
                    break;

                case true when isPlayerTurn:
                    Debug.WriteLine("ManageActions > isPlayerTurn = true");
                    await ShufflePictureBoxes();
                    break;

                case true when isDisplaySequence:
                    Debug.WriteLine("ManageActions > isDisplaySequence = true");
                    await ShufflePictureBoxes();
                    ReplaceTileOnBoardAndInSequence();
                    break;

                case true when isSetCounters:
                    ReplaceAllTiles();
                    break;

                case true when setSequences == int.MaxValue:
                    isHardLevel = true;
                    await ShufflePictureBoxes();
                    ReplaceTileOnBoardAndInSequence();
                    ReplaceAllTiles();
                    break;
            }

            actionTaken = true;
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
    }
}
