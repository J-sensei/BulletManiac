using BulletManiac.Managers;
using System;
using System.Collections.Generic;

namespace BulletManiac.AI
{
    /// <summary>
    /// Manage different flock list using dictionary
    /// </summary>
    public static class FlockManager
    {
        static Dictionary<string, HashSet<Flock>> flockBank = new Dictionary<string, HashSet<Flock>>();

        public static void Add(string key, Flock flock)
        {
            if(flockBank.ContainsKey(key))
            {
                flockBank[key].Add(flock);
            }
            else
            {
                flockBank.Add(key, new HashSet<Flock>());
                flockBank[key].Add(flock);
            }
        }

        public static HashSet<Flock> Find(string key)
        {
            if (flockBank.ContainsKey(key))
            {
                return flockBank[key];
            }
            else
            {
                GameManager.Log("Flock Manager", "Key '" + key + "' is not found in the flock bank.");
                throw new NullReferenceException("Key is not found");
            }
        }

        public static void Remove(string key, Flock flock)
        {
            flockBank[key].Remove(flock);
        }

        /// <summary>
        /// Reset the flock bank
        /// </summary>
        public static void Clear()
        {
            flockBank.Clear();
        }
    }
}
