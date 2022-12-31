using BulletManiac.Collision;
using BulletManiac.Tiled;
using BulletManiac.Tiled.AI;
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
            //resources.LoadLevel("Level1", new Level(GameManager.Resources.FindTiledMap("Level1"), 9, 8));
            //resources.LoadLevel("Level2", new Level(GameManager.Resources.FindTiledMap("Level2"), 9, 8));
            //resources.LoadLevel("Level3", new Level(GameManager.Resources.FindTiledMap("Level3"), 9, 8));

            resources.LoadTiledMap("DefaultMap", "Level/Level1");
            resources.LoadTiledMap("DefaultMap2", "Level/Level2");
            resources.LoadLevel("Level1", new Level(GameManager.Resources.FindTiledMap("DefaultMap"), 9, 8, new Color(7, 24, 33)));
            resources.LoadLevel("Level2", new Level(GameManager.Resources.FindTiledMap("DefaultMap2"), 9, 8, new Color(7, 24, 33)));
        }

        /// <summary>
        /// Initialize level manager with a inital level
        /// </summary>
        /// <param name="level"></param>
        public LevelManager(Level level, TiledMapRenderer tiledMapRenderer, PathTester pathTester)
        {
            levels = new List<Level>();
            currentLevelIndex = 0;
            levels.Add(level);
            levels.Add(GameManager.Resources.FindLevel("Level2"));
            this.tiledMapRenderer = tiledMapRenderer;
            this.pathTester = pathTester;

            tiledMapRenderer.LoadMap(CurrentLevel.Map);
            CollisionManager.ChangeTileCollision(CurrentLevel.Obstacles);
            pathTester.ChangeLevel(CurrentLevel);
        }

        public void Add(Level level)
        {
            levels.Add(level);
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
