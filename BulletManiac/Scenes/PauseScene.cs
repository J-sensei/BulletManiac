using BulletManiac.Entity.UI;
using BulletManiac.Managers;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Scenes
{
    public class PauseScene : Scene
    {
        const string TITLE = "Pause";
        private Texture2D texture;
        private EntityManager entityManager = new();
        private Vector2 titlePosition;

        public override void LoadContent()
        {
            ResourcesManager.LoadTexture("Transition_Texture", "UI/Transition_Texture");

            ResourcesManager.LoadSpriteFonts("Font_Normal", "UI/Font/Font_Normal");
            ResourcesManager.LoadSpriteFonts("Font_Title", "UI/Font/Font_Title");
            ResourcesManager.LoadSoundEffect("Button_Hover", "Audio/UI/Button_Hover");
            ResourcesManager.LoadSoundEffect("Button_Click", "Audio/UI/Button_Click");

            Button.LoadContent();
            base.LoadContent();
        }

        float waitToClose = 0.25f;
        public override void Initialize()
        {
            texture = ResourcesManager.FindTexture("Transition_Texture");
            var screenSize = Game1.GraphicsDeviceInstance.Viewport.Bounds;

            // Title
            float offsetX = ResourcesManager.FindSpriteFont("Font_Title").MeasureString(TITLE).X / 2;
            titlePosition = new Vector2(screenSize.Width / 2 - offsetX, screenSize.Height / 2 - 300f);

            // Buttons
            Button resumeBtn = new Button(new Vector2(screenSize.Width / 2, screenSize.Height / 2), new Vector2(3.8f, 2.5f), "Resume");
            Button restartBtn = new Button(new Vector2(screenSize.Width / 2, screenSize.Height / 2 + 70f), new Vector2(3.8f, 2.5f), "Restart");
            Button menuBtn = new Button(new Vector2(screenSize.Width / 2, screenSize.Height / 2 + 140f), new Vector2(3.8f, 2.5f), "Main Menu");
            Button quitBtn = new Button(new Vector2(screenSize.Width / 2, screenSize.Height / 2 + 210f), new Vector2(3.8f, 2.5f), "Quit");

            resumeBtn.ClickEvent += Unpause;
            restartBtn.ClickEvent += Restart;
            menuBtn.ClickEvent += MainMenu;
            quitBtn.ClickEvent += Exit;

            entityManager.AddUIObject(resumeBtn);
            entityManager.AddUIObject(restartBtn);
            entityManager.AddUIObject(menuBtn);
            entityManager.AddUIObject(quitBtn);
            waitToClose = 0.25f;
        }

        void MainMenu(object sender, System.EventArgs e)
        {
            SceneManager.LoadScene(0);
        }

        void Restart(object sender, System.EventArgs e)
        {
            SceneManager.LoadScene(1);
        }

        void Exit(object sender, System.EventArgs e)
        {
            Game1.Instance.Exit();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            
        }

        public override void DrawUI(SpriteBatch spriteBatch, GameTime gameTime)
        {
            for (int x = 8; x - 16 < Game1.GraphicsDeviceInstance.Viewport.Width; x += 16)
            {
                for (int y = 8; y - 16 < Game1.GraphicsDeviceInstance.Viewport.Height; y += 16)
                {
                    spriteBatch.Draw(texture, new Vector2(x, y), null, Color.Black * 0.5f, 0f, new Vector2(8, 8), 1f, SpriteEffects.None, 0f);
                }
            }

            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Title"), TITLE, titlePosition, Color.White);

            // Buttons
            entityManager.DrawUI(spriteBatch, gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            Cursor.Instance.ChangeMode(CursorMode.Mouse); // Normal mouse cursor
            waitToClose -= Time.DeltaTime;
            entityManager.Update(gameTime);

            // Need to wait cooldown to prevent open & close the menu at the same time
            if (InputManager.GetKey(Keys.Escape) && waitToClose <= 0f)
            {
                // Unpause the game
                UnpauseAction();
            }
        }

        void Unpause(object sender, System.EventArgs e)
        {
            UnpauseAction();
        }

        void UnpauseAction()
        {
            SceneManager.GetScene(1).StartUpdate();
            SceneManager.CloseScene(2);
        }
    }
}
