﻿using BulletManiac.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Managers
{
    /// <summary>
    /// Managing the life cycle of game objects througout the game
    /// </summary>
    public class EntitiesManager
    {
        /// <summary>
        /// Current running game object entities
        /// </summary>
        private readonly List<GameObject> runningObjects = new List<GameObject>();
        /// <summary>
        /// Game objects waiting to be add into the current list
        /// </summary>
        private readonly List<GameObject> objectQueue = new List<GameObject>();

        private bool updating;

        public GameObject Find(string name)
        {
            return runningObjects.Find(x => x.Name.Equals(name));
        }

        public GameObject Find(GameObject gameObject)
        {
            return runningObjects.Find(x => x.Equals(gameObject));
        }

        public void Add(GameObject gameObject)
        {
            // Do not add game objec when updating the list
            if (updating)
            {
                objectQueue.Add(gameObject);
            }
            else
            {
                runningObjects.Add(gameObject);
            }
        }

        public void Initialize()
        {
            foreach (var gameObj in runningObjects)
            {
                gameObj.Initialize();
            }
        }

        public void Update(GameTime gameTime)
        {
            updating = true;
            foreach (var gameObj in runningObjects)
            {
                gameObj.Update(gameTime);
            }
            updating = false;

            foreach (var gameObject in objectQueue)
            {
                runningObjects.Add(gameObject);
            }
            objectQueue.Clear();

            var destroyObjects = runningObjects.Where(x => x.IsDestroyed).ToList();

            foreach (var gameObj in destroyObjects)
            {
                runningObjects.Remove(gameObj);
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var gameObj in runningObjects)
            {
                gameObj.Draw(spriteBatch, gameTime);
            }
        }
    }
}
