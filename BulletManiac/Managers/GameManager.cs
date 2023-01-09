using BulletManiac.AI;
using BulletManiac.Collision;
using BulletManiac.Entity;
using BulletManiac.Entity.Enemy;
using BulletManiac.Entity.Player;
using BulletManiac.Entity.UI;
using BulletManiac.Particle;
using BulletManiac.SpriteAnimation;
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
        public static Camera MainCamera { get; private set; }
        /// <summary>
        /// Global accessible player
        /// </summary>
        public static Player Player { get; private set; }
        /// <summary>
        /// Current Level of the game
        /// </summary>
        public static Level CurrentLevel { get { return levelManager.CurrentLevel; } }
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

        // Pathfinding
        private static PathTester pathTester;
        public static PathfindingAlgorithm CurrentPathfindingAlgorithm = PathfindingAlgorithm.AStar;
        
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
        /// Spawner, responsible to handle the spawn
        /// </summary>
        private static Spawner spawner = new();

        /// <summary>
        /// Is player eliminated all the enemies
        /// </summary>
        public static bool IsLevelFinish { get { return spawner.IsFinish && entityManager.EnemyCount <= 0; } }

        /// <summary>
        /// Play the transition effect of the game
        /// </summary>
        private static TransitionEffect transitionEffect;
        /// <summary>
        /// Frame and Update counter for the game
        /// </summary>
        private static FrameCounter fpsCounter = new();

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
        public static GameObject FindNearestEnemy(GameObject gameObject)
        {
            return entityManager.FindNearestEnemy(gameObject);
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
            ResourcesManager.LoadTexture("Bullet1", "SpriteSheet/Bullet/Bullets1_16x16");
            ResourcesManager.LoadTexture("BulletEffect", "SpriteSheet/Bullet/BulletEffect_16x16");
            ResourcesManager.LoadTexture("Walking_Smoke", "SpriteSheet/Effect/Smoke_Walking");
            ResourcesManager.LoadTexture("Destroy_Smoke", "SpriteSheet/Effect/Smoke_Destroy");
            ResourcesManager.LoadTexture("Spawn_Smoke", "SpriteSheet/Effect/Smoke_Spawn");
            ResourcesManager.LoadTexture("Player_Pistol", "SpriteSheet/Gun/[FULL]PistolV1.01");
            ResourcesManager.LoadTexture("Shadow", "SpriteSheet/Effect/Shadow");

            // Load Player Sprites
            ResourcesManager.LoadTexture("Player_SpriteSheet", "SpriteSheet/Player/AnimationSheet_Player");

            // Load Enemy Sprites
            ResourcesManager.LoadTexture("Bat_Flying", "SpriteSheet/Enemy/Bat/Flying");
            ResourcesManager.LoadTexture("Bat_Attack", "SpriteSheet/Enemy/Bat/Attack");
            ResourcesManager.LoadTexture("Bat_Hit", "SpriteSheet/Enemy/Bat/Hit");

            ResourcesManager.LoadTexture("Shadow_Idle", "SpriteSheet/Enemy/Shadow/Idle");
            ResourcesManager.LoadTexture("Shadow_Move", "SpriteSheet/Enemy/Shadow/Move");
            ResourcesManager.LoadTexture("Shadow_Hit", "SpriteSheet/Enemy/Shadow/Hit");
            ResourcesManager.LoadTexture("Shadow_Death", "SpriteSheet/Enemy/Shadow/Death");
            ResourcesManager.LoadTexture("Shadow_Attack", "SpriteSheet/Enemy/Shadow/Attack");

            ResourcesManager.LoadTexture("SuicideShadow_Idle", "SpriteSheet/Enemy/Suicide Shadow/Idle");
            ResourcesManager.LoadTexture("SuicideShadow_Move", "SpriteSheet/Enemy/Suicide Shadow/Move");
            ResourcesManager.LoadTexture("SuicideShadow_Attack", "SpriteSheet/Enemy/Suicide Shadow/Attack");
            ResourcesManager.LoadTexture("SuicideShadow_Explode", "SpriteSheet/Enemy/Suicide Shadow/Explode");
            ResourcesManager.LoadTexture("Summoner_SpriteSheet", "SpriteSheet/Enemy/Summoner/SpriteSheet");

            // Load UI Sprites
            ResourcesManager.LoadTexture("Crosshair_SpriteSheet", "SpriteSheet/UI/Crosshair");
            ResourcesManager.LoadTexture("Transition_Texture", "UI/Transition_Texture");
            ResourcesManager.LoadTexture("Bullet_Fill", "UI/Bullet/bullet_fill");
            ResourcesManager.LoadTexture("Bullet_Empty", "UI/Bullet/bullet_empty");
            ResourcesManager.LoadSpriteFonts("DebugFont", "UI/Font/DebugFont");

            // Load Debug UI Sprites
            ResourcesManager.LoadTexture("Debug_Direction", "SpriteSheet/DebugUI/direction_16x16");
            ResourcesManager.LoadTexture("Debug_Path", "SpriteSheet/DebugUI/path_16x16");

            // Load Tiled Map level
            ResourcesManager.LoadTiledMap("Level1", "Tiled/Level/Level1");
            ResourcesManager.LoadTiledMap("Level2", "Tiled/Level/Level2");
            ResourcesManager.LoadTiledMap("Level3", "Tiled/Level/Level3");

            // Load Sound Effect
            ResourcesManager.LoadSoundEffect("Footstep1", "Audio/Footstep/Footstep1");
            ResourcesManager.LoadSoundEffect("Footstep2", "Audio/Footstep/Footstep2");
            ResourcesManager.LoadSoundEffect("Footstep3", "Audio/Footstep/Footstep3");
            ResourcesManager.LoadSoundEffect("Player_Hurt", "Audio/Player/Player_Hurt");
            ResourcesManager.LoadSoundEffect("Gun_Shoot", "Audio/Gun/Gun_Shoot");
            ResourcesManager.LoadSoundEffect("Mag_In", "Audio/Gun/Mag_In");
            ResourcesManager.LoadSoundEffect("Pistol_Cock", "Audio/Gun/Pistol_Cock");
            ResourcesManager.LoadSoundEffect("Bullet_Hit", "Audio/Gun/Bullet_Hit");
            ResourcesManager.LoadSoundEffect("Bat_Death", "Audio/Enemy/Bat Death");
            ResourcesManager.LoadSoundEffect("Shadow_Death", "Audio/Enemy/Shadow_Death");
            ResourcesManager.LoadSoundEffect("SuicideShadow_Explosion", "Audio/Enemy/SuicideShadow_Explosion");
            ResourcesManager.LoadSoundEffect("SuicideShadow_Attacking", "Audio/Enemy/SuicideShadow_Attacking");
            ResourcesManager.LoadSoundEffect("SuicideShadow_AttackStart", "Audio/Enemy/SuicideShadow_AttackStart");
            ResourcesManager.LoadSoundEffect("Enemy_Spawn", "Audio/Enemy/Enemy_Spawn");

            Animation.LoadAnimations();
            Bat.LoadContent();
            LevelManager.LoadContent();

            // Shader
            ResourcesManager.LoadEffect("Color_Overlay", "Shader/ColorOverlay");
        }
        
        public static void LoadContent(ContentManager content)
        {
            LoadDefaultResources(); // Load default resources needed for the game to start

            // Add cursor and plpayer
            AddGameObjectUI(new Cursor()); // Add the game cursor
            Player = new Player(new Vector2(50f)); // Create Player in the game
            AddGameObject(Player); // Add player
            MagazineUI megazineUI = new MagazineUI(Player.Gun, new Vector2(CurrentResolution.X - 100, CurrentResolution.Y - 200)); // Gun Megazine UI
            AddGameObjectUI(megazineUI);

            pathTester = new PathTester(ResourcesManager.FindLevel("Level1-1")); // Pathfinding Tester
            levelManager = new LevelManager(tiledMapRenderer, pathTester); // Level Manager 
            Player.Position = CurrentLevel.SpawnPosition;

            spawner.Start(); // Start the spawner immediately

            // Transition Effect initialization
            transitionEffect = new TransitionEffect(ResourcesManager.FindTexture("Transition_Texture"));
            transitionEffect.Initialize();
        }

        public static void Update(GameTime gameTime)
        {
            InputManager.Update(gameTime); // Update the input manager
            tiledMapRenderer.Update(gameTime); // Tiled Map Update

            // If transition is ongoing, dont update these things
            if (transitionEffect.Finish)
            {
                NavigationAgent.GlobalUpdate();
                SteeringAgent.GlobalUpdate();
                CollisionManager.Update(gameTime); // Collision Update
                entityManager.Update(gameTime); // Entity Manager Update
                spawner.Update(gameTime);
            }

            // Camera Update
            MainCamera.Update(GraphicsDevice.Viewport);
            MainCamera.Follow(Player); // Always follow the player

            // Update debug status
            if (InputManager.GetKey(Keys.F12)) Debug = !Debug;
            if (InputManager.GetKey(Keys.R)) levelManager.ChangeLevel(); // Test change level

            // Test Enemy
            if (InputManager.GetKey(Keys.F))
            {
                var pos = CurrentLevel.TileGraph.RandomPosition;
                spawner.Spawn(new Bat(pos), pos);
            }
            if (InputManager.GetKey(Keys.G))
            {
                var pos = CurrentLevel.TileGraph.RandomPosition;
                spawner.Spawn(new Shadow(pos), pos);
            }
            if (InputManager.GetKey(Keys.H))
            {
                var pos = CurrentLevel.TileGraph.RandomPosition;
                spawner.Spawn(new SuicideShadow(pos), pos);
            }
            if (InputManager.GetKey(Keys.J))
            {
                var pos = CurrentLevel.TileGraph.RandomPosition;
                spawner.Spawn(new Summoner(pos), pos);
            }

            if (InputManager.GetKey(Keys.Q))
            {
                transitionEffect.Reset();
                transitionEffect.Start();
            }

            if (Debug)
                pathTester.Update(gameTime);

            fpsCounter.Update(gameTime);

            // Execute transition logics
            if(transitionDuration <= 0f && levelUpdated)
            {
                transitionEffect.Start();
                levelUpdated = false;
                transitionDuration = TRANSITION_DURATION;
            }
            else if(transitionDuration > 0f && levelUpdated)
            {
                transitionDuration -= Time.DeltaTime;
            }
            transitionEffect.Update(gameTime);
        }

        // Test Level variables
        private static int floor = 1; // Record how many floor player has cleared
        static bool levelUpdated = false;
        const float TRANSITION_DURATION = 0.5f;
        static float transitionDuration = TRANSITION_DURATION;
        /// <summary>
        /// Switch to a new level when player cleated a level
        /// </summary>
        public static void UpdateLevel()
        {
            levelManager.ChangeLevel(0);
            transitionEffect.Reset();
            spawner.Start();
            Player.Position = CurrentLevel.SpawnPosition;
            levelUpdated = true;
            floor++;
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

            fpsCounter.Draw(spriteBatch, ResourcesManager.FindSpriteFont("DebugFont"), new Vector2(150f, 5f), Color.Red);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("DebugFont"), "Player HP: " + Player.HP.ToString("N0"), new Vector2(5f, 5f), Color.Red);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("DebugFont"), "Enemy Count: " + entityManager.EnemyCount, new Vector2(5f, 20f), Color.Red);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("DebugFont"), "Floor: " + floor, new Vector2(5f, 40f), Color.Red);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("DebugFont"), "Is Level Finish: " + IsLevelFinish, new Vector2(5f, 60f), Color.Red);
            transitionEffect.Draw(spriteBatch, gameTime);
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
