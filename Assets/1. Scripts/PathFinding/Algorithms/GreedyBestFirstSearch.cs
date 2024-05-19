using System;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public class GreedyBestFirstSearch : MonoBehaviour
    {
        public static List<Tile> FindPath(TileGrid grid, Tile start, Tile end, List<IVisualStep> outSteps)
        {
            outSteps.Add(new MarkStartTileStep(start));
            outSteps.Add(new MarkEndTileStep(end));

            Comparison<Tile> heuristicComparison = (lhs, rhs) =>
            {
                float lhsCost = PathFinderUtilities.GetEuclideanHeuristicCost(lhs, end);
                float rhsCost = PathFinderUtilities.GetEuclideanHeuristicCost(rhs, end);

                return lhsCost.CompareTo(rhsCost);
            };

            MinHeap<Tile> frontier = new MinHeap<Tile>(heuristicComparison);
            frontier.Add(start);

            HashSet<Tile> visited = new HashSet<Tile>() { start };

            start.PrevTile = null;

            while(frontier.Count > 0)
            {
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
                        continue;
                    }

                    if(!visited.Contains(neighbor))
                    {
                        frontier.Add(neighbor);
                        visited.Add(neighbor);
                        neighbor.PrevTile = current;

                        if(neighbor != end)
                        {
                            outSteps.Add(new PushTileInFrontierStep(neighbor, 0));
                        }
                    }
                }
            }

            List<Tile> path = PathFinderUtilities.BacktrackToPath(end);

            foreach(var tile in path)
            {
                if(tile == start || tile == end)
                {
                    continue;
                }

                outSteps.Add(new MarkPathTileStep(tile));
            }
            return path;
        }
    }
}
