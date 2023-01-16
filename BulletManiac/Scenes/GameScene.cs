using BulletManiac.Managers;
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
    /// <summary>
    /// Main game scene of the game
    /// </summary>
    public class GameScene : Scene
    {
        public override void LoadContent()
        {
            GameManager.LoadContent();
            base.LoadContent();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            GameManager.Draw(spriteBatch, gameTime);
        }

        public override void DrawUI(SpriteBatch spriteBatch, GameTime gameTime)
        {
            GameManager.DrawUI(spriteBatch, gameTime);
        }

        public override void Initialize()
        {
            GameManager.Initialize();
            AudioManager.PlayMusic("BGM1");
        }

        public override void Update(GameTime gameTime)
        {
            GameManager.Update(gameTime); // Update all the game stuffs
            ClearColor = GameManager.CurrentLevel.BackgroundColor;
        }
    }
}
