using BulletManiac.Managers;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletManiac.Tiled
{
    /// <summary>
    /// Tileset contains each tile position from tile sprite sheet
    /// </summary>
    public class Tileset
    {
        /// <summary>
        /// Configuration of the tile
        /// </summary>
        private TilesetData tilesetData;
        /// <summary>
        /// Sprite Sheet of the tileset
        /// </summary>
        private Texture2D spriteSheet;
        /// <summary>
        /// Each tile sprite cropped from the sprite sheet
        /// </summary>
        private Rectangle[] bounds;

        public Point TileSize
        {
            get
            {
                return new Point(tilesetData.TileWidth, tilesetData.TileHeight);
            }
        }

        public Tileset(TilesetData tilesetData)
        {
            this.tilesetData = tilesetData;
            spriteSheet = GameManager.Resources.LoadTextureRaw(tilesetData.ImagePath);

            // Calculate the bounds of each image
            bounds = new Rectangle[tilesetData.TileCount];
            int row = 1; // start as first row
            for(int i = 0; i < bounds.Length; i++)
            {
                // Each the end of the column, increment the row to next row
                if (i % tilesetData.Columns == 0 && i != 0)
                {
                    row++;
                }
                // Add bound area to each tile
                bounds[i] = new Rectangle((i % tilesetData.Columns) * tilesetData.TileWidth, 
                                        (row - 1) * tilesetData.TileHeight, 
                                        tilesetData.TileWidth, tilesetData.TileHeight);
            }
        }

        /// <summary>
        /// Get the tile texture based on the id given (start from 1 followed by Tiled standard)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Texture2D Get(int id)
        {
            // Prevent accessing array out of bound
            if (id <= 0 || id > tilesetData.TileCount) return null;
            //return tiles[id - 1];
            return Extensions.CropTexture2D(spriteSheet, bounds[id - 1]);
        }
    }
}
