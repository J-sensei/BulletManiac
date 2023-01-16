using BulletManiac.Entity.UI;
using BulletManiac.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
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
        const string FOOTER = "By Liew Jiann Shen & Fong Zheng Wei";
        readonly Color STRING_COLOR = new Color(238f, 236f, 231f);

        private EntityManager entityManager;
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
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Title"), TITLE, titlePosition, STRING_COLOR);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), VERSION, versionPosition, STRING_COLOR);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), FOOTER, footerPosition, STRING_COLOR);
            entityManager.DrawUI(spriteBatch, gameTime);
        }

        private Vector2 titlePosition;
        private Vector2 versionPosition;
        private Vector2 footerPosition;
        public override void Initialize()
        {
            entityManager = new();
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
            Button quitBtn = new Button(new Vector2(screenSize.Width / 2, screenSize.Height / 2 + 210f), new Vector2(3.2f, 2.5f), "Quit");

            playBtn.ClickEvent += Play;
            quitBtn.ClickEvent += Exit;
            settingBtn.ClickEvent += Option;

            entityManager.AddUIObject(tutorialBtn);
            entityManager.AddUIObject(playBtn);
            entityManager.AddUIObject(settingBtn);
            entityManager.AddUIObject(quitBtn);

            AudioManager.PlayMusic("BGM1");
            gameStart = false;
        }

        bool gameStart = true;
        void Play(object sender, System.EventArgs e)
        {
            Cursor.Instance.ChangeMode(CursorMode.Loading); // Normal mouse cursor
            SceneManager.LoadScene(1);
        }

        void Option(object sender, System.EventArgs e)
        {
            SceneManager.LoadScene(3);
        }

        void Exit(object sender, System.EventArgs e)
        {
            Game1.Instance.Exit();
        }

        public override void Update(GameTime gameTime)
        {
            if(!gameStart)
                Cursor.Instance.ChangeMode(CursorMode.Mouse); // Normal mouse cursor
            entityManager.Update(gameTime); // If hover button, then it will change to hover mouse
        }
    }
}
