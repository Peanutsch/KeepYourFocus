using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;
using System.Windows.Forms;
using XnaColor = Microsoft.Xna.Framework.Color;
using XnaKeys = Microsoft.Xna.Framework.Input.Keys;
using XnaButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

namespace KeepYourFocus.MonoGame
{
    /// <summary>
    /// MonoGameGame is the main game class for the "Keep Your Focus" memory sequence game.
    /// It implements the Simon Says-like gameplay where players must repeat an increasingly 
    /// complex sequence of color selections. The game alternates between computer turns 
    /// (displaying the sequence) and player turns (accepting input).
    /// </summary>
    public class MonoGameGame : Game
    {
        #region Fields

        #region Graphics & Rendering
        /// <summary>Manages graphics device settings and properties</summary>
        private GraphicsDeviceManager graphics = null!;

        /// <summary>Used to draw all sprites and textures to the screen</summary>
        private SpriteBatch spriteBatch = null!;
        #endregion

        /// <summary>The correct sequence of colors that must be repeated. Grows by one tile each round.</summary>
        public List<string> correctSequence = new();

        /// <summary>The player's current input sequence. Cleared after successful completion.</summary>
        public List<string> playerSequence = new();

        /// <summary>Total number of correctly completed sequences. Increments with each successful round.</summary>
        public int score = 0;

        /// <summary>Current difficulty level. Equals score + 1 to show progression.</summary>
        public int level = 1;

        /// <summary>Flag indicating whether it's the computer's turn to display the sequence</summary>
        public bool isComputerTurn = true;

        /// <summary>Flag to track if a new tile has been added to the sequence for this computer turn</summary>
        private bool computerTurnNewTileAdded = false;

        /// <summary>Flag to track if the "get ready" message box has been shown for this computer turn</summary>
        private bool computerTurnMessageBoxShown = false;

        /// <summary>Elapsed time during computer turn, measured in seconds</summary>
        private double computerTurnTimer = 0;

        /// <summary>The color chosen by the computer for this turn. Used for visual highlighting.</summary>
        private string? computerChosenColor;

        /// <summary>Index of the current tile being played in the computer sequence</summary>
        private int computerSequenceIndex = 0;

        /// <summary>Timer for displaying each individual tile during computer's turn</summary>
        private double computerTileDuration = 0.5;

        /// <summary>The color of the tile clicked by the player. Used for visual feedback.</summary>
        private string? playerClickedColor;

        /// <summary>Timer for displaying the player's tile click highlight</summary>
        private double playerClickTimer = 0;

        /// <summary>Duration to show the player's click highlight</summary>
        private double playerClickDuration = 0.3;

        /// <summary>Display size of each tile in pixels (150x150)</summary>
        private const int TILE_SIZE = 150;

        /// <summary>Screen position of the red tile (top-left)</summary>
        private Vector2 redTilePos = new(50, 50);

        /// <summary>Screen position of the blue tile (top-right)</summary>
        private Vector2 blueTilePos = new(250, 50);

        /// <summary>Screen position of the orange tile (bottom-left)</summary>
        private Vector2 orangeTilePos = new(50, 250);

        /// <summary>Screen position of the green tile (bottom-right)</summary>
        private Vector2 greenTilePos = new(250, 250);

        /// <summary>Texture for red tile (can be loaded PNG or generated placeholder)</summary>
        private Texture2D? redTile;

        /// <summary>Texture for blue tile (can be loaded PNG or generated placeholder)</summary>
        private Texture2D? blueTile;

        /// <summary>Texture for orange tile (can be loaded PNG or generated placeholder)</summary>
        private Texture2D? orangeTile;

        /// <summary>Texture for green tile (can be loaded PNG or generated placeholder)</summary>
        private Texture2D? greenTile;

        /// <summary>White texture used for drawing highlight borders around tiles</summary>
        private Texture2D? whiteBorder;

        /// <summary>SpriteFont for rendering text UI. Null if font asset fails to load (fallback mode)</summary>
        private SpriteFont? font;

        /// <summary>Random number generator for selecting random tiles from the array</summary>
        private Random rnd = new();

        /// <summary>Stores the previous frame's mouse state for detecting new clicks</summary>
        private MouseState prevMouseState;
        #endregion

        #region Initialization Methods

        /// <summary>
        /// Constructor initializes the MonoGame game with default settings.
        /// Sets up the graphics device manager and content directory.
        /// </summary>
        public MonoGameGame()
        {
            // Initialize graphics device manager with 500x500 window
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 500;
            graphics.PreferredBackBufferHeight = 500;

            // Set the root directory for loading content assets (PNG textures, fonts, etc.)
            Content.RootDirectory = "Content";

            // Show mouse cursor for better player experience
            IsMouseVisible = true;

            // Initialize sequence as empty - computer will add first tile during first turn
            // This prevents having duplicate tiles in the first sequence
        }

        /// <summary>
        /// Initialize is called before the first Update and is used to perform initial setup.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            // Set the window title to identify the game
            Window.Title = "Keep Your Focus - MonoGame Edition";
            Debug.WriteLine("✓ Game initialized");
        }

        #endregion

        #region Content Loading

        /// <summary>
        /// LoadContent is called once per game and is the place to load all your content.
        /// Attempts to load PNG tile textures and a SpriteFont for UI text.
        /// If assets are missing, generates placeholder textures to ensure the game runs.
        /// </summary>
        protected override void LoadContent()
        {
            // Create the SpriteBatch used for drawing sprites
            spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Font Loading
            // Attempt to load the SpriteFont asset for rendering text UI
            try
            {
                font = Content.Load<SpriteFont>("Fonts/arial");
                Debug.WriteLine("✓ Font loaded");
            }
            catch (Exception ex)
            {
                // If font is missing, log error and continue without text rendering
                Debug.WriteLine($"⚠ Font loading error: {ex.Message}");
                Debug.WriteLine("Creating default font (XNA built-in)...");
                font = null; // Will trigger fallback UI rendering (colored boxes)
            }
            #endregion

            #region Tile Texture Loading
            // Attempt to load tile PNG textures from Content/Tiles directory
            try
            {
                // Load each colored tile texture from the content pipeline
                redTile = Content.Load<Texture2D>("Tiles/red_tile512");
                blueTile = Content.Load<Texture2D>("Tiles/blue_tile512");
                orangeTile = Content.Load<Texture2D>("Tiles/orange_tile512");
                greenTile = Content.Load<Texture2D>("Tiles/green_tile512");
                Debug.WriteLine("✓ All tile assets loaded");
            }
            catch (Exception ex)
            {
                // If PNG textures are missing, create solid-colored placeholder rectangles
                Debug.WriteLine($"⚠ Tile asset loading error: {ex.Message}");
                Debug.WriteLine("Creating placeholder textures...");

                // Generate placeholder textures with solid colors (fallback mode)
                redTile = CreateColoredTexture(XnaColor.Red);
                blueTile = CreateColoredTexture(XnaColor.Blue);
                orangeTile = CreateColoredTexture(XnaColor.Orange);
                greenTile = CreateColoredTexture(XnaColor.Green);

                Debug.WriteLine("✓ Placeholder textures created");
            }
            #endregion

            #region Highlight Border Texture
            // Create a white 1x1 pixel texture for drawing highlight borders
            whiteBorder = new Texture2D(GraphicsDevice, 1, 1);
            whiteBorder.SetData(new[] { XnaColor.White });
            #endregion
        }

        /// <summary>
        /// Creates a placeholder texture with a solid color.
        /// Used when PNG assets cannot be loaded to ensure the game remains playable.
        /// </summary>
        /// <param name="color">The color to fill the entire texture with</param>
        /// <returns>A Texture2D object with dimensions TILE_SIZE x TILE_SIZE filled with the specified color</returns>
        private Texture2D CreateColoredTexture(XnaColor color)
        {
            // Create a new texture with TILE_SIZE x TILE_SIZE dimensions
            Texture2D texture = new Texture2D(GraphicsDevice, TILE_SIZE, TILE_SIZE);

            // Create a color array with one entry per pixel
            XnaColor[] data = new XnaColor[TILE_SIZE * TILE_SIZE];

            // Fill all pixels with the specified color
            for (int i = 0; i < data.Length; i++)
                data[i] = color;

            // Apply the pixel data to the texture
            texture.SetData(data);
            return texture;
        }

        #endregion

        #region Game Loop Methods

        /// <summary>
        /// Update is called once per frame and contains game logic.
        /// Handles computer turns (timing and color selection) and player input (tile clicks).
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            // Get current keyboard and mouse input states
            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            #region Input Handling
            // Check for ESC key to exit the game
            if (keyState.IsKeyDown(XnaKeys.Escape))
                Exit();
            #endregion

            #region Computer Turn Logic
            if (isComputerTurn)
            {
                // On first frame of computer turn, add a new tile to the sequence
                if (!computerTurnNewTileAdded)
                {
                    // Generate and add a new random tile to the sequence
                    string newTile = GetRandomTile();
                    correctSequence.Add(newTile);
                    computerTurnNewTileAdded = true;
                    computerSequenceIndex = 0;
                    computerTurnTimer = 0;

                    // Immediately show the first tile
                    computerChosenColor = correctSequence[computerSequenceIndex];
                    Debug.WriteLine($"🤖 Computer turn started. Sequence length: {correctSequence.Count}");
                    Debug.WriteLine($"  ► Playing tile {computerSequenceIndex + 1}/{correctSequence.Count}: {computerChosenColor}");
                }

                // Show the "get ready" message box on first frame
                if (!computerTurnMessageBoxShown)
                {
                    computerTurnMessageBoxShown = true;
                    System.Windows.Forms.MessageBox.Show(
                        $"Level {level}\n\nWatch the sequence carefully!\n\nSequence length: {correctSequence.Count}",
                        "Computer Turn",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Information);
                }

                // Increment timer for current tile display
                computerTurnTimer += gameTime.ElapsedGameTime.TotalSeconds;

                // Check if it's time to show the next tile
                if (computerTurnTimer >= computerTileDuration)
                {
                    computerTurnTimer = 0;
                    computerChosenColor = null; // Clear highlight from previous tile
                    computerSequenceIndex++;

                    // If there are more tiles to show, display the next one
                    if (computerSequenceIndex < correctSequence.Count)
                    {
                        computerChosenColor = correctSequence[computerSequenceIndex];
                        Debug.WriteLine($"  ► Playing tile {computerSequenceIndex + 1}/{correctSequence.Count}: {computerChosenColor}");
                    }
                    else
                    {
                        // All tiles have been displayed, switch to player turn
                        isComputerTurn = false;
                        computerSequenceIndex = 0;
                        computerChosenColor = null;
                        computerTurnNewTileAdded = false;
                        computerTurnMessageBoxShown = false;
                        Debug.WriteLine("↓ Switched to Player Turn");
                    }
                }
            }
            #endregion

            #region Player Turn Logic
            else
            {
                // Update player click highlight timer
                if (playerClickedColor != null)
                {
                    playerClickTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (playerClickTimer >= playerClickDuration)
                    {
                        playerClickedColor = null; // Clear the highlight
                    }
                }

                // Handle tile clicks during player's turn
                if (mouseState.LeftButton == XnaButtonState.Pressed)
                {
                    // Only register a click if mouse button was just pressed (not held)
                    if (prevMouseState.LeftButton == XnaButtonState.Released)
                    {
                        HandleTileClick(mouseState.X, mouseState.Y);
                    }
                }
            }
            #endregion

            // Store current mouse state for next frame's comparison
            prevMouseState = mouseState;
            base.Update(gameTime);
        }

        /// <summary>
        /// Draw is called once per frame and renders all game visuals.
        /// Draws the four colored tiles and updates the UI with game state information.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen with cornflower blue background
            GraphicsDevice.Clear(XnaColor.CornflowerBlue);

            // Begin drawing sprites
            spriteBatch.Begin();

            #region Draw Game Tiles
            // Draw each tile, highlighting the computer's chosen tile during computer turn
            // Also highlight the player's clicked tile for visual feedback
            DrawTile(redTile, redTilePos, "Red", 
                (computerChosenColor == "Red" && isComputerTurn) || playerClickedColor == "Red");
            DrawTile(blueTile, blueTilePos, "Blue", 
                (computerChosenColor == "Blue" && isComputerTurn) || playerClickedColor == "Blue");
            DrawTile(orangeTile, orangeTilePos, "Orange", 
                (computerChosenColor == "Orange" && isComputerTurn) || playerClickedColor == "Orange");
            DrawTile(greenTile, greenTilePos, "Green", 
                (computerChosenColor == "Green" && isComputerTurn) || playerClickedColor == "Green");
            #endregion

            #region Draw UI
            // Render text-based UI if font is available
            if (font != null)
            {
                // Draw score in top-left
                spriteBatch.DrawString(font, $"Score: {score}", new Vector2(20, 420), XnaColor.White);

                // Draw level in top-right
                spriteBatch.DrawString(font, $"Level: {level}", new Vector2(300, 420), XnaColor.White);

                // Draw turn indicator (yellow for computer, green for player)
                if (isComputerTurn)
                    spriteBatch.DrawString(font, $"Computer chose: {computerChosenColor}", new Vector2(80, 450), XnaColor.Yellow);
                else
                    spriteBatch.DrawString(font, "Your Turn - Click Tiles", new Vector2(80, 450), XnaColor.LimeGreen);
            }
            else
            {
                // Fallback UI rendering when font is unavailable
                DrawUIFallback();
            }
            #endregion

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        #region Rendering Methods

        /// <summary>
        /// Renders a fallback UI using colored rectangles instead of text.
        /// Called when the SpriteFont fails to load.
        /// Provides visual feedback for score, level, and turn indicator.
        /// </summary>
        private void DrawUIFallback()
        {
            if (redTile == null || blueTile == null || greenTile == null) return;

            // Draw semi-transparent black box for score area (top-left)
            spriteBatch.Draw(redTile, new XnaRectangle(20, 420, 100, 30), XnaColor.Black * 0.5f);

            // Draw semi-transparent black box for level area (top-right)
            spriteBatch.Draw(blueTile, new XnaRectangle(300, 420, 100, 30), XnaColor.Black * 0.5f);

            // Draw semi-transparent colored box for turn indicator (bottom-center)
            // Yellow if computer turn, green if player turn
            if (isComputerTurn)
                spriteBatch.Draw(greenTile, new XnaRectangle(80, 450, 250, 30), XnaColor.Yellow * 0.5f);
            else
                spriteBatch.Draw(greenTile, new XnaRectangle(80, 450, 250, 30), XnaColor.LimeGreen * 0.5f);
        }

        /// <summary>
        /// Draws a single tile at the specified position.
        /// If highlighted, applies brightness increase and draws a glowing white border.
        /// </summary>
        /// <param name="texture">The tile texture to draw</param>
        /// <param name="pos">The screen position (top-left) where the tile will be drawn</param>
        /// <param name="color">The color name (for debugging purposes)</param>
        /// <param name="isHighlighted">If true, brightens the tile and draws a white border</param>
        private void DrawTile(Texture2D? texture, Vector2 pos, string color, bool isHighlighted = false)
        {
            if (texture == null) return;

            // Draw the base tile texture
            // If highlighted, apply 1.5x brightness multiplier
            spriteBatch.Draw(texture, 
                new XnaRectangle((int)pos.X, (int)pos.Y, TILE_SIZE, TILE_SIZE),
                isHighlighted ? XnaColor.White * 1.5f : XnaColor.White);

            // If this tile is the computer's choice, draw a glowing border
            if (isHighlighted)
            {
                DrawBorder(pos, TILE_SIZE, XnaColor.White * 0.8f, 5);
            }
        }

        /// <summary>
        /// Draws a border around a tile by drawing four rectangles (top, bottom, left, right).
        /// Creates a glowing effect when called with semi-transparent white color.
        /// </summary>
        /// <param name="pos">The top-left position of the border</param>
        /// <param name="size">The dimensions of the bordered area (assumed square)</param>
        /// <param name="color">The color of the border lines</param>
        /// <param name="thickness">The width/height of the border lines in pixels</param>
        private void DrawBorder(Vector2 pos, int size, XnaColor color, int thickness)
        {
            if (whiteBorder == null) return;

            // Draw top border
            spriteBatch.Draw(whiteBorder, new XnaRectangle((int)pos.X, (int)pos.Y, size, thickness), color);

            // Draw bottom border
            spriteBatch.Draw(whiteBorder, new XnaRectangle((int)pos.X, (int)pos.Y + size - thickness, size, thickness), color);

            // Draw left border
            spriteBatch.Draw(whiteBorder, new XnaRectangle((int)pos.X, (int)pos.Y, thickness, size), color);

            // Draw right border
            spriteBatch.Draw(whiteBorder, new XnaRectangle((int)pos.X + size - thickness, (int)pos.Y, thickness, size), color);
        }

        #endregion

        #region Game Logic & Input

        /// <summary>
        /// Processes a player's tile click during their turn.
        /// Validates the clicked tile against the expected sequence and updates game state accordingly.
        /// If the player completes the sequence correctly, advances to the next round.
        /// If the player makes a mistake, triggers game over and resets.
        /// </summary>
        /// <param name="mouseX">X coordinate of the mouse click</param>
        /// <param name="mouseY">Y coordinate of the mouse click</param>
        private void HandleTileClick(int mouseX, int mouseY)
        {
            // Convert mouse coordinates to a Vector2
            Vector2 clickPos = new(mouseX, mouseY);

            #region Detect Clicked Tile
            // Determine which tile was clicked by checking collision rectangles
            string? clickedTile = null;
            if (IsTileClicked(clickPos, redTilePos))
                clickedTile = "Red";
            else if (IsTileClicked(clickPos, blueTilePos))
                clickedTile = "Blue";
            else if (IsTileClicked(clickPos, orangeTilePos))
                clickedTile = "Orange";
            else if (IsTileClicked(clickPos, greenTilePos))
                clickedTile = "Green";
            #endregion

            // Only process if a valid tile was clicked
            if (clickedTile != null)
            {
                Debug.WriteLine($"Tile clicked: {clickedTile}");

                // Show visual feedback by highlighting the clicked tile
                playerClickedColor = clickedTile;
                playerClickTimer = 0;

                // Add the clicked tile to the player's sequence
                playerSequence.Add(clickedTile);

                #region Validate Click
                // Check if the clicked tile matches the expected tile at this position
                if (playerSequence.Count <= correctSequence.Count && 
                    playerSequence[playerSequence.Count - 1] == correctSequence[playerSequence.Count - 1])
                {
                    // Correct click! Check if player has completed the entire sequence
                    if (playerSequence.Count == correctSequence.Count)
                    {
                        // Player completed the full sequence correctly!
                        Debug.WriteLine("Sequence correct!");

                        // Increment score and update level
                        score++;
                        level = score + 1;

                        // Clear player input and switch to computer turn
                        // (computer turn will add the next tile to the sequence)
                        playerSequence.Clear();
                        isComputerTurn = true;

                        Debug.WriteLine($"📈 Score: {score}, Level: {level}, Sequence length: {correctSequence.Count}");
                    }
                }
                #endregion

                #region Wrong Click
                else
                {
                    // Player clicked wrong tile or sequence length exceeded
                    Debug.WriteLine($"❌ Wrong tile! Expected: {correctSequence[playerSequence.Count - 1]}, Got: {clickedTile}");
                    Debug.WriteLine("Game Over!");

                    // Reset game to initial state
                    ResetGame();
                }
                #endregion
            }
        }

        /// <summary>
        /// Determines if a click position falls within a tile's rectangular bounds.
        /// </summary>
        /// <param name="clickPos">The mouse click position</param>
        /// <param name="tilePos">The top-left corner of the tile</param>
        /// <returns>True if the click is within the tile; false otherwise</returns>
        private bool IsTileClicked(Vector2 clickPos, Vector2 tilePos)
        {
            // Check if click is within horizontal bounds [tilePos.X, tilePos.X + TILE_SIZE]
            // AND within vertical bounds [tilePos.Y, tilePos.Y + TILE_SIZE]
            return clickPos.X >= tilePos.X && clickPos.X <= tilePos.X + TILE_SIZE &&
                   clickPos.Y >= tilePos.Y && clickPos.Y <= tilePos.Y + TILE_SIZE;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Selects a random tile color from the available options.
        /// Used by the computer to generate the next tile in the sequence.
        /// </summary>
        /// <returns>A randomly selected color name (Red, Blue, Orange, or Green)</returns>
        private string GetRandomTile()
        {
            // Array of available tile colors
            string[] tiles = { "Red", "Blue", "Orange", "Green" };

            // Return a random tile from the array
            return tiles[rnd.Next(tiles.Length)];
        }

        /// <summary>
        /// Resets the game to its initial state.
        /// Called when the player makes a mistake or when starting a new game.
        /// </summary>
        private void ResetGame()
        {
            // Clear both sequences to start fresh
            correctSequence.Clear();
            playerSequence.Clear();

            // Reset score and level to initial values
            score = 0;
            level = 1;

            // Reset computer turn state
            isComputerTurn = true;
            computerSequenceIndex = 0;
            computerChosenColor = null;
            computerTurnTimer = 0;
            computerTurnNewTileAdded = false;
            computerTurnMessageBoxShown = false;

            // Reset player click feedback
            playerClickedColor = null;
            playerClickTimer = 0;

            // Note: correctSequence starts empty; computer will populate it on first turn
        }

        #endregion
    }
}
