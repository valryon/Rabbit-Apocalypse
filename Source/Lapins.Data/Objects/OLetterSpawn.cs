using Lapins.Data.Levels;
using Lapins.Data.Ogmo;
using Lapins.Engine.Content;
using Lapins.Engine.Physics;
using Lapins.Engine.World;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Lapins.Engine.Core;
using System;

namespace Lapins.Data.Objects
{
    /// <summary>
    /// Potential letter spawn
    /// </summary>
    [OgmoObjectId("letterSpawn")]
    public class OLetterSpawn : OgmoObjectEntity
    {
        public static List<string> Letters = new List<string>{
                                           "L","A","P","I","N"
                                    };

        private static List<string> UnusedLetters;

        public static void Intialize()
        {
            UnusedLetters = new List<string>(Letters);
        }

        public static int LetterCount = Letters.Count;

        public OLetterSpawn(Vector2 location)
            : base("", location)
        { }

        public override void Update(GameTime gameTime)
        {
            if (UnusedLetters.Count > 0)
            {
                string sletter = UnusedLetters.GetRandomElement<string>();
                UnusedLetters.Remove(sletter);

                //Spawn letter
                Letter letter = new Letter(location, sletter);
                Level.CurrentLevel.AddEntity(Level.CurrentLevel.MiddleGroundLayer, letter, false);
            }
            IsAlive = false;
        }

        public override void Draw(Engine.Graphics.SpriteBatchProxy spriteBatch)
        { }

        public override Rectangle BaseSrcRect
        {
            get { return Rectangle.Empty; }
        }

        public override Engine.World.Entity Clone()
        {
            return new OKeysSpawn(location);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [TextureContent(AssetName = "letters", AssetPath = "gfxs/entities/letters")]
    public class Letter : Entity, IComparable<Letter>
    {
        public string TheLetter { get; set; }

        public Letter(Vector2 location, string letter)
            : base("letters", location, RectForLetter(letter), Vector2.One)
        {
            TheLetter = letter;
            hitbox = new Hitbox(SrcRect);
        }
        public override Engine.World.Entity Clone()
        {
            return new Letter(location, TheLetter);
        }

        public override void Draw(Engine.Graphics.SpriteBatchProxy spriteBatch)
        {
            spriteBatch.Draw(Application.MagicContentManager.GetTexture(assetName), dRect, sRect, Color, (float)rotation, spriteOrigin, flip, 1.0f);
        }

        public static Rectangle RectForLetter(string letter)
        {
            int index = OLetterSpawn.Letters.IndexOf(letter);
            return new Rectangle(32 * index, 0, 32, 32);
        }

        public int CompareTo(Letter other)
        {
            return OLetterSpawn.Letters.IndexOf(this.TheLetter).CompareTo(OLetterSpawn.Letters.IndexOf(other.TheLetter));
        }
    }
}
