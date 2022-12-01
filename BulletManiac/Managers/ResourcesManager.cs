using BulletManiac.Tiled;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BulletManiac.Managers
{
    /// <summary>
    /// Manage all resources throughout the game
    /// </summary>
    public class ResourcesManager
    {
        private ContentManager contentManager;
        private readonly Dictionary<string, Texture2D> Textures = new();
        private readonly Dictionary<string, TilesetData> TilesetData = new();

        public void Load(ContentManager contentManager)
        {
            this.contentManager = contentManager;
        }

        public void Add(string name, Texture2D texture)
        {
            Textures.Add(name, texture);
        }
        public void Add(string name, TilesetData data)
        {
            TilesetData.Add(name, data);
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

        public void LoadTilesetData(string name, string path)
        {
            TilesetData data = contentManager.Load<TilesetData>(path);
            Add(name, data);
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

        public TilesetData FindTilesetData(string name)
        {
            return TilesetData[name];
        }
    }
}
