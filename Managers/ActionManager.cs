using KeepYourFocus.Helpers;
using System.Diagnostics;

namespace KeepYourFocus.Managers
{
    /// <summary>
    /// Manages difficulty-based game actions including shuffling and tile replacement.
    /// Encapsulates action dispatch logic to separate concerns from the main game form.
    /// Ensures each game phase only triggers one action per turn via the actionTaken flag.
    /// </summary>
    public class ActionManager
    {
        private readonly TileManager tileManager;
        private readonly Func<Task> shuffleDelegate;

        /// <summary>
        /// Initializes the ActionManager with required manager instances and delegates.
        /// </summary>
        /// <param name="tileManager">The tile manager instance for tile-related operations.</param>
        /// <param name="shuffleDelegate">Async delegate that performs the shuffle operation.</param>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        public ActionManager(TileManager tileManager, Func<Task> shuffleDelegate)
        {
            this.tileManager = tileManager ?? throw new ArgumentNullException(nameof(tileManager));
            this.shuffleDelegate = shuffleDelegate ?? throw new ArgumentNullException(nameof(shuffleDelegate));
        }

        /// <summary>
        /// Dispatches difficulty-based actions (shuffling, tile replacement) based on the current game phase.
        /// Prevents duplicate actions per turn and returns the updated actionTaken flag.
        /// Actions are prioritized: Computer Turn → Player Turn → Display Sequence → Level-Up → Hard Mode.
        /// </summary>
        /// <param name="isComputerTurn">Whether it's currently the computer's turn phase.</param>
        /// <param name="isPlayerTurn">Whether it's currently the player's turn phase.</param>
        /// <param name="isDisplaySequence">Whether the sequence is currently being displayed.</param>
        /// <param name="isSetCounters">Whether we're in the counter-update/level-up phase.</param>
        /// <param name="actionTaken">Flag preventing duplicate actions; will be updated if an action executes.</param>
        /// <param name="counterLevels">Current game level (difficulty).</param>
        /// <param name="levelUp">Whether a level-up is currently in progress.</param>
        /// <param name="isHardLevel">Whether hard (endless) difficulty mode is active.</param>
        /// <param name="setSequences">Sequences per round; int.MaxValue indicates hard mode.</param>
        /// <param name="pictureBoxes">The four game board PictureBox controls.</param>
        /// <param name="correctOrder">The correct sequence for the current round.</param>
        /// <param name="clickHandler">Event handler for tile click events.</param>
        /// <returns>True if an action was executed, false otherwise.</returns>
        public async Task<bool> ExecutePhaseActionsAsync(
            bool isComputerTurn, bool isPlayerTurn, bool isDisplaySequence, bool isSetCounters,
            bool actionTaken, int counterLevels, bool levelUp, bool isHardLevel,
            int setSequences, PictureBox[] pictureBoxes, List<string> correctOrder, EventHandler clickHandler)
        {
            Debug.WriteLine("\n=== [ActionManager.ExecutePhaseActionsAsync] Starting ===");

            // Phase 1: Shuffle during computer's turn
            if (isComputerTurn && !actionTaken)
            {
                Debug.WriteLine("> Computer Turn: Executing shuffle action");
                await shuffleDelegate();
                Debug.WriteLine("=== [ActionManager.ExecutePhaseActionsAsync] Complete (Computer Turn) ===\n");
                return true;
            }

            // Phase 2: Shuffle during player's turn
            if (isPlayerTurn && !actionTaken)
            {
                Debug.WriteLine("> Player Turn: Executing shuffle action");
                await shuffleDelegate();
                Debug.WriteLine("=== [ActionManager.ExecutePhaseActionsAsync] Complete (Player Turn) ===\n");
                return true;
            }

            // Phase 3: Shuffle during sequence display
            if (isDisplaySequence && !actionTaken)
            {
                Debug.WriteLine("> Display Sequence: Executing shuffle action");
                await shuffleDelegate();
                Debug.WriteLine("=== [ActionManager.ExecutePhaseActionsAsync] Complete (Display Sequence) ===\n");
                return true;
            }

            // Phase 4: Replace all tiles on level-up
            if (isSetCounters && !actionTaken)
            {
                Debug.WriteLine("> Level-Up: Executing tile replacement on all positions");
                tileManager.ReplaceAllTiles(pictureBoxes, counterLevels, levelUp, isHardLevel, isDisplaySequence, clickHandler);
                Debug.WriteLine("=== [ActionManager.ExecutePhaseActionsAsync] Complete (Level-Up) ===\n");
                return true;
            }

            // Phase 5: Hard mode - apply ALL challenges simultaneously
            if (setSequences == int.MaxValue && !actionTaken)
            {
                Debug.WriteLine("> Hard Mode: Executing ALL challenges simultaneously");

                // Execute shuffle
                await shuffleDelegate();

                // Execute tile replacement in sequence
                tileManager.ReplaceTileOnBoardAndInSequence(correctOrder, counterLevels, isHardLevel, isDisplaySequence, clickHandler);

                // Execute full board replacement
                tileManager.ReplaceAllTiles(pictureBoxes, counterLevels, levelUp, isHardLevel, isDisplaySequence, clickHandler);

                Debug.WriteLine("=== [ActionManager.ExecutePhaseActionsAsync] Complete (Hard Mode - All Challenges) ===\n");
                return true;
            }

            Debug.WriteLine("=== [ActionManager.ExecutePhaseActionsAsync] No actions triggered ===\n");
            return false;
        }
    }
}
