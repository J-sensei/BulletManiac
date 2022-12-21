using BulletManiac.Managers;
using BulletManiac.Tiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Utilities
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
            this.srcPosition = Point.Zero;
            this.destPosition = Point.Zero;
            this.path = new LinkedList<Tile>();

            pathTexture = GameManager.Resources.FindTexture("Debug_Path");
        }

        public PathTester(Level gameLevel) : this(gameLevel.TileGraph, gameLevel.Map)
        {
            
        }

        public void ChangeLevel(Level gameLevel)
        {
            this.tileGraph = gameLevel.TileGraph;
            this.map = gameLevel.Map;
            this.srcPosition = Point.Zero;
            this.destPosition = Point.Zero;
        }

        public void Update(GameTime gameTime)
        {
            Tile srcTile = Tile.ToTile(GameManager.Player.Position, map.TileWidth, map.TileHeight);
            Point srcPoint = new Point(srcTile.Col, srcTile.Row);


                srcPosition = srcPoint;
            //Console.WriteLine("SRC: " + srcPosition + " DEST: " + destPoint);

            // Calculate path
            if (InputManager.MouseLeftClick)
            {
                path.Clear();
                Tile destTile = Tile.ToTile(Camera.ScreenToWorld(InputManager.MousePosition), map.TileWidth, map.TileHeight);
                Point destPoint = new Point(destTile.Col, destTile.Row);
                    destPosition = destPoint;

                try
                {
                    // Calculate srcTile and destTile
                    Tile src = Tile.ToTile(GameManager.Player.Position, map.TileWidth, map.TileHeight);
                    Tile dest = Tile.ToTile(Camera.ScreenToWorld(InputManager.MousePosition), map.TileWidth, map.TileHeight);
                    // Calculate path
                    path = Dijkstra.Compute(tileGraph, src, dest);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to perform pathfinding {0}", e.Message);
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

                    index = (dCol + 1) + 3 * (dRow + 1);
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
