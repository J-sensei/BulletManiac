using BulletManiac.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Collision
{
    /// <summary>
    /// Game Object that needs collision will inherit this interface
    /// </summary>
    public interface ICollidable : IDisposable
    {
        /// <summary>
        /// Collidable Game Object
        /// </summary>
        public GameObject GameObject { get; }
        /// <summary>
        /// Tag to verify the collidable
        /// </summary>
        public string Tag { get; }
    }

    public class Collidable : ICollidable
    {
        public Collidable(GameObject gameObject, string tag = "")
        {
            GameObject = gameObject;
            Tag = tag;
        }

        

        public GameObject GameObject { get; private set; }
        public string Tag { get; private set; }

        public void Dispose()
        {
            GameObject.Dispose();
            GameObject = null;
            Tag = null;
        }
    }
}
