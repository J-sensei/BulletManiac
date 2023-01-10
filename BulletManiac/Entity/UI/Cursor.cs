using BulletManiac.Managers;
using BulletManiac.SpriteAnimation;
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
        public enum CursorMode
        {
            Crosshair, Mouse, MouseAction, Loading
        }

        const int SIZE = 16;
        readonly Vector2 baseScale;
        private Rectangle uvBound;
        private CursorMode cursorMode;
        private Animation loadingAnimation;

        public Cursor() : base(ResourcesManager.FindTexture("Cursor"))
        {
            // Load sprite and set the origin
            name = "Cursor";
            uvBound = new Rectangle(4 * SIZE, 3 * SIZE, SIZE, SIZE); // Get the target sprite from the spritesheet
            origin = new Vector2(8f);
            cursorMode = CursorMode.Crosshair;
            loadingAnimation = new Animation(ResourcesManager.FindTexture("Cursor"), 7, 16, 16, 0.12f, 6);
            //ChangeMode(CursorMode.Loading); // TEST
            baseScale = new Vector2(0.8f);
        }

        protected override Rectangle CalculateBound()
        {
            Vector2 pos = position - (origin * scale / 2f);
            return new Rectangle((int)pos.X, (int)pos.Y, (int)(texture.Width * scale.X / 2f), (int)(texture.Height * scale.Y / 2f));
        }

        public override void Initialize()
        {
            scale = baseScale * GameManager.CurrentCameraZoom; // Larger scale for the cursor
        }

        public override void Update(GameTime gameTime)
        {
            scale = baseScale * GameManager.CurrentCameraZoom;
            position = InputManager.MousePosition;

            if(cursorMode == CursorMode.Loading)
            {
                loadingAnimation.Update(gameTime);
            }
            base.Update(gameTime);
        }

        public void ChangeMode(CursorMode mode)
        {
            if(mode == CursorMode.Crosshair)
            {
                uvBound = new Rectangle(4 * SIZE, 3 * SIZE, SIZE, SIZE); // Get the target sprite from the spritesheet
                origin = new Vector2(8f);
            }
            else if(mode == CursorMode.Mouse)
            {
                uvBound = new Rectangle(0 * SIZE, 0 * SIZE, SIZE, SIZE);
                origin = new Vector2(0f);
            }
            else if (mode == CursorMode.MouseAction)
            {
                uvBound = new Rectangle(1 * SIZE, 1 * SIZE, SIZE, SIZE);
                origin = new Vector2(0f);
            }
            else if(mode == CursorMode.Loading)
            {
                origin = new Vector2(8f);
            }
            cursorMode = mode;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //spriteBatch.Draw(spriteSheet, Position, Color.White);
            if(cursorMode == CursorMode.Loading)
            {
                DrawAnimation(loadingAnimation, spriteBatch, gameTime);
            }
            else
            {
                DrawTexture(uvBound, spriteBatch, gameTime);
            }
        }
    }
}
