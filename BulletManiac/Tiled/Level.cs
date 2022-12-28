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
        /// <summary>
        /// The difficulty value
        /// </summary>
        public float Difficulty;
        /// <summary>
        /// Map of the level
        /// </summary>
        public TiledMap Map;
        public TileGraph TileGraph;
        /// <summary>
        /// Initial player spawn position
        /// </summary>
        public Vector2 SpawnPosition;
        public Rectangle Bound;

        public Level(TiledMap map, int colStart, int rowStart)
        {
            Map = map;
            TiledMapTileLayer wallLayer = Map.GetLayer<TiledMapTileLayer>(WALL_LAYER_NAME);
            // Construct the TileGraph
            TileGraph = new TileGraph();
            TileGraph.CreatePathsFromMap(wallLayer, colStart, rowStart);
            Bound = new Rectangle(wallLayer.TileWidth + 10, wallLayer.TileHeight + 10,
                                    (wallLayer.Width * wallLayer.TileWidth) - (wallLayer.TileWidth + 10),
                                    (wallLayer.Height * wallLayer.TileHeight) - (wallLayer.TileHeight + 10));
        }
    }
}
