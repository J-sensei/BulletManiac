using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Tiled
{
    /// <summary>
    /// What a level will contain
    /// </summary>
    public class Level
    {
        const string WALL_LAYER_NAME = "Wall";
        const string OBSTACLE_LAYER_NAME = "Obstacle";
        /// <summary>
        /// The difficulty value
        /// </summary>
        public int Difficulty { get; private set; }
        /// <summary>
        /// Map of the level
        /// </summary>
        public TiledMap Map { get; private set; }
        public TileGraph TileGraph { get; private set; }
        /// <summary>
        /// Initial player spawn position
        /// </summary>
        public Vector2 SpawnPosition { get; private set; }
        public Rectangle Bound { get; private set; }
        /// <summary>
        /// Obstacle tiles that cannot walk through
        /// </summary>
        public List<Tile> Obstacles { get; private set; }
        public Color BackgroundColor { get; private set; } = Color.CornflowerBlue;

        public Level(TiledMap map, int colStart, int rowStart, int difficulty = 0)
        {
            Map = map;
            Difficulty = difficulty;
            TiledMapTileLayer wallLayer = Map.GetLayer<TiledMapTileLayer>(WALL_LAYER_NAME);
            // Construct the TileGraph
            TileGraph = new TileGraph();
            TileGraph.CreatePathsFromMap(wallLayer, colStart, rowStart);
            Bound = new Rectangle(wallLayer.TileWidth + 10, wallLayer.TileHeight + 10,
                                    (wallLayer.Width * wallLayer.TileWidth) - (wallLayer.TileWidth + 10),
                                    (wallLayer.Height * wallLayer.TileHeight) - (wallLayer.TileHeight + 10));

            TiledMapTileLayer obstacleLayer = Map.GetLayer<TiledMapTileLayer>(OBSTACLE_LAYER_NAME);
            Obstacles = Tile.CalculateTileCollision(obstacleLayer, Map.Width, Map.Height, Map.TileWidth, Map.TileHeight);
        }

        public Level(TiledMap map, int colStart, int rowStart, Color backgroundColor, int difficulty = 0) : this(map, colStart, rowStart, difficulty)
        {
            BackgroundColor = backgroundColor;
        }
    }
}
