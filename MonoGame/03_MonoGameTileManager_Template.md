# MonoGameTileManager.cs - Tile Management Template

Create this file at: `MonoGame/MonoGameTileManager.cs`

```csharp
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace KeepYourFocus.MonoGame
{
    /// <summary>
    /// MonoGame version of TileManager - manages tile state and positioning.
    /// Adapted from Windows Forms PictureBox-based version to use Vector2 positions.
    /// </summary>
    public class MonoGameTileManager
    {
        public Dictionary<string, Vector2> TilePositions { get; set; } = new();

        private Dictionary<string, Vector2> originalPositions = new();
        private readonly Random rnd = new();

        public MonoGameTileManager()
        {
            InitializeDefaultPositions();
        }

        /// <summary>
        /// Sets up default tile positions for a 2x2 grid layout.
        /// </summary>
        private void InitializeDefaultPositions()
        {
            const int tileSize = 150;
            const int spacing = 50;

            originalPositions = new Dictionary<string, Vector2>
            {
                { "Red", new Vector2(spacing, spacing) },
                { "Blue", new Vector2(spacing + tileSize + spacing, spacing) },
                { "Orange", new Vector2(spacing, spacing + tileSize + spacing) },
                { "Green", new Vector2(spacing + tileSize + spacing, spacing + tileSize + spacing) }
            };

            ResetTilePositions();
        }

        /// <summary>
        /// Returns all tile positions to their original layout.
        /// </summary>
        public void ResetTilePositions()
        {
            TilePositions = new Dictionary<string, Vector2>(originalPositions);
        }

        /// <summary>
        /// Shuffles current tiles to random positions.
        /// Uses Fisher-Yates algorithm for unbiased shuffling.
        /// </summary>
        public void ShuffleTilePositions()
        {
            List<string> tileNames = new() { "Red", "Blue", "Orange", "Green" };
            List<Vector2> positions = new(originalPositions.Values);

            // Fisher-Yates shuffle
            for (int i = positions.Count - 1; i > 0; i--)
            {
                int randomIndex = rnd.Next(i + 1);
                (positions[i], positions[randomIndex]) = (positions[randomIndex], positions[i]);
            }

            // Reassign positions
            for (int i = 0; i < tileNames.Count; i++)
            {
                TilePositions[tileNames[i]] = positions[i];
            }

            Debug.WriteLine("[MonoGameTileManager] Tiles shuffled successfully");
        }

        /// <summary>
        /// Gets the tile color name at a given screen position (with tolerance for hit detection).
        /// </summary>
        public string GetTileAtPosition(Vector2 position, float tolerance = 75)
        {
            foreach (var tile in TilePositions)
            {
                float distance = Vector2.Distance(tile.Value, position);
                if (distance < tolerance)
                    return tile.Key;
            }
            return null;
        }

        /// <summary>
        /// Returns all available tile color names that can be displayed.
        /// </summary>
        public static List<string> GetAvailableTiles()
        {
            return new List<string>
            {
                "Red", "Blue", "Orange", "Green",
                "CaribBlue", "Grey", "Indigo", "Maroon", "Olive", "Pink"
            };
        }
    }
}
```

## Usage Example

```csharp
// In MonoGameGame.cs

private MonoGameTileManager tileManager = new();

// In Update()
private void HandleTileInput(MouseState mouseState)
{
    if (mouseState.LeftButton == ButtonState.Pressed)
    {
        Vector2 clickPos = new Vector2(mouseState.X, mouseState.Y);
        string clickedTile = tileManager.GetTileAtPosition(clickPos, 75);

        if (clickedTile != null)
        {
            // Process tile click
            OnTileClicked(clickedTile);
        }
    }
}

// Shuffle tiles when needed
private void OnLevelUp()
{
    tileManager.ShuffleTilePositions();
}
```

## Key Differences from Windows Forms Version

| Aspect | Windows Forms | MonoGame |
|--------|---------------|----------|
| Position Storage | PictureBox.Location | Vector2 |
| Click Detection | PictureBox.Click event | Manual Rectangle intersection |
| Layout Management | Designer/Anchors | Manual Vector2 positions |
| Tile Replacement | Replace PictureBox.Image | Track tile name changes |
