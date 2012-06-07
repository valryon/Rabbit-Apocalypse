using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lapins.Engine.Core;

namespace Lapins.Data.Commands
{
    /// <summary>
    /// Some common flags used in the game
    /// </summary>
    public static class LapinsScript
    {
        /// <summary>
        /// Boolean waiting for the player to spawn
        /// </summary>
        public static string IsPlayerInstanciated = "isPlayerInstanciated";

        /// <summary>
        /// Integer for player lives count
        /// </summary>
        public static string PlayerLives = "playerLives";

        /// <summary>
        /// Integer for player lives max limit
        /// </summary>
        public static string PlayerLivesMax = "playerLivesMax";

        /// <summary>
        /// Integer for score
        /// </summary>
        public static string Score = "score";

        /// <summary>
        /// Float for timeleft (seconds)
        /// </summary>
        public static string TimeLeft = "timeLeft";

        /// <summary>
        /// TextTo display via the hud
        /// </summary>
        public static string TextToDisplay = "textToDisplay";


        /// <summary>
        /// Float for total time (seconds)
        /// </summary>
        public static string TotalTime = "totalTime";

        /// <summary>
        /// Boolean to tell if the player has the ship keys
        /// </summary>
        public static string HasKeys = "hasKeys";

        /// <summary>
        /// Integer to count number of collected carrots
        /// </summary>
        public static string CarrotsCount = "carrotsCount";

        /// <summary>
        /// Integer to end the game. 0=not set, 1=win, 2=lose
        /// </summary>
        public static string EscapeState = "escapeState";

        /// <summary>
        /// List of Letter of collected letters
        /// </summary>
        public static string CollectedLetters = "collectedLetters";

        /// <summary>
        /// Add points to the score
        /// </summary>
        /// <param name="points"></param>
        public static void AddToScore(int points)
        {
            int score = Application.ScriptManager.GetFlag<int>(LapinsScript.Score);
            score += points;
            Application.ScriptManager.SetFlag(LapinsScript.Score, score);
        }
    }
}
