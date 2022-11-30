using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Managers
{
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
