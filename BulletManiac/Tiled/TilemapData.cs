namespace BulletManiac.Tiled
{
    /// <summary>
    /// Use to load custom tile map data file from the content
    /// </summary>
    public class TilemapData
    {
        public TilesetSource[] TileSources { get; set; }
        public TilemapLayer[] Layers { get; set; }
    }

    public class TilesetSource
    {
        /// <summary>
        /// First Grid start ID for the tileset
        /// </summary>
        public int FirstGrid { get ; set; }
        /// <summary>
        /// Which tileset to use
        /// </summary>
        public string Source { get; set; }
    }

    public class TilemapLayer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Data { get; set; }
    }
}
