# Program.cs Update - Switch to MonoGame

Modify `Program.cs` in the project root to support MonoGame:

## Option 1: Simple Switch (Recommended for Testing)

```csharp
using KeepYourFocus.MonoGame;

namespace KeepYourFocus
{
    static class Program
    {
        // Comment out Windows Forms version below and uncomment MonoGame version to switch

        // === WINDOWS FORMS VERSION ===
        //[STAThread]
        //static void Main()
        //{
        //    ApplicationConfiguration.Initialize();
        //    Application.Run(new Focus());
        //}

        // === MONOGAME VERSION ===
        static void Main()
        {
            using (var game = new MonoGameGame())
                game.Run();
        }
    }
}
```

## Option 2: Conditional Compilation (Best for Flexibility)

```csharp
#define USE_MONOGAME  // Comment/uncomment to switch versions

using KeepYourFocus.MonoGame;

namespace KeepYourFocus
{
    static class Program
    {
#if USE_MONOGAME
        /// <summary>MonoGame version - cross-platform</summary>
        static void Main()
        {
            using (var game = new MonoGameGame())
                game.Run();
        }
#else
        /// <summary>Windows Forms version - Windows only</summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Focus());
        }
#endif
    }
}
```

## Option 3: Runtime Selection (Advanced)

```csharp
using System;
using KeepYourFocus.MonoGame;

namespace KeepYourFocus
{
    static class Program
    {
        static void Main(string[] args)
        {
            // Check for --monogame command line argument
            bool useMonoGame = args.Contains("--monogame");

            if (useMonoGame)
            {
                Console.WriteLine("Starting MonoGame version...");
                using (var game = new MonoGameGame())
                    game.Run();
            }
            else
            {
                Console.WriteLine("Starting Windows Forms version...");
                ApplicationConfiguration.Initialize();
                Application.Run(new Focus());
            }
        }
    }
}
```

**Usage:**
```bash
dotnet run --monogame
```

---

## Build Configuration

If you want different build profiles, update `.csproj`:

```xml
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
</PropertyGroup>

<!-- MonoGame build profile -->
<PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'MonoGame|AnyCPU'">
    <DefineConstants>USE_MONOGAME</DefineConstants>
</PropertyGroup>
```

Then build with:
```bash
dotnet build -c MonoGame
```

---

## Dependency Notes

### Windows Forms Version Requires
- `UseWindowsForms` = true in .csproj
- System.Drawing, System.Windows.Forms references
- Windows platform only

### MonoGame Version Requires
- `MonoGame.Framework.DesktopGL` NuGet package
- No Windows Forms dependencies
- Content folder with assets
- Cross-platform capable

---

## Troubleshooting

**Issue: "MonoGameGame not found"**
```
Solution: Ensure MonoGame/MonoGameGame.cs file exists and is properly named
```

**Issue: "Content not loading"**
```
Solution: Verify Content/ folder exists in project root with assets
```

**Issue: "Both versions trying to run"**
```
Solution: Make sure one set of code is commented out in Program.cs
```
