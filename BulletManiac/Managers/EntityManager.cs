using BulletManiac.Entity;
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
    public class EntityManager
    {
        /// <summary>
        /// Current running game object entities
        /// </summary>
        private readonly List<GameObject> gameObjects = new List<GameObject>();
        /// <summary>
        /// Game objects waiting to be add into the current list
        /// </summary>
        private readonly List<GameObject> gameObjectQueue = new List<GameObject>();

        /// <summary>
        /// Current running UI related game object
        /// </summary>
        private readonly List<GameObject> gameObjectsUI = new();
        /// <summary>
        /// UI Game objects waiting to be add into the list 
        /// </summary>
        private readonly List<GameObject> gameObjectUIQueue = new();

        /// <summary>
        /// Determine if the game object list is updating
        /// </summary>
        private bool updating;

        /// <summary>
        /// Find the game object by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject Find(string name)
        {
            return gameObjects.Find(x => x.Name.Equals(name));
        }

        /// <summary>
        /// Find the game object by game object reference
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public GameObject Find(GameObject gameObject)
        {
            return gameObjects.Find(x => x.Equals(gameObject));
        }

        /// <summary>
        /// Add new game object into the entity manager
        /// </summary>
        /// <param name="gameObject"></param>
        public void Add(GameObject gameObject)
        {
            // Do not add game objec when updating the list
            if (updating)
            {
                gameObjectQueue.Add(gameObject);
            }
            else
            {
                gameObjects.Add(gameObject);
            }
        }

        /// <summary>
        /// Add new UI game object into the entity manager
        /// </summary>
        /// <param name="gameObject"></param>
        public void AddUIObject(GameObject gameObject)
        {
            if (updating)
            {
                gameObjectUIQueue.Add(gameObject);
            }
            else
            {
                gameObjectsUI.Add(gameObject);
            }
        }

        /// <summary>
        /// Initialize the default game object when the game start
        /// </summary>
        public void Initialize()
        {
            // Game objects
            foreach (var gameObj in gameObjects)
            {
                gameObj.Initialize();
            }

            // UI Game objects
            foreach (var gameObj in gameObjectsUI)
            {
                gameObj.Initialize();
            }
        }

        public void Update(GameTime gameTime)
        {
            updating = true; // Game objects is start to update
            // Game objects
            foreach (var gameObj in gameObjects)
            {
                gameObj.Update(gameTime);
            }
            // UI Game objects
            foreach(var gameObj in gameObjectsUI)
            {
                gameObj.Update(gameTime);
            }
            updating = false; // Game objects finish updated

            // Queue to be add into the list
            foreach (var gameObject in gameObjectQueue)
            {
                gameObjects.Add(gameObject);
                gameObject.Initialize(); // Called the initalize after new game object is added into the list
            }
            foreach (var gameObject in gameObjectUIQueue)
            {
                gameObjectsUI.Add(gameObject);
                gameObject.Initialize();
            }

            // Clear the queue
            gameObjectQueue.Clear();
            gameObjectUIQueue.Clear();

            // Remove the destroyed game object
            var destroyObjects = gameObjects.Where(x => x.IsDestroyed).ToList();
            destroyObjects.AddRange(gameObjectsUI.Where(x => x.IsDestroyed).ToList());

            foreach (var gameObj in destroyObjects)
            {
                gameObjects.Remove(gameObj);
            }
        }

        /// <summary>
        /// Draw the game entities
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var gameObj in gameObjects)
            {
                gameObj.Draw(spriteBatch, gameTime);
            }
        }

        /// <summary>
        /// Draw the game UI
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public void DrawUI(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach(var gameObj in gameObjectsUI)
            {
                gameObj.Draw(spriteBatch, gameTime);
            }
        }
    }
}
