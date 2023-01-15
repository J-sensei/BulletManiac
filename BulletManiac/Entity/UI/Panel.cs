using BulletManiac.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Entity.UI
{
    public class Panel : GameObject
    {
        private SpriteFont font;
        private string text;
        public Panel(Vector2 position, Vector2 scale, string text = "ABC") : base(ResourcesManager.FindTexture("Transition_Texture"))
        {
            this.scale = scale;
            this.text = text;
            origin = new Vector2(8f);
            color = new Color(7, 24, 33) * 0.7f;
            font = ResourcesManager.FindSpriteFont("Font_Small");
            this.position = position;
        }

        protected override Rectangle CalculateBound()
        {
            return new Rectangle((int)(position.X - (origin.X * scale.X)), (int)(position.Y - (origin.Y * scale.Y)), (int)(16f * scale.X), (int)(16f * scale.Y));
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
            if (!string.IsNullOrEmpty(text))
            {
                float x = (Bound.X + (Bound.Width / 2)) - (font.MeasureString(text).X / 2);
                float y = (Bound.Y + (Bound.Height / 2)) - (font.MeasureString(text).Y / 2) - 4f;
                y += 3f;
                spriteBatch.DrawString(font, text, new Vector2(x, y), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
