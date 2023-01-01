using BulletManiac.AI;
using BulletManiac.Collision;
using BulletManiac.Entity;
using BulletManiac.Entity.Enemy;
using BulletManiac.Entity.Player;
using BulletManiac.Entity.UI;
using BulletManiac.Tiled;
using BulletManiac.Tiled.AI;
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
    /// Testing using different pathfinding algorithm
    /// </summary>
    public enum PathfindingAlgorithm
    {
        Dijkstra, AStar
    }

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
        #endregion

        // Level
        public static Player Player { get; private set; }
        public static List<Level> Levels = new();
        public static Level CurrentLevel { get { return levelManager.CurrentLevel; } }
        public static int CurrentLevelIndex;

        // Pathfinding
        private static PathTester pathTester;
        public static PathfindingAlgorithm CurrentPathfindingAlgorithm = PathfindingAlgorithm.AStar;

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
        private static LevelManager levelManager;

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
        }

        private static void LoadDefaultResources()
        {
            // Load Sprites
            Resources.LoadTexture("Bullet1", "SpriteSheet/Bullet/Bullets1_16x16");
            Resources.LoadTexture("BulletEffect", "SpriteSheet/Bullet/BulletEffect_16x16");
            Resources.LoadTexture("Walking_Smoke", "SpriteSheet/Effect/Smoke_Walking");
            Resources.LoadTexture("Destroy_Smoke", "SpriteSheet/Effect/Smoke_Destroy");
            Resources.LoadTexture("Player_Pistol", "SpriteSheet/Gun/[FULL]PistolV1.01");
            Resources.LoadTexture("Shadow", "SpriteSheet/Effect/Shadow");

            // Load Player Sprites
            Resources.LoadTexture("Player_SpriteSheet", "SpriteSheet/Player/AnimationSheet_Player");

            // Load Enemy Sprites
            Resources.LoadTexture("Bat_Flying", "SpriteSheet/Enemy/Bat/Flying");
            Resources.LoadTexture("Bat_Attack", "SpriteSheet/Enemy/Bat/Attack");
            Resources.LoadTexture("Bat_Hit", "SpriteSheet/Enemy/Bat/Hit");

            Resources.LoadTexture("Shadow_Idle", "SpriteSheet/Enemy/Shadow/Idle");
            Resources.LoadTexture("Shadow_Move", "SpriteSheet/Enemy/Shadow/Move");
            Resources.LoadTexture("Shadow_Hit", "SpriteSheet/Enemy/Shadow/Hit");
            Resources.LoadTexture("Shadow_Death", "SpriteSheet/Enemy/Shadow/Death");
            Resources.LoadTexture("Shadow_Attack", "SpriteSheet/Enemy/Shadow/Attack");

            Resources.LoadTexture("SuicideShadow_Idle", "SpriteSheet/Enemy/Suicide Shadow/Idle");
            Resources.LoadTexture("SuicideShadow_Move", "SpriteSheet/Enemy/Suicide Shadow/Move");
            Resources.LoadTexture("SuicideShadow_Attack", "SpriteSheet/Enemy/Suicide Shadow/Attack");
            Resources.LoadTexture("SuicideShadow_Explode", "SpriteSheet/Enemy/Suicide Shadow/Explode");
            Resources.LoadTexture("Summoner_SpriteSheet", "SpriteSheet/Enemy/Summoner/SpriteSheet");

            // Load UI Sprites
            Resources.LoadTexture("Crosshair_SpriteSheet", "SpriteSheet/UI/Crosshair");
            Resources.LoadTexture("Bullet_Fill", "UI/Bullet/bullet_fill");
            Resources.LoadTexture("Bullet_Empty", "UI/Bullet/bullet_empty");
            Resources.LoadSpriteFonts("DebugFont", "UI/Font/DebugFont");

            // Load Debug UI Sprites
            Resources.LoadTexture("Debug_Direction", "SpriteSheet/DebugUI/direction_16x16");
            Resources.LoadTexture("Debug_Path", "SpriteSheet/DebugUI/path_16x16");

            // Load Tiled Map level
            Resources.LoadTiledMap("Level1", "Tiled/Level/Level1");
            Resources.LoadTiledMap("Level2", "Tiled/Level/Level2");
            Resources.LoadTiledMap("Level3", "Tiled/Level/Level3");

            // Load Sound Effect
            Resources.LoadSoundEffect("Footstep1", "Audio/Footstep/Footstep1");
            Resources.LoadSoundEffect("Footstep2", "Audio/Footstep/Footstep2");
            Resources.LoadSoundEffect("Footstep3", "Audio/Footstep/Footstep3");
            Resources.LoadSoundEffect("Player_Hurt", "Audio/Player/Player_Hurt");
            Resources.LoadSoundEffect("Gun_Shoot", "Audio/Gun/Gun_Shoot");
            Resources.LoadSoundEffect("Mag_In", "Audio/Gun/Mag_In");
            Resources.LoadSoundEffect("Pistol_Cock", "Audio/Gun/Pistol_Cock");
            Resources.LoadSoundEffect("Bullet_Hit", "Audio/Gun/Bullet_Hit");
            Resources.LoadSoundEffect("Bat_Death", "Audio/Enemy/Bat Death");
            Resources.LoadSoundEffect("Shadow_Death", "Audio/Enemy/Shadow_Death");
            Resources.LoadSoundEffect("SuicideShadow_Explosion", "Audio/Enemy/SuicideShadow_Explosion");
            Resources.LoadSoundEffect("SuicideShadow_Attacking", "Audio/Enemy/SuicideShadow_Attacking");
            Resources.LoadSoundEffect("SuicideShadow_AttackStart", "Audio/Enemy/SuicideShadow_AttackStart");

            Animation.LoadAnimations(Resources);
            Bat.LoadContent(Resources);
            LevelManager.LoadContent(Resources);

            // Shader
            Resources.LoadEffect("Color_Overlay", "Shader/ColorOverlay");
        }
        
        public static void LoadContent(ContentManager content)
        {
            Resources.Load(content); // Initialize the Resource Manager
            LoadDefaultResources(); // Load default resources needed for the game to start

            // Add cursor and plpayer
            AddGameObjectUI(new Cursor()); // Add the game cursor
            Player = new Player(new Vector2(50f)); // Create Player in the game
            AddGameObject(Player); // Add player
            MagazineUI megazineUI = new MagazineUI(Player.Gun);
            AddGameObjectUI(megazineUI);

            // Pathfinding
            pathTester = new PathTester(Resources.FindLevel("Level1-1"));
            levelManager = new LevelManager(tiledMapRenderer, pathTester);
        }

        static FrameCounter fpsCounter = new();
        public static void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds; // Update the delta time variable
            InputManager.Update(gameTime); // Update the input manager
            tiledMapRenderer.Update(gameTime); // Tiled Map Update

            NavigationAgent.GlobalUpdate();
            SteeringAgent.GlobalUpdate();
            CollisionManager.Update(gameTime); // Collision Update
            entityManager.Update(gameTime); // Entity Manager Update

            // Camera Update
            MainCamera.Update(GraphicsDevice.Viewport);
            MainCamera.Follow(FindGameObject("Player")); // Always follow the player

            // Update debug status
            if (InputManager.GetKey(Keys.F12)) Debug = !Debug;
            if (InputManager.GetKey(Keys.R)) levelManager.ChangeLevel(); // Test change level
            // Test Enemy
            if(InputManager.GetKey(Keys.G))
                AddGameObject(new Shadow(CurrentLevel.TileGraph.RandomPosition));
            if (InputManager.GetKey(Keys.F))
                AddGameObject(new Bat(CurrentLevel.TileGraph.RandomPosition));
            if (InputManager.GetKey(Keys.H))
                AddGameObject(new SuicideShadow(CurrentLevel.TileGraph.RandomPosition));
            if (InputManager.GetKey(Keys.J))
                AddGameObject(new Summoner(CurrentLevel.TileGraph.RandomPosition));

            if (Debug)
                pathTester.Update(gameTime);

            fpsCounter.Update(gameTime);
        }

        /// <summary>
        /// Draw game objects on the game world layer
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            tiledMapRenderer.Draw(viewMatrix: MainCamera.Transform); // Render the Tiled

            // Debug draw for the tiles collision
            if (Debug)
            {
                foreach (Tile t in CollisionManager.TileBounds)
                {
                    t.Draw(spriteBatch, gameTime);
                }
                TileGraph.DebugDrawGraph(spriteBatch, CurrentLevel.TileGraph);
                pathTester.Draw(spriteBatch);
            }

            entityManager.Draw(spriteBatch, gameTime);       
        }

        /// <summary>
        /// Draw game object on the UI layer
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public static void DrawUI(SpriteBatch spriteBatch, GameTime gameTime)
        {
            entityManager.DrawUI(spriteBatch, gameTime);
            
            fpsCounter.Draw(spriteBatch, Resources.FindSpriteFont("DebugFont"), new Vector2(150f, 5f), Color.Red);
            spriteBatch.DrawString(Resources.FindSpriteFont("DebugFont"), "Player HP: " + Player.HP.ToString("N0"), new Vector2(5f, 5f), Color.Red);
            spriteBatch.DrawString(Resources.FindSpriteFont("DebugFont"), "Enenmy Count: " + entityManager.EnemyCount, new Vector2(5f, 20f), Color.Red);
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
