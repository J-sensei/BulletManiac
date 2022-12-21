using BulletManiac.Entity;
using BulletManiac.Managers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Tiled.Pathfinding
{
    /// <summary>
    /// Navigation State to determine certain action for the agent (Finite State Machine)
    /// </summary>
    public enum NavigationState 
    { 
        /// <summary>
        /// Agent is STOP
        /// </summary>
        STOP, 
        /// <summary>
        /// Agent is MOVING
        /// </summary>
        MOVING 
    }

    /// <summary>
    /// Help to execute pathfinding algorithm and apply move to the game object
    /// </summary>
    public class NavigationAgent
    {
        /// <summary>
        /// Game Object that is using this agent
        /// </summary>
        private GameObject user;
        /// <summary>
        /// Current Navigation State of the agent
        /// </summary>
        private NavigationState currentState = NavigationState.STOP; // The agent should stop by default
        private Tile srcTile; // User current standing tile
        private Tile destTile; // Tile that user want to go
        private LinkedList<Tile> path = null; // Calculated path that help to navigate the user to move

        private int tileWidth;
        private int tileHeight;

        private const float DEFAULT_COOLDOWN = 4f;
        private float currentCD = DEFAULT_COOLDOWN;

        public NavigationAgent(GameObject user)
        {
            this.user = user;
            tileWidth = GameManager.CurrentLevel.Map.TileWidth;
            tileHeight = GameManager.CurrentLevel.Map.TileHeight;
            srcTile = Tile.ToTile(user.Position, tileWidth, tileHeight);
        }

        public void Initialize()
        {

        }

        public void Update(GameTime gameTime, Vector2 target)
        {
            if(currentState == NavigationState.STOP)
            {
                currentCD -= GameManager.DeltaTime;
                if(currentCD <= 0)
                {
                    CalculatePath(target);
                    currentCD = DEFAULT_COOLDOWN;
                }
            }
            else if(currentState == NavigationState.MOVING)
            {
                // If the user reached the destination
                if (user.Position.Equals(Tile.ToPosition(destTile, tileWidth, tileHeight)))
                {
                    // Update source tile to destination tile
                    srcTile = destTile;
                    destTile = null;

                    // Change to STOP state
                    currentState = NavigationState.STOP;
                }
                else
                {
                    try
                    {
                        Tile headTile = path.First.Value; // throw exception if path is empty
                        Vector2 headPosition = Tile.ToPosition(headTile, tileWidth, tileHeight);

                        // If current position reached the head position
                        if (user.Position.Equals(headPosition))
                        {
                            //Console.WriteLine("Reach head tile. Removing head tile. Get next node from path.");
                            path.RemoveFirst();

                            // Get the next destination position
                            headTile = path.First.Value; // throw exception if path is empty
                        }

                        // Move
                        //if(OnMove != null)
                        //    OnMove.Invoke(headPosition);
                        user.Position = Move(user.Position, headPosition, GameManager.DeltaTime, 100.0);
                    }
                    catch(Exception ex)
                    {
                        if (ex is NullReferenceException || ex is InvalidOperationException)
                        {
                            GameManager.Log("Navigation Agent", "Path empty or Head tile unreachable.");

                            // For safety, clear the path in case the exception was not caused by empty path.
                            path.Clear();

                            // Update source tile to destination tile
                            srcTile = Tile.ToTile(user.Position, tileWidth, tileHeight);
                            destTile = null;

                            // Change to STOP state.
                            currentState = NavigationState.STOP;
                        }
                        else throw;
                    }
                }
            }
        }

        private void CalculatePath(Vector2 target)
        {
            destTile = Tile.ToTile(target, tileWidth, tileHeight); // Get the destination tile
            // Make sure the destination is contains inside the tile graph AND not equal to the current standing tile
            if (GameManager.CurrentLevel.TileGraph.Nodes.Contains(destTile) && !destTile.Equals(srcTile))
            {
                if (GameManager.CurrentPathfindingAlgorithm == PathfindingAlgorithm.Dijkstra)
                {
                    // 1. Compute an Dijkstra path
                    path = Dijkstra.Compute(GameManager.CurrentLevel.TileGraph, srcTile, destTile);
                    // 2. Remove source tile from path
                    path.RemoveFirst();
                }
                else if (GameManager.CurrentPathfindingAlgorithm == PathfindingAlgorithm.AStar)
                {
                    // 1. Compute an A* path
                    path = AStar.Compute(GameManager.CurrentLevel.TileGraph, srcTile, destTile, AStar.Euclidean);
                    // 2. Remove source tile from path
                    path.RemoveFirst();
                }
                else
                {
                    GameManager.Log("Navigation Agent", "No pathfinding algorithm is selected, hence no path is calculated.");
                }
                // Change to MOVING state
                currentState = NavigationState.MOVING;
            }
            else
            {
                GameManager.Log("Navigation Agent", "Destination Tile is not in the Tile Graph or Destination Tile is same with Source Tile.");
            }
        }

        public delegate void MoveAction(Vector2 destination);
        public MoveAction OnMove;

        private Vector2 Move(Vector2 src, Vector2 dest, double elapsedSeconds, double speed)
        {
            Vector2 dP = dest - src;
            float distance = dP.Length();
            float step = (float)(speed * elapsedSeconds);

            if (step < distance)
            {
                dP.Normalize();
                return src + (dP * step);
            }
            else
                return dest;
        }
    }
}
