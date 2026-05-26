# MonoGame Setup Checklist

## ✅ STAP 1: NuGet Packages Installeren

### Option A: Package Manager Console (Visual Studio)
```powershell
Install-Package MonoGame.Framework.DesktopGL
```

### Option B: .NET CLI
```bash
cd C:\Users\MichielterElst\OneDrive\ -\ VisieGroepBV\Documenten\GitHub\KeepYourFocus
dotnet add package MonoGame.Framework.DesktopGL
```

### Option C: Edit Focus.csproj Direct
```xml
<ItemGroup>
    <PackageReference Include="MonoGame.Framework.DesktopGL" Version="3.8.1.29" />
</ItemGroup>
```

**Verification:**
```bash
dotnet list package
```
Should show `MonoGame.Framework.DesktopGL` in the output.

---

## ✅ STAP 2: Content Pipeline Setup (Optional)

### Install MGCB Editor
```bash
dotnet tool install --global dotnet-mgcb-editor
```

### Create Content Folder Structure
```
ProjectRoot/
├── Content/
│   ├── Content.mgcb        (Create via MGCB Editor)
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
│   │   └── (other colors)
│   └── Fonts/
│       └── arial.spritefont
```

---

## ✅ STAP 3: Copy Assets

### From Your Current Project:
```
Source: png/ → Destination: Content/Tiles/
Source: sounds/ → Destination: Content/Sounds/
```

**Windows Command:**
```bash
xcopy /I /Y "png\*" "Content\Tiles\"
xcopy /I /Y "sounds\*.wav" "Content\Sounds\"
```

**PowerShell:**
```powershell
Copy-Item "png\*" "Content\Tiles\" -Recurse -Force
Copy-Item "sounds\*.wav" "Content\Sounds\" -Recurse -Force
```

---

## ✅ STAP 4: Create Spritefont (Optional)

Create `Content/arial.spritefont`:
```xml
<?xml version="1.0" encoding="utf-8"?>
<XnaContent xmlns:Graphics="Microsoft.Xna.Framework.Content.Pipeline.Graphics">
  <Asset Type="Graphics:FontDescription">
    <FontName>Arial</FontName>
    <Size>14</Size>
    <Spacing>0</Spacing>
    <UseKerning>true</UseKerning>
    <CharacterRegions>
      <CharacterRegion>
        <Start>&#32;</Start>
        <End>&#126;</End>
      </CharacterRegion>
    </CharacterRegions>
  </Asset>
</XnaContent>
```

---

## ✅ STAP 5: Project File Updates

Edit `Focus.csproj`:

### A. Remove Windows Forms (Optional)
```xml
<!-- BEFORE -->
<UseWindowsForms>true</UseWindowsForms>

<!-- AFTER (if switching to MonoGame only) -->
<UseWindowsForms>false</UseWindowsForms>
```

### B. Add Content Pipeline Target (Optional)
```xml
<ItemGroup>
  <MonoGameContentReference Include="Content\Content.mgcb" />
</ItemGroup>

<Import Project="$(MSBuildExtensionsPath)\MonoGame\v3.0\MonoGame.Content.Builder.targets" />
```

---

## ✅ STAP 6: Build & Test

### Build Project
```bash
dotnet build
```

**Expected Output:**
```
✓ Build succeeded
✓ 0 errors
```

### Run Tests
```bash
dotnet run
```

**Expected Result:**
- Game window opens (MonoGame window)
- Background color shows (CornflowerBlue)
- Can close with Escape key

---

## ✅ STAP 7: Switch to MonoGame Version

### Option A: Replace Program.cs
```csharp
// Program.cs
using KeepYourFocus.MonoGame;

static void Main()
{
    using (var game = new MonoGameGame())
        game.Run();
}
```

### Option B: Conditional Compilation
```csharp
// Program.cs
#if MONOGAME
    using KeepYourFocus.MonoGame;

    static void Main()
    {
        using (var game = new MonoGameGame())
            game.Run();
    }
#else
    // Windows Forms version
    namespace KeepYourFocus
    {
        internal static class Program
        {
            [STAThread]
            static void Main()
            {
                ApplicationConfiguration.Initialize();
                Application.Run(new Focus());
            }
        }
    }
#endif
```

### Option C: Command Line Switch
```bash
dotnet run -c Release /p:DefineConstants="MONOGAME"
```

---

## ✅ STAP 8: Verify Asset Loading

### Test Code:
```csharp
protected override void LoadContent()
{
    spriteBatch = new SpriteBatch(GraphicsDevice);

    try
    {
        // Test texture loading
        var texture = Content.Load<Texture2D>("Tiles/red_tile512");
        System.Diagnostics.Debug.WriteLine("✓ Texture loaded successfully");
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"✗ Failed to load texture: {ex.Message}");
    }

    try
    {
        // Test sound loading
        var sound = Content.Load<SoundEffect>("Sounds/beep");
        System.Diagnostics.Debug.WriteLine("✓ Sound loaded successfully");
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"✗ Failed to load sound: {ex.Message}");
    }
}
```

**Check Output Window** for success/error messages.

---

## ✅ STAP 9: Common Issues & Fixes

### Issue: "MonoGame not found"
```
Solution: Run: dotnet add package MonoGame.Framework.DesktopGL
```

### Issue: "Content.Load() returns null"
```
Solution: 
1. Check Content folder exists in project root
2. Verify file paths in Load() match actual files
3. Run: dotnet build-content (if using MGCB)
```

### Issue: "Window won't display"
```
Solution:
1. Check graphics initialization in constructor
2. Verify base.Initialize() and base.LoadContent() are called
3. Check graphics.PreferredBackBufferWidth/Height are set
```

### Issue: "Sound doesn't play"
```
Solution:
1. Ensure WAV files are in Content/Sounds/
2. Load with correct path: Content.Load<SoundEffect>("Sounds/beep")
3. Check soundEffect.Play() is called
4. Verify audio device is not muted
```

### Issue: "Game runs but no graphics shown"
```
Solution:
1. Check GraphicsDevice.Clear() is called in Draw()
2. Verify spriteBatch.Begin() and End() wrap drawing
3. Check base.Draw() is called
4. Ensure textures are loaded in LoadContent()
```

---

## ✅ STAP 10: Recommended Development Order

### Phase 1: Foundation (1-2 hours)
- [ ] Install MonoGame package
- [ ] Create MonoGameGame.cs with empty Update/Draw
- [ ] Draw 4 colored tiles
- [ ] Get input working (click detection)
- [ ] Test build & run

### Phase 2: Basic Gameplay (2-3 hours)
- [ ] Add sound manager
- [ ] Implement tile click handling
- [ ] Create sequence validation logic
- [ ] Add score/level display
- [ ] Test complete game loop

### Phase 3: Game Flow (2-3 hours)
- [ ] Implement computer turn animation
- [ ] Add game over detection
- [ ] Create main menu screen
- [ ] Add difficulty selection
- [ ] Test full workflow

### Phase 4: Polish (2-3 hours)
- [ ] Add tile highlight animations
- [ ] Improve UI rendering
- [ ] Add high score display
- [ ] Create game over screen
- [ ] Sound effect tweaks

### Phase 5: Optimization (1 hour)
- [ ] Performance profiling
- [ ] Memory management
- [ ] Cross-platform testing (if needed)
- [ ] Final bug fixes

---

## ✅ STAP 11: Deployment Options

### Windows Desktop
```bash
dotnet publish -c Release -r win-x64
```

### Linux
```bash
dotnet publish -c Release -r linux-x64
```

### macOS
```bash
dotnet publish -c Release -r osx-x64
```

---

## ✅ STAP 12: Final Checklist

Before considering MonoGame migration complete:

- [ ] Game window opens without errors
- [ ] All 4 tiles display correctly
- [ ] Mouse clicks register on tiles
- [ ] Tile sounds play when clicked
- [ ] Sequence generation works
- [ ] Correct sequence validation works
- [ ] Level progression works
- [ ] Score tracking works
- [ ] Game over is detected
- [ ] Game can be restarted
- [ ] All original features replicated
- [ ] No performance issues (60 FPS stable)
- [ ] Builds successfully without warnings
- [ ] Tested on target platform(s)

---

## 📚 Resources

- **MonoGame Official Docs:** https://docs.monogame.net/
- **MonoGame Content Pipeline:** https://docs.monogame.net/articles/tools/mgcb_editor.html
- **XNA Framework Samples:** https://github.com/Microsoft/MonoGame-Samples
- **Community Forum:** https://community.monogame.net/

---

## 🎯 Success Indicators

✅ You're ready when:
- `dotnet build` succeeds with no errors
- `dotnet run` opens a MonoGame window
- Assets load without errors in Debug output
- Basic gameplay works (click tiles, sequence validates)
- No exceptions in runtime

🚀 You're done when:
- All features from Windows Forms version work in MonoGame
- Performance is acceptable (no lag, smooth animations)
- Tested and working on your target platform
- Code is clean and documented

---

## 💡 Pro Tips

1. **Start Minimal:** Use QUICK_START_MINIMAL.cs as your base
2. **Test Incrementally:** Add one feature, test, then move to next
3. **Use Debug.WriteLine():** For troubleshooting without MessageBox
4. **Keep Game Logic Separate:** Reuse managers from Windows Forms version
5. **Comment Your Code:** MonoGame is different - document the why
6. **Version Control:** Commit after each working feature

---

**Date Created:** 2024
**Status:** Ready for Implementation
**Time Estimate:** 8-12 hours for full migration
**Difficulty:** Medium (if you know .NET and C#)
