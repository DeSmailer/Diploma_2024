using System;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public class GreedyBestFirstSearch : MonoBehaviour
    {
        public static List<Tile> FindPath(TileGrid grid, Tile start, Tile end, List<IVisualStep> outSteps)
        {
            // Visual stuff
            outSteps.Add(new MarkStartTileStep(start));
            outSteps.Add(new MarkEndTileStep(end));
            // ~Visual stuff

            Comparison<Tile> heuristicComparison = (lhs, rhs) =>
            {
                float lhsCost = PathFinderUtilities.GetEuclideanHeuristicCost(lhs, end);
                float rhsCost = PathFinderUtilities.GetEuclideanHeuristicCost(rhs, end);

                return lhsCost.CompareTo(rhsCost);
            };

            MinHeap<Tile> frontier = new MinHeap<Tile>(heuristicComparison);
            frontier.Add(start);

            HashSet<Tile> visited = new HashSet<Tile>();
            visited.Add(start);

            start.PrevTile = null;

            while(frontier.Count > 0)
            {
                Tile current = frontier.Remove();

                // Visual stuff
                if(current != start && current != end)
                {
                    outSteps.Add(new VisitTileStep(current));
                }
                // ~Visual stuff

                if(current == end)
                {
                    break;
                }

                foreach(var neighbor in grid.GetNeighbors(current))
                {
                    if(!visited.Contains(neighbor))
                    {
                        frontier.Add(neighbor);
                        visited.Add(neighbor);
                        neighbor.PrevTile = current;

                        // Visual stuff
                        if(neighbor != end)
                        {
                            outSteps.Add(new PushTileInFrontierStep(neighbor, 0));
                        }
                        // ~Visual stuff
                    }
                }
            }

            List<Tile> path = PathFinderUtilities.BacktrackToPath(end);

            // Visual stuff
            foreach(var tile in path)
            {
                if(tile == start || tile == end)
                {
                    continue;
                }

                outSteps.Add(new MarkPathTileStep(tile));
            }
            // ~Visual stuff
            return path;
        }
    }
}

