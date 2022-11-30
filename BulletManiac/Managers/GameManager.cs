using BulletManiac.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletManiac.Managers
{
    public static class GameManager
    {
        static EntitiesManager entitiesManager = new();
        public static ResourcesManager Resources = new();

        public static void AddGameObject(GameObject gameObject)
        {
            entitiesManager.Add(gameObject);
        }

        public static GameObject FindGameObject(string name)
        {
            return entitiesManager.Find(name);
        }

        public static GameObject FindGameObject(GameObject gameObject)
        {
            return entitiesManager.Find(gameObject);
        }

        public static void Initialize()
        {
            entitiesManager.Initialize();
        }

        public static void Update(GameTime gameTime)
        {
            InputManager.Update(gameTime);
            entitiesManager.Update(gameTime);
        }

        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            entitiesManager.Draw(spriteBatch, gameTime);
        }
    }
}
