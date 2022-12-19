using BulletManiac.Collision;
using BulletManiac.Entity;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using System;

namespace BulletManiac.Tiled
{
    /// <summary>
    /// Basic tile data from the Tiled map 
    /// </summary>
    public class Tile : GameObject
    {
        private int row;
        private int col;

        public int Row { get { return row; } set { row = value; } }
        public int Col { get { return col; } set { col = value; } }
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }

        public Tile(int col, int row)
        {
            this.row = row;
            this.col = col;
        }

        public Tile(int col, int row, int width, int height)
        {
            this.row = row;
            this.col = col;
            TileWidth = width;
            TileHeight = height;
        }

        public override string ToString()
        {
            return String.Format("Tile ({0}, {1})", Col, Row);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Tile rhs = (Tile)obj;
                return (row == rhs.row) && (col == rhs.col);
            }
        }

        public override int GetHashCode()
        {
            return (Row << 2) ^ Col;
        }

        /// <summary>
        /// Convert the tile row and col to the world position
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="tileWidth"></param>
        /// <param name="tileHeight"></param>
        /// <returns></returns>
        public static Vector2 ToPosition(Tile tile, int tileWidth, int tileHeight)
        {
            return new Vector2(tile.col * tileWidth, tile.row * tileHeight);
        }

        /// <summary>
        /// Convert position to the tile
        /// </summary>
        /// <param name="position"></param>
        /// <param name="tileWidth"></param>
        /// <param name="tileHeight"></param>
        /// <returns></returns>
        public static Tile ToTile(Vector2 position, int tileWidth, int tileHeight)
        {
            return new Tile((int)(position.X / tileWidth), (int)(position.Y / tileHeight), tileWidth, tileHeight);
        }

        /// <summary>
        /// Add collision block tiles into the game
        /// </summary>
        /// <param name="mapLayer"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="tileWidth"></param>
        /// <param name="tileHeight"></param>
        public static void AddTileCollision(TiledMapTileLayer mapLayer, int width, int height, int tileWidth, int tileHeight)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    bool hasTile = mapLayer.TryGetTile((ushort)i, (ushort)j, out TiledMapTile? tile);
                    if (hasTile && tile.Value.GlobalIdentifier != 0)
                    {
                        // Add collision to the collision manager
                        Tile t = new Tile(i, j, tileWidth, tileHeight);
                        CollisionManager.AddTileBound(t);
                    }
                }
            }
        }

        /// <summary>
        /// Bound of a tile
        /// </summary>
        /// <returns></returns>
        protected override Rectangle CalculateBound()
        {
            return new Rectangle((col * TileWidth), (row * TileHeight), TileWidth, TileHeight);
        }
    }
}
