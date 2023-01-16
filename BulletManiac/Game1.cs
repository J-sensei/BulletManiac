using BulletManiac.Entity.UI;
using BulletManiac.Managers;
using BulletManiac.Scenes;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BulletManiac
{
    public class Game1 : Game
    {
        const int INITIAL_SCENE = 4;
        /// <summary>
        /// Singleton of the Game (Monogame)
        /// </summary>
        public static Game Instance { get; private set; } 
        public static GraphicsDevice GraphicsDeviceInstance { get; private set; }
        public static GraphicsDeviceManager GraphicsDeviceManagerInstance { get; private set; }
        public static bool ShowFPS { get; set; } = true;
        private FrameCounter fpsCounter = new();
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        /// <summary>
        /// Entities that will run throught out the game
        /// </summary>
        private EntityManager entityManager = new();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this) { PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8 }; ;
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            Instance = this;
        }

        protected override void Initialize()
        {
            GraphicsDeviceInstance = _graphics.GraphicsDevice;
            GraphicsDeviceManagerInstance = _graphics;
            GameManager.GraphicsDevice = _graphics.GraphicsDevice; // Initialize the Graphics Device in the Game Manager first
            Window.Title = "Bullet Maniac";

            // Initialize objects
            InputManager.Initialize();
            //GameManager.Initialize();
            GameManager.UpdateScreenSize(_graphics); // Update the default resolution

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ResourcesManager.Initialize(Content);
            ResourcesManager.LoadSong("BGM1", "Audio/Song/BGM1");
            ResourcesManager.LoadSong("BGM2", "Audio/Song/BGM2");
            ResourcesManager.LoadSong("BGM3", "Audio/Song/BGM3");
            ResourcesManager.LoadSong("BGM4", "Audio/Song/BGM4");
            ResourcesManager.LoadSong("MainMenu", "Audio/Song/MainMenu");
            ResourcesManager.LoadSong("Result", "Audio/Song/Result");
            ResourcesManager.LoadSpriteFonts("Font_Normal", "UI/Font/Font_Normal");

            SceneManager.Add(new MainMenuScene());
            SceneManager.Add(new GameScene());
            SceneManager.Add(new PauseScene());
            SceneManager.Add(new OptionScene());
            SceneManager.Add(new GameOverScene());
            SceneManager.LoadScene(INITIAL_SCENE);

            ResourcesManager.LoadTexture("Cursor", "SpriteSheet/UI/Cursor");
            Cursor.Instance = new Cursor();
            entityManager.AddUIObject(Cursor.Instance); // Add the game cursor
            entityManager.Initialize();

            //GameManager.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            Time.totalTime = (float)gameTime.TotalGameTime.TotalSeconds; // Total time of the program
            Time.deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds; // Update the delta time variable

            if(ShowFPS) fpsCounter.Update(gameTime);

            InputManager.Update(gameTime); // Update the input manager
            entityManager.Update(gameTime);
            //GameManager.Update(gameTime); // Update all the game stuffs
            SceneManager.Update(gameTime);
            base.Update(gameTime);

            // Test Resolution change
            if (InputManager.GetKey(Keys.P))
            {
                GameManager.CurrentResolutionIndex = ++GameManager.CurrentResolutionIndex % 4;
                GameManager.UpdateScreenSize(_graphics);
            }

            IsMouseVisible = GameManager.Debug; // Show mouse cursor in debug mode
        }

        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(GameManager.CurrentLevel.BackgroundColor); // Default screen clear color (Same color as the map)
            //GraphicsDevice.Clear(Color.CornflowerBlue); // Default screen clear color (Same color as the map)
            GraphicsDevice.Clear(SceneManager.ClearColor);

            // Game World Layer
            // SpriteBatch Begin settings make sure the texture sprite is clean when scale up
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, transformMatrix: Camera.Main.Transform);
            SceneManager.Draw(_spriteBatch, gameTime);
            entityManager.Draw(_spriteBatch, gameTime);
            fpsCounter.Draw(_spriteBatch);
            _spriteBatch.End();

            // Game UI Layer
            // Render the game UI without affect by the Camera
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            SceneManager.DrawUI(_spriteBatch, gameTime);
            entityManager.DrawUI(_spriteBatch, gameTime);

            if (ShowFPS)
            {
                _spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), "FPS: " + fpsCounter.FPS.ToString(), new Vector2(5f), Color.White, 0f, Vector2.Zero, new Vector2(0.5f), SpriteEffects.None, 0f);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}