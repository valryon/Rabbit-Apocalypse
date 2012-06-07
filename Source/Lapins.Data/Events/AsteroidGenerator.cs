using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Lapins.Data.Entities;
using Lapins.Data.Levels;
using Lapins.Engine.Core;
using Lapins.Data.Commands;

namespace Lapins.Data.Events
{
    /// <summary>
    /// Look at the remaining time and generate dangerous Asteroid over the level
    /// </summary>
    public class AsteroidGenerator
    {
        //all timing are in ms
        private const float MaxCoolDown = 20000;
        private const float MinCoolDown = 500;
        private const float StartTime = 8 * 60000; //begining of apocalypse, bitch!
        private const float EndTime = 1 * 60000; // time after what nothing can be worst :3
        private const float Delta = (MaxCoolDown - MinCoolDown) / (StartTime - EndTime);

        private Layer _layer;
        private bool _isBackground;
        private float _nextAsteroidCooldown;

        public AsteroidGenerator(Layer layer, bool isBackground)
        {
            _layer = layer;
            _isBackground = isBackground;
        }

        public void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.Milliseconds;
            _nextAsteroidCooldown -= elapsedTime;

            if (_nextAsteroidCooldown <= 0)
            {
                var asteroid = GenerateAsteroid();
                Level.CurrentLevel.AddEntity(_layer, asteroid,false);

                // Re-assign cooldown
                _nextAsteroidCooldown = getNextCooldown(Application.ScriptManager.GetFlag<float>(LapinsScript.TimeLeft) * 1000);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_time"></param>
        /// <returns></returns>
        /// <remarks>Just a linear function, not really choupinou</remarks>
        private float getNextCooldown(float _time)
        {
            if (_time > StartTime) return MaxCoolDown;
            if (_time < EndTime) return MinCoolDown;
            return _time    
                * Delta + (MinCoolDown - (Delta * EndTime));       
        }



        private Asteroid GenerateAsteroid()
        {
            float scale;
            Vector2 location;
            Color color = Color.White;
            Rectangle cameraVisibilityRect = Application.GameStateManager.CurrentGameState.SceneCamera.VisibilityRectangle;
            bool collidable;

            float timeLeft = Application.ScriptManager.GetFlag<float>(LapinsScript.TimeLeft);
            float totalTime = Application.ScriptManager.GetFlag<float>(LapinsScript.TotalTime);
            float apocalypseFactor = (totalTime / (timeLeft + 0.01f)); // Avoid division per zero

            // Bigger asteroids with incoming apocalypse 
            if (_isBackground)
            {
                scale = Application.Random.GetRandomFloat(0.05f * apocalypseFactor, 0.5f * apocalypseFactor);
                scale = MathHelper.Clamp(scale, 0.2f, 2f);
                location = Application.Random.GetRandomVector2(cameraVisibilityRect.Left - 300, cameraVisibilityRect.Right + 300, cameraVisibilityRect.Top - 150, cameraVisibilityRect.Top - 100);
                color = Color.White * 0.3f;
                collidable = false;
            }
            else
            {
                scale = Application.Random.GetRandomFloat(0.15f * apocalypseFactor, 0.55f * apocalypseFactor);
                scale = MathHelper.Clamp(scale, 0.5f, 2f);
                location = Application.Random.GetRandomVector2(cameraVisibilityRect.Left - 300, cameraVisibilityRect.Right + 300, cameraVisibilityRect.Top - 150, cameraVisibilityRect.Top - 100);
                collidable = true;
            }

            var ast = new Asteroid(location,
                        Application.Random.GetRandomVector2(-100, 100, 200, 300),
                        new Vector2(scale, scale),
                        collidable);

            ast.Color = color;

            return ast;
        }

    }
}

