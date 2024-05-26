using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace PathFinding
{
    public class AStar : MonoBehaviour
    {
        public static List<Tile> FindPath(TileGrid grid, Tile start, Tile end, List<IVisualStep> outSteps, out long executionTime, out int nodesVisited, out int pathLength, out long memoryUsage)
        {

            long memoryBefore = GC.GetTotalMemory(true);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();


            outSteps.Add(new MarkStartTileStep(start));
            outSteps.Add(new MarkEndTileStep(end));

            nodesVisited = 0;

            foreach(var tile in grid.Tiles)
            {
                tile.Cost = int.MaxValue;
            }

            start.Cost = 0;

            Comparison<Tile> heuristicComparison = (lhs, rhs) =>
            {
                float lhsCost = lhs.Cost + PathFinderUtilities.GetEuclideanHeuristicCost(lhs, end);
                float rhsCost = rhs.Cost + PathFinderUtilities.GetEuclideanHeuristicCost(rhs, end);

                return lhsCost.CompareTo(rhsCost);
            };

            MinHeap<Tile> frontier = new MinHeap<Tile>(heuristicComparison);
            frontier.Add(start);

            HashSet<Tile> visited = new HashSet<Tile>() { start };

            start.PrevTile = null;

            while(frontier.Count > 0)
            {
                nodesVisited++;
                Tile current = frontier.Remove();

                if(current != start && current != end)
                {
                    outSteps.Add(new VisitTileStep(current));
                }

                if(current == end)
                {
                    break;
                }

                foreach(var neighbor in grid.GetNeighbors(current))
                {
                    if(neighbor.Weight == TileWeights.Infinity)
                    {
                        // Skip this neighbor as it is an obstacle
                        continue;
                    }

                    int newNeighborCost = current.Cost + neighbor.Weight;
                    if(newNeighborCost < neighbor.Cost)
                    {
                        neighbor.Cost = newNeighborCost;
                        neighbor.PrevTile = current;
                    }

                    if(!visited.Contains(neighbor))
                    {
                        frontier.Add(neighbor);
                        visited.Add(neighbor);

                        if(neighbor != end)
                        {
                            outSteps.Add(new PushTileInFrontierStep(neighbor, neighbor.Cost));
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
