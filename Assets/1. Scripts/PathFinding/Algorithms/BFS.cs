using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PathFinding
{
    public static class BFS
    {
        public static List<Tile> FindPath(TileGrid grid, Tile start, Tile end, List<IVisualStep> outSteps, out long executionTime, out int nodesVisited, out int pathLength, out long memoryUsage)
        {
            long memoryBefore = GC.GetTotalMemory(true);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            outSteps.Add(new MarkStartTileStep(start));
            outSteps.Add(new MarkEndTileStep(end));

            HashSet<Tile> visited = new HashSet<Tile>() { start };
            Queue<Tile> frontier = new Queue<Tile>();
            frontier.Enqueue(start);
            start.PrevTile = null;

            nodesVisited = 0;

            while(frontier.Count > 0)
            {
                Tile current = frontier.Dequeue();
                nodesVisited++;

                if(current == end)
                {
                    break;
                }

                foreach(var neighbor in grid.GetNeighbors(current))
                {
                    if(neighbor.Weight == TileWeights.Infinity || visited.Contains(neighbor))
                    {
                        continue;
                    }

                    neighbor.PrevTile = current;
                    visited.Add(neighbor);
                    frontier.Enqueue(neighbor);

                    if(neighbor != end)
                    {
                        outSteps.Add(new PushTileInFrontierStep(neighbor, 0));
                    }
                }

                // Додати крок візуалізації для відвідування кожної клітинки
                if(current != start && current != end)
                {
                    outSteps.Add(new VisitTileStep(current));
                }
            }

            List<Tile> path = PathFinderUtilities.BacktrackToPath(end);
            pathLength = path.Count;

            foreach(var tile in path)
            {
                if(tile == start || tile == end)
                {
                    continue;
                }

                outSteps.Add(new MarkPathTileStep(tile));
            }

            stopwatch.Stop();
            executionTime = stopwatch.ElapsedMilliseconds;

            long memoryAfter = GC.GetTotalMemory(true);
            memoryUsage = memoryAfter - memoryBefore;

            return path;
        }
    }
}
