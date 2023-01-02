using BulletManiac.Collision;
using BulletManiac.Tiled;
using BulletManiac.Tiled.AI;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
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
        private List<Level> levels;
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
        /// <param name="resources"></param>
        public static void LoadContent(ResourcesManager resources)
        {
            // Map 
            resources.LoadTiledMap("Level1-1", "Level/Level1-1");
            resources.LoadTiledMap("Level1-2", "Level/Level1-2");
            resources.LoadTiledMap("Level1-3", "Level/Level1-3");
            resources.LoadTiledMap("Level2-1", "Level/Level2-1");
            resources.LoadTiledMap("Level2-2", "Level/Level2-2");

            // Level
            resources.LoadLevel("Level1-1", new Level(GameManager.Resources.FindTiledMap("Level1-1"), 8, 9, new Color(7, 24, 33), 
                                                        Tile.ToPosition(new Tile(8 ,15), 16, 16), 1));
            resources.LoadLevel("Level1-2", new Level(GameManager.Resources.FindTiledMap("Level1-2"), 3, 9, new Color(7, 24, 33),
                                                        Tile.ToPosition(new Tile(8, 15), 16, 16), 1));
            resources.LoadLevel("Level1-3", new Level(GameManager.Resources.FindTiledMap("Level1-3"), 8, 9, new Color(7, 24, 33),
                                                        Tile.ToPosition(new Tile(8, 15), 16, 16), 2));
            resources.LoadLevel("Level2-1", new Level(GameManager.Resources.FindTiledMap("Level2-1"), 10, 4, new Color(7, 24, 33),
                                                        Tile.ToPosition(new Tile(10, 20), 16, 16), 3));
            resources.LoadLevel("Level2-2", new Level(GameManager.Resources.FindTiledMap("Level2-2"), 10, 12, new Color(7, 24, 33),
                                                        Tile.ToPosition(new Tile(10, 20), 16, 16), 4));
        }

        /// <summary>
        /// Initialize level manager with a inital level
        /// </summary>
        /// <param name="level"></param>
        public LevelManager(TiledMapRenderer tiledMapRenderer, PathTester pathTester)
        {
            levels = new List<Level>();
            currentLevelIndex = 0;
            
            // Load all the levels
            levels.Add(GameManager.Resources.FindLevel("Level1-1"));
            levels.Add(GameManager.Resources.FindLevel("Level1-2"));
            levels.Add(GameManager.Resources.FindLevel("Level1-3"));
            levels.Add(GameManager.Resources.FindLevel("Level2-1"));
            levels.Add(GameManager.Resources.FindLevel("Level2-2"));

            this.tiledMapRenderer = tiledMapRenderer;
            this.pathTester = pathTester;

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
