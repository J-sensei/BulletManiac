using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Scenes
{
    public static class SceneManager
    {
        private static List<Scene> states = new();
        private static int currentStateIndex = 0;

        public static void Add(Scene state)
        {
            states.Add(state);
        }

        public static void Update(GameTime gameTime)
        {
            states[currentStateIndex].Update(gameTime);
        }

        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            states[currentStateIndex].Draw(spriteBatch, gameTime);
        }
    }
}
