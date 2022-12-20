using BulletManiac.Entity;
using BulletManiac.Managers;
using BulletManiac.Tiled;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Collision
{
    public static class CollisionManager
    {
        private static readonly List<ICollidable> collidables = new List<ICollidable>();

        /// <summary>
        /// List of tile bound to check the collision with tile
        /// </summary>
        public static readonly List<Tile> TileBounds = new();

        /// <summary>
        /// Add the tile into the collision count
        /// </summary>
        /// <param name="tileGameObject"></param>
        /// <exception cref="NullReferenceException"></exception>
        public static void AddTileBound(Tile tileGameObject)
        {
            if (tileGameObject == null)
            {
                throw new NullReferenceException("[Collision Manager] Tile Game Object is null, not able to add into the collision");
            }
            TileBounds.Add(tileGameObject);
        }

        /// <summary>
        /// Use to check the collision of game object with tiles, offset shift the current bound of the game object
        /// </summary>
        /// <param name="target"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static bool CheckTileCollision(GameObject target, Vector2 offset)
        {
            Rectangle b = target.Bound;
            Rectangle bound = new Rectangle((int)(b.X + offset.X), (int)(b.Y + offset.Y), b.Width, b.Height);
            for(int i = 0; i < TileBounds.Count; i++)
            {
                if (IsCollided_AABB(bound, TileBounds[i].Bound))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Clear the Tile Bounds game objects
        /// </summary>
        public static void ClearTileCollision()
        {
            for (int i = 0; i < TileBounds.Count; i++)
            {
                TileBounds[i].Dispose();
                TileBounds[i] = null;
            }
            TileBounds.Clear();
        }

        public static ICollidable Add(GameObject gameObject, string tag = "")
        {
            if(gameObject == null)
            {
                throw new NullReferenceException("[Collision Manager] Game Object is null, not able to add into the collision");
            }

            ICollidable collidable = new Collidable(gameObject, tag);
            collidables.Add(collidable);
            return collidable;
        }

        /// <summary>
        /// Update the collision calculation
        /// </summary>
        /// <param name="gameTime"></param>
        public static void Update(GameTime gameTime)
        {
            for (int i = 0; i < collidables.Count - 1; i++)
            {
                var go1 = collidables[i];

                // GameObject may be flagged for deletion due to previous GameObject
                // calling OnCollision
                if (go1.GameObject.IsDestroyed)
                {
                    GameManager.Log("Collision Manager", $"{ collidables[i].GameObject.Name} is DESTROYED, skipping.");
                    continue;
                }

                for (int j = i + 1; j < collidables.Count; j++)
                {
                    var go2 = collidables[j];

                    // Don't check for collision between GameObjects with same tag
                    if (go1.Tag == go2.Tag) continue;

                    if (IsCollided_AABB(go1, go2))
                    {
                        GameManager.Log("Collision Manager", go1.GameObject.Name + " and " + go2.GameObject.Name + " is COLLIDING");

                        go1.GameObject.CollisionEvent(go2.GameObject);
                        go2.GameObject.CollisionEvent(go1.GameObject);
                    }
                }
            }

            Destroy();
        }

        /// <summary>
        /// Destroy not active game object in the list
        /// </summary>
        /// <returns></returns>
        private static void Destroy()
        {
            for (int i = collidables.Count - 1; i >= 0; i--)
            {
                if (collidables[i].GameObject.IsDestroyed)
                {
                    GameManager.Log("Collision Manager", $"Deleting {collidables[i].GameObject.Name} from collision manager.");

                    collidables[i].Dispose();
                    collidables.RemoveAt(i);
                }
            }
        }

        private static bool IsCollided_AABB(ICollidable obj1, ICollidable obj2)
        {
            GameObject gameObject1 = obj1.GameObject;
            GameObject gameObject2 = obj2.GameObject;

            Rectangle obj1Bound = gameObject1.Bound;
            Rectangle obj2Bound = gameObject2.Bound;

            Vector2 obj1_TL = new(obj1Bound.X, obj1Bound.Y);
            Vector2 obj2_TL = new(obj2Bound.X, obj2Bound.Y);
            Vector2 obj1_BR = new Vector2(obj1Bound.X, obj1Bound.Y) + new Vector2(obj1Bound.Width, obj1Bound.Height);
            Vector2 obj2_BR = new Vector2(obj2Bound.X, obj2Bound.Y) + new Vector2(obj2Bound.Width, obj2Bound.Height);

            if (obj1_BR.X < obj2_TL.X || obj2_BR.X < obj1_TL.X ||
                obj1_BR.Y < obj2_TL.Y || obj2_BR.Y < obj1_TL.Y)
                return false;
            else
                return true;
        }

        public static bool IsCollided_AABB(Rectangle rectX, Rectangle rectY)
        {
            if (rectX == Rectangle.Empty || rectY == Rectangle.Empty) return false;

            Vector2 obj1_TL = new(rectX.X, rectX.Y);
            Vector2 obj2_TL = new(rectY.X, rectY.Y);
            Vector2 obj1_BR = new Vector2(rectX.X, rectX.Y) + new Vector2(rectX.Width, rectX.Height);
            Vector2 obj2_BR = new Vector2(rectY.X, rectY.Y) + new Vector2(rectY.Width, rectY.Height);

            if (obj1_BR.X < obj2_TL.X || obj2_BR.X < obj1_TL.X ||
                obj1_BR.Y < obj2_TL.Y || obj2_BR.Y < obj1_TL.Y)
                return false;
            else
                return true;
        }
    }
}
