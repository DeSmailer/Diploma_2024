using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PathFinding
{
    public static class BidirectionalSearch
    {
        private static int visited = 0;
        public static List<Tile> FindPath(TileGrid grid, Tile start, Tile end, List<IVisualStep> outSteps, out long executionTime, out int nodesVisited, out int pathLength, out long memoryUsage)
        {
            long memoryBefore = GC.GetTotalMemory(true);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            outSteps.Add(new MarkStartTileStep(start));
            outSteps.Add(new MarkEndTileStep(end));

            HashSet<Tile> visitedFromStart = new HashSet<Tile>() { start };
            HashSet<Tile> visitedFromEnd = new HashSet<Tile>() { end };

            Queue<Tile> frontierFromStart = new Queue<Tile>();
            Queue<Tile> frontierFromEnd = new Queue<Tile>();

            frontierFromStart.Enqueue(start);
            frontierFromEnd.Enqueue(end);

            start.PrevTile = null;
            end.PrevTile = null;

            Tile meetingPoint = null;
            visited = 0;

            while(frontierFromStart.Count > 0 && frontierFromEnd.Count > 0)
            {
                if(ProcessFrontier(grid, frontierFromStart, visitedFromStart, visitedFromEnd, outSteps, ref meetingPoint, true))
                {
                    stopwatch.Stop();
                    executionTime = stopwatch.ElapsedMilliseconds;

                    long memoryAfter = GC.GetTotalMemory(true);
                    memoryUsage = memoryAfter - memoryBefore;

                    List<Tile> path = BuildPath(start, end, meetingPoint, outSteps, true);
                    pathLength = path.Count;
                    nodesVisited = visited;
                    return path;
                }

                if(ProcessFrontier(grid, frontierFromEnd, visitedFromEnd, visitedFromStart, outSteps, ref meetingPoint, false))
                {
                    stopwatch.Stop();
                    executionTime = stopwatch.ElapsedMilliseconds;

                    long memoryAfter = GC.GetTotalMemory(true);
                    memoryUsage = memoryAfter - memoryBefore;

                    List<Tile> path = BuildPath(start, end, meetingPoint, outSteps, false);
                    pathLength = path.Count;
                    nodesVisited = visited;
                    return path;
                }
            }

            stopwatch.Stop();
            executionTime = stopwatch.ElapsedMilliseconds;

            long memoryAfterFinal = GC.GetTotalMemory(true);
            memoryUsage = memoryAfterFinal - memoryBefore;

            pathLength = 0;
            nodesVisited = visited;
            return new List<Tile>(); // Return empty path if no path is found
        }

        private static bool ProcessFrontier(TileGrid grid, Queue<Tile> frontier, HashSet<Tile> visitedFromThisSide, HashSet<Tile> visitedFromOtherSide, List<IVisualStep> outSteps, ref Tile meetingPoint, bool fromStart)
        {
            if(frontier.Count > 0)
            {
                Tile current = frontier.Dequeue();

                if(visitedFromOtherSide.Contains(current))
                {
                    meetingPoint = current;
                    return true;
                }

                foreach(var neighbor in grid.GetNeighbors(current))
                {
                    if(neighbor.Weight == TileWeights.Infinity || visitedFromThisSide.Contains(neighbor))
                    {
                        continue;
                    }

                    if(fromStart)
                    {
                        neighbor.PrevTile = current;
                    }
                    else
                    {
                        neighbor.NextTile = current;
                    }
                    visitedFromThisSide.Add(neighbor);
                    visited++;
                    frontier.Enqueue(neighbor);

                    outSteps.Add(new VisitTileStep(current));

                    if(!visitedFromOtherSide.Contains(neighbor))
                    {
                        outSteps.Add(new PushTileInFrontierStep(neighbor, 0));
                    }
                }
            }

            return false;
        }

        private static List<Tile> BuildPath(Tile start, Tile end, Tile meetingPoint, List<IVisualStep> outSteps, bool fromStart)
        {
            List<Tile> pathFromStart = new List<Tile>();
            List<Tile> pathFromEnd = new List<Tile>();

            // Build path from start to meeting point
            Tile current = meetingPoint;
            while(current != null)
            {
                pathFromStart.Add(current);
                current = current.PrevTile;
            }

            // Build path from end to meeting point
            current = meetingPoint;
            while(current != null)
            {
                pathFromEnd.Add(current);
                current = current.NextTile;
            }

            pathFromStart.Reverse();
            pathFromEnd.RemoveAt(0); // Remove the duplicate meeting point
            pathFromStart.AddRange(pathFromEnd);

            foreach(var tile in pathFromStart)
            {
                if(tile == start)
                {
                    outSteps.Add(new MarkStartTileStep(tile));
                }
                else if(tile == end)
                {
                    outSteps.Add(new MarkEndTileStep(tile));
                }
                else
                {
                    outSteps.Add(new MarkPathTileStep(tile));
                }
            }

            return pathFromStart;
        }
    }
}
