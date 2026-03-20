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
        // Single sound player for all tile beep sounds (all use the same file)
        private readonly SoundPlayer tileBeepSound;

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

            tileBeepSound = new SoundPlayer(Path.Combine(rootPath, @"sounds\beep.wav"));
            transitionSound = new SoundPlayer(Path.Combine(rootPath, @"sounds\transistion.wav"));
            buttonClickSound = new SoundPlayer(Path.Combine(rootPath, @"sounds\buttonclick.wav"));
            wrongSound = new SoundPlayer(Path.Combine(rootPath, @"sounds\wrong.wav"));
            correctSound = new SoundPlayer(Path.Combine(rootPath, @"sounds\correct.wav"));
            startupSound = new SoundPlayer(Path.Combine(rootPath, @"sounds\startupSound.wav"));
        }

        /// <summary>
        /// Plays the sound effect associated with the specified tile color.
        /// All tiles currently use the same beep sound.
        /// </summary>
        /// <param name="tile">The tile color name (e.g. "Red", "Blue").</param>
        public void PlayTileSound(string tile)
        {
            tileBeepSound.Play();
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
