using BulletManiac.Collision;
using BulletManiac.Entity;
using BulletManiac.Managers;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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
        private Tile srcTile; // User current standing tile
        private Tile destTile; // Tile that user want to go
        private LinkedList<Tile> path = null; // Calculated path that help to navigate the user to move

        private int tileWidth;
        private int tileHeight;

        private const float DEFAULT_COOLDOWN = 1f;
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

        public void Update(GameTime gameTime, GameObject target)
        {
            if(currentState == NavigationState.STOP)
            {
                currentCD -= GameManager.DeltaTime;
                if(currentCD <= 0)
                {
                    CalculatePath(target.Position);
                    currentCD = DEFAULT_COOLDOWN;
                }
            }
            else if(currentState == NavigationState.MOVING)
            {
                // If the user reached the destination
                Vector2 diff2 = Tile.ToPosition(destTile, tileWidth, tileHeight) - user.Position;
                float distance2 = diff2.Length();
                //if (user.Position.Equals(Tile.ToPosition(destTile, tileWidth, tileHeight)))
                if(distance2 < 1f)
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
                        Vector2 diff = headPosition - user.Position;
                        float distance = diff.Length();
                        //Console.WriteLine(distance + " " + Extensions.Approximately(distance, 0.0f));
                        //if (user.Position.Equals(headPosition))
                        if (distance <= 1f)
                        {
                            path.RemoveFirst();
                            // Get the next destination position
                            headTile = path.First.Value; // throw exception if path is empty
                        }

                        // Move
                        //if(OnMove != null)
                        //    OnMove.Invoke(headPosition);
                        //user.Position = Move(user.Position, headPosition, GameManager.DeltaTime, 100.0);
                        user.Position += Seek(headPosition, 50f) * GameManager.DeltaTime;
                        //MoveAvoid(user, headPosition);
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

        private void MoveAvoid(GameObject source, Vector2 target)
        {
            float pullDistance = Vector2.Distance(target, source.Position);

            if (pullDistance > 1)
            {
                Vector2 pull = (target - source.Position) * (1 / pullDistance); //the target tries to 'pull us in'
                Vector2 totalPush = Vector2.Zero;

                int contenders = 0;
                List <Tile> obstacles = CollisionManager.TileBounds;
                for (int i = 0; i < CollisionManager.TileBounds.Count; ++i)
                {

                    //draw a vector from the obstacle to the ship, that 'pushes the ship away'
                    Vector2 push = source.Position - obstacles[i].Position;

                    //calculate how much we are pushed away from this obstacle, the closer, the more push
                    float distance = (Vector2.Distance(source.Position, obstacles[i].Position) - 2f) - 5f;
                    //only use push force if this object is close enough such that an effect is needed
                    if (distance < 2f * 3)
                    {
                        ++contenders; //note that this object is actively pushing

                        if (distance < 0.0001f) //prevent division by zero errors and extreme pushes
                        {
                            distance = 0.0001f;
                        }
                        float weight = 1 / distance;

                        totalPush += push * weight;
                    }
                }

                pull *= Math.Max(1, 4 * contenders); //4 * contenders gives the pull enough force to pull stuff trough (tweak this setting for your game!)
                pull += totalPush;

                //Normalize the vector so that we get a vector that points in a certain direction, which we van multiply by our desired speed
                pull.Normalize();
                //Set the ships new position;
                source.Position += (pull * 50f) * GameManager.DeltaTime;
            }
        }
        private Vector2 Seek(Vector2 target, float currentSpeed)
        {
            return Vector2.Normalize(target - user.Position) * currentSpeed;
        }
    }
}
