using System;
using Lapins.Engine.Core;
using Lapins.Engine.Score;
using Microsoft.Xna.Framework;

namespace Lapins.Save
{
    [Serializable]
    public class LapinsSaveData
    {
        public const int HighscoresLinesCount = 10;

        // Serializable fields
        // -- Savegame attributes
        public string Version;
        public string ApplicationName;
        public DateTime Date;

        // -- Parameters
        public int ResolutionWidth;
        public int ResolutionHeight;
        public bool IsFullscreen;

        // -- Score
        public Highscores Highscores;

        public LapinsSaveData()
        {
            Version = Application.Version;
            ApplicationName = Application.Name;
            Date = DateTime.Now;
            Highscores = new Highscores(HighscoresLinesCount);

            IsFullscreen = false;
            ResolutionWidth = 1280;
            ResolutionHeight = 768;
        }
    }
}
