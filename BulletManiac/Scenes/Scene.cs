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
        /// The load content will only run once in the game
        /// </summary>
        public bool IsContentLoaded { get; private set; } = false;
        /// <summary>
        /// Determine if this state need to update
        /// </summary>
        public bool IsUpdate { get; private set; }
        /// <summary>
        /// Determine if this state need to draw
        /// </summary>
        public bool IsDrawing { get; private set; }

        public Scene()
        {

        }

        public void Stop()
        {
            IsUpdate = false;
            IsDrawing = false;
        }

        public void Start()
        {
            IsUpdate = true;
            IsDrawing = true;
        }
        public void StartDrawing() => IsDrawing = true;
        public void StopDrawing() => IsDrawing = false;
        public void StartUpdate() => IsUpdate = true;
        public void StopUpdate() => IsUpdate = false;

        public virtual void LoadContent()
        {
            IsContentLoaded = true;
        }
        public virtual void UnloadContent()
        {
            IsContentLoaded = false;
        }
        public abstract void Initialize();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
        public abstract void DrawUI(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
