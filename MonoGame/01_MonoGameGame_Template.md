# MonoGameGame.cs - Main Game Class Template

Create this file at: `MonoGame/MonoGameGame.cs`

Replace namespace and adapt imports based on your project structure.

```csharp
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using KeepYourFocus.Managers;
using System.Diagnostics;

namespace KeepYourFocus.MonoGame
{
    /// <summary>
    /// Main MonoGame implementation of the KeepYourFocus Simon Says-style memory game.
    /// Handles game initialization, update loop, rendering, and input handling.
    /// </summary>
    public class MonoGameGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // Game Content
        private SpriteFont arial;
        private Texture2D redTile, blueTile, orangeTile, greenTile;
        private Dictionary<string, Texture2D> tileTextures = new();

        // Managers
        private MonoGameSoundManager soundManager;
        private MonoGameTileManager tileManager;

        // Game State
        public List<string> correctOrder = new();
        public List<string> playerOrder = new();
        public List<string> previousTiles = new();

        // Game Variables
        public bool computer = false;
        public bool gameTime = false;
        public int counterSequences = 1;
        public int counterLevels = 1;
        public int counterRounds = 0;
        public int setSequences = 6;

        // Constants
        private const int TILE_SIZE = 150;
        private Vector2 tile1Pos = new Vector2(100, 100);
        private Vector2 tile2Pos = new Vector2(300, 100);
        private Vector2 tile3Pos = new Vector2(100, 300);
        private Vector2 tile4Pos = new Vector2(300, 300);

        public MonoGameGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 600;
            graphics.PreferredBackBufferHeight = 600;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            soundManager = new MonoGameSoundManager();
            tileManager = new MonoGameTileManager();
        }

        protected override void Initialize()
        {
            base.Initialize();
            Window.Title = "Keep Your Focus - MonoGame Edition";
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            try
            {
                arial = Content.Load<SpriteFont>("arial");
                redTile = Content.Load<Texture2D>("Tiles/red_tile512");
                blueTile = Content.Load<Texture2D>("Tiles/blue_tile512");
                orangeTile = Content.Load<Texture2D>("Tiles/orange_tile512");
                greenTile = Content.Load<Texture2D>("Tiles/green_tile512");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Content loading error: {ex.Message}");
                // Create placeholder textures if needed
            }

            soundManager.LoadContent(Content);
            soundManager.PlayStartup();
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            if (keyState.IsKeyDown(Keys.Escape))
                Exit();

            HandleTileInput(mouseState);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            // Draw tiles
            spriteBatch.Draw(redTile, new Rectangle((int)tile1Pos.X, (int)tile1Pos.Y, TILE_SIZE, TILE_SIZE), Color.White);
            spriteBatch.Draw(blueTile, new Rectangle((int)tile2Pos.X, (int)tile2Pos.Y, TILE_SIZE, TILE_SIZE), Color.White);
            spriteBatch.Draw(orangeTile, new Rectangle((int)tile3Pos.X, (int)tile3Pos.Y, TILE_SIZE, TILE_SIZE), Color.White);
            spriteBatch.Draw(greenTile, new Rectangle((int)tile4Pos.X, (int)tile4Pos.Y, TILE_SIZE, TILE_SIZE), Color.White);

            // Draw UI
            if (arial != null)
            {
                spriteBatch.DrawString(arial, $"Level: {counterLevels}", new Vector2(20, 20), Color.White);
                spriteBatch.DrawString(arial, $"Sequence: {counterSequences}/{setSequences}", new Vector2(20, 50), Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private MouseState previousMouseState;

        private void HandleTileInput(MouseState mouseState)
        {
            if (mouseState.LeftButton == ButtonState.Pressed && 
                previousMouseState.LeftButton == ButtonState.Released)
            {
                Vector2 clickPos = new Vector2(mouseState.X, mouseState.Y);

                // Check tile clicks
                if (IsInTile(clickPos, tile1Pos))
                    soundManager.PlayTileSound("Red");
                else if (IsInTile(clickPos, tile2Pos))
                    soundManager.PlayTileSound("Blue");
                else if (IsInTile(clickPos, tile3Pos))
                    soundManager.PlayTileSound("Orange");
                else if (IsInTile(clickPos, tile4Pos))
                    soundManager.PlayTileSound("Green");
            }

            previousMouseState = mouseState;
        }

        private bool IsInTile(Vector2 clickPos, Vector2 tilePos)
        {
            return clickPos.X >= tilePos.X && clickPos.X <= tilePos.X + TILE_SIZE &&
                   clickPos.Y >= tilePos.Y && clickPos.Y <= tilePos.Y + TILE_SIZE;
        }
    }
}
```

## Notes

1. **Namespace Conflicts:** Use fully qualified names for MonoGame types to avoid Windows Forms conflicts
2. **Content Loading:** Ensure `Content` folder exists in project root
3. **Sound Manager:** Needs to be created separately (see MonoGameSoundManager.cs template)
4. **Tile Manager:** Can be adapted from existing TileManager (see MonoGameTileManager.cs template)
