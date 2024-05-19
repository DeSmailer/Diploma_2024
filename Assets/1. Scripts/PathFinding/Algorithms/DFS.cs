using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PathFinding
{
    public static class DFS
    {
        public static List<Tile> FindPath(TileGrid grid, Tile start, Tile end, List<IVisualStep> outSteps, out long executionTime, out int nodesVisited, out int pathLength, out long memoryUsage)
        {
            long memoryBefore = GC.GetTotalMemory(true);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            outSteps.Add(new MarkStartTileStep(start));
            outSteps.Add(new MarkEndTileStep(end));

            HashSet<Tile> visited = new HashSet<Tile>();
            Stack<Tile> stack = new Stack<Tile>();
            stack.Push(start);

            start.PrevTile = null;
            nodesVisited = 0;

            while(stack.Count > 0)
            {
                Tile current = stack.Pop();
                nodesVisited++;

                if(current == end)
                {
                    break;
                }

                if(!visited.Contains(current))
                {
                    visited.Add(current);

                    if(current != start && current != end)
                    {
                        outSteps.Add(new VisitTileStep(current));
                    }

                    foreach(var neighbor in grid.GetNeighbors(current))
                    {
                        if(neighbor.Weight == TileWeights.Infinity || visited.Contains(neighbor))
                        {
                            continue;
                        }

                        if(!visited.Contains(neighbor))
                        {
                            stack.Push(neighbor);
                            neighbor.PrevTile = current;

                            if(neighbor != end)
                            {
                                outSteps.Add(new PushTileInFrontierStep(neighbor, 0));
                            }
                        }
                    }
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
