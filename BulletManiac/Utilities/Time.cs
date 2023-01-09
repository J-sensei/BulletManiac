using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Utilities
{
    /// <summary>
    /// Time information of the game
    /// </summary>
    public static class Time
    {
        /// <summary>
        /// Time elpased, second
        /// </summary>
        public static float DeltaTime { get => deltaTime; }
        /// <summary>
        /// Total time, second
        /// </summary>
        public static float TotalTime { get => totalTime; }
        public static float deltaTime;
        public static float totalTime;
    }
}
