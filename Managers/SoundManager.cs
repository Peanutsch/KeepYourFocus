using Simon_Says.Helpers;
using System.Media;

namespace Simon_Says.Managers
{
    /// <summary>
    /// Manages loading and playback of all game sound effects.
    /// Each tile color has its own <see cref="SoundPlayer"/> instance,
    /// along with dedicated players for UI and game-state sounds.
    /// </summary>
    public class SoundManager
    {
        // Tile-specific sound players (one per tile color)
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

        // Game event sound players
        private readonly SoundPlayer transitionSound;
        private readonly SoundPlayer buttonClickSound;
        private readonly SoundPlayer wrongSound;
        private readonly SoundPlayer correctSound;
        private readonly SoundPlayer startupSound;

        /// <summary>
        /// Initializes all <see cref="SoundPlayer"/> instances by resolving
        /// sound file paths relative to the project root.
        /// </summary>
        public SoundManager()
        {
            string rootPath = PathHelper.GetRootPath();
            string soundPathBeepALL = Path.Combine(rootPath, @"sounds\beep.wav");

            string soundPathTransition = Path.Combine(rootPath, @"sounds\transistion.wav");
            string soundPathButtonClick = Path.Combine(rootPath, @"sounds\buttonclick.wav");
            string soundPathWrong = Path.Combine(rootPath, @"sounds\wrong.wav");
            string soundPathCorrect = Path.Combine(rootPath, @"sounds\correct.wav");
            string soundPathStartupSound = Path.Combine(rootPath, @"sounds\startupSound.wav");

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
        }

        /// <summary>
        /// Plays the sound effect associated with the specified tile color.
        /// </summary>
        /// <param name="tile">The tile color name (e.g. "Red", "Blue").</param>
        public void PlayTileSound(string tile)
        {
            switch (tile)
            {
                case "Red": redSound.Play(); break;
                case "Blue": blueSound.Play(); break;
                case "Orange": orangeSound.Play(); break;
                case "Green": greenSound.Play(); break;
                case "CaribBlue": caribBlueSound.Play(); break;
                case "Grey": greySound.Play(); break;
                case "Indigo": indigoSound.Play(); break;
                case "Maroon": maroonSound.Play(); break;
                case "Olive": oliveSound.Play(); break;
                case "Pink": pinkSound.Play(); break;
            }
        }

        /// <summary>Plays the tile-shuffle transition sound.</summary>
        public void PlayTransition() => transitionSound.Play();
        /// <summary>Plays the UI button click sound.</summary>
        public void PlayButtonClick() => buttonClickSound.Play();
        /// <summary>Plays the wrong-answer sound.</summary>
        public void PlayWrong() => wrongSound.Play();
        /// <summary>Plays the correct-answer sound.</summary>
        public void PlayCorrect() => correctSound.Play();
        /// <summary>Plays the application startup sound.</summary>
        public void PlayStartup() => startupSound.Play();
    }
}
