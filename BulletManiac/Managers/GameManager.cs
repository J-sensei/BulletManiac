using BulletManiac.AI;
using BulletManiac.Collision;
using BulletManiac.Entity;
using BulletManiac.Entity.Bullets;
using BulletManiac.Entity.Enemies;
using BulletManiac.Entity.Players;
using BulletManiac.Entity.PowerUps;
using BulletManiac.Entity.UI;
using BulletManiac.Particle;
using BulletManiac.Scenes;
using BulletManiac.SpriteAnimation;
using BulletManiac.Tiled;
using BulletManiac.Tiled.AI;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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
        const int FINAL_FLOOR = 10;
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
        /// Global accessible player (Singleton)
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
            1.375f, 2.75f, 5.5f, 11f
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
        private static EntityManager entityManager;
        private static LevelManager levelManager;

        /// <summary>
        /// Spawner, responsible to handle the spawn
        /// </summary>
        private static Spawner spawner;

        /// <summary>
        /// Is player eliminated all the enemies
        /// </summary>
        public static bool IsLevelFinish { get { return spawner.IsFinish && entityManager.EnemyCount <= 0; } }
        public static bool spawnPowerUp = false;

        /// <summary>
        /// Play the transition effect of the game
        /// </summary>
        private static TransitionEffect transitionEffect;
        /// <summary>
        /// Frame and Update counter for the game
        /// </summary>
        private static FrameCounter fpsCounter = new();

        public static float TimePass { get; private set; }
        public static string TimePassString
        {
            get
            {
                var ts = TimeSpan.FromSeconds(TimePass);
                return string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
            }
        }

        /// <summary>
        /// Change resolution of the game
        /// </summary>
        /// <param name="graphics"></param>
        public static void UpdateScreenSize(GraphicsDeviceManager graphics)
        {
            graphics.PreferredBackBufferWidth = CurrentResolution.X;
            graphics.PreferredBackBufferHeight = CurrentResolution.Y;
            graphics.ApplyChanges();

            Camera.Main.AdjustZoom(CurrentCameraZoom); // Adjust the Camera zoom level after the resolution changed
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

        private const string GAMEOVER_STRING = "YOU DIED";
        private static Vector2 gameOverStringOrigin;
        private static float gameOverColorTransparency = 0f;
        public static void Initialize()
        {
            entityManager = new();

            if(tiledMapRenderer == null)
                tiledMapRenderer = new TiledMapRenderer(GraphicsDevice); // Initialize Tiled
            entityManager.Clear();
            // Add cursor and player
            Player = new Player(new Vector2(50f)); // Create Player in the game
            AddGameObject(Player); // Add player
            MagazineUI megazineUI = new MagazineUI(Player.Gun, new Vector2(CurrentResolution.X - 80, CurrentResolution.Y - 50)); // Gun Megazine UI
            AddGameObjectUI(megazineUI);
            //AddGameObjectUI(new Button(new Vector2(500f), "TEST"));

            pathTester = new PathTester(ResourcesManager.FindLevel(LevelManager.INITIAL_LEVEL)); // Pathfinding Tester
            levelManager = new LevelManager(tiledMapRenderer, pathTester); // Level Manager 
            Player.Position = CurrentLevel.SpawnPosition;

            // Transition Effect initialization
            transitionEffect = new TransitionEffect(ResourcesManager.FindTexture("Transition_Texture"));
            transitionEffect.Initialize();

            // Status reset
            Floor = 1; // Reset the floor count
            TimePass = 0f;
            Difficulty = 1;
            // Reset Modifier
            Bullet.SpeedModifier = 1.0f;
            Bullet.DamageMultiplier = 1.0f;
            //Spawning
            spawnPowerUp = false;
            doorOpened = false;
            // YOU DIED effect for game over
            gameOverStringOrigin = ResourcesManager.FindSpriteFont("Font_Title").MeasureString(GAMEOVER_STRING);
            gameOverColorTransparency = 0f;

            ApplyTransition(); // Run the transition when the game start

            GameResult.Reset();
            spawner = new();
            spawner.Start(); // Start the spawner immediately
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
            ResourcesManager.LoadTexture("Bullet_Destroy", "SpriteSheet/Effect/Bullet_Destroy");

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
            ResourcesManager.LoadTexture("Transition_Texture", "UI/Transition_Texture");
            ResourcesManager.LoadTexture("Bullet_Empty", "UI/Bullet/bullet_empty");
            ResourcesManager.LoadTexture("Bullet_Default", "UI/Bullet/bullet_default");
            ResourcesManager.LoadTexture("Bullet_Track", "UI/Bullet/bullet_track");
            ResourcesManager.LoadTexture("Bullet_Shotgun", "UI/Bullet/bullet_shotgun");
            ResourcesManager.LoadTexture("Bullet_Explosion", "UI/Bullet/bullet_explosion");
            ResourcesManager.LoadSpriteFonts("DebugFont", "UI/Font/DebugFont");
            ResourcesManager.LoadSpriteFonts("Font_Player", "UI/Font/Font_Player");
            ResourcesManager.LoadTexture("HP_Background", "UI/ProgressBar/HP_Background");
            ResourcesManager.LoadTexture("HP_Foreground", "UI/ProgressBar/HP_Foreground");
            ResourcesManager.LoadTexture("health_bar", "UI/ProgressBar/health_bar");
            ResourcesManager.LoadTexture("health_bar_decoration", "UI/ProgressBar/health_bar_decoration");

            // Load Debug UI Sprites
            ResourcesManager.LoadTexture("Debug_Direction", "SpriteSheet/DebugUI/direction_16x16");
            ResourcesManager.LoadTexture("Debug_Path", "SpriteSheet/DebugUI/path_16x16");
            ResourcesManager.LoadTexture("Skull_Icon", "UI/Skull_Icon");

            // Load Sound Effect
            ResourcesManager.LoadSoundEffect("Footstep1", "Audio/Footstep/Footstep1");
            ResourcesManager.LoadSoundEffect("Footstep2", "Audio/Footstep/Footstep2");
            ResourcesManager.LoadSoundEffect("Footstep3", "Audio/Footstep/Footstep3");
            ResourcesManager.LoadSoundEffect("Player_Hurt", "Audio/Player/Player_Hurt");
            ResourcesManager.LoadSoundEffect("Mag_In", "Audio/Gun/Mag_In");
            ResourcesManager.LoadSoundEffect("Pistol_Cock", "Audio/Gun/Pistol_Cock");
            ResourcesManager.LoadSoundEffect("Bullet_Hit", "Audio/Gun/Bullet_Hit");
            ResourcesManager.LoadSoundEffect("Bat_Death", "Audio/Enemy/Bat Death");
            ResourcesManager.LoadSoundEffect("Shadow_Death", "Audio/Enemy/Shadow_Death");
            ResourcesManager.LoadSoundEffect("SuicideShadow_Explosion", "Audio/Enemy/SuicideShadow_Explosion");
            ResourcesManager.LoadSoundEffect("SuicideShadow_Attacking", "Audio/Enemy/SuicideShadow_Attacking");
            ResourcesManager.LoadSoundEffect("SuicideShadow_AttackStart", "Audio/Enemy/SuicideShadow_AttackStart");
            ResourcesManager.LoadSoundEffect("Enemy_Spawn", "Audio/Enemy/Enemy_Spawn");
            ResourcesManager.LoadSoundEffect("Door_Open", "Audio/Level/Door_Open");
            ResourcesManager.LoadSoundEffect("Pause", "Audio/UI/Pause");
            ResourcesManager.LoadSoundEffect("Dash", "Audio/Player/Dash");

            // Bullet shoot sound effects
            ResourcesManager.LoadSoundEffect("Default_Shoot", "Audio/Gun/Default_Shoot");
            ResourcesManager.LoadSoundEffect("Track_Shoot", "Audio/Gun/Track_Shoot");
            ResourcesManager.LoadSoundEffect("Shotgun_Shoot", "Audio/Gun/Shotgun_Shoot");
            ResourcesManager.LoadSoundEffect("Explosion_Shoot", "Audio/Gun/Explosion_Shoot");

            // Main Menu
            ResourcesManager.LoadSpriteFonts("Font_Normal", "UI/Font/Font_Normal");
            ResourcesManager.LoadSpriteFonts("Font_Title", "UI/Font/Font_Title");
            ResourcesManager.LoadSoundEffect("Button_Hover", "Audio/UI/Button_Hover");
            ResourcesManager.LoadSoundEffect("Button_Click", "Audio/UI/Button_Click");
            ResourcesManager.LoadSpriteFonts("Font_Small", "UI/Font/Font_Small");

            // Power Up
            ResourcesManager.LoadTexture("PowerUp_Heart", "UI/PowerUp/Heart");
            ResourcesManager.LoadTexture("PowerUp_Heart_Animated", "UI/PowerUp/HeartAnimated");
            ResourcesManager.LoadTexture("Bullet_Capacity", "UI/PowerUp/BulletCapacity");
            ResourcesManager.LoadTexture("Bullet_Speed", "UI/PowerUp/BulletSpeed");
            ResourcesManager.LoadTexture("Bullet_Damage", "UI/PowerUp/BulletDamage");

            Animation.LoadAnimations();
            Bat.LoadContent();
            LevelManager.LoadContent();
            Button.LoadContent();

            // Shader
            ResourcesManager.LoadEffect("Color_Overlay", "Shader/ColorOverlay");
        }
        
        public static void LoadContent()
        {
            LoadDefaultResources(); // Load default resources needed for the game to start
        }
        
        public static void Update(GameTime gameTime)
        {
            tiledMapRenderer.Update(gameTime); // Tiled Map Update

            // If transition is ongoing, dont update these things
            if (transitionEffect.Finish)
            {
                Cursor.Instance.ChangeMode(CursorMode.Crosshair);
                NavigationAgent.GlobalUpdate();
                SteeringAgent.GlobalUpdate();
                CollisionManager.Update(gameTime); // Collision Update
                entityManager.Update(gameTime); // Entity Manager Update
                spawner.Update(gameTime);

                if(!IsLevelFinish && !Player.IsGameOver)
                    TimePass += Time.DeltaTime;
            }
            else
            {
                Cursor.Instance.ChangeMode(CursorMode.Loading);
            }

            // Camera Update
            Camera.Main.Update(GraphicsDevice.Viewport);
            Camera.Main.Follow(Player); // Always follow the player

            // Debug
            if (InputManager.GetKey(Keys.F12)) Debug = !Debug;
            //if (InputManager.GetKey(Keys.R)) levelManager.ChangeLevel(); // Test change level

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

            if (InputManager.GetKey(Keys.Escape) && transitionEffect.Finish) // prevent player open pause menu when transition is playing
            {
                // Pause the game
                //ResourcesManager.FindSoundEffect("Pause").Play();
                AudioManager.Play("Pause");
                SceneManager.OpenScene(2);
                SceneManager.GetScene(1).StopUpdate();
            }

            // Spawn Power Up
            if (!spawnPowerUp && IsLevelFinish)
            {
                spawnPowerUp = true;
                AddGameObject(GetPowerUp());
            }
            
            // Open the door is level is finish
            if (IsLevelFinish && !doorOpened)
            {
                CurrentLevel.DoorOpen();
                doorOpened = true;
            }

            if (Player.IsGameOver)
            {
                // Transition effect of YOU DIED
                gameOverColorTransparency += 0.5f * Time.DeltaTime; 
                if(gameOverColorTransparency > 1.5f)
                {
                    SceneManager.LoadScene(4); // Go to game over scene
                }
            }

            if (Debug)
                pathTester.Update(gameTime); // Debug path tester

            fpsCounter.Update(gameTime); // Debug FPS counter

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

        /// <summary>
        /// Record how many floor player has cleared
        /// </summary>
        public static int Floor { get; private set; } = 1;
        static bool levelUpdated = false;
        const float TRANSITION_DURATION = 0.25f; // How long delay the transition start (For the camera move happen and teleportation happen without player seeing it)
        static float transitionDuration = TRANSITION_DURATION;
        private static bool doorOpened = false;
        public static int Difficulty { get; private set; } = 1;
        /// <summary>
        /// Switch to a new level when player cleated a level
        /// </summary>
        public static void UpdateLevel()
        {
            levelManager.ChangeLevel(Difficulty); // Change the level of the current difficulty
            entityManager.ClearBullets(); // Clear bullets from the previous level
            transitionEffect.Reset(); // Reset the transition effect
            spawner.Start(); // Start the spawning
            Player.Position = CurrentLevel.SpawnPosition; // Update player position to the inital position of the level
            levelUpdated = true; // Start the transition to hide updating when level the changing
            doorOpened = false; // Door the close now
            Floor++; // Update floor record
            spawnPowerUp = false;

            if (Floor % 2 == 0)
            {
                Difficulty++;
                Difficulty = Math.Clamp(Difficulty, 1, 10); // Clamp the difficulty
            }

            // Every 5 floor change bgm
            if(Floor % 5 == 0)
            {
                char x = AudioManager.CurrentSong[AudioManager.CurrentSong.Length - 1];
                int n = int.Parse(x.ToString());
                n = (n + 1) % 4;
                if (n == 0) n = 4;
                string newTrack = "BGM" + n;
                AudioManager.PlayMusic(newTrack);
            }
        }

        static GameObject GetPowerUp()
        {
            Vector2 position = CurrentLevel.DoorBound[0].Center.ToVector2();
            position.Y += 20f;
            int r = Extensions.Random.Next(0, 4);
            switch (r)
            {
                case 0:
                    return new Heart(position);
                case 1:
                    return new BulletCapacity(position);
                case 2:
                    return new BulletSpeed(position);
                case 3:
                    return new BulletDamage(position);
            }
            return null;
        }

        public static void ApplyTransition()
        {
            transitionEffect.Reset();
            levelUpdated = true;
        }

        /// <summary>
        /// Draw game objects on the game world layer
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            tiledMapRenderer.Draw(viewMatrix: Camera.Main.Transform); // Render the Tiled

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

            // UI
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), "HP: " + Player.HP.ToString("N0") + "/" + Player.MaxHP.ToString("N0"), new Vector2(100f, 85f), Color.Gray);
            spriteBatch.Draw(ResourcesManager.FindTexture("Skull_Icon"), new Vector2(46f, 138f),  null, Color.White, 0f, new Vector2(8f), new Vector2(2f), SpriteEffects.None, 0f);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), " x" + entityManager.EnemyCount, new Vector2(53f, 130f), Color.Gray);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), "FLOOR: " + Floor, new Vector2(35f, 180f), Color.Gray);
            spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), "TIME: " + TimePassString, new Vector2(35f, 230f), Color.Gray);
            if (Debug)
            {
                fpsCounter.Draw(spriteBatch, ResourcesManager.FindSpriteFont("DebugFont"), new Vector2(150f, 5f), Color.Red);
                spriteBatch.DrawString(ResourcesManager.FindSpriteFont("DebugFont"), "Player HP: " + Player.HP.ToString("N0"), new Vector2(5f, 5f), Color.Red);
                spriteBatch.DrawString(ResourcesManager.FindSpriteFont("DebugFont"), "Enemy Count: " + entityManager.EnemyCount, new Vector2(5f, 20f), Color.Red);
                spriteBatch.DrawString(ResourcesManager.FindSpriteFont("DebugFont"), "Floor: " + Floor, new Vector2(5f, 40f), Color.Red);
                spriteBatch.DrawString(ResourcesManager.FindSpriteFont("DebugFont"), "Is Level Finish: " + IsLevelFinish, new Vector2(5f, 60f), Color.Red);
            }

            if (Player.IsGameOver)
            {
                spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Title"), "YOU DIED", new Vector2(CurrentResolution.X / 2, CurrentResolution.Y / 2), Color.Red * gameOverColorTransparency, 0, (gameOverStringOrigin / 2), 1f, SpriteEffects.None, 0f);
            }

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
