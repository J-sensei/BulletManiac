using BulletManiac.Tiled;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;

namespace BulletManiac.Managers
{
    /// <summary>
    /// Manage all resources throughout the game
    /// </summary>
    public class ResourcesManager
    {
        private ContentManager contentManager;
        private readonly Dictionary<string, Texture2D> Textures = new();
        /// <summary>
        /// Tileset generated when loading tileset data
        /// </summary>
        private readonly Dictionary<string, Tileset> Tilesets = new();
        private readonly Dictionary<string, Tilemap> Tilemaps = new();

        /// <summary>
        /// Store Tiled Map (Monogame Extended) data 
        /// </summary>
        private readonly Dictionary<string, TiledMap> TiledMaps = new();

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
            //Add(name, texture);
            if (!Textures.ContainsKey(name))
            {
                Textures.Add(name, texture);
            }
            else
            {
                Console.WriteLine("[Resources Manager] Duplicate name '" + name + "' is failed to add into Texture resources.");
            }
        }
        /// <summary>
        /// Load the texture directly from the content manager
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Texture2D LoadTextureRaw(string path)
        {
            return contentManager.Load<Texture2D>(path);
        }

        public void LoadTileset(string path)
        {
            TilesetData data = contentManager.Load<TilesetData>(path);

            // If this data is already inside the resources, do not add it
            if (!Tilesets.ContainsKey(data.ResourcesName))
            {
                Tileset tileset = new Tileset(data);

                Tilesets.Add(data.ResourcesName, tileset);
            }
            else
            {
                Console.WriteLine("[Resources Manager] Duplicate name '" + data.ResourcesName + "' is failed to add into Tileset resources.");
            }
        }

        public void LoadTilemap(string name, string path)
        {
            // If this name is already inside the resources, do not add it
            if (!Tilemaps.ContainsKey(name))
            {
                TilemapData data = contentManager.Load<TilemapData>(path);
                Tilemap map = new Tilemap(null, data);
                Tilemaps.Add(name, map);
            }
            else
            {
                Console.WriteLine("[Resources Manager] Duplicate name '" + name + "' is failed to add into Tilemap resources.");
            }
        }

        /// <summary>
        /// Load Tiled Map (Monogame Extended)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        public void LoadTiledMap(string name, string path)
        {
            if (!Tilemaps.ContainsKey(name))
            {
                TiledMap data = contentManager.Load<TiledMap>(path);
                TiledMaps.Add(name, data);
            }
            else
            {
                Console.WriteLine("[Resources Manager] Duplicate name '" + name + "' is failed to add into TiledMap resources.");
            }
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

        public Tileset FindTileset(string name)
        {
            return Tilesets[name];
        }

        public Tilemap FindTilemap(string name)
        {
            return Tilemaps[name];
        }

        public TiledMap FindTiledMap(string name)
        {
            return TiledMaps[name];
        }
    }
}
