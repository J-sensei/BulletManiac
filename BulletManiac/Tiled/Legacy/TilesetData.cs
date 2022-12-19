namespace BulletManiac.Tiled.Legacy
{
    /// <summary>
    /// Tileset Data use to configure the tileset correctly
    /// </summary>
    public class TilesetData
    {
        /// <summary>
        /// Version (Generated from Tiled)
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Tiled version (Generated from Tiled)
        /// </summary>
        public string TiledVersion { get; set; }
        /// <summary>
        /// Image path to load from the content
        /// </summary>
        public string ImagePath { get; set; }
        /// <summary>
        /// Name given to the resources manager
        /// </summary>
        public string ResourcesName { get; set; }
        /// <summary>
        /// Width of each individual tile
        /// </summary>
        public int TileWidth { get; set; }
        /// <summary>
        /// Height of each individual tile
        /// </summary>
        public int TileHeight { get; set; }
        /// <summary>
        /// Total tile after cropped from sprite sheet
        /// </summary>
        public int TileCount { get; set; }
        /// <summary>
        /// columns for a row
        /// </summary>
        public int Columns { get; set; }
        /// <summary>
        /// Width of the sprite sheet
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Height of the sprite sheet
        /// </summary>
        public int Height { get; set; }
    }
}
