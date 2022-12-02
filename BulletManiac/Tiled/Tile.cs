using BulletManiac.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletManiac.Tiled
{
    public class Tile
    {
        private readonly Texture2D texture;
        private readonly Vector2 position;

        public Tile(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 offset)
        {
            if(texture != null)
                spriteBatch.Draw(texture, (position + offset) * GameManager.CurrentGameScale, null, Color.White, 0f, Vector2.Zero, new Vector2(1f, 1f) * GameManager.CurrentGameScale, SpriteEffects.None, 0f);
        }
    }
}
