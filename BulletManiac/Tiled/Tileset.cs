using BulletManiac.Managers;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
        private Texture2D[] tiles;
        /// <summary>
        /// Bounding rectangle of each individual tile
        /// </summary>
        private readonly Rectangle[] bounds;

        public Tileset(TilesetData tilesetData)
        {
            this.tilesetData = tilesetData;
            spriteSheet = GameManager.Resources.LoadTextureRaw(tilesetData.ImagePath);

            // Calculate the bounds of each image
            bounds = new Rectangle[tilesetData.TileCount];
            tiles = new Texture2D[tilesetData.TileCount];

            int row = 1; // start as first row
            for(int i = 0; i < bounds.Length; i++)
            {
                Rectangle bound = new Rectangle((i % tilesetData.Columns) * tilesetData.TileWidth, (row - 1) * tilesetData.TileHeight, tilesetData.TileWidth, tilesetData.TileHeight);
                bounds[i] = bound;
                // Each the end of the column, increment the row to next row
                if(i % tilesetData.Columns == 0 && i != 0)
                {
                    row++;
                }

                // crop the tiles
                tiles[i] = Extensions.CropTexture2D(spriteSheet, bounds[i]);
            }

            Array.Clear(bounds, 0, bounds.Length); // Clear the bound array as its no use anymore
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
            return tiles[id - 1];
        }
    }
}
