using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Managers
{
    /// <summary>
    /// Manage all resources throughout the game
    /// </summary>
    public class ResourcesManager
    {
        private ContentManager contentManager;
        private readonly Dictionary<string, Texture2D> Textures = new();

        public void Load(ContentManager contentManager)
        {
            this.contentManager = contentManager;
        }

        public void Add(string name, Texture2D texture)
        {
            Textures.Add(name, texture);
        }

        /// <summary>
        /// Load the texture by specify the name and path using content manager
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        public void LoadTexture(string name, string path)
        {
            Texture2D texture = contentManager.Load<Texture2D>(path);
            Add(name, texture);
        }

        public Texture2D FindTexture(string name)
        {
            if (Textures[name] != null)
            {
                return Textures[name];
            }
            else
            {
                throw new NullReferenceException("Name of the texture is not found in the resources");
            }
        }

        public void RemoveTexture(string name)
        {
            Textures.Remove(name);
        }
    }
}
