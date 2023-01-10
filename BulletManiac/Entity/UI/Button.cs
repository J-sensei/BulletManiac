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
    public class Button : GameObject
    {
        private SpriteFont font;
        private bool isHovering;
        private bool hoverSound = false;
        
        public string Text { get; set; }
        public Color StringColor { get; set; } = Color.Black;
        public bool Cliked { get; private set; }
        public EventHandler ClickEvent;
        public Rectangle uvBound;
        const int SIZE = 64;

        public Button(Vector2 position, Vector2 scale, string text = "") : base(ResourcesManager.FindTexture("Buttons"))
        {
            uvBound = new Rectangle(4 * SIZE, 4 * SIZE, SIZE, SIZE);
            Text = text;
            font = ResourcesManager.FindSpriteFont("Font_Normal");
            this.position = position;
            origin = new Vector2(32f, 32f);
            this.scale = scale;
            //scale = new Vector2(2.5f, 2.5f);
        }

        protected override Rectangle CalculateBound()
        {
            return new Rectangle((int)(position.X - (origin.X * scale.X)), (int)(position.Y - (origin.Y * scale.Y) + (origin.Y * scale.Y / 2)), (int)(64 * scale.X), (int)(64 * scale.Y / 3));
        }

        public static void LoadContent()
        {
            ResourcesManager.LoadTexture("Buttons", "SpriteSheet/UI/Buttons");
        }

        bool clicked = false;
        public override void Update(GameTime gameTime)
        {
            Rectangle mouseRect = new Rectangle((int)InputManager.MousePosition.X, (int)InputManager.MousePosition.Y, 1, 1);

            if (mouseRect.Intersects(Bound))
            {
                isHovering = true;
                if (!hoverSound)
                {
                    // Player hover sound
                    ResourcesManager.FindSoundEffect("Button_Hover").Play();
                    hoverSound = true;
                }

                Cursor.Instance.ChangeMode(CursorMode.MouseAction); // Hover effect
                if (InputManager.MouseLeftHold)
                {
                    uvBound = new Rectangle(5 * SIZE, 4 * SIZE, SIZE, SIZE); // Mouse click effect
                    if (!clicked)
                    {
                        clicked = true;
                        ResourcesManager.FindSoundEffect("Button_Click").Play();
                    }
                }
                else
                {
                    uvBound = new Rectangle(7 * SIZE, 4 * SIZE, SIZE, SIZE);
                }

                if (InputManager.MouseLeftReleased && clicked)
                {
                    // Click Action
                    if (ClickEvent != null)
                        ClickEvent.Invoke(this, new EventArgs());
                    else
                        GameManager.Log("Button", "Click Event is empty");

                    clicked = false;
                }

            }
            else
            {
                isHovering = false;
                uvBound = new Rectangle(4 * SIZE, 4 * SIZE, SIZE, SIZE);
                hoverSound = false;
                clicked = false;
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //spriteBatch.Draw(texture, position, null, color, 0f, origin, 1f, SpriteEffects.None, 0f);
            DrawTexture(uvBound, spriteBatch, gameTime);
            if (!string.IsNullOrEmpty(Text))
            {
                float x = (Bound.X + (Bound.Width / 2)) - (font.MeasureString(Text).X / 2);
                float y = (Bound.Y + (Bound.Height / 2)) - (font.MeasureString(Text).Y / 2) - 4f;

                spriteBatch.DrawString(font, Text, new Vector2(x, y), StringColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
