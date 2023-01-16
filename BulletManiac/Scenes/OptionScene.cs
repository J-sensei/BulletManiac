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
    public class OptionScene : Scene
    {
        static bool fullscreen = false;
        static bool vsycn = false;
        const string TITLE = "Setting";
        const string SUB_TITLE = "Game options";
        readonly Color STRING_COLOR = new Color(238f, 236f, 231f);

        private EntityManager entityManager;
        public override void LoadContent()
        {
            ResourcesManager.LoadSpriteFonts("Font_Normal", "UI/Font/Font_Normal");
            ResourcesManager.LoadSpriteFonts("Font_Title", "UI/Font/Font_Title");
            ResourcesManager.LoadSoundEffect("Button_Hover", "Audio/UI/Button_Hover");
            ResourcesManager.LoadSoundEffect("Button_Click", "Audio/UI/Button_Click");
            ResourcesManager.LoadSoundEffect("Pause", "Audio/UI/Pause");

            Button.LoadContent();
            base.LoadContent();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

        }

        public override void DrawUI(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Title"), TITLE, titlePosition, STRING_COLOR);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), SUB_TITLE, subtitlePosition, STRING_COLOR);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), "Master Volume: " + (AudioManager.MasterVolume * 100f).ToString("N0") + "%",
                                    new Vector2(screenSize.Width / 2 - 300f, screenSize.Height / 2 - 100f - 20f), STRING_COLOR);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), "SFX Volume: " + (AudioManager.SFXVolume * 100f).ToString("N0") + "%",
                        new Vector2(screenSize.Width / 2 - 300f, screenSize.Height / 2 - 50f - 20f), STRING_COLOR);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), "BGM Volume: " + (AudioManager.BGMVolume * 100f).ToString("N0") + "%",
                        new Vector2(screenSize.Width / 2 - 300f, screenSize.Height / 2 + - 20f), STRING_COLOR);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), "Full Screen: " + (fullscreen ? "Yes" : "No"),
                        new Vector2(screenSize.Width / 2 - 300f, screenSize.Height / 2 + 75f - 20f), STRING_COLOR);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), "VSync: " + (vsycn ? "Yes" : "No"),
                        new Vector2(screenSize.Width / 2 - 300f, screenSize.Height / 2 + 150f - 20f), STRING_COLOR);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), "Show FPS: " + (Game1.ShowFPS ? "Yes" : "No"),
                        new Vector2(screenSize.Width / 2 - 300f, screenSize.Height / 2 + 225f - 20f), STRING_COLOR);
            entityManager.DrawUI(spriteBatch, gameTime);
        }

        private Vector2 titlePosition;
        private Vector2 subtitlePosition;

        Rectangle screenSize;
        Button masterDown;
        Button masterUp;
        Button sfxDown;
        Button sfxUp;
        Button bgmDown;
        Button bgmUp;

        Button fullScreennBtn;
        Button vsyncBtn;
        Button showFpsBtn;
        public override void Initialize()
        {
            entityManager = new();

            ClearColor = new Color(7, 24, 33);
            screenSize = Game1.GraphicsDeviceInstance.Viewport.Bounds;

            // Title
            float offsetX = ResourcesManager.FindSpriteFont("Font_Title").MeasureString(TITLE).X / 2;
            titlePosition = new Vector2(screenSize.Width / 2 - offsetX, screenSize.Height / 2 - 300f);

            // Sub title 
            float x = ResourcesManager.FindSpriteFont("Font_Normal").MeasureString(SUB_TITLE).X / 2;
            subtitlePosition = new Vector2(screenSize.Width / 2 - x, screenSize.Height / 2 - 220f);

            // Buttons
            masterDown = new Button(new Vector2(screenSize.Width / 2 + 200f, screenSize.Height / 2 - 100f), new Vector2(0.8f, 2f), "-", false);
            masterUp = new Button(new Vector2(screenSize.Width / 2 + 280f, screenSize.Height / 2 - 100f), new Vector2(0.8f, 2f), "+", false);
            sfxDown = new Button(new Vector2(screenSize.Width / 2 + 200f, screenSize.Height / 2 - 50f), new Vector2(0.8f, 2f), "-", false);
            sfxUp = new Button(new Vector2(screenSize.Width / 2 + 280f, screenSize.Height / 2 - 50f), new Vector2(0.8f, 2f), "+", false);
            bgmDown = new Button(new Vector2(screenSize.Width / 2 + 200f, screenSize.Height / 2 ), new Vector2(0.8f, 2f), "-", false);
            bgmUp = new Button(new Vector2(screenSize.Width / 2 + 280f, screenSize.Height / 2), new Vector2(0.8f, 2f), "+", false);

            fullScreennBtn = new Button(new Vector2(screenSize.Width / 2 + 230f, screenSize.Height / 2 + 75f), new Vector2(3f, 2.5f), "Toggle");
            vsyncBtn = new Button(new Vector2(screenSize.Width / 2 + 230f, screenSize.Height / 2 + 150f), new Vector2(3f, 2.5f), "Toggle");
            showFpsBtn = new Button(new Vector2(screenSize.Width / 2 + 230f, screenSize.Height / 2 + 225f), new Vector2(3f, 2.5f), "Toggle");

            Button mainMenuBtn = new Button(new Vector2(screenSize.Width / 2, screenSize.Height / 2 + 300f), new Vector2(4f, 2.5f), "Main Menu");

            mainMenuBtn.ClickEvent += MainMenu;
            masterDown.ClickEvent += MasterVolumeDown;
            masterUp.ClickEvent += MasterVolumeUp;
            sfxDown.ClickEvent += SFXVolumeDown;
            sfxUp.ClickEvent += SFXVolumeUp;
            bgmDown.ClickEvent += BGMVolumeDown;
            bgmUp.ClickEvent += BGMVolumeUp;

            fullScreennBtn.ClickEvent += FullScreen;
            vsyncBtn.ClickEvent += VSync;
            showFpsBtn.ClickEvent += ShowFPS;

            entityManager.AddUIObject(masterDown);
            entityManager.AddUIObject(masterUp);
            entityManager.AddUIObject(sfxDown);
            entityManager.AddUIObject(sfxUp);
            entityManager.AddUIObject(bgmDown);
            entityManager.AddUIObject(bgmUp);
            entityManager.AddUIObject(fullScreennBtn);
            entityManager.AddUIObject(vsyncBtn);
            entityManager.AddUIObject(showFpsBtn);

            entityManager.AddUIObject(mainMenuBtn);
        }

        private float amount = 0.1f;
        void MasterVolumeUp(object sender, System.EventArgs e) => AudioManager.AdjustMasterVolume(amount);
        void MasterVolumeDown(object sender, System.EventArgs e) => AudioManager.AdjustMasterVolume(-amount);
        void SFXVolumeUp(object sender, System.EventArgs e) => AudioManager.AdjustSFXVolume(amount);
        void SFXVolumeDown(object sender, System.EventArgs e) => AudioManager.AdjustSFXVolume(-amount);
        void BGMVolumeUp(object sender, System.EventArgs e) => AudioManager.AdjustBGMVolume(amount);
        void BGMVolumeDown(object sender, System.EventArgs e) => AudioManager.AdjustBGMVolume(-amount);
        void FullScreen(object sender, System.EventArgs e)
        {
            fullscreen = !fullscreen;
            Game1.GraphicsDeviceManagerInstance.ToggleFullScreen();
        }
        void VSync(object sender, System.EventArgs e)
        {
            vsycn = !vsycn;
            if (vsycn)
            {
                Game1.GraphicsDeviceManagerInstance.SynchronizeWithVerticalRetrace = true;
                Game1.Instance.IsFixedTimeStep = false;
            }
            else
            {
                Game1.GraphicsDeviceManagerInstance.SynchronizeWithVerticalRetrace = false;
                Game1.Instance.IsFixedTimeStep = true;
            }
        }

        void ShowFPS(object sender, System.EventArgs e) => Game1.ShowFPS = !Game1.ShowFPS;

        void MainMenu(object sender, System.EventArgs e)
        {
            SceneManager.LoadScene(0);
        }

        public override void Update(GameTime gameTime)
        {
            Cursor.Instance.ChangeMode(CursorMode.Mouse); // Normal mouse cursor
            entityManager.Update(gameTime); // If hover button, then it will change to hover mouse
        }

        
    }
}
