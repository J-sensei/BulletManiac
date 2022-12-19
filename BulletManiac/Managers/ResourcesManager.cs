using BulletManiac.Tiled.Legacy;
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
        #region Legacy
        /// <summary>
        /// Tileset generated when loading tileset data
        /// </summary>
        private readonly Dictionary<string, Tileset> Tilesets = new();
        private readonly Dictionary<string, Tilemap> Tilemaps = new();
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
        public Tileset FindTileset(string name)
        {
            return Tilesets[name];
        }

        public Tilemap FindTilemap(string name)
        {
            return Tilemaps[name];
        }
        #endregion

        /// <summary>
        /// Content Manager helps to load the XNB content build by the MGCB
        /// </summary>
        private ContentManager contentManager;

        /// <summary>
        /// Textures bank, store texture need to use in the game
        /// </summary>
        private readonly Dictionary<string, Texture2D> Textures = new();
        /// <summary>
        /// Store Tiled Map (Monogame Extended) data 
        /// </summary>
        private readonly Dictionary<string, TiledMap> TiledMaps = new();

        /// <summary>
        /// Initialize the content manager
        /// </summary>
        /// <param name="contentManager"></param>
        public void Load(ContentManager contentManager)
        {
            this.contentManager = contentManager;
        }

        /// <summary>
        /// Load the texture by specify the name and path using content manager
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        public void LoadTexture(string name, string path)
        {
            // If same texture is loaded, no need to add again into the Dictionary
            if (!Textures.ContainsKey(name))
            {
                Texture2D texture = contentManager.Load<Texture2D>(path);
                Textures.Add(name, texture);
            }
            else
            {
                GameManager.Log("Resources Manager", "Duplicate name '" + name + "' is failed to add into Texture resources.");
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
                GameManager.Log("Resources Manager", "Duplicate name '" + name + "' is failed to add into TiledMap resources.");
            }
        }

        /// <summary>
        /// Find and get the texture by the name in the Dictionary
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
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

        /// <summary>
        /// Find the Tiled Map
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TiledMap FindTiledMap(string name)
        {
            return TiledMaps[name];
        }

        public void RemoveTexture(string name)
        {
            Textures.Remove(name);
        }
    }
}
