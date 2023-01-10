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
        private static List<Scene> scenes = new();
        private static int currentStateIndex = 0;
        public static Color ClearColor
        {
            get
            {
                if (scenes[currentStateIndex] != null) return scenes[currentStateIndex].ClearColor;
                else return Color.CornflowerBlue;
            }
        }       

        public static void Add(Scene state)
        {
            scenes.Add(state);
        }

        /// <summary>
        /// Load the scene, will restart all the variables
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void LoadScene(int index)
        {
            if(index < scenes.Count && index >= 0)
            {
                // Stop all the scenes
                foreach (Scene scene in scenes)
                {
                    scene.Stop();
                }

                if (!scenes[index].IsContentLoaded)
                    scenes[index].LoadContent();

                scenes[index].Initialize();
                scenes[index].Start();
                currentStateIndex = index;
            }
            else
            {
                throw new ArgumentException("Index is out of bound");
            }
        }

        /// <summary>
        /// Open the open overlay current scene
        /// </summary>
        /// <param name="index"></param>
        public static void OpenScene(int index)
        {
            if (!scenes[index].IsContentLoaded)
                scenes[index].LoadContent();

            scenes[index].Initialize();
            scenes[index].Start();
        }

        public static void CloseScene(int index)
        {
            scenes[index].Stop();
        }

        public static Scene GetScene(int index)
        {
            return scenes[index];
        }

        public static void Update(GameTime gameTime)
        {
            foreach (Scene scene in scenes)
            {
                if(scene.IsUpdate)
                    scene.Update(gameTime);
            }
        }

        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach(Scene scene in scenes)
            {
                if (scene.IsDrawing)
                    scene.Draw(spriteBatch, gameTime);
            }
        }

        public static void DrawUI(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (Scene scene in scenes)
            {
                if (scene.IsDrawing)
                    scene.DrawUI(spriteBatch, gameTime);
            }
        }
    }
}
