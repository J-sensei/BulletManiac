using BulletManiac.Entity;
using BulletManiac.Tiled;
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
        private static List<float> ScaleList = new List<float>()
        {
            1.5f, 3f, 6f, 12f
        };

        public static int CurrentResolutionIndex = 1;
        public static Vector2 CurrentResolution { 
            get
            {
                return ResolutionList[CurrentResolutionIndex];
            } 
        }

        public static float CurrentGameScale { get { return ScaleList[CurrentResolutionIndex]; } }

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
        static Tilemap map;
        public static void Initialize()
        {
            // Init Resolution setting
            Resources.LoadTileset("Tileset/CosmicLilac_Tileset");
            Resources.LoadTileset("Tileset/FD_Dungeon_Free");
            Resources.LoadTilemap("Test", "Tilemap/Cosmic_Test"); // Test loading tilemap
            //Resources.LoadTileset("Tileset/FD_Dungeon_Free_32x32");
            //Resources.LoadTilemap("Dungeon_Test_32x32", "Tilemap/Dungeon_Test_32x32"); // Test loading tilemap
            //map = new Tilemap(Resources.FindTileset("CosmicLilac_Tiles"));
            //Tileset t = new();
            //t.Load(Resources.FindXml("Test"));
            entitiesManager.Initialize();
        }

        public static void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            InputManager.Update(gameTime);

            Resources.FindTilemap("Test").Update(gameTime); // test update tilemap
            //Resources.FindTilemap("Dungeon_Test_32x32").Update(gameTime);
            entitiesManager.Update(gameTime);

        }

        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //Resources.FindTilemap("Dungeon_Test_32x32").Draw(spriteBatch, gameTime); // Test draw tilemap
            Resources.FindTilemap("Test").Draw(spriteBatch, gameTime); // Test draw tilemap

            //map.Draw(spriteBatch, gameTime);
            // Test drawing tile
            //spriteBatch.Draw(Resources.FindTileset("CosmicLilac_Tiles").Get(29), Vector2.Zero, null, Color.White, 0f, Vector2.Zero, new Vector2(3f, 3f), SpriteEffects.None, 0f);
            entitiesManager.Draw(spriteBatch, gameTime);       
        }
    }
}
