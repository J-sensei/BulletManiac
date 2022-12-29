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
        private Rectangle uvBound;

        public Cursor() : base(GameManager.Resources.FindTexture("Crosshair_SpriteSheet"))
        {
            // Load sprite and set the origin
            name = "Cursor";
            uvBound = new Rectangle(0, 32, 16, 16); // Get the target sprite from the spritesheet
            origin = new Vector2(8f);
        }

        protected override Rectangle CalculateBound()
        {
            Vector2 pos = position - (origin * scale / 2f);
            return new Rectangle((int)pos.X, (int)pos.Y, (int)(texture.Width * scale.X / 2f), (int)(texture.Height * scale.Y / 2f));
        }

        public override void Initialize()
        {
            scale = Vector2.One * GameManager.CurrentCameraZoom; // Larger scale for the cursor
        }

        public override void Update(GameTime gameTime)
        {
            scale = Vector2.One * GameManager.CurrentCameraZoom;
            position = InputManager.MousePosition;

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //spriteBatch.Draw(spriteSheet, Position, Color.White);
            DrawTexture(uvBound, spriteBatch, gameTime);
        }
    }
}
