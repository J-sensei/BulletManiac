using BulletManiac.Collision;
using BulletManiac.Tiled;
using BulletManiac.Tiled.AI;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Managers
{
    /// <summary>
    /// Manage, load, and switch levels
    /// </summary>
    public class LevelManager
    {
        public const string INITIAL_LEVEL = "Level1";
        private static List<Level> levels;
        public Level CurrentLevel
        {
            
            get { return levels[currentLevelIndex]; }
        }
        private int currentLevelIndex;

        TiledMapRenderer tiledMapRenderer;
        PathTester pathTester;

        /// <summary>
        /// Load levels
        /// </summary>
        /// <param name="ResourcesManager"></param>
        public static void LoadContent()
        {
            // Map 
            ResourcesManager.LoadTiledMap("Map1", "Level/Map1");
            ResourcesManager.LoadTiledMap("Map2", "Level/Map2");
            ResourcesManager.LoadTiledMap("Map3", "Level/Map3");
            ResourcesManager.LoadTiledMap("Map4", "Level/Map4");
            ResourcesManager.LoadTiledMap("Map5", "Level/Map5");
            ResourcesManager.LoadTiledMap("Map6", "Level/Map6");
            ResourcesManager.LoadTiledMap("Map7", "Level/Map7");
            ResourcesManager.LoadTiledMap("Map8", "Level/Map8");
            ResourcesManager.LoadTiledMap("Map9", "Level/Map9");
            ResourcesManager.LoadTiledMap("Map10", "Level/Map10");

            // Level
            ResourcesManager.LoadLevel("Level1", new Level(ResourcesManager.FindTiledMap("Map1"), 8, 9, new Color(7, 24, 33),
                                                        Tile.ToPosition(new Tile(8, 14), 16, 16)));
            ResourcesManager.LoadLevel("Level2", new Level(ResourcesManager.FindTiledMap("Map1"), 8, 14, new Color(7, 24, 33),
                                            Tile.ToPosition(new Tile(8, 14), 16, 16)));
            ResourcesManager.LoadLevel("Level3", new Level(ResourcesManager.FindTiledMap("Map3"), 8, 14, new Color(7, 24, 33),
                                Tile.ToPosition(new Tile(8, 14), 16, 16)));
            ResourcesManager.LoadLevel("Level4", new Level(ResourcesManager.FindTiledMap("Map4"), 8, 14, new Color(7, 24, 33),
                                Tile.ToPosition(new Tile(8, 14), 16, 16)));
            ResourcesManager.LoadLevel("Level5", new Level(ResourcesManager.FindTiledMap("Map5"), 8, 15, new Color(7, 24, 33),
                                Tile.ToPosition(new Tile(8, 15), 16, 16)));
            ResourcesManager.LoadLevel("Level6", new Level(ResourcesManager.FindTiledMap("Map6"), 8, 15, new Color(7, 24, 33),
                                Tile.ToPosition(new Tile(8, 15), 16, 16)));
            ResourcesManager.LoadLevel("Level7", new Level(ResourcesManager.FindTiledMap("Map7"), 8, 15, new Color(7, 24, 33),
                    Tile.ToPosition(new Tile(8, 15), 16, 16)));
            ResourcesManager.LoadLevel("Level8", new Level(ResourcesManager.FindTiledMap("Map8"), 8, 15, new Color(7, 24, 33),
                    Tile.ToPosition(new Tile(8, 15), 16, 16)));
            ResourcesManager.LoadLevel("Level9", new Level(ResourcesManager.FindTiledMap("Map9"), 8, 9, new Color(7, 24, 33),
                    Tile.ToPosition(new Tile(8, 14), 16, 16)));
            ResourcesManager.LoadLevel("Level10", new Level(ResourcesManager.FindTiledMap("Map10"), 8, 15, new Color(7, 24, 33),
                    Tile.ToPosition(new Tile(8, 15), 16, 16)));

            // Load all the levels
            levels = new List<Level>();
            for(int i = 0; i < 10; i++)
            {
                levels.Add(ResourcesManager.FindLevel("Level" + (i + 1).ToString()));
            }
        }

        /// <summary>
        /// Initialize level manager with a inital level
        /// </summary>
        /// <param name="level"></param>
        public LevelManager(TiledMapRenderer tiledMapRenderer, PathTester pathTester)
        {
            currentLevelIndex = 0;

            this.tiledMapRenderer = tiledMapRenderer;
            this.pathTester = pathTester;

            CurrentLevel.DoorClose(); // Make sure the first level door is close 
            tiledMapRenderer.LoadMap(CurrentLevel.Map); // Load tile map into the tile renderer
            CollisionManager.ChangeTileCollision(CurrentLevel.Obstacles); // Load the obstacles
            pathTester.ChangeLevel(CurrentLevel); // Change the debug path tester
        }

        public void Add(Level level)
        {
            levels.Add(level);
        }

        /// <summary>
        /// Change level based on the current difficulty
        /// </summary>
        /// <param name="difficulty"></param>
        public void ChangeLevel(int difficulty)
        {
            CurrentLevel.DoorClose(); // Close the door

            // Level Difficulty = current difficulty || previous difficulty (e.g. diffculty 5 will be randomly 5 or 4)
            //List<int> targetIndexes = new();
            //for(int i = 0; i < levels.Count; i++)
            //{
            //    if (levels[i].Difficulty == difficulty || levels[i].Difficulty == difficulty - 1)
            //    {
            //        targetIndexes.Add(i);
            //    }
            //}
            //currentLevelIndex = targetIndexes.Shuffle(Extensions.Random).Take(1).FirstOrDefault();
            currentLevelIndex = Extensions.Random.Next(levels.Count); // Testing
            tiledMapRenderer.LoadMap(CurrentLevel.Map);
            CollisionManager.ChangeTileCollision(CurrentLevel.Obstacles);
            pathTester.ChangeLevel(CurrentLevel);
        }

        public void ChangeLevel()
        {
            currentLevelIndex = (currentLevelIndex + 1) % levels.Count;
            tiledMapRenderer.LoadMap(CurrentLevel.Map);
            CollisionManager.ChangeTileCollision(CurrentLevel.Obstacles);
            pathTester.ChangeLevel(CurrentLevel);
        }
    }
}
