namespace KeepYourFocus.Helpers
{
    /// <summary>
    /// Game timing constants for consistent, configurable delays throughout the game.
    /// All values are in milliseconds. Adjust these constants to fine-tune the game's pace.
    /// </summary>
    public static class GameTiming
    {
        #region === Sequence and Animation Timing ===
        /// <summary>Delay before sequence display begins (allows UI to settle).</summary>
        public const int SequenceStartDelay = 500;

        /// <summary>Delay between revealing successive tiles in the sequence.</summary>
        public const int TileRevealDelay = 500;

        /// <summary>How long a tile stays highlighted during sequence playback.</summary>
        public const int TileHighlightDuration = 150;

        /// <summary>Delay between successive tile highlights (silence/gap between notes).</summary>
        public const int TileBetweenDelay = 50;

        /// <summary>Delay after sequence completes before player's turn begins.</summary>
        public const int SequenceEndDelay = 500;
        #endregion

        #region === Player Feedback Timing ===
        /// <summary>How long a player's clicked tile stays highlighted (visible feedback).</summary>
        public const int PlayerClickHighlightDuration = 250;

        /// <summary>Delay after incorrect player input before game over is triggered.</summary>
        public const int PlayerClickFeedbackDelay = 500;
        #endregion

        #region === Round Transition Timing ===
        /// <summary>How long "CORRECT" message displays after successful sequence.</summary>
        public const int CorrectSequenceDuration = 1500;

        /// <summary>How long "LEVEL UP" message displays during level advancement.</summary>
        public const int LevelUpMessageDuration = 1000;

        /// <summary>How long "NEXT SEQUENCE" message displays between rounds.</summary>
        public const int NextSequenceMessageDuration = 1000;

        /// <summary>Delay between round completion and the next computer's turn starting.</summary>
        public const int RoundTransitionDelay = 2750;
        #endregion

        #region === Difficulty Action Timing ===
        /// <summary>Delay before shuffle transition sound plays.</summary>
        public const int PreShuffleDelay = 500;

        /// <summary>Delay after shuffle transition sound before visual repositioning.</summary>
        public const int PostShuffleDelay = 500;

        /// <summary>Delay before distraction label message appears.</summary>
        public const int DistractionMessagePreDelay = 250;

        /// <summary>How long distraction label message displays on screen.</summary>
        public const int DistractionMessageDuration = 750;
        #endregion

        #region === UI Refresh Timing ===
        /// <summary>Delay before retry button is enabled after game over.</summary>
        public const int RetryButtonDelay = 500;
        #endregion
    }
}
