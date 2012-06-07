using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Lapins.Engine.Core;
using Lapins.Engine.Utils;
using Lapins.Engine.Content;
using Microsoft.Xna.Framework.Audio;
using Lapins.Engine.Input.Devices;

namespace Lapins.Data.Particules
{
    /// <summary>
    /// Simple particules effect creation
    /// </summary>
    [SoundEffectContent(AssetName = "s_explosion1", AssetPath = "sfxs/explosion_1")]
    [SoundEffectContent(AssetName = "s_explosion2", AssetPath = "sfxs/explosion_2")]
    [SoundEffectContent(AssetName = "s_explosion3", AssetPath = "sfxs/explosion_3")]
    [SoundEffectContent(AssetName = "s_explosion4", AssetPath = "sfxs/explosion_4")]
    public static class SpecialEffectsHelper
    {
        private static Timer _shakeCameraTimer;

        private static List<SoundEffect> _explosionSounds;

        /// <summary>
        /// Create explosion with blast
        /// </summary>
        public static void MakeExplosion(Vector2 loc, float size, Color color, bool background, bool sound = false, int iteration = 8)
        {
            var particuleManager = Application.GameStateManager.CurrentGameState.ParticuleManager;
            var random = Application.Random;

            for (int i = 0; i < iteration; i++)
            {
                particuleManager.AddParticule(
                    new Smoke(loc,
                        random.GetRandomVector2(-100f, 100f, -100f, 100f),
                        random.GetRandomFloat(size / 2, size * 2),
                        color,
                        background));
            }

            for (int i = 0; i < iteration; i++)
            {
                particuleManager.AddParticule(
                    new Fire(loc,
                        random.GetRandomVector2(-80f, 80f, -80f, 80f),
                        size,
                        background));
            }

            if (sound)
            {
                playExplosionSound();
            }
        }

        /// <summary>
        /// Create customizable explosion
        /// </summary>
        public static void MakeCircularExplosion(Vector2 loc, float energy, float size, Color color, bool background, bool sound = false, int iteration = 8)
        {
            var particuleManager = Application.GameStateManager.CurrentGameState.ParticuleManager;
            var random = Application.Random;

            for (int i = 0; i < iteration; i++)
            {
                var trajectory = random.GetRandomTrajectory(energy);

                particuleManager.AddParticule(
                    new Smoke(loc,
                        trajectory,
                        random.GetRandomFloat(size / 2, size * 2),
                        color * 0.75f,
                        background));
            }

            for (int i = 0; i < iteration; i++)
            {
                particuleManager.AddParticule(
                    new Fire(loc,
                        random.GetRandomVector2(-80f, 80f, -80f, 80f),
                        size,
                        background));
            }

            for (int i = 0; i < iteration; i++)
            {
                particuleManager.AddParticule(
                    new Piece(loc,
                        random.GetRandomVector2(-1000f, 1000f, -1000f, 1000f),
                        size / 2,
                        color,
                        background));
            }

            if (sound)
            {
                playExplosionSound();
            }
        }

        private static void playExplosionSound()
        {
            if (_explosionSounds == null)
            {
                _explosionSounds = new List<SoundEffect>()
                {
                    Application.MagicContentManager.GetSound("s_explosion1"),
                    Application.MagicContentManager.GetSound("s_explosion2"),
                    Application.MagicContentManager.GetSound("s_explosion3"),
                    Application.MagicContentManager.GetSound("s_explosion4"),
                };
            }

            _explosionSounds.GetRandomElement<SoundEffect>().Play(0.3f, 0f, 0f);
        }

        /// <summary>
        /// Create blood explosion
        /// </summary>
        public static void MakeBlood(Vector2 loc, float energy, float size, bool background, int iteration = 8)
        {
            var particuleManager = Application.GameStateManager.CurrentGameState.ParticuleManager;
            var random = Application.Random;

            Color bloodColor = Color.White;

            for (int i = 0; i < iteration; i++)
            {
                var trajectory = random.GetRandomTrajectory(energy, Math.PI, Math.PI * 2);

                particuleManager.AddParticule(
                    new Blood(loc,
                        trajectory,
                        random.GetRandomFloat(size / 4, size),
                        bloodColor,
                        background));
            }


        }

        public static void MakeSmoke(Vector2 loc, Vector2 trajectory, float size, Color color, bool background, int iteration = 4)
        {
            var particuleManager = Application.GameStateManager.CurrentGameState.ParticuleManager;
            var random = Application.Random;

            for (int i = 0; i < iteration; i++)
            {
                particuleManager.AddParticule(
                    new Smoke(loc,
                        trajectory,
                        random.GetRandomFloat(size, size),
                        color,
                        background));
            }
        }

        /// <summary>
        /// Shake the screen for few milliseconds
        /// </summary>
        /// <param name="amplitude"></param>
        /// <param name="speed"></param>
        /// <param name="length">seconds</param>
        public static void ShakeScreen(Vector2 amplitude, float length)
        {
            var camera = Application.GameStateManager.CurrentGameState.SceneCamera;

            camera.ShakeFactor = amplitude;
            camera.ShakeSpeed = new Vector2(50f, 50f);

            if (_shakeCameraTimer != null)
                _shakeCameraTimer.Stop();

            _shakeCameraTimer = Timer.Create(length, false, (t =>
            {
                camera.ShakeFactor = Vector2.Zero;
            }));

            // Make devices rumble
            foreach (var device in Application.InputManager.GetLinkedDevices(Engine.Input.LogicalPlayerIndex.One))
            {
                device.Rumble(amplitude / new Vector2(5f));
            }
        }
    }
}
