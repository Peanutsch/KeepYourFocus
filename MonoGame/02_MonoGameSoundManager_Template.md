# MonoGameSoundManager.cs - Sound Management Template

Create this file at: `MonoGame/MonoGameSoundManager.cs`

```csharp
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace KeepYourFocus.MonoGame
{
    /// <summary>
    /// MonoGame-based sound manager using SoundEffect for cross-platform audio support.
    /// Replaces System.Media.SoundPlayer from Windows Forms version.
    /// </summary>
    public class MonoGameSoundManager
    {
        private SoundEffect tileBeepSound;
        private SoundEffect transitionSound;
        private SoundEffect buttonClickSound;
        private SoundEffect wrongSound;
        private SoundEffect correctSound;
        private SoundEffect startupSound;

        private bool soundsLoaded = false;

        /// <summary>
        /// Loads all sound effects from the MonoGame Content pipeline.
        /// Call this in Game.LoadContent()
        /// </summary>
        public void LoadContent(ContentManager content)
        {
            try
            {
                tileBeepSound = content.Load<SoundEffect>("Sounds/beep");
                transitionSound = content.Load<SoundEffect>("Sounds/transistion");
                buttonClickSound = content.Load<SoundEffect>("Sounds/buttonclick");
                wrongSound = content.Load<SoundEffect>("Sounds/wrong");
                correctSound = content.Load<SoundEffect>("Sounds/correct");
                startupSound = content.Load<SoundEffect>("Sounds/startupSound");

                soundsLoaded = true;
                Debug.WriteLine("✓ All sounds loaded successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Error loading sounds: {ex.Message}");
                soundsLoaded = false;
            }
        }

        public void PlayTileSound(string tile) => PlayIfLoaded(tileBeepSound);
        public void PlayTransition() => PlayIfLoaded(transitionSound);
        public void PlayButtonClick() => PlayIfLoaded(buttonClickSound);
        public void PlayWrong() => PlayIfLoaded(wrongSound);
        public void PlayCorrect() => PlayIfLoaded(correctSound);
        public void PlayStartup() => PlayIfLoaded(startupSound);

        private void PlayIfLoaded(SoundEffect sound)
        {
            if (soundsLoaded && sound != null)
                sound.Play();
        }
    }
}
```

## Asset Folder Structure

```
ProjectRoot/
├── Content/
│   ├── Content.mgcb          (MonoGame Content Builder file)
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
│   │   └── green_tile512.png
│   └── Fonts/
│       └── arial.spritefont (or other font)
```

## Content.mgcb Setup

If using MGCB Editor, add assets like:

```
/importer:FontDescriptionImporter
/processor:FontDescriptionProcessor
/processorParam:PremultiplyAlpha=True
/build:Fonts/arial.spritefont

/importer:TextureImporter
/processor:TextureProcessor
/processorParam:ColorKeyEnabled=False
/build:Tiles/red_tile512.png

/importer:WavImporter
/processor:SoundEffectProcessor
/processorParam:Quality=Best
/build:Sounds/beep.wav
```

## Key Differences from Windows Forms

- **SoundPlayer** (System.Media) → **SoundEffect** (MonoGame)
- No need for file paths - Content pipeline handles it
- Cross-platform compatible
- Better performance
