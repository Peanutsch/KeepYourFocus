# Windows Forms ↔ MonoGame Architecture Comparison

## Side-by-Side: Key Differences

### 1. ENTRY POINT

**Windows Forms:**
```csharp
[STAThread]
static void Main()
{
    ApplicationConfiguration.Initialize();
    Application.Run(new Focus()); // Form-based
}
```

**MonoGame:**
```csharp
static void Main()
{
    using (var game = new MonoGameGame()) // Game-based
        game.Run();
}
```

---

### 2. GAME LOOP

**Windows Forms:**
```
Constructor
    ↓
Form.Shown event
    ↓
Event-driven: OnClick, OnMouseMove, OnKeyDown
    ↓
Timer tick events (for animations)
    ↓
Manual redraw with Control.Invalidate()
```

**MonoGame:**
```
Initialize()
    ↓
LoadContent()
    ↓
[Update() → Draw()] LOOP (60 FPS default)
    ↓
Continuous render cycle
```

---

### 3. TILE MANAGEMENT

**Windows Forms:**
```csharp
PictureBox[] pictureBoxes = new[] { 
    pictureBox1, pictureBox2, pictureBox3, pictureBox4 
};

public void HighlightTile(PictureBox pb, bool highlight)
{
    pb.BorderStyle = BorderStyle.FixedSingle;
    pb.BackColor = highlight ? Color.White : Color.Transparent;
}
```

**MonoGame:**
```csharp
Dictionary<string, Vector2> TilePositions = new()
{
    { "Red", new Vector2(50, 50) },
    { "Blue", new Vector2(250, 50) },
    // ...
};

private void HighlightTile(string tile, bool highlight)
{
    // Track in highlightedTile variable
    // Render with different color in Draw()
}
```

**Key Difference:** 
- Windows Forms = Direct UI control manipulation
- MonoGame = Data structures + rendering pipeline

---

### 4. SOUND PLAYBACK

**Windows Forms:**
```csharp
using System.Media;

private readonly SoundPlayer tileBeepSound;

public SoundManager()
{
    tileBeepSound = new SoundPlayer(pathToFile);
}

public void PlayTileSound(string tile)
{
    tileBeepSound.Play();
}
```

**MonoGame:**
```csharp
using Microsoft.Xna.Framework.Audio;

private SoundEffect tileBeepSound;

public void LoadContent(ContentManager content)
{
    tileBeepSound = content.Load<SoundEffect>("Sounds/beep");
}

public void PlayTileSound(string tile)
{
    tileBeepSound.Play();
}
```

**Key Difference:**
- Windows Forms = Direct file loading from disk
- MonoGame = Content Pipeline with preprocessing

---

### 5. GAME STATE FLAGS

**Windows Forms:**
```csharp
public bool computer = false;
public bool startButton = true;
bool isComputerTurn = false;
bool isPlayerTurn = false;
bool isDisplaySequence = false;
bool isSetCounters = false;
```

**MonoGame:**
```csharp
enum GamePhase
{
    Menu,
    ComputerTurn,
    PlayerTurn,
    LevelUp,
    GameOver
}

GamePhase currentPhase = GamePhase.Menu;
```

**Key Difference:**
- Windows Forms = Multiple boolean flags (error-prone)
- MonoGame = Single enum (safer, clearer)

---

### 6. INPUT HANDLING

**Windows Forms:**
```csharp
private void pictureBox1_Click(object sender, EventArgs e)
{
    OnTileClicked("Red");
}

private void Form_KeyDown(object sender, KeyEventArgs e)
{
    if (e.KeyCode == Keys.Escape) Close();
}
```

**MonoGame:**
```csharp
protected override void Update(GameTime gameTime)
{
    KeyboardState keyState = Keyboard.GetState();
    MouseState mouseState = Mouse.GetState();

    if (keyState.IsKeyDown(Keys.Escape))
        Exit();

    HandleTileInput(mouseState);
}
```

**Key Difference:**
- Windows Forms = Event-driven (async)
- MonoGame = Poll-based (synchronous in Update loop)

---

### 7. RENDERING

**Windows Forms:**
```csharp
// Implicit - handled by framework
pictureBox.Image = Image.FromFile(path);
pictureBox.Visible = true;

// Custom painting
protected override void OnPaint(PaintEventArgs e)
{
    e.Graphics.DrawString(...);
}
```

**MonoGame:**
```csharp
protected override void Draw(GameTime gameTime)
{
    GraphicsDevice.Clear(Color.CornflowerBlue);

    spriteBatch.Begin();

    spriteBatch.Draw(texture, position, Color.White);
    spriteBatch.DrawString(font, text, position, Color.White);

    spriteBatch.End();
}
```

**Key Difference:**
- Windows Forms = Implicit, layered controls
- MonoGame = Explicit draw order, batch rendering

---

### 8. TIMING & DELAYS

**Windows Forms:**
```csharp
// Using Task.Delay() for async waits
private async Task DisplaySequence()
{
    foreach (string tile in correctOrder)
    {
        HighlightTile(tile, true);
        soundManager.PlayTileSound(tile);
        await Task.Delay(500); // Half-second delay
        HighlightTile(tile, false);
    }
}
```

**MonoGame:**
```csharp
// Using GameTime.ElapsedGameTime
private double sequenceTimer = 0;

private void HandleComputerTurn(GameTime gameTime)
{
    sequenceTimer += gameTime.ElapsedGameTime.TotalSeconds;

    if (sequenceTimer > 0.5) // Half-second
    {
        sequenceTimer = 0;
        // Show next tile
    }
}
```

**Key Difference:**
- Windows Forms = Async/await with Task.Delay()
- MonoGame = Frame-based timing with GameTime

---

### 9. UI ELEMENTS

**Windows Forms:**
```csharp
// Built-in controls
Button startBTN = new Button();
TextBox playerNameInput = new TextBox();
RichTextBox scoreDisplay = new RichTextBox();
CheckedListBox difficultyOptions = new CheckedListBox();

// Automatic layout & styling
startBTN.Text = "Start";
startBTN.Visible = true;
startBTN.Click += StartButton_Click;
```

**MonoGame:**
```csharp
// Manual rendering & layout
private Rectangle startButtonRect = new Rectangle(100, 200, 200, 50);
private bool startButtonHovered = false;

private void DrawButtons()
{
    Color btnColor = startButtonHovered ? Color.LimeGreen : Color.Green;
    spriteBatch.Draw(buttonTexture, startButtonRect, btnColor);
    spriteBatch.DrawString(arial, "Start", startButtonPos, Color.White);
}

private void HandleButtonClick(Vector2 clickPos)
{
    if (startButtonRect.Contains(clickPos.ToPoint()))
    {
        OnStartClicked();
    }
}
```

**Key Difference:**
- Windows Forms = High-level controls with events
- MonoGame = Low-level shapes, text, rectangles + manual hit detection

---

### 10. FILE I/O

**Both Are Similar:**
```csharp
// ScoreManager.cs works in both versions
File.ReadAllLines(path);
File.WriteAllText(path, content);

// PathHelper.cs works in both versions
Path.Combine(directory, file);
```

**Difference in Asset Loading:**
- Windows Forms: `Image.FromFile(path)` - loads from disk directly
- MonoGame: `Content.Load<T>(assetName)` - uses Content Pipeline

---

## Migration Checklist

- [x] Game loop structure (event-driven → frame-based)
- [x] Tile rendering (PictureBox → Texture2D + Vector2)
- [x] Sound management (SoundPlayer → SoundEffect)
- [x] Input handling (event handlers → polling)
- [x] State management (multiple flags → enum)
- [ ] UI rendering (built-in controls → manual drawing)
- [ ] Game Over screen (MessageBox → overlay UI)
- [ ] High score display (RichTextBox → custom rendering)
- [ ] Difficulty menu (CheckedListBox → button-based)
- [ ] Animation timing (Task.Delay → GameTime)

---

## Performance Considerations

| Aspect | Windows Forms | MonoGame |
|--------|---------------|----------|
| Rendering | On-demand | Continuous 60 FPS |
| CPU Usage | Lower (event-driven) | Consistent |
| Graphics | GDI+ | DirectX/OpenGL |
| Scalability | Limited | Excellent |
| Cross-platform | Windows only | Multiple OS |

---

## Memory Management

**Windows Forms:**
```csharp
// Must manually dispose resources
pictureBox.Image?.Dispose();
Form.Dispose();
```

**MonoGame:**
```csharp
// using statement handles disposal
using (var game = new MonoGameGame())
    game.Run();

// Content.Load automatically manages
// But should explicitly unload when needed
```

---

## Debugging Tips

**Windows Forms:**
- Set breakpoints in event handlers
- Use Visual Studio Form Designer
- MessageBox for quick debugging

**MonoGame:**
- Debug.WriteLine() → Output Window
- Visual Studio Graphics Debugger for rendering issues
- Use GameTime to measure frame times

---

## Recommended Migration Path

1. **Start with basic structure**
   - MonoGameGame class with empty Update/Draw
   - Load assets in LoadContent

2. **Implement tile rendering**
   - Draw static tiles in Draw()
   - Verify texture loading

3. **Add input handling**
   - Detect tile clicks
   - Play sounds

4. **Implement game loop**
   - Sequence display
   - Player validation
   - State transitions

5. **Polish UI**
   - Menus, buttons
   - Score display
   - Game over screen

6. **Add animations**
   - Tile highlights
   - Level transitions
   - Score animations
