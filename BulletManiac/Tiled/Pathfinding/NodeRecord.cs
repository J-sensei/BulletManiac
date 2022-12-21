using BulletManiac.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Tiled.Pathfinding
{
    /// <summary>
    /// NodeRecord is designed to be used for node selection in priority queue. This class is made to support both Dijkstra and A*
    /// </summary>
    public class NodeRecord : IComparable<NodeRecord>
    {
        public Tile Self { get; set; }
        public Tile From { get; set; }
        public ulong CostSoFar { get; set; }
        /// <summary>
        /// Heuristic value is use for A* Algorithm
        /// </summary>
        public ulong Heuristic { get; set; }
        public ulong HeuristicCost { get { return CostSoFar + Heuristic; } }
        public AStarState State { get; set; } = AStarState.None; // default is None

        public NodeRecord(Tile self, Tile from, ulong costSoFar, ulong heuristic)
        {
            Self = self;
            From = from;
            CostSoFar = costSoFar;
            Heuristic = heuristic;
        }

        // Not used, but was used in legacy version
        // Keeping it for historic purposes
        public int CompareTo(NodeRecord rhs)
        {
            ulong f1 = CostSoFar + Heuristic;
            ulong f2 = rhs.CostSoFar + rhs.Heuristic;
            return (int)(f1 - f2);
        }

        public override string ToString()
        {
            return Self.ToString();
        }
    }

    public enum AStarState
    {
        /// <summary>
        /// None = Not seen
        /// </summary>
        None = 0,
        /// <summary>
        /// Opened = Seen and is in the Priority Queue (also called 'OpenSet' / 'OpenList')
        /// </summary>
        Opened,
        /// <summary>
        /// Closed = Seen and has been taken out of the Priority Queue.
        /// </summary>
        Closed
    };
}
