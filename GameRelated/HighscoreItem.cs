using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeepYourFocus.GameRelated
{
    public class HighscoreItem
    {
        public string PlayerName
        {
            get; set;
        }
        public int Round
        {
            get; set;
        }
        public int LevelReached
        {
            get; set;
        }
        public string LevelName
        {
            get; set;
        }
        public string DateToday
        {
            get; set;
        }
        public string GameTime
        {
            get; set;
        }
        public HighscoreItem()
        {
        }
        public HighscoreItem(string playerName, int round, int levelReached, string levelName, string dateToday, string gameTime)
        {
            this.PlayerName = playerName;
            this.Round = round;
            this.LevelReached = levelReached;
            this.LevelName = levelName;
            this.DateToday = dateToday;
            this.GameTime = gameTime;
        }

        public static HighscoreItem[] GetHighscores(string data_filepath)
        {
            throw new NotImplementedException();
        }
    }
}
