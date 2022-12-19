using BulletManiac.Entity;
using BulletManiac.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletManiac.Tiled.Legacy
{
    public class Tile : GameObject
    {
        public Tile(Texture2D texture, Vector2 position)
        {
            name = "Tile [" + position + "]";
            this.texture = texture;
            this.position = position;
            scale = new Vector2(1f, 1f);
        }

        public Tile(Texture2D texture, Vector2 position, Vector2 scale)
        {
            name = "Tile";
            this.texture = texture;
            this.position = position;
            this.scale = scale;
        }

        protected override Rectangle CalculateBound()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Take offset to move 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="offset"></param>
        public void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            // If the texture is null, dont draw anything
            if (texture != null)
                spriteBatch.Draw(texture, (position + offset) * GameManager.CurrentGameScale * scale,
                                            null, Color.White, 0f, Vector2.Zero, scale * GameManager.CurrentGameScale,
                                            SpriteEffects.None, 0f);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, Vector2 offset)
        {
            // If the texture is null, dont draw anything
            if (texture != null)
                spriteBatch.Draw(texture, (position + offset) * GameManager.CurrentGameScale * scale,
                                            null, Color.White, 0f, Vector2.Zero, scale * GameManager.CurrentGameScale,
                                            SpriteEffects.None, 0f);
        }
    }
}
