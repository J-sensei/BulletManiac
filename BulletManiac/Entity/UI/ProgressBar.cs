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
    public class ProgressBar : GameObject
    {
        private readonly Texture2D background;
        private readonly Texture2D foreground;
        private float maxValue;
        private float currentValue;
        /// <summary>
        /// Use to indicate how much to draw the bar
        /// </summary>
        private Rectangle barBound;

        // Animated bar
        private float targetValue;
        private readonly float animationSpeed = 40f;
        private Rectangle animationRect;
        private Vector2 animationPositon;
        private Color animationShade;

        public ProgressBar(Texture2D background, Texture2D foreground, float maxValue, Vector2 position, Vector2 scale)
        {
            this.scale = scale;
            this.position = position;
            this.background = background;
            this.foreground = foreground;
            this.maxValue = maxValue;
            currentValue = maxValue;
            barBound = new Rectangle(0, 0, (int)(foreground.Width), (int)(foreground.Height));
            targetValue = maxValue;
            animationRect = new Rectangle((int)(foreground.Width), 0, 0, (int)(foreground.Height));
            animationPositon = position;
            animationShade = Color.DarkGray;
        }

        public override void Update(GameTime gameTime)
        {
            int x;
            if(targetValue < currentValue)
            {
                currentValue -= animationSpeed * Time.DeltaTime;
                if (currentValue < targetValue) currentValue = targetValue;
                x = (int)(targetValue / maxValue * (foreground.Width));
                animationShade = Color.Gray;
            }
            else
            {
                currentValue += animationSpeed * Time.DeltaTime;
                if (currentValue > targetValue) currentValue = targetValue;
                x = (int)(currentValue / maxValue * (foreground.Width));
                animationShade = Color.DarkGray * 0.5f;
            }

            barBound.Width = x;
            animationRect.X = x;
            animationRect.Width = (int)(MathF.Abs(currentValue - targetValue) / maxValue * foreground.Width);
            animationPositon.X = position.X + (x * scale.X);
            base.Update(gameTime);
        }

        public void UpdateValue(float value)
        {
            if (value == currentValue) return;
            //currentValue = value;
            targetValue = value;
            //barBound.Width = (int)(currentValue / maxValue * foreground.Width);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(background, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
            spriteBatch.Draw(foreground, position, barBound, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
            spriteBatch.Draw(foreground, animationPositon, animationRect, animationShade, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
        }

        protected override Rectangle CalculateBound()
        {
            return Rectangle.Empty;
        }
    }
}
