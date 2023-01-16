using BulletManiac.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BulletManiac.Particle
{
    /// <summary>
    /// Apply transition effect on top of every game objects and elements
    /// </summary>
    public class TransitionEffect
    {
        private readonly Texture2D texture;
        private Color color = new Color(7, 24, 33);
        private List<FadeItem> items = new();
        private float speed = 90f;
        private bool active = false;
        public bool Finish { 
            get 
            {
                return (items.Where(x => x.Scale > 0).Count() == 0);
            } 
        }

        public TransitionEffect(Texture2D texture)
        {
            this.texture = texture;
        }

        public void Initialize()
        {
            for (int x = 8; x - 16 < Game1.GraphicsDeviceInstance.Viewport.Width; x += 16)
            {
                for (int y = 8; y - 16 < Game1.GraphicsDeviceInstance.Viewport.Height; y += 16)
                {
                    items.Add(new FadeItem
                    {
                        X = x,
                        Y = y,
                        Delay = (x * 20) + 2000
                    });
                }
            }
        }

        public void Reset()
        {
            foreach (var item in items)
            {
                item.Reset();   
            }
        }

        public void Start() => active = true;

        public void Update(GameTime gameTime)
        {
            if (!active) return;
            if (Finish) active = false;
            foreach (var item in items)
            {
                item.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds * speed);
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var item in items)
            {
                spriteBatch.Draw(texture, new Vector2(item.X, item.Y), null, color, 0f, new Vector2(8, 8), item.Scale, SpriteEffects.None, 0f);
            }
        }
    }
}
