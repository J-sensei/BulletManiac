using BulletManiac.Managers;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Tiled
{
    /// <summary>
    /// Search and store the walkable paths of a map / level
    /// </summary>
    public class TileGraph
    {
        /// <summary>
        /// 8 direction of moving row (y axis)
        /// </summary>
        public static readonly int[] MoveRow = { -1, -1, -1, 0, 0, 1, 1, 1 };
        /// <summary>
        /// 8 direction of moving column (x axis)
        /// </summary>
        public static readonly int[] MoveCol = { -1, 0, 1, -1, 1, -1, 0, 1 };
        /// <summary>
        /// Diagonal : 2, Non-Diagonal : 1
        /// </summary>
        private readonly ulong[] costs = { 2, 1, 2, 1, 1, 2, 1, 2 };
        /// <summary>
        /// // 4 directions for the search to walk (Up, Left, Right, Down)
        /// </summary>
        //private readonly int[] directions = { 1, 3, 4, 6 };
        private readonly int[] directions = { 0, 1, 2, 3, 4, 5, 6, 7 }; // Move diagnosal

        private HashSet<Tile> nodes; // Nodes represent a 
        private Dictionary<Tile, ulong[]> connections; // Connection of a tile to other tiles

        public HashSet<Tile> Nodes { get { return nodes; } }
        public Dictionary<Tile, ulong[]> Connections { get { return connections; } }

        public Vector2 RandomPosition
        {
            get
            {
                return Tile.ToPosition(nodes.ElementAt(Extensions.Random.Next(nodes.Count)),
                        GameManager.CurrentLevel.Map.TileWidth,
                        GameManager.CurrentLevel.Map.TileHeight);
            }
        }

        public Tile RandomNode
        {
            get { return nodes.ElementAt(Extensions.Random.Next(nodes.Count)); }
        }

        public Vector2 RandomPositionAwayFromDistance(float distance)
        {
            HashSet<Tile> targetNodes = nodes.Where(x => (GameManager.Player.Position - Tile.ToPosition(x, GameManager.CurrentLevel.Map.TileWidth,
                                        GameManager.CurrentLevel.Map.TileHeight)).Length() > distance).ToHashSet();
            if (targetNodes.Count() > 0)
            {
                Vector2 pos = Tile.ToPosition(targetNodes.ElementAt(Extensions.Random.Next(targetNodes.Count)),
                    GameManager.CurrentLevel.Map.TileWidth,
                    GameManager.CurrentLevel.Map.TileHeight);
                return pos;
            }
            else
            {
                GameManager.Log("Tile Graph", "RandomPositionAwayFromDistance - Distance is too large, returning Zero.");
                return Vector2.Zero;
            }
        }

        public TileGraph()
        {
            nodes = new HashSet<Tile>();
            connections = new Dictionary<Tile, ulong[]>();
        }

        /// <summary>
        /// Create walkable path information from the map layer
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="colStart"></param>
        /// <param name="rowStart"></param>
        public void CreatePathsFromMap(TiledMapTileLayer layer, int colStart, int rowStart)
        {
            bool hasTile = layer.TryGetTile((ushort)colStart, (ushort)rowStart, out TiledMapTile? tile); // Try to get the start tile

            if (hasTile)
            {
                BFSConstructGraph(layer, colStart, rowStart);
            }
            else
            {
                GameManager.Log("Tile Graph", "ColStart or RowStart is Invalid.");
            }
        }

        /// <summary>
        /// Construct path information using breath first search
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="colStart"></param>
        /// <param name="rowStart"></param>
        private void BFSConstructGraph(TiledMapTileLayer layer, int colStart, int rowStart)
        {
            Tile startTile = new Tile(colStart, rowStart);
            nodes.Add(startTile); // Add the initial tile into the nodes first

            Queue<Tile> q = new Queue<Tile>(); // Create a queue to contain Tile object for BFS traversal
            q.Enqueue(startTile);

            while(q.Count > 0)
            {
                Tile currentTile = q.Dequeue();
                
                // Loop each direction possible
                foreach(int direction in directions)
                {
                    // Get neigbour tile
                    int neighbourRow = currentTile.Row + MoveRow[direction];
                    int neighbourCol = currentTile.Col + MoveCol[direction];

                    // A valid neighbour satisfies the following criteria:
                    // (1) Row and column is within the number of rows and columns respectively
                    // (2) A tile exists in map layer at location (column, row)
                    // (3) Global Identifier is 0 (i.e. navigable area, walkable tile)
                    if((neighbourRow >= 0 && neighbourRow < layer.Height) && (neighbourCol >= 0 && neighbourCol < layer.Width) && // Row and column is within the number of rows and columns respectively
                        layer.TryGetTile((ushort)neighbourCol, (ushort)neighbourRow, out TiledMapTile? neighbourTiledMapTile) && // A tile exists in map layer at location (column, row)
                        neighbourTiledMapTile.Value.GlobalIdentifier == 0) // Global Identifier is 0 (i.e. navigable area, walkable tile)
                    {
                        Tile neighbourTile = new Tile(neighbourCol, neighbourRow);

                        // Add neigbour tile into the node if not created yet and not in the nodes
                        if (!nodes.Contains(neighbourTile))
                        {
                            nodes.Add(neighbourTile);
                            q.Enqueue(neighbourTile);
                        }

                        // Create a new connection joining the current node with the neighbour node
                        if(!connections.TryGetValue(currentTile, out ulong[] weights))
                        {
                            weights = new ulong[costs.Length]; // 8 ulong
                            weights[direction] = costs[direction]; // Add the cost into the weights
                            connections.Add(currentTile, weights); // Create new connection with the weights just create
                        }
                        else
                        {
                            connections[currentTile][direction] = costs[direction]; // Modify the weights data
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draw the correct direction tile for each node, depending on its connections
        /// </summary>
        /// <param name="spriteBatch"></param>
        public static void DebugDrawGraph(SpriteBatch spriteBatch, TileGraph tileGraph)
        {
            Texture2D debugTexture = GameManager.Resources.FindTexture("Debug_Direction");
            int[] count = { 0, 1, 0, 8, 2, 0, 4, 0 }; // Direction texture crop position
            
            foreach (Tile tile in tileGraph.Nodes)
            {
                Vector2 position = new Vector2(tile.Col * 16, tile.Row * 16);
                Rectangle rect = new Rectangle();

                ulong[] weights = tileGraph.Connections[tile];
                int index = 0;

                for (int i = 0; i < weights.Length; ++i)
                    if (weights[i] != 0) // As long as the weight is not 0, it is a valid path
                        index += count[i];

                rect.X = index * debugTexture.Height;
                rect.Y = 0;
                rect.Width = rect.Height = debugTexture.Height;
                spriteBatch.Draw(debugTexture, position, rect, Color.White);
            }
        }
    }
}
