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
        /// <summary>
        /// The difficulty value
        /// </summary>
        public float Difficulty;
        /// <summary>
        /// Map of the level
        /// </summary>
        public TiledMap Map;
        /// <summary>
        /// Initial player spawn position
        /// </summary>
        public Vector2 SpawnPosition;
    }
}
