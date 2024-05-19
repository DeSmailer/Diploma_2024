using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace PathFinding
{
    public static class DynamicProgrammingMaze
    {
        public static List<Tile> FindPath(TileGrid grid, Tile start, Tile end, List<IVisualStep> outSteps, out long executionTime, out int nodesVisited, out int pathLength, out long memoryUsage)
        {
            long memoryBefore = GC.GetTotalMemory(true);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            outSteps.Add(new MarkStartTileStep(start));
            outSteps.Add(new MarkEndTileStep(end));

            // Initialize distances
            int[,] distances = new int[grid.Rows, grid.Cols];
            for(int r = 0; r < grid.Rows; r++)
            {
                for(int c = 0; c < grid.Cols; c++)
                {
                    distances[r, c] = int.MaxValue;
                }
            }

            distances[start.Row, start.Col] = 0;

            Queue<Tile> frontier = new Queue<Tile>();
            frontier.Enqueue(start);
            nodesVisited = 0;

            while(frontier.Count > 0)
            {
                Tile current = frontier.Dequeue();
                nodesVisited++;

                if(current != start && current != end)
                {
                    outSteps.Add(new VisitTileStep(current));
                }

                foreach(var neighbor in grid.GetNeighbors(current))
                {
                    if(neighbor.Weight == TileWeights.Infinity)
                    {
                        continue;
                    }

                    int newDist = distances[current.Row, current.Col] + neighbor.Weight;
                    if(newDist < distances[neighbor.Row, neighbor.Col])
                    {
                        distances[neighbor.Row, neighbor.Col] = newDist;
                        neighbor.PrevTile = current;
                        frontier.Enqueue(neighbor);

                        if(neighbor != end)
                        {
                            outSteps.Add(new PushTileInFrontierStep(neighbor, newDist));
                        }
                    }
                }
            }

            List<Tile> path = ReconstructPath(end, start, outSteps);
            pathLength = path.Count;

            stopwatch.Stop();
            executionTime = stopwatch.ElapsedMilliseconds;

            long memoryAfter = GC.GetTotalMemory(true);
            memoryUsage = memoryAfter - memoryBefore;

            return path;
        }

        private static List<Tile> ReconstructPath(Tile end, Tile start, List<IVisualStep> outSteps)
        {
            List<Tile> path = new List<Tile>();
            Tile current = end;
            while(current != null)
            {
                path.Add(current);
                current = current.PrevTile;
            }

            path.Reverse();

            foreach(var tile in path)
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

            return path;
        }
    }
}
