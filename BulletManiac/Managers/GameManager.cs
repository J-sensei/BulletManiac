using BulletManiac.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BulletManiac.Managers
{
    public static class GameManager
    {
        private static List<Vector2> ResolutionList = new List<Vector2>()
        {
            new Vector2(320, 180), // Scale 2
            new Vector2(640, 360), // Scale 3
            new Vector2(1280, 720), // Scale 4
            new Vector2(2560, 1080), // Scale 5
        };
        public static int CurrentResolutionIndex = 1;
        public static Vector2 CurrentResolution { 
            get
            {
                return ResolutionList[CurrentResolutionIndex];
            } 
        }

        public static ResourcesManager Resources = new();

        private static EntitiesManager entitiesManager = new();

        /// <summary>
        /// Time different between each frames of the game
        /// </summary>
        public static float DeltaTime { get; private set; }

        public static void AddGameObject(GameObject gameObject)
        {
            entitiesManager.Add(gameObject);
        }

        public static GameObject FindGameObject(string name)
        {
            return entitiesManager.Find(name);
        }

        public static GameObject FindGameObject(GameObject gameObject)
        {
            return entitiesManager.Find(gameObject);
        }

        public static void Initialize()
        {
            // Init Resolution setting

            entitiesManager.Initialize();
        }

        public static void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            InputManager.Update(gameTime);
            entitiesManager.Update(gameTime);
        }

        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            entitiesManager.Draw(spriteBatch, gameTime);
        }
    }
}
