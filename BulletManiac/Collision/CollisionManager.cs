﻿using BulletManiac.Entity;
using BulletManiac.Managers;
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
                    if(GameManager.Debug)
                        Console.WriteLine($"{collidables[i].GameObject.Name} is DESTROYED, skipping.");
                    continue;
                }

                for (int j = i + 1; j < collidables.Count; j++)
                {
                    var go2 = collidables[j];

                    // Don't check for collision between GameObjects with same tag
                    if (go1.Tag == go2.Tag) continue;

                    if (IsCollided_AABB(go1, go2))
                    {
                        if (GameManager.Debug)
                            Console.WriteLine(go1.GameObject.Name + " and " + go2.GameObject.Name + " COLLIDED");

                        //if(go1.GameObject.CollisionAction != null)
                        //    go1.GameObject.CollisionAction.Invoke(go2.GameObject);
                        //if (go2.GameObject.CollisionAction != null)
                        //    go2.GameObject.CollisionAction.Invoke(go1.GameObject);

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
                    if (GameManager.Debug)
                        Console.WriteLine($"Deleting {collidables[i].GameObject.Name} from collision manager.");
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
    }
}
