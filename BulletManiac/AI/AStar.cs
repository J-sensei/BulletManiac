using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Tiled.AI
{
    public class AStar
    {
        public delegate ulong Heuristic(Tile start, Tile end);

        public static LinkedList<Tile> Compute(TileGraph tileGraph, Tile start, Tile goal, Heuristic heuristic)
        {
            // Create node records to store data used by AStar
            Dictionary<Tile, NodeRecord> records = new Dictionary<Tile, NodeRecord>();

            // Init cost and heuristic value
            foreach (Tile node in tileGraph.Nodes)
            {
                records.Add(node, new NodeRecord(node, null, ulong.MaxValue, heuristic(node, goal)));
            }
            records[start] = new NodeRecord(start, null, 0, heuristic(start, goal));

            PriorityQueue<NodeRecord, ulong> priorityQueue = new PriorityQueue<NodeRecord, ulong>();

            // Push the node record for start node to the priority queue
            NodeRecord startRecord = records[start];
            priorityQueue.Enqueue(startRecord, startRecord.HeuristicCost);
            startRecord.State = AStarState.Opened;

            NodeRecord endRecord = records[goal];
            NodeRecord currentRecord, neighbourRecord;

            int[] moveRow = TileGraph.MoveRow;
            int[] moveCol = TileGraph.MoveCol;

            while(priorityQueue.Count > 0)
            {
                currentRecord = priorityQueue.Dequeue();
                currentRecord.State = AStarState.Closed;

                if (currentRecord.Self == goal) break; // Found the goal

                // Get the cost to neighbours of "curRecord.self"
                ulong[] connections = tileGraph.Connections[currentRecord.Self];

                for(int i = 0; i < connections.Length; i++)
                {
                    if (connections[i] != 0)
                    {
                        int col = currentRecord.Self.Col + moveCol[i];
                        int row = currentRecord.Self.Row + moveRow[i];

                        Tile neighbourTile = new Tile(col, row);
                        neighbourRecord = records[neighbourTile];
                    }
                    else
                    {
                        continue;
                    }

                    // Ignore neighbours in Closed state
                    if (neighbourRecord.State == AStarState.Closed) continue;

                    ulong newCost = currentRecord.CostSoFar + connections[i];
                    // Found shorter path
                    if(neighbourRecord.CostSoFar > newCost)
                    {
                        neighbourRecord.CostSoFar = newCost;
                        neighbourRecord.From = currentRecord.Self;
                    }

                    // Update priority queue
                    if(!(neighbourRecord.State == AStarState.Opened))
                    {
                        priorityQueue.Enqueue(neighbourRecord, neighbourRecord.HeuristicCost);
                        neighbourRecord.State = AStarState.Opened;
                    }
                }
            }
            return ConstructPath(records, start, goal);
        }

        private static LinkedList<Tile> ConstructPath(Dictionary<Tile, NodeRecord> nodeRecords, Tile start, Tile end)
        {
            LinkedList<Tile> path = new LinkedList<Tile>();

            for (NodeRecord cur = nodeRecords[end]; cur.From != null;)
            {
                path.AddFirst(cur.Self);
                cur = nodeRecords[cur.From];
            }
            path.AddFirst(start);

            return path;
        }

        // Manhattan distance between two tiles
        public static ulong Manhattan(Tile start, Tile end)
        {
            // Hard coded for now, need to find a way to reference
            int tileWidth = 16;
            int tileHeight = 16;

            Vector2 s = Tile.ToPosition(start, tileWidth, tileHeight);
            Vector2 e = Tile.ToPosition(end, tileWidth, tileHeight);

            float dx = e.X - s.X;
            float dy = e.Y - s.Y;
            return (ulong)(Math.Abs(dx) + Math.Abs(dy));
        }

        // Euclidean distance between two tiles
        public static ulong Euclidean(Tile start, Tile end)
        {
            int tileWidth = 16;
            int tileHeight = 16;

            Vector2 s = Tile.ToPosition(start, tileWidth, tileHeight);
            Vector2 e = Tile.ToPosition(end, tileWidth, tileHeight);

            return (ulong)(e - s).Length();
        }
    }
}
