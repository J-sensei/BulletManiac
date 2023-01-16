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
    public class GameOverScene : Scene
    {
        const string TITLE = "Game Over";
        const string SUB_TITLE = "Results";
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

        float yOffset = -20f;
        public override void DrawUI(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Title"), TITLE, titlePosition, STRING_COLOR);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), SUB_TITLE, subtitlePosition, STRING_COLOR);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), "Floor: " + GameResult.Floor + " (+"+GameResult.FloorScore+")",
                                    new Vector2(screenSize.Width / 2 - 300f, screenSize.Height / 2 - 100f + yOffset), STRING_COLOR);
            //spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), "Level Clear: Yes",
            //            new Vector2(screenSize.Width / 2 - 300f, screenSize.Height / 2 - 100f + yOffset), STRING_COLOR);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), "Time Completed: " + GameManager.TimePassString + "s" + " (-" + GameResult.TimeScore + ")",
                        new Vector2(screenSize.Width / 2 - 300f, screenSize.Height / 2 - 50f + yOffset), STRING_COLOR);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), "Bat Eliminated: " + GameResult.Bat + " (+" + GameResult.Bat + ")",
                        new Vector2(screenSize.Width / 2 - 300f, screenSize.Height / 2 + yOffset), STRING_COLOR);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), "Shadow Eliminated: " + GameResult.Shadow + " (+" + (GameResult.Shadow*2) + ")",
                        new Vector2(screenSize.Width / 2 - 300f, screenSize.Height / 2 + 50f + yOffset), STRING_COLOR);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), "Suicide Shadow Eliminated: " +GameResult.SuicideShadow + " (+" + (GameResult.SuicideShadow*3) + ")",
                        new Vector2(screenSize.Width / 2 - 300f, screenSize.Height / 2 + 100f + yOffset), STRING_COLOR);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), "Summoner Eliminated: " + +GameResult.Summoner + " (+" + (GameResult.Summoner*4) + ")",
                        new Vector2(screenSize.Width / 2 - 300f, screenSize.Height / 2 + 150f + yOffset), STRING_COLOR);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), "Enemy Eliminated: " + (GameResult.Bat+ GameResult.Shadow+ GameResult.SuicideShadow+ GameResult.Summoner),
                        new Vector2(screenSize.Width / 2 - 300f, screenSize.Height / 2 + 200f + yOffset), STRING_COLOR);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), "Final Score: " + GameResult.Score,
                        new Vector2(screenSize.Width / 2 - 300f, screenSize.Height / 2 + 250f + yOffset), STRING_COLOR);

            entityManager.DrawUI(spriteBatch, gameTime);
        }

        private Vector2 titlePosition;
        private Vector2 subtitlePosition;

        Rectangle screenSize;
        public override void Initialize()
        {
            entityManager = new();
            GameResult.Calculate(); // Calculate the result

            ClearColor = new Color(7, 24, 33);
            screenSize = Game1.GraphicsDeviceInstance.Viewport.Bounds;

            // Title
            float offsetX = ResourcesManager.FindSpriteFont("Font_Title").MeasureString(TITLE).X / 2;
            titlePosition = new Vector2(screenSize.Width / 2 - offsetX, screenSize.Height / 2 - 300f);

            // Sub title 
            float x = ResourcesManager.FindSpriteFont("Font_Normal").MeasureString(SUB_TITLE).X / 2;
            subtitlePosition = new Vector2(screenSize.Width / 2 - x, screenSize.Height / 2 - 220f);

            Button retryBtn = new Button(new Vector2(screenSize.Width / 2 - 200f, screenSize.Height / 2 + 315f), new Vector2(4f, 2.5f), "Retry");
            Button mainMenuBtn = new Button(new Vector2(screenSize.Width / 2 + 200f, screenSize.Height / 2 + 315f), new Vector2(4f, 2.5f), "Main Menu");

            mainMenuBtn.ClickEvent += MainMenu;
            retryBtn.ClickEvent += Retry;

            entityManager.AddUIObject(mainMenuBtn);
            entityManager.AddUIObject(retryBtn);
            AudioManager.PlayMusic("Result");
        }

        void MainMenu(object sender, System.EventArgs e) => SceneManager.LoadScene(0);
        void Retry(object sender, System.EventArgs e) => SceneManager.LoadScene(1);

        public override void Update(GameTime gameTime)
        {
            Cursor.Instance.ChangeMode(CursorMode.Mouse); // Normal mouse cursor
            entityManager.Update(gameTime); // If hover button, then it will change to hover mouse
        }

    }
}
