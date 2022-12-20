using BulletManiac.Collision;
using BulletManiac.Entity;
using BulletManiac.Entity.Player;
using BulletManiac.Entity.UI;
using BulletManiac.Tiled;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System;
using System.Collections.Generic;

namespace BulletManiac.Managers
{
    /// <summary>
    /// Global class accessing to various functions
    /// </summary>
    public static class GameManager
    {
        #region Public Properties
        /// <summary>
        /// Showing the debug log and debug box
        /// </summary>
        public static bool Debug { get; private set; } = false;
        /// <summary>
        /// Global reference of the Graphics Device of the game
        /// </summary>
        public static GraphicsDevice GraphicsDevice { get; set; }
        /// <summary>
        /// Time different between each frames of the game
        /// </summary>
        public static float DeltaTime { get; private set; }
        public static Camera MainCamera { get; private set; }
        #endregion

        #region Resolution Settings
        /// <summary>
        /// List of resolutions available
        /// </summary>
        private static List<Point> resolutionList = new List<Point>()
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
        private static List<float> scaleList = new List<float>()
        {
            0.5f, 1f, 2f, 4f
        }; // Legacy
        /// <summary>
        /// List of zoom level for different resolution, used by Camera
        /// </summary>
        private static List<float> gameZoomLevelList = new List<float>()
        {
            1.5f, 3f, 6f, 12f
        };
        /// <summary>
        /// Current resolution choices from the resolution list
        /// </summary>
        public static int CurrentResolutionIndex { get; set; } = 2;
        public static Point CurrentResolution { get { return resolutionList[CurrentResolutionIndex]; } }
        public static float CurrentGameScale { get { return scaleList[CurrentResolutionIndex]; } }
        public static float CurrentCameraZoom { get { return gameZoomLevelList[CurrentResolutionIndex]; } }

        // Level
        public static Level CurrentLevel;
        #endregion

        /// <summary>
        /// Load / Unload and Find the resources of the game
        /// </summary>
        public static ResourcesManager Resources { get; set; } = new ResourcesManager();
        /// <summary>
        /// Render the Tiled map (Monogame Extended)
        /// </summary>
        private static TiledMapRenderer tiledMapRenderer { get; set; }
        /// <summary>
        /// Managing entities in the game
        /// </summary>
        private static EntityManager entityManager = new();

        /// <summary>
        /// Change resolution of the game
        /// </summary>
        /// <param name="graphics"></param>
        public static void UpdateScreenSize(GraphicsDeviceManager graphics)
        {
            graphics.PreferredBackBufferWidth = CurrentResolution.X;
            graphics.PreferredBackBufferHeight = CurrentResolution.Y;
            graphics.ApplyChanges();

            MainCamera.AdjustZoom(CurrentCameraZoom); // Adjust the Camera zoom level after the resolution changed
        }

        /// <summary>
        /// Add new game object into the game (Affect by the Camera)
        /// </summary>
        /// <param name="gameObject"></param>
        public static void AddGameObject(GameObject gameObject)
        {
            entityManager.Add(gameObject);
        }
        /// <summary>
        /// Add new UI game object into the game (NOT Affect by the Camera)
        /// </summary>
        /// <param name="gameObject"></param>
        public static void AddGameObjectUI(GameObject gameObject)
        {
            entityManager.AddUIObject(gameObject);
        }
        /// <summary>
        /// Find the game object in the game by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GameObject FindGameObject(string name)
        {
            return entityManager.Find(name);
        }
        /// <summary>
        /// Find the game object in the game by reference
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static GameObject FindGameObject(GameObject gameObject)
        {
            return entityManager.Find(gameObject);
        }
        
        public static void Initialize()
        {
            MainCamera = new Camera(); // Create main camera for the game (Controlling zoom level to zoom in the tiles)
            tiledMapRenderer = new TiledMapRenderer(GraphicsDevice); // Initialize Tiled

            InputManager.Initialize();
            entityManager.Initialize();
        }

        private static void LoadDefaultResources()
        {
            // Load textures
            Resources.LoadTexture("Bullet1", "SpriteSheet/Bullet/Bullets1_16x16");
            Resources.LoadTexture("Crosshair_SpriteSheet", "SpriteSheet/UI/Crosshair"); // Load the spritesheet to the resources

            // Load player sprites
            Resources.LoadTexture("Player_Death", "SpriteSheet/Player/Owlet_Monster_Death_8");
            Resources.LoadTexture("Player_Idle", "SpriteSheet/Player/Owlet_Monster_Idle_4");
            Resources.LoadTexture("Player_Walk", "SpriteSheet/Player/Owlet_Monster_Walk_6");
            Resources.LoadTexture("Player_Run", "SpriteSheet/Player/Owlet_Monster_Run_6");
            Resources.LoadTexture("Player_Throw", "SpriteSheet/Player/Owlet_Monster_Throw_4");

            // Load Tiled Map level
            Resources.LoadTiledMap("Level0", "Tiled/Level0");
            Resources.LoadTiledMap("Level1", "Tiled/Level/Level1");
        }

        public static void LoadContent(ContentManager content)
        {
            GameManager.Resources.Load(content); // Initialize the Resource Manager
            LoadDefaultResources(); // Load default resources needed for the game to start

            // Add cursor and plpayer
            AddGameObjectUI(new Cursor()); // Add the game cursor
            AddGameObject(new Player(new Vector2(50f))); // Add player

            // Set current level
            CurrentLevel = new();
            CurrentLevel.Map = Resources.FindTiledMap("Level1");
            tiledMapRenderer.LoadMap(CurrentLevel.Map);
            Tile.AddTileCollision(CurrentLevel.Map.GetLayer<TiledMapTileLayer>("Wall"), CurrentLevel.Map.Width, CurrentLevel.Map.Height, CurrentLevel.Map.TileWidth, CurrentLevel.Map.TileHeight);
        }

        public static void Update(GameTime gameTime)
        {
            #region Legacy Tile Map Test Code
            //Resources.FindTilemap("Test").Update(gameTime); // test update tilemap
            //Resources.FindTilemap("Dungeon_Test_32x32").Update(gameTime);
            #endregion

            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds; // Update the delta time
            InputManager.Update(gameTime); // Update the input manager

            entityManager.Update(gameTime); // Entity Manager Update
            CollisionManager.Update(gameTime); // Collision Update

            // Camera Update
            MainCamera.Update(GraphicsDevice.Viewport);
            MainCamera.Follow(FindGameObject("Player")); // Always follow the player

            tiledMapRenderer.Update(gameTime); // Tiled Map Update

            // Update debug status
            if (InputManager.GetKey(Keys.F12)) Debug = !Debug;
        }

        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            #region Legacy Tile Map Test Code
            //Resources.FindTilemap("Dungeon_Test_32x32").Draw(spriteBatch, gameTime); // Test draw tilemap
            //Resources.FindTilemap("Test").Draw(spriteBatch, gameTime); // Test draw tilemap
            #endregion
            tiledMapRenderer.Draw(viewMatrix: MainCamera.Transform); // Render the Tiled

            // Debug draw for the tiles collision
            foreach (Tile t in CollisionManager.TileBounds)
            {
                t.Draw(spriteBatch, gameTime);
            }

            entityManager.Draw(spriteBatch, gameTime);       
        }
        public static void DrawUI(SpriteBatch spriteBatch, GameTime gameTime)
        {
            entityManager.DrawUI(spriteBatch, gameTime);
        }

        /// <summary>
        /// Log the debug log in the game
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public static void Log(string source = "Unknown", string message = "")
        {
            if(Debug)
                Console.WriteLine(@"[{0} - {1}] {2}", source, DateTime.Now.ToString("HH:mm:ss tt"), message);
        }
    }
}
