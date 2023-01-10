using BulletManiac.Entity.UI;
using BulletManiac.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Scenes
{
    public class MainMenuScene : Scene
    {
        const string TITLE = "Bullet Maniac";
        const string VERSION = "v1.0";
        const string FOOTER = "By XXX and XXX";
        private readonly EntityManager entityManager = new();
        public override void LoadContent()
        {
            ResourcesManager.LoadSpriteFonts("Font_Normal", "UI/Font/Font_Normal");
            ResourcesManager.LoadSpriteFonts("Font_Title", "UI/Font/Font_Title");
            ResourcesManager.LoadSoundEffect("Button_Hover", "Audio/UI/Button_Hover");
            ResourcesManager.LoadSoundEffect("Button_Click", "Audio/UI/Button_Click");

            Button.LoadContent();
            base.LoadContent();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            
        }

        public override void DrawUI(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Title"), TITLE, titlePosition, Color.White);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), VERSION, versionPosition, Color.White);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), FOOTER, footerPosition, Color.White);
            entityManager.DrawUI(spriteBatch, gameTime);
        }

        private Vector2 titlePosition;
        private Vector2 versionPosition;
        private Vector2 footerPosition;
        public override void Initialize()
        {
            ClearColor = new Color(7, 24, 33);
            var screenSize = Game1.GraphicsDeviceInstance.Viewport.Bounds;

            // Title
            float offsetX = ResourcesManager.FindSpriteFont("Font_Title").MeasureString(TITLE).X / 2;
            titlePosition = new Vector2(screenSize.Width / 2 - offsetX, screenSize.Height / 2 - 300f);

            // Version 
            float x = ResourcesManager.FindSpriteFont("Font_Normal").MeasureString(VERSION).X / 2;
            versionPosition = new Vector2(screenSize.Width / 2 + offsetX - x * 2, screenSize.Height / 2 - 220f);

            // Footer
            x = ResourcesManager.FindSpriteFont("Font_Normal").MeasureString(FOOTER).X / 2;
            footerPosition = new Vector2(screenSize.Width / 2 - x, screenSize.Height / 2 + 310f);
            // Buttons
            Button tutorialBtn = new Button(new Vector2(screenSize.Width / 2, screenSize.Height / 2), new Vector2(3.2f, 2.5f), "Tutorial");
            Button playBtn = new Button(new Vector2(screenSize.Width / 2, screenSize.Height / 2 + 70f), new Vector2(3.2f, 2.5f), "Play");
            Button settingBtn = new Button(new Vector2(screenSize.Width / 2, screenSize.Height / 2 + 140f), new Vector2(3.2f, 2.5f), "Setting");
            Button quitBtn = new Button(new Vector2(screenSize.Width / 2, screenSize.Height / 2 + 210f), new Vector2(3.2f, 2.5f), "Exit");

            playBtn.ClickEvent += Play;
            quitBtn.ClickEvent += Exit;

            entityManager.AddUIObject(tutorialBtn);
            entityManager.AddUIObject(playBtn);
            entityManager.AddUIObject(settingBtn);
            entityManager.AddUIObject(quitBtn);
        }

        void Play(object sender, System.EventArgs e)
        {
            SceneManager.LoadScene(1);
        }

        void Exit(object sender, System.EventArgs e)
        {
            Game1.Instance.Exit();
        }

        public override void Update(GameTime gameTime)
        {
            Cursor.Instance.ChangeMode(CursorMode.Mouse); // Normal mouse cursor
            entityManager.Update(gameTime); // If hover button, then it will change to hover mouse
        }
    }
}
