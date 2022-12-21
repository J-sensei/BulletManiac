using BulletManiac.Tiled;
using System;
using System.Collections.Generic;

namespace BulletManiac.Utilities
{
    /// <summary>
    /// Algorithm to perform pathfinding on the tile environment
    /// </summary>
    public static class Dijkstra
    {
        public static LinkedList<Tile> Compute(TileGraph tileGraph, Tile start, Tile goal)
        {
            // Create records to store data used by this pathfinding algorithm
            Dictionary<Tile, NodeRecord> records = new Dictionary<Tile, NodeRecord>();
            // Priority queue use to decide which node to process
            PriorityQueue<NodeRecord, ulong> priorityQueue = new PriorityQueue<NodeRecord, ulong>();

            NodeRecord startNode = new NodeRecord(start, null, 0, 0); // Start node (from nothing so its null) and no cost and heuristic (Not A*)
            priorityQueue.Enqueue(startNode, 0);
            records[start] = startNode; // Add start nodes to records

            // Move data
            int[] moveRow = TileGraph.MoveRow;
            int[] moveCol = TileGraph.MoveCol;

            while(priorityQueue.Count > 0)
            {
                NodeRecord currentRecord = priorityQueue.Dequeue();
                Tile currentTile = currentRecord.Self;

                if (currentTile == goal) break; // Found the goal

                // Get the cost to neighbours of "currentTile" or "curRecord.self
                ulong[] connections = tileGraph.Connections[currentTile];

                // Loop each connection
                for(int i = 0; i < connections.Length; i++)
                {
                    // cost of 0 is treated as no connection, so skip iteration.
                    if (connections[i] == 0) continue;

                    ulong newCost = currentRecord.CostSoFar + connections[i];
                    Tile neighbourTile = new Tile(currentTile.Col + moveCol[i], currentTile.Row + moveRow[i]);

                    bool successGetValue = records.TryGetValue(neighbourTile, out NodeRecord neighbourRecord); // Try to get neighbour node from the records

                    // We only want to enqueue new elements to priority queue if either one condition is true:
                    // 1. record exist, and newCost is lower than recorded cost
                    // 2. record does not exist
                    bool shouldEnqueue = false;

                    // 1. record exist, and newCost is lower than recorded cost
                    if (successGetValue && (newCost < neighbourRecord.CostSoFar))
                    {
                        shouldEnqueue = true;
                        neighbourRecord.CostSoFar = newCost;
                    }
                    else if (!successGetValue)
                    {
                        shouldEnqueue = true;
                        neighbourRecord = new NodeRecord(neighbourTile, currentTile, newCost, 0);
                        records[neighbourTile] = neighbourRecord;
                    }

                    if (shouldEnqueue)
                    {
                        ulong priority = newCost;
                        priorityQueue.Enqueue(neighbourRecord, priority);
                    }
                }
            }

            return ConstructPath(records, start, goal);
        }

        private static LinkedList<Tile> ConstructPath(Dictionary<Tile, NodeRecord> nodeRecords, Tile start, Tile goal)
        {
            LinkedList<Tile> path = new LinkedList<Tile>();

            for (NodeRecord cur = nodeRecords[goal]; cur.From != null;)
            {
                path.AddFirst(cur.Self);
                cur = nodeRecords[cur.From];
            }
            path.AddFirst(start);

            return path;
        }
    }
}
