using KeepYourFocus;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simon_Says
{
    class GameOverManager
    {
        public static bool Computer { get; set; }
        public static bool StartButton { get; set; }
        public static bool GameTime { get; set; }

        private readonly GameTimer _gameTimer;
        private readonly GameSounds _sounds;
        private readonly PlayerField _playerField;
        public GameOverManager(GameTimer gameTimer, GameSounds sounds, PlayerField playerField)
        {
            _gameTimer = gameTimer;
            _sounds = sounds;
            _playerField = playerField;
        }

        // Initialize setup when Game Over
        public async void GameOver()
        {
            // Set flags
            Computer = false;
            StartButton = true;
            GameTime = false;

            _playerField.pictureBox1.Enabled = false;
            _playerField.pictureBox2.Enabled = false;
            _playerField.pictureBox3.Enabled = false;
            _playerField.pictureBox4.Enabled = false;

            _playerField.textBoxHighscore.Visible = true;
            _playerField.textBoxShowResults.Visible = true;

            _playerField.linkLabelGitHub.Visible = true;
            _playerField.linkLabelGitHub.Enabled = true;
            _playerField.linkLabelEmail.Visible = true;
            _playerField.linkLabelEmail.Enabled = true;

            // Stop Stopwatch
            _gameTimer.InitializeGameStopwatch();

            _sounds.PlayWrong();

            // Save score
            await VerifyPlayerRank(counterRounds, counterLevels, _playerField.richTextBoxShowLevelName.Text);

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
            string playerName = _playerField.textBoxInputName.Text.ToUpper().Trim();

            if (string.IsNullOrWhiteSpace(playerName))
            {
                playerName = "ANONYMOUS";
                Debug.WriteLine($"Input playerName is empty. Forced playerName is {playerName}\n");
            }

            //textBoxInputName.Enabled = false;
            _playerField.textBoxInputName.Visible = false;
            _playerField.buttonEnter.Enabled = false;
            _playerField.buttonEnter.Visible = false;

            TextBoxHighscores();

            _playerField.checkedListBoxDifficulty.Visible = true;

            _playerField.startBTN.TextAlign = ContentAlignment.MiddleCenter;
            _playerField.startBTN.Text = "Click to Start";
            _playerField.startBTN.Enabled = true;
            startButton = true;
            _playerField.richTextBoxShowRounds.Text = $"Good Luck!";

            Debug.WriteLine($"Input name is {playerName}\n");
            Debug.WriteLine("\nProcessInputName() end");
            return playerName;
        }

        // Get playerName via TextBoxInputName
        private async Task<string> PlayerName()
        {
            playerNameTcs = new TaskCompletionSource<string>();

            _playerField.textBoxInputName.Clear();
            _playerField.textBoxInputName.PlaceholderText = "YourNameHere";
            _playerField.textBoxInputName.Visible = true;
            _playerField.textBoxInputName.Enabled = true;
            _playerField.buttonEnter.Visible = true;
            _playerField.buttonEnter.Enabled = true;

            _playerField.textBoxInputName.Focus();

            InitializeKeyEnter();

            string playerName = await playerNameTcs.Task; // return input as playerName and complete task

            return playerName;
        }
    }
}
