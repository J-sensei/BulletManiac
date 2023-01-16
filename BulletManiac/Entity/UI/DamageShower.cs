using BulletManiac.Managers;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Entity.UI
{
    /// <summary>
    /// Display the damage when enemy take the damage
    /// </summary>
    public class DamageShower : GameObject
    {
        private readonly SpriteFont font;
        private readonly string text;
        private Color color = Color.DarkRed;
        private float colorTransparancy = 1f;
        public DamageShower(int damage, Vector2 position)
        {
            font = ResourcesManager.FindSpriteFont("Font_Player");
            this.position = position;
            text = damage.ToString();
            origin = font.MeasureString(text);
        }

        protected override Rectangle CalculateBound()
        {
            return Rectangle.Empty;
        }

        public override void Update(GameTime gameTime)
        {
            color = Color.DarkRed * colorTransparancy;

            colorTransparancy -= (1f * Time.DeltaTime);
            position.Y -= 30f * Time.DeltaTime;

            if (colorTransparancy <= 0f) Destroy(this);
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(font, text, position, color, 0f, origin / 2f, 0.4f, SpriteEffects.None, 0f);
        }
    }
}
