using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public class AStar : MonoBehaviour
    {
        public static List<Tile> FindPath(TileGrid grid, Tile start, Tile end, List<IVisualStep> outSteps)
        {
            outSteps.Add(new MarkStartTileStep(start));
            outSteps.Add(new MarkEndTileStep(end));

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

                        // Visual stuff
                        if(neighbor != end)
                        {
                            outSteps.Add(new PushTileInFrontierStep(neighbor, neighbor.Cost));
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

