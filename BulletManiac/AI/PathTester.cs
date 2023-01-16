using BulletManiac.Managers;
using BulletManiac.Tiled;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Tiled.AI
{
    /// <summary>
    /// Path tester use to test the pathfinding algorithm from player position to the user cursor position
    /// </summary>
    public class PathTester
    {
        private TileGraph tileGraph;
        private TiledMap map;

        private readonly Texture2D pathTexture;

        private Point srcPosition;
        private Point destPosition;

        private LinkedList<Tile> path;

        public PathTester(TileGraph tileGraph, TiledMap map)
        {
            this.tileGraph = tileGraph;
            this.map = map;
            srcPosition = Point.Zero;
            destPosition = Point.Zero;
            path = new LinkedList<Tile>();

            pathTexture = ResourcesManager.FindTexture("Debug_Path");
        }

        public PathTester(Level gameLevel) : this(gameLevel.TileGraph, gameLevel.Map)
        {

        }

        public void ChangeLevel(Level gameLevel)
        {
            tileGraph = gameLevel.TileGraph;
            map = gameLevel.Map;
            srcPosition = Point.Zero;
            destPosition = Point.Zero;
        }

        private Stopwatch stopwatch = Stopwatch.StartNew();
        public void Update(GameTime gameTime)
        {
            // Calculate path
            if (InputManager.MouseLeftClick)
            {
                path.Clear(); // Clear previous path data
                Tile srcTile = Tile.ToTile(GameManager.Player.Position, map.TileWidth, map.TileHeight);
                srcPosition.X = srcTile.Col;
                srcPosition.Y = srcTile.Row;

                Tile destTile = Tile.ToTile(Camera.ScreenToWorld(InputManager.MousePosition), map.TileWidth, map.TileHeight);
                destPosition.X = destTile.Col;
                destPosition.Y = destTile.Row;

                try
                {
                    // Calculate srcTile and destTile
                    Tile src = Tile.ToTile(GameManager.Player.Position, map.TileWidth, map.TileHeight);
                    Tile dest = Tile.ToTile(Camera.ScreenToWorld(InputManager.MousePosition), map.TileWidth, map.TileHeight);

                    stopwatch.Reset(); // Debug calculate the time to run pathfinding algorithms
                    // Calculate path
                    if (GameManager.CurrentPathfindingAlgorithm == PathfindingAlgorithm.Dijkstra)
                    {
                        stopwatch.Start();
                        path = Dijkstra.Compute(tileGraph, src, dest);
                        stopwatch.Stop();
                        GameManager.Log("Path Tester", "Time Ran the Dijkstra Algorithm: " + stopwatch.ElapsedMilliseconds + "ms.");
                    }
                    else if(GameManager.CurrentPathfindingAlgorithm == PathfindingAlgorithm.AStar)
                    {
                        stopwatch.Start();
                        path = AStar.Compute(tileGraph, src, dest, AStar.Euclidean);
                        stopwatch.Stop();
                        GameManager.Log("Path Tester", "Time Ran the A* Algorithm: " + stopwatch.ElapsedMilliseconds + "ms.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("[Path Tester] Failed to perform pathfinding {0}", e.Message);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 position = Vector2.Zero;
            Rectangle rect;
            rect.Width = map.TileWidth;
            rect.Height = map.TileHeight;

            for (LinkedListNode<Tile> cur = path.First; cur != null; cur = cur.Next)
            {
                int index = 4; // Refers to the centre tile from path texture (no direction)
                if (cur.Next != null)
                {
                    LinkedListNode<Tile> next = cur.Next;

                    int dCol = next.Value.Col - cur.Value.Col;
                    int dRow = next.Value.Row - cur.Value.Row;

                    index = dCol + 1 + 3 * (dRow + 1);
                }

                position.X = cur.Value.Col * map.TileWidth;
                position.Y = cur.Value.Row * map.TileHeight;

                rect.X = index * rect.Width;
                rect.Y = 0;

                spriteBatch.Draw(pathTexture, position, rect, Color.White);
            }
        }
    }
}
