using BulletManiac.Entity.Player;
using BulletManiac.Managers;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BulletManiac
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true; // Move is visible
        }

        private Camera camera;
        protected override void Initialize()
        {
            // Pass in content manager to the resources manager to make sure it will load the resources after initialization
            //_graphics.PreferredBackBufferWidth = (int)GameManager.CurrentResolution.X;
            //_graphics.PreferredBackBufferHeight = (int)GameManager.CurrentResolution.Y;
            //_graphics.ApplyChanges();
            // Camera
            camera = new();
            GameManager.MainCamera = camera;
            GameManager.GraphicsDevice = _graphics.GraphicsDevice;

            GameManager.Resources.Load(Content);
            GameManager.AddGameObject(new Player(new Vector2(5f))); // Test code

            var p = new Player(new Vector2(45f));
            p.move = false;
            GameManager.AddGameObject(p); // Test code

            GameManager.InitializeTileRenderer(_graphics.GraphicsDevice); // Init tile rendere first
            GameManager.Initialize(); // Initialize all the game stuffs
            GameManager.UpdateScreenSize(_graphics);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            GameManager.Update(gameTime); // Update all the game stuffs
            camera.Update(_graphics.GraphicsDevice.Viewport);
            camera.Follow(GameManager.FindGameObject("Player"));

            base.Update(gameTime);

            // Teat Resolution change
            if (InputManager.GetKey(Keys.P))
            {
                GameManager.CurrentResolutionIndex = ++GameManager.CurrentResolutionIndex % 4;
                GameManager.UpdateScreenSize(_graphics);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // SpriteBatch Begin settings make sure the texture sprite is clean when scale up
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, transformMatrix: camera.Transform);
            GameManager.Draw(_spriteBatch, gameTime); // GameManager contains all the stuffs to draw
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}