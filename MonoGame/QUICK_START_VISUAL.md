# 🎮 MONOGAME SPEL - QUICK START VISUAL GUIDE

## 3 SIMPELE STAPPEN

### STAP 1️⃣: DRUK OP F5
```
Visual Studio → [Start/Play Button] of Druk F5
```

**Wat gebeurt:**
- Visual Studio bouwt het project
- MonoGame window opent
- Game begint!

---

### STAP 2️⃣: KLIK OP TILES
```
Game Screen:

    [RED]      [BLUE]

  [ORANGE]    [GREEN]

    ↓ Klik erop! ↓
```

**Besturing:**
- **LEFT CLICK** → Klik op tile
- **ESC** → Sluit af

---

### STAP 3️⃣: SPEEL!
```
Your Turn - Click Tiles (in groen geschreven)

Score: 0          Level: 1

Klik in de juiste volgorde!
```

**Hoe het werkt:**
1. Computer genereert een volgorde
2. Jij klikt op tiles in die volgorde
3. Voor elke goede sequentie +1 punt
4. Falout? Game Over!

---

## 🎯 VOLLEDIGE GAME FLOW

```
┌─────────────────────────────────────┐
│    GAME START                       │
│    Initiële sequentie: [Red]        │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│    COMPUTER'S TURN                  │
│    (Text is geel)                   │
│    Geeft sequentie aan              │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│    YOUR TURN                        │
│    (Text is groen)                  │
│    Wacht op jouw klik               │
└──────┬──────────────────┬───────────┘
       │                  │
       │ CORRECT          │ WRONG
       ▼                  ▼
┌─────────────────────────────────────┐
│    SCORE +1                         │
│    Add new tile to sequence         │
│    Back to COMPUTER'S TURN          │
└─────────────────────────────────────┘
       vs          ┌─────────────────────────────────────┐
                   │    GAME OVER!                       │
                   │    Score & Level shown              │
                   │    Game resets                      │
                   └─────────────────────────────────────┘
```

---

## 🎨 SCHERM LAYOUT

```
┌─────────────────────────────┐
│     Keep Your Focus         │ ← Title
├─────────────────────────────┤
│                             │
│  [RED]      [BLUE]          │
│                             │
│ [ORANGE]   [GREEN]          │
│                             │
├─────────────────────────────┤
│ Score: 0      Level: 1      │
│ Computer's Turn (geel)      │
└─────────────────────────────┘
```

---

## ⌨️ TOETSENBORD SHORTCUTS

| Toets | Functie |
|-------|---------|
| **F5** | Start game (Visual Studio) |
| **ESC** | Sluit game af |
| **ALT+F4** | Force close |
| **CTRL+F5** | Run zonder debugger |
| **Left Click** | Klik op tile |

---

## 🔧 DIRECT PROBLEMEN FIXEN

### Problem: Game opent niet
```
✅ Solution:
1. Sluit Visual Studio volledig
2. Open opnieuw
3. Clean: CTRL+ALT+B dan Clean Solution
4. Build: CTRL+SHIFT+B
5. Run: F5
```

### Problem: Tiles reageren niet
```
✅ Solution:
1. Check game window in focus (blauwe titel bar)
2. Klik op midden van tile
3. Probeer opnieuw
```

### Problem: "MonoGameGame not found"
```
✅ Solution:
1. Check MonoGame/MonoGameGame.cs bestaat
2. Rebuild solution: CTRL+SHIFT+B
3. Ziet Visual Studio het bestand?
```

---

## 📊 DEBUG OUTPUT CHECKEN

**Waar:** Visual Studio → View → Output (of CTRL+ALT+O)

**Expected messages:**
```
✓ Game initialized
✓ All assets loaded
Tile clicked: Red
Sequence correct!
Wrong tile! Game Over!
```

---

## 🎮 GAME TIPS

**Pro Tips:**
- Reageer snel! Tiles worden sneller naarmate je vordert
- Concentreer je op het patroon
- Meer klicks = Hoger score
- Game slaat geen tiles over

**Bonus:**
- Score = Aantal correcte sequenties
- Level = Score + 1
- Iedere tile toevoegen = +1 length

---

## 🚀 TERUG NAAR WINDOWS FORMS (Optioneel)

Als je terug wilt naar het originele spel:

```
1. Open Program.cs
2. Comment uit:
   // using (var game = new MonoGameGame())
   // game.Run();

3. Uncomment:
   ApplicationConfiguration.Initialize();
   Application.Run(new Focus());

4. Run: F5
```

---

## 📈 VOLGENDE STAP

Wanneer je het spel speelt en het werkt:

**Optie 1: Geluid Toevoegen**
- Copy .wav files naar Content/Sounds/
- Voeg MonoGameSoundManager toe

**Optie 2: Eigen Assets**
- Copy PNG files naar Content/Tiles/
- Copy WAV files naar Content/Sounds/
- Update code om ze te laden

**Optie 3: Geavanceerde Features**
- Computer turn animations
- Menu system
- High score storage
- Difficulty levels

---

## 💾 BESTANDEN DIE VERANDERD ZIJN

```
✅ Program.cs
   → Nu MonoGame game laden

✅ MonoGame/MonoGameGame.cs
   → Nieuwe game class

✅ Focus.csproj
   → MonoGame dependencies added
```

---

## 🎯 CHECKLIST

```
[ ] F5 ingedrukt
[ ] Game window opent
[ ] 4 tiles zichtbaar
[ ] Score/Level display zichtbaar
[ ] Kunnen klikken op tiles
[ ] Game reageert correct/wrong
[ ] ESC sluit af
[ ] WERKT! 🎉
```

---

**TL;DR:**
```
F5 → Speel → Veel plezier! 🎮
```

Veel succes! 🚀
