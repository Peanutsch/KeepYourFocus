using System;
using System.IO;
using System.Media;

public class GameSounds
{
    private readonly string basePath = "Sounds";

    // Lazy initializers for color sounds
    private Lazy<SoundPlayer> RedSound => Load("red.wav");
    private Lazy<SoundPlayer> BlueSound => Load("blue.wav");
    private Lazy<SoundPlayer> OrangeSound => Load("orange.wav");
    private Lazy<SoundPlayer> GreenSound => Load("green.wav");
    private Lazy<SoundPlayer> CaribBlueSound => Load("caribblue.wav");
    private Lazy<SoundPlayer> GreySound => Load("grey.wav");
    private Lazy<SoundPlayer> IndigoSound => Load("indigo.wav");
    private Lazy<SoundPlayer> MaroonSound => Load("maroon.wav");
    private Lazy<SoundPlayer> OliveSound => Load("olive.wav");
    private Lazy<SoundPlayer> PinkSound => Load("pink.wav");

    // Lazy initializers for feedback/transition sounds
    private Lazy<SoundPlayer> TransitionSound => Load("transition.wav");
    private Lazy<SoundPlayer> ButtonClickSound => Load("click.wav");
    private Lazy<SoundPlayer> WrongSound => Load("wrong.wav");
    private Lazy<SoundPlayer> CorrectSound => Load("correct.wav");
    private Lazy<SoundPlayer> StartupSound => Load("startup.wav");

    private Lazy<SoundPlayer> Load(string fileName) =>
        new Lazy<SoundPlayer>(() => new SoundPlayer(Path.Combine(basePath, fileName)));

    // Color sound methods
    public void PlayRed() => RedSound.Value.Play();
    public void PlayBlue() => BlueSound.Value.Play();
    public void PlayOrange() => OrangeSound.Value.Play();
    public void PlayGreen() => GreenSound.Value.Play();
    public void PlayCaribBlue() => CaribBlueSound.Value.Play();
    public void PlayGrey() => GreySound.Value.Play();
    public void PlayIndigo() => IndigoSound.Value.Play();
    public void PlayMaroon() => MaroonSound.Value.Play();
    public void PlayOlive() => OliveSound.Value.Play();
    public void PlayPink() => PinkSound.Value.Play();

    // Feedback/transition sound methods
    public void PlayTransition() => TransitionSound.Value.Play();
    public void PlayButtonClick() => ButtonClickSound.Value.Play();
    public void PlayWrong() => WrongSound.Value.Play();
    public void PlayCorrect() => CorrectSound.Value.Play();
    public void PlayStartup() => StartupSound.Value.Play();
}
