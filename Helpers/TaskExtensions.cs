using System.Diagnostics;

namespace KeepYourFocus.Helpers
{
    /// <summary>
    /// Static helper methods for task-based asynchronous operations.
    /// Provides semantic delay methods with optional cancellation support and debug logging.
    /// All delays require a semantic label for tracking and debugging purposes.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Asynchronously waits for a specified duration with a semantic label for debugging.
        /// Logs the delay operation completion with the provided label.
        /// Semantic labels are required to maintain clear code intent and aid in debugging.
        /// </summary>
        /// <param name="delayLabel">Semantic description of the delay (e.g., "Tile highlight", "Round transition").
        /// This label is used for debug logging and helps track timing throughout the game.</param>
        /// <param name="milliseconds">The delay duration in milliseconds.</param>
        /// <param name="cancellationToken">Optional cancellation token for graceful shutdown.</param>
        /// <returns>A task that completes after the specified delay.</returns>
        /// <example>
        /// <code>
        /// await DelayAsync("Player click feedback", GameTiming.PlayerClickFeedbackDelay);
        /// </code>
        /// </example>
        public static async Task DelayAsync(string delayLabel, int milliseconds, CancellationToken cancellationToken = default)
        {
            // Early exit for zero or negative durations
            if (milliseconds <= 0)
                return;

            try
            {
                Debug.WriteLine($"> {delayLabel} ({milliseconds} ms)");
                await Task.Delay(milliseconds, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"[TaskExtensions.DelayAsync] '{delayLabel}' was cancelled");
                throw;
            }
        }
    }
}
