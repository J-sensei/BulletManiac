using BulletManiac.Managers;
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
            _graphics = new GraphicsDeviceManager(this) { PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8 }; ;
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            GameManager.GraphicsDevice = _graphics.GraphicsDevice; // Initialize the Graphics Device in the Game Manager first

            // Initialize objects
            GameManager.Initialize();
            GameManager.UpdateScreenSize(_graphics); // Update the default resolution

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            GameManager.LoadContent(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            GameManager.Update(gameTime); // Update all the game stuffs
            base.Update(gameTime);

            // Teat Resolution change
            if (InputManager.GetKey(Keys.P))
            {
                GameManager.CurrentResolutionIndex = ++GameManager.CurrentResolutionIndex % 4;
                GameManager.UpdateScreenSize(_graphics);
            }

            IsMouseVisible = GameManager.Debug; // Show mouse cursor in debug mode
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(GameManager.CurrentLevel.BackgroundColor); // Default screen clear color (Same color as the map)
            //GraphicsDevice.Clear(Color.CornflowerBlue); // Default screen clear color (Same color as the map)

            // Game World Layer
            // SpriteBatch Begin settings make sure the texture sprite is clean when scale up
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, transformMatrix: GameManager.MainCamera.Transform);
            GameManager.Draw(_spriteBatch, gameTime); // GameManager contains all the stuffs to draw
            _spriteBatch.End();

            // Game UI Layer
            // Render the game UI without affect by the Camera
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            GameManager.DrawUI(_spriteBatch, gameTime);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}