# MonoGame Conversion Guide for Keep Your Focus

## Overview

This folder contains a **MonoGame template** for converting the Windows Forms version of "Keep Your Focus" to a cross-platform MonoGame application.

### What's Included

- **MonoGameGame.cs** - Main game class inheriting from MonoGame's Game class
- **MonoGameSoundManager.cs** - Sound effects manager using MonoGame audio
- **MonoGameTileManager.cs** - Tile/board management system adapted for MonoGame
- **MonoGameProgram.cs** - Program entry point template

## Key Features

✅ **Game Logic Preserved**
- All tile sequences and game state management preserved from Windows Forms version
- Scoring system compatible (uses same ScoreManager)
- Sound system adapted to MonoGame's audio framework

✅ **Cross-Platform Ready**
- MonoGame supports Windows, macOS, Linux, and mobile platforms
- Uses Vector2 positions instead of Windows Forms control positions
- Graphics rendered through MonoGame's SpriteBatch

✅ **Modular Design**
- Manager classes can be reused from the original version
- Existing Helpers (PathHelper, ScoreManager) still compatible
- Easy to extend with new features

## Setup Instructions

### 1. Install MonoGame NuGet Package

```bash
cd C:\path\to\KeepYourFocus
dotnet add package MonoGame.Framework.DesktopGL
```

**Alternative:** If using Visual Studio Package Manager Console:
```
Install-Package MonoGame.Framework.DesktopGL
```

### 2. Set Up MonoGame Content Pipeline (Optional but Recommended)

For full MonoGame pipeline support with Content Manager:

```bash
dotnet tool install --global dotnet-mgcb-editor
mgcb-editor
```

Or in Visual Studio:
- Create a `Content` folder in the project root
- Add a `Content.mgcb` file
- Import PNG images and WAV sounds through the MGCB Editor

### 3. Create Asset Directories

```
ProjectRoot/
├── Content/
│   ├── Sounds/
│   │   ├── beep.wav
│   │   ├── buttonclick.wav
│   │   ├── correct.wav
│   │   ├── transistion.wav
│   │   ├── wrong.wav
│   │   └── startupSound.wav
│   ├── Tiles/
│   │   ├── red_tile512.png
│   │   ├── blue_tile512.png
│   │   ├── orange_tile512.png
│   │   ├── green_tile512.png
│   │   └── (other color tiles)
│   └── Fonts/
│       └── arial.spritefont
```

### 4. Switch to MonoGame Version

To run the MonoGame version instead of Windows Forms:

**Option A: Modify Program.cs**
```csharp
// In Program.cs, replace the current Main with:
[STAThread]
static void Main()
{
    using (var game = new MonoGameGame())
        game.Run();
}
```

**Option B: Use MSBuild Conditional**
```xml
<!-- In Focus.csproj -->
<PropertyGroup>
    <DefineConstants>MONOGAME</DefineConstants>
</PropertyGroup>
```

Then use `#if MONOGAME` directives to switch versions.

## Architecture Comparison

### Windows Forms Version
```
Focus (Form)
├── TileManager (PictureBox array)
├── SoundManager (System.Media.SoundPlayer)
├── ActionManager
└── ScoreManager
```

### MonoGame Version
```
MonoGameGame (Game)
├── MonoGameTileManager (Vector2 positions)
├── MonoGameSoundManager (SoundEffect)
├── ActionManager (reused)
└── ScoreManager (reused)
```

## Converting Additional Features

### 1. Game Over Screen
Currently: Uses MessageBox
Need to implement: MonoGame UI overlay

```csharp
// TODO: Add UI rendering in MonoGameGame.Draw()
private void DrawGameOverScreen(SpriteBatch batch)
{
    // Draw semi-transparent overlay
    // Draw score, player name input, restart button
}
```

### 2. Tile Animations
MonoGame makes it easy to add animations:

```csharp
private void AnimateTileClick(string tile, GameTime gameTime)
{
    // Implement scaling/fading animation
    // Update tile position and opacity
}
```

### 3. Difficulty Selection UI
Current: CheckedListBox in Windows Forms
Need to implement: MonoGame button/menu system

```csharp
private void DrawDifficultyMenu(SpriteBatch batch)
{
    // Draw Easy, Default, Hard buttons
    // Handle mouse input
}
```

### 4. High Score Display
Current: RichTextBox in Windows Forms
Need to implement: MonoGame text rendering with formatting

## Using Existing Managers

The following classes from the original project can be reused directly:

✅ **ScoreManager.cs** - No changes needed
✅ **PathHelper.cs** - No changes needed
✅ **TaskExtensions.cs** - May need updates for async/await patterns

### Classes That Need Adaptation

❌ **TileManager.cs** - Windows Forms specific (PictureBox)
   → **MonoGameTileManager.cs** - MonoGame version provided

❌ **SoundManager.cs** - Uses System.Media.SoundPlayer
   → **MonoGameSoundManager.cs** - MonoGame version provided

## Game Loop Flow

### MonoGame Update Cycle
```
Initialize() → LoadContent() → [Update() → Draw()] (loop)
```

### Game State Management
```csharp
// Computer's Turn Phase
if (isComputerTurn && !actionTaken)
{
    // Display sequence
    // Wait for player input
    // Transition to player turn
}

// Player's Turn Phase
if (isPlayerTurn && !actionTaken)
{
    // Wait for click input
    // Validate sequence
    // Handle correct/incorrect
}
```

## Asset Loading

### Two Approaches:

**1. MonoGame Content Pipeline (Recommended)**
```csharp
// In LoadContent()
texture = Content.Load<Texture2D>("Tiles/red_tile512");
sound = Content.Load<SoundEffect>("Sounds/beep");
```

**2. Direct File Loading (Fallback)**
```csharp
// Using System.IO
texture = Texture2D.FromStream(GraphicsDevice, File.OpenRead("path/to/image.png"));
```

## Troubleshooting

### Issue: Content not loading
**Solution:** Ensure Content folder is in the project root and paths in Load() match file names

### Issue: Sound not playing
**Solution:** Check that WAV files are in Content/Sounds and paths are correct

### Issue: Game window won't appear
**Solution:** Check graphics device initialization and ensure window width/height are reasonable

### Issue: Mouse input not working
**Solution:** Verify `IsMouseVisible = true` in constructor

## Next Steps for Full Implementation

1. **Implement Game Over Screen**
   - Add UI rendering for score display
   - Implement player name input
   - Add restart button

2. **Add Difficulty Selection Menu**
   - Create startup menu screen
   - Handle difficulty selection
   - Initialize game with selected difficulty

3. **Improve Tile Animations**
   - Add click animations (scale/flash effect)
   - Add sequence display animations
   - Add level-up transition effects

4. **Create Main Menu**
   - Game title display
   - Start/Quit buttons
   - Settings menu

5. **Cross-Platform Testing**
   - Test on Linux/macOS if targeting those platforms
   - Adjust content paths for different OS separators

## References

- [MonoGame Documentation](https://docs.monogame.net/)
- [MonoGame Content Pipeline](https://docs.monogame.net/articles/tools/mgcb_editor.html)
- [XNA Framework (MonoGame basis)](https://learn.microsoft.com/en-us/windows/uwp/gaming/introduction)

## Support

For issues or questions:
- Check the original project: https://github.com/Peanutsch/KeepYourFocus
- Review MonoGame examples and documentation
- Test incrementally with simplified versions

---

**Status:** Template Ready for Implementation
**Last Updated:** 2024
**Compatibility:** .NET 8, MonoGame 3.8+
