using System.Media;

namespace KeepYourFocus
{
    public class SoundManager
    {
        private readonly SoundPlayer redSound;
        private readonly SoundPlayer blueSound;
        private readonly SoundPlayer orangeSound;
        private readonly SoundPlayer greenSound;
        private readonly SoundPlayer caribBlueSound;
        private readonly SoundPlayer greySound;
        private readonly SoundPlayer indigoSound;
        private readonly SoundPlayer maroonSound;
        private readonly SoundPlayer oliveSound;
        private readonly SoundPlayer pinkSound;

        private readonly SoundPlayer transitionSound;
        private readonly SoundPlayer buttonClickSound;
        private readonly SoundPlayer wrongSound;
        private readonly SoundPlayer correctSound;
        private readonly SoundPlayer startupSound;

        public SoundManager()
        {
            string rootPath = PathHelper.GetRootPath();
            string soundPathBeepALL = Path.Combine(rootPath, @"sounds\beep.wav");

            string soundPathTransition = Path.Combine(rootPath, @"sounds\transistion.wav");
            string soundPathButtonClick = Path.Combine(rootPath, @"sounds\buttonclick.wav");
            string soundPathWrong = Path.Combine(rootPath, @"sounds\wrong.wav");
            string soundPathCorrect = Path.Combine(rootPath, @"sounds\correct.wav");
            string soundPathStartupSound = Path.Combine(rootPath, @"sounds\startupSound.wav");

            redSound = new SoundPlayer(soundPathBeepALL);
            blueSound = new SoundPlayer(soundPathBeepALL);
            orangeSound = new SoundPlayer(soundPathBeepALL);
            greenSound = new SoundPlayer(soundPathBeepALL);
            caribBlueSound = new SoundPlayer(soundPathBeepALL);
            greySound = new SoundPlayer(soundPathBeepALL);
            indigoSound = new SoundPlayer(soundPathBeepALL);
            maroonSound = new SoundPlayer(soundPathBeepALL);
            oliveSound = new SoundPlayer(soundPathBeepALL);
            pinkSound = new SoundPlayer(soundPathBeepALL);

            transitionSound = new SoundPlayer(soundPathTransition);
            buttonClickSound = new SoundPlayer(soundPathButtonClick);
            wrongSound = new SoundPlayer(soundPathWrong);
            correctSound = new SoundPlayer(soundPathCorrect);
            startupSound = new SoundPlayer(soundPathStartupSound);
        }

        public void PlayTileSound(string tile)
        {
            switch (tile)
            {
                case "Red": redSound.Play(); break;
                case "Blue": blueSound.Play(); break;
                case "Orange": orangeSound.Play(); break;
                case "Green": greenSound.Play(); break;
                case "CaribBlue": caribBlueSound.Play(); break;
                case "Grey": greySound.Play(); break;
                case "Indigo": indigoSound.Play(); break;
                case "Maroon": maroonSound.Play(); break;
                case "Olive": oliveSound.Play(); break;
                case "Pink": pinkSound.Play(); break;
            }
        }

        public void PlayTransition() => transitionSound.Play();
        public void PlayButtonClick() => buttonClickSound.Play();
        public void PlayWrong() => wrongSound.Play();
        public void PlayCorrect() => correctSound.Play();
        public void PlayStartup() => startupSound.Play();
    }
}
