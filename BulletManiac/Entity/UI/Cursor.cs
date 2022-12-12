using BulletManiac.Managers;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletManiac.Entity.UI
{
    /// <summary>
    /// Cursor pointer that follow the mouse movement
    /// </summary>
    public class Cursor : GameObject
    {
        private Texture2D spriteSheet; // Sprite Sheet of the cursor

        public Cursor()
        {
            GameManager.Resources.LoadTexture("Crosshair_SpriteSheet", "SpriteSheet/UI/Crosshair"); // Load the spritesheet to the resources
        }

        protected override Rectangle CalculateBound()
        {
            Vector2 pos = position - (origin * scale / 2f);
            return new Rectangle((int)pos.X, (int)pos.Y, (int)(texture.Width * scale.X / 2f), (int)(texture.Height * scale.Y / 2f));
        }

        public override void Initialize()
        {
            spriteSheet = GameManager.Resources.FindTexture("Crosshair_SpriteSheet");

            Rectangle cropBound = new Rectangle(0, 32, 16, 16); // Get the target sprite from the spritesheet
            texture = Extensions.CropTexture2D(spriteSheet, cropBound);
            origin = texture.Bounds.Center.ToVector2();
            scale = new Vector2(6f); // Larger scale for the cursor
        }

        public override void Update(GameTime gameTime)
        {
            position = InputManager.MousePosition;

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //spriteBatch.Draw(spriteSheet, Position, Color.White);
            base.Draw(spriteBatch, gameTime);
        }
    }
}
