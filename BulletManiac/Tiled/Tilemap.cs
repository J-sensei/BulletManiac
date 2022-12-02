using BulletManiac.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletManiac.Tiled
{
    public class Tilemap
    {
        private Vector2 offset;

        private readonly TilesetRender tilesetRender;
        private readonly Layer[] layers;

        public Tilemap(Tileset tileset, TilemapData tilemapData)
        {
            tileset = GameManager.Resources.FindTileset(tilemapData.TileSources[0].Source);

            // Initialize tileset render and layers
            tilesetRender = new TilesetRender(tilemapData);
            layers = new Layer[tilemapData.Layers.Length];
            for (int i = 0; i < tilemapData.Layers.Length; i++)
            {
                layers[i] = new Layer(tilesetRender, tilemapData.Layers[i]);
            }
        }

        public void Update(GameTime gameTime)
        {
            var x = InputManager.Direction;
            x.Normalize();
            offset += InputManager.Direction * 25f * GameManager.DeltaTime; // Test
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Render all layers
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].Draw(spriteBatch, offset);

            }
        }
    }

    /// <summary>
    /// Tilesets to render for a tilemap
    /// </summary>
    public class TilesetRender
    {
        private TilemapData tilemapData;
        private Tileset[] tilesets;
        private int[] firstGrids;

        public Point TileSize
        {
            get
            {
                return tilesets[0].TileSize;
            }
        }

        public TilesetRender(TilemapData TilemapData)
        {
            tilemapData = TilemapData;
            tilesets = new Tileset[tilemapData.TileSources.Length];
            firstGrids = new int[tilemapData.TileSources.Length];
            for(int i = 0; i < tilemapData.TileSources.Length; i++)
            {
                tilesets[i] = GameManager.Resources.FindTileset(tilemapData.TileSources[i].Source); // Get correct tileset
                firstGrids[i] = tilemapData.TileSources[i].FirstGrid;
            }
        }

        /// <summary>
        /// Get the correct texture from tilesets available
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Texture2D GetTexture(int id)
        {
            Texture2D result = null;

            for(int i = tilemapData.TileSources.Length - 1; i >=0; i--)
            {
                int sourceId = (id + 1) - firstGrids[i];
                if(sourceId > 0)
                {
                    result = tilesets[i].Get(sourceId);
                    break;
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Layer of a tile map
    /// </summary>
    public class Layer
    {
        private int id;
        private string name;
        private int[,] data; // Map data, ids to render tile
        private Tile[,] tiles; // Tiles to render
        private TilesetRender tilesetRender;
        private Point tileSize;

        public Layer(TilesetRender tilesetRender, TilemapLayer layer)
        {
            id = layer.ID;
            name = layer.Name;
            this.tilesetRender = tilesetRender;
            tileSize = tilesetRender.TileSize;

            // Convert data string to 2d int
            string map = layer.Data;
            data = new int[layer.Width, layer.Height];

            string[] rowStrings = map.Split('\n');

            int x = 0;
            int y = 0;
            foreach (string str in rowStrings)
            {
                if (String.IsNullOrEmpty(str)) continue;
                string newStr = string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries)); // Remove white spaces
                string[] parts = newStr.Split(','); // Split comma
                foreach (string part in parts)
                {
                    if (!String.IsNullOrEmpty(part))
                    {
                        data[x, y] = Convert.ToInt32(part);
                        x++;
                    }
                }
                x = 0;
                y++;

                if (y >= layer.Height) break;
            }

            tiles = new Tile[data.GetLength(0), data.GetLength(1)]; // Initialize tiles size
            for (int row = 0; row < tiles.GetLength(1); row++)
            {
                for (int col = 0; col < tiles.GetLength(0); col++)
                {
                    // Calculate tile texture and origin positions
                    tiles[col, row] = new Tile(tilesetRender.GetTexture(data[col, row]), MapToScreen(col, row));
                }
            }
        }

        /// <summary>
        /// Calculate tiles position
        /// </summary>
        /// <param name="mapX"></param>
        /// <param name="mapY"></param>
        /// <returns></returns>
        Vector2 MapToScreen(int mapX, int mapY)
        {
            float x = (mapX * tileSize.X);
            float y = (mapY * tileSize.Y);

            return new Vector2(x, y);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                for (int x = 0; x < tiles.GetLength(0); x++)
                {
                    // Render tiles with offset
                    tiles[x, y].Draw(spriteBatch, offset);
                }
            }
        }
    }
}
