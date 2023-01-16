using BulletManiac.Managers;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Tiled
{
    /*
        Level difficulty will be rated from 1 - 10
        Every 2 floor is cleared will increase 1 difficulty rated until 10
        Spawn Number = Difficulty * Based Spawn Number
        
    */
    /// <summary>
    /// What a level will contain
    /// </summary>
    public class Level
    {
        const string WALL_LAYER_NAME = "Wall";
        const string OBSTACLE_LAYER_NAME = "Obstacle";

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
        public List<Rectangle> DoorBound { get; private set; } = new();

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

            TiledMapTileLayer obstacleLayer = Map.GetLayer<TiledMapTileLayer>(OBSTACLE_LAYER_NAME);
            Obstacles = Tile.CalculateTileCollision(obstacleLayer, Map.Width, Map.Height, Map.TileWidth, Map.TileHeight);
            TiledMapTileLayer door = Map.GetLayer<TiledMapTileLayer>("Door Open");
            for (int i = 0; i < Map.Width; i++)
            {
                for (int j = 0; j < Map.Height; j++)
                {
                    bool hasTile = door.TryGetTile((ushort)i, (ushort)j, out TiledMapTile? tile);
                    if (hasTile && tile.Value.GlobalIdentifier != 0)
                    {
                        DoorBound.Add(new Rectangle(i * Map.TileWidth, j * Map.TileHeight, Map.TileWidth, Map.TileHeight / 2));
                        break;
                    }
                }
            }
            Map.GetLayer<TiledMapTileLayer>("Door Open").Opacity = 0;
        }

        public bool TouchingDoor(Rectangle rect)
        {
            if (!GameManager.IsLevelFinish) return false; // Make sure the level is finished in order to access the door
            for(int i = 0; i < DoorBound.Count; i++)
            {
                if (rect.Intersects(DoorBound[i]))
                {
                    return true;
                }
            }
            return false;
        }

        //bool playDoorSound = true;
        public void DoorOpen()
        {
            Map.GetLayer<TiledMapTileLayer>("Door Open").Opacity = 1;
            //ResourcesManager.FindSoundEffect("Door_Open").Play();
            AudioManager.Play("Door_Open");
        }

        public void DoorClose()
        {
            Map.GetLayer<TiledMapTileLayer>("Door Open").Opacity = 0;
        }

        public Level(TiledMap map, int colStart, int rowStart, Color backgroundColor, Vector2 spawnPosition) : this(map, colStart, rowStart)
        {
            BackgroundColor = backgroundColor;
            SpawnPosition = spawnPosition;
        }
    }
}
