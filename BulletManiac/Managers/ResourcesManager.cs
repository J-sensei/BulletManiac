using BulletManiac.Entity;
using Microsoft.Xna.Framework.Audio;
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
        /// Sound Effects to use across the game (Load WAV format file)
        /// </summary>
        private readonly Dictionary<string, SoundEffect> SoundEffects = new();
        /// <summary>
        /// Font used to render string as the game ui
        /// </summary>
        private readonly Dictionary<string, SpriteFont> SpriteFonts = new();

        private readonly Dictionary<string, Animation> Animations = new(); // Test Code

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
            if (!TiledMaps.ContainsKey(name))
            {
                TiledMap data = contentManager.Load<TiledMap>(path);
                TiledMaps.Add(name, data);
            }
            else
            {
                GameManager.Log("Resources Manager", "Duplicate name '" + name + "' is failed to add into TiledMap resources.");
            }
        }
        public void LoadAnimation(string name, Animation animation)
        {
            if (!Animations.ContainsKey(name))
            {
                Animations.Add(name, animation);
            }
            else
            {
                GameManager.Log("Resources Manager", "Duplicate name '" + name + "' is failed to add into Animations resources.");
            }
        }

        public void LoadSoundEffect(string name, string path)
        {
            if (!SoundEffects.ContainsKey(name))
            {
                SoundEffect data = contentManager.Load<SoundEffect>(path);
                SoundEffects.Add(name, data);
            }
            else
            {
                GameManager.Log("Resources Manager", "Duplicate name '" + name + "' is failed to add into Sound Effect resources.");
            }
        }

        public void LoadSpriteFonts(string name, string path)
        {
            if (!SpriteFonts.ContainsKey(name))
            {
                SpriteFont data = contentManager.Load<SpriteFont>(path);
                SpriteFonts.Add(name, data);
            }
            else
            {
                GameManager.Log("Resources Manager", "Duplicate name '" + name + "' is failed to add into Sprite Fonts resources.");
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

        public Animation FindAnimation(string name)
        {
            return Animations[name];
        }

        public SoundEffect FindSoundEffect(string name)
        {
            return SoundEffects[name];
        }

        public SpriteFont FindSpriteFont(string name)
        {
            return SpriteFonts[name];
        }

        public void RemoveTexture(string name)
        {
            Textures.Remove(name);
        }
    }
}
