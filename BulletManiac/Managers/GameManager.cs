using BulletManiac.Entity;
using BulletManiac.Tiled;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled.Renderers;
using System;
using System.Collections.Generic;

namespace BulletManiac.Managers
{
    public static class GameManager
    {
        private static List<Point> ResolutionList = new List<Point>()
        {
            new Point(320, 180), // Scale 2
            new Point(640, 360), // Scale 3
            new Point(1280, 720), // Scale 4
            new Point(2560, 1080), // Scale 5
        };
        //private static List<float> ScaleList = new List<float>()
        //{
        //    1.5f, 3f, 6f, 12f
        //};
        private static List<float> ScaleList = new List<float>()
        {
            0.5f, 1f, 2f, 4f
        };
        private static List<float> SpeedScaleList = new List<float>()
        {
            0.5f, 1f, 2f, 4f
        };

        private static List<float> GameZoomLevelList = new List<float>()
        {
            1.5f, 3f, 6f, 12f
        };

        public static TiledMapRenderer TiledMapRenderer { get; set; }
        public static Camera MainCamera;

        public static int CurrentResolutionIndex = 2;
        public static Point CurrentResolution { 
            get
            {
                return ResolutionList[CurrentResolutionIndex];
            } 
        }

        public static float CurrentGameScale { get { return ScaleList[CurrentResolutionIndex]; } }
        public static float CurrentSpeedScale { get { return SpeedScaleList[CurrentResolutionIndex]; } }
        public static float CurrentCameraZoom { get { return GameZoomLevelList[CurrentResolutionIndex]; } }

        public static ResourcesManager Resources = new();

        private static EntitiesManager entitiesManager = new();

        /// <summary>
        /// Time different between each frames of the game
        /// </summary>
        public static float DeltaTime { get; private set; }
        public static Point ScreenSize { get; private set; }

        #region Game settings
        public static void UpdateScreenSize(GraphicsDeviceManager graphics)
        {
            graphics.PreferredBackBufferWidth = CurrentResolution.X;
            graphics.PreferredBackBufferHeight = CurrentResolution.Y;
            graphics.ApplyChanges();

            Console.WriteLine("Camera Zoom: " + CurrentCameraZoom + " , Index: " + CurrentResolutionIndex);
            MainCamera.AdjustZoom(CurrentCameraZoom);

            ScreenSize = CurrentResolution;
        }
        #endregion

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

        public static void InitializeTileRenderer(GraphicsDevice graphicsDevice)
        {
            TiledMapRenderer = new TiledMapRenderer(graphicsDevice);
        }
        
        public static void Initialize()
        {
            //Resources.LoadTileset("Tileset/CosmicLilac_Tileset");
            //Resources.LoadTileset("Tileset/FD_Dungeon_Free");
            //Resources.LoadTilemap("Test", "Tilemap/Cosmic_Test"); // Test loading tilemap

            // Test Tiled Map
            Resources.LoadTiledMap("Level0", "Tiled/Level0");
            TiledMapRenderer.LoadMap(Resources.FindTiledMap("Level0"));

            //Resources.LoadTileset("Tileset/FD_Dungeon_Free_32x32");
            //Resources.LoadTilemap("Dungeon_Test_32x32", "Tilemap/Dungeon_Test_32x32"); // Test loading tilemap
            //map = new Tilemap(Resources.FindTileset("CosmicLilac_Tiles"));
            //Tileset t = new();
            //t.Load(Resources.FindXml("Test"));
            entitiesManager.Initialize();
        }

        public static void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds; // Update the delta time
            InputManager.Update(gameTime); // Update the input manager

            //Resources.FindTilemap("Test").Update(gameTime); // test update tilemap
            //Resources.FindTilemap("Dungeon_Test_32x32").Update(gameTime);
            TiledMapRenderer.Update(gameTime);
            entitiesManager.Update(gameTime);
        }

        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //Resources.FindTilemap("Dungeon_Test_32x32").Draw(spriteBatch, gameTime); // Test draw tilemap
            //Resources.FindTilemap("Test").Draw(spriteBatch, gameTime); // Test draw tilemap
            TiledMapRenderer.Draw(MainCamera.Transform);
            entitiesManager.Draw(spriteBatch, gameTime);       
        }
    }
}
