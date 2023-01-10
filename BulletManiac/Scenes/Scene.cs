using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Scenes
{
    public abstract class Scene
    {
        /// <summary>
        /// Determine if this state need to update
        /// </summary>
        public bool Pause { get; set; }
        /// <summary>
        /// Determine if this state need to draw
        /// </summary>
        public bool Drawing { get; set; }

        public Scene()
        {

        }

        public abstract void LoadContent();
        public abstract void UnloadContent();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
