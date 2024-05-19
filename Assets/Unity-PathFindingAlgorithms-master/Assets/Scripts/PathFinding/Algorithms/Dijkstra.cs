using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public class Dijkstra : MonoBehaviour
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

            HashSet<Tile> visited = new HashSet<Tile> { start };

            MinHeap<Tile> frontier = new MinHeap<Tile>((lhs, rhs) => lhs.Cost.CompareTo(rhs.Cost));
            frontier.Add(start);

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
                    int newNeighborCost = current.Cost + neighbor.Weight;
                    if(newNeighborCost < neighbor.Cost)
                    {
                        neighbor.Cost = newNeighborCost;
                        neighbor.PrevTile = current;
                    }

                    if(!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        frontier.Add(neighbor);

                        if(neighbor != end)
                        {
                            outSteps.Add(new PushTileInFrontierStep(neighbor, neighbor.Cost));
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