using BulletManiac.AI;
using BulletManiac.Collision;
using BulletManiac.Entity;
using BulletManiac.Managers;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Tiled.AI
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
        public NavigationState CurrentState { get { return currentState; } }
        private Tile srcTile; // User current standing tile
        private Tile destTile; // Tile that user want to go
        private LinkedList<Tile> path = null; // Calculated path that help to navigate the user to move

        private int tileWidth;
        private int tileHeight;
        
        private float speed;
        public XDirection CurrentXDir { get; private set; }

        // Pathfinding Execution variables
        private static Queue<Vector2> targetPosQueue = new();
        private static Queue<NavigationAgent> pathfindQueue = new();
        private static Stopwatch stopwatch = new();
        private static int executeCount = 0;
        private const int MAX_EXECUTE_COUNT = 5; // Maximum pathfinding to run per frame

        public NavigationAgent(GameObject user, float speed = 50f)
        {
            this.user = user;
            this.speed = speed;
            tileWidth = GameManager.CurrentLevel.Map.TileWidth;
            tileHeight = GameManager.CurrentLevel.Map.TileHeight;
            srcTile = Tile.ToTile(user.Position, tileWidth, tileHeight);
        }

        public void Initialize()
        {

        }

        /// <summary>
        /// Execute pathfinding with the position provide
        /// </summary>
        /// <param name="position"></param>
        public void Pathfind(Vector2 position)
        {
            if (!pathfindQueue.Contains(this))
            {
                pathfindQueue.Enqueue(this);
                targetPosQueue.Enqueue(position);
            }
        }

        /// <summary>
        /// Update execution of the pathfinding queue
        /// </summary>
        public static void GlobalUpdate()
        {
            executeCount = 0;
            while(pathfindQueue.Count > 0 && executeCount <= MAX_EXECUTE_COUNT)
            {
                var agent = pathfindQueue.Dequeue();
                var targetPos = targetPosQueue.Dequeue();
                if (GameManager.FindGameObject(agent.user) == null) continue; // If user the destroyed, skip
                agent.CalculatePath(targetPos); // Will change navigation to moving
                executeCount++;
            }
        }

        public void Update(GameTime gameTime)
        {
            if(currentState == NavigationState.MOVING)
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

                        // If current position approximately reached the head position
                        if (user.Position.Equals(headPosition))
                        {
                            path.RemoveFirst();
                            // Get the next destination position
                            headTile = path.First.Value; // throw exception if path is empty
                            //Console.WriteLine(path.Count);
                        }

                        // Move
                        Vector2 movePos = Move(user.Position, headPosition, GameManager.DeltaTime, speed);
                        user.Position = movePos; // Move the user
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

        public void Reset()
        {
            destTile = null;
            currentState = NavigationState.STOP;
            if(path != null)
                path.Clear();
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
                    //Console.WriteLine("RUN A*");
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
                GameManager.Log("Navigation Agent", "Destination Tile is not in the Tile Graph or Destination Tile is same with Source Tile. (" + user.Name + ")");
            }
        }

        private Vector2 Move(Vector2 src, Vector2 dest, double elapsedSeconds, double speed)
        {
            Vector2 dP = dest - src;
            float distance = dP.Length();
            float step = (float)(speed * elapsedSeconds);

            if (step < distance)
            {
                dP.Normalize();
                Vector2 amount = dP * step;

                if (amount.X > 0f)
                    CurrentXDir = XDirection.Right;
                else if (amount.X < 0f)
                    CurrentXDir = XDirection.Left;

                return src + amount;
            }
            else
                return dest;
        }
    }
}
