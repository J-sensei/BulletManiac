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
        public static GraphicsDevice GraphicsDeviceInstance { get; private set; }
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
            //_graphics.SynchronizeWithVerticalRetrace = true; //Vsync
            //IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            GraphicsDeviceInstance = _graphics.GraphicsDevice;
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
            SceneManager.Add(new MainMenuScene());
            SceneManager.Add(new GameScene());
            SceneManager.LoadScene(1);

            ResourcesManager.LoadTexture("Cursor", "SpriteSheet/UI/Cursor");
            Cursor.Instance = new Cursor();
            entityManager.AddUIObject(Cursor.Instance); // Add the game cursor
            entityManager.Initialize();

            //GameManager.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Time.totalTime = (float)gameTime.TotalGameTime.TotalSeconds; // Total time of the program
            Time.deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds; // Update the delta time variable

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

            //if (InputManager.GetKey(Keys.Q))
            //{
            //    SceneManager.GetScene(0).StopUpdate();
            //}
            //if (InputManager.GetKey(Keys.E))
            //{
            //    SceneManager.LoadScene(0);
            //}

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
            //GameManager.Draw(_spriteBatch, gameTime); // GameManager contains all the stuffs to draw
            SceneManager.Draw(_spriteBatch, gameTime);
            entityManager.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();

            // Game UI Layer
            // Render the game UI without affect by the Camera
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            //GameManager.DrawUI(_spriteBatch, gameTime);
            SceneManager.DrawUI(_spriteBatch, gameTime);
            entityManager.DrawUI(_spriteBatch, gameTime);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}