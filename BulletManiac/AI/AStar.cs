using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

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

            while(priorityQueue.Count > 0)
            {
                currentRecord = priorityQueue.Dequeue();
                currentRecord.State = AStarState.Closed;

                if (currentRecord.Self.Col == goal.Col && currentRecord.Self.Row == goal.Row) break; // Found the goal

                // Get the cost to neighbours of "curRecord.self"
                ulong[] connections = tileGraph.Connections[currentRecord.Self];

                for(int i = 0; i < connections.Length; i++)
                {
                    if (connections[i] != 0)
                    {
                        int col = currentRecord.Self.Col + TileGraph.MoveCol[i];
                        int row = currentRecord.Self.Row + TileGraph.MoveRow[i];

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

        //public static LinkedList<Tile> Compute(TileGraph tileGraph, Tile start, Tile goal, Heuristic heuristic)
        //{
        //    // Create records to store data used by this pathfinding algorithm
        //    Dictionary<Tile, NodeRecord> records = new Dictionary<Tile, NodeRecord>();
        //    // Priority queue use to decide which node to process
        //    PriorityQueue<NodeRecord, ulong> priorityQueue = new PriorityQueue<NodeRecord, ulong>();
            
        //    NodeRecord startNode = new NodeRecord(start, null, ulong.MaxValue, heuristic(start, goal)); // Start node (from nothing so its null) and no cost and heuristic (Not A*)
        //    priorityQueue.Enqueue(startNode, 0);
        //    records[start] = startNode; // Add start nodes to records

        //    // Move data
        //    int[] moveRow = TileGraph.MoveRow;
        //    int[] moveCol = TileGraph.MoveCol;

        //    while (priorityQueue.Count > 0)
        //    {
        //        NodeRecord currentRecord = priorityQueue.Dequeue();
        //        Tile currentTile = currentRecord.Self;

        //        if (currentRecord.Self.Col == goal.Col && currentRecord.Self.Row == goal.Row) break; // Found the goal

        //        // Get the cost to neighbours of "currentTile" or "curRecord.self
        //        ulong[] connections = tileGraph.Connections[currentTile];

        //        // Loop each connection
        //        for (int i = 0; i < connections.Length; i++)
        //        {
        //            // cost of 0 is treated as no connection, so skip iteration.
        //            if (connections[i] == 0) continue;
                    
        //            ulong newCost = currentRecord.CostSoFar + connections[i];
        //            Tile neighbourTile = new Tile(currentTile.Col + moveCol[i], currentTile.Row + moveRow[i]);

        //            bool successGetValue = records.TryGetValue(neighbourTile, out NodeRecord neighbourRecord); // Try to get neighbour node from the records

        //            // We only want to enqueue new elements to priority queue if either one condition is true:
        //            // 1. record exist, and newCost is lower than recorded cost
        //            // 2. record does not exist
        //            bool shouldEnqueue = false;

        //            // 1. record exist, and newCost is lower than recorded cost
        //            if (successGetValue && newCost < neighbourRecord.CostSoFar)
        //            {
        //                shouldEnqueue = true;
        //                neighbourRecord.CostSoFar = newCost;
        //            }
        //            else if (!successGetValue)
        //            {
        //                shouldEnqueue = true;
        //                neighbourRecord = new NodeRecord(neighbourTile, currentTile, newCost, heuristic(neighbourTile, goal));
        //                records[neighbourTile] = neighbourRecord;
        //            }

        //            if (shouldEnqueue)
        //            {
        //                priorityQueue.Enqueue(neighbourRecord, neighbourRecord.HeuristicCost);
        //            }
        //        }
        //    }

        //    return ConstructPath(records, start, goal);
        //}

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
