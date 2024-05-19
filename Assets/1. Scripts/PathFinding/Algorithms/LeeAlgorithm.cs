using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public static class LeeAlgorithm
    {
        public static List<Tile> FindPath(TileGrid grid, Tile start, Tile end, List<IVisualStep> outSteps)
        {
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

            Queue<Tile> frontier = new Queue<Tile>();
            frontier.Enqueue(start);
            distances[start.Row, start.Col] = 0;

            while(frontier.Count > 0)
            {
                Tile current = frontier.Dequeue();

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
                    if(neighbor.Weight == TileWeights.Infinity || distances[neighbor.Row, neighbor.Col] != int.MaxValue)
                    {
                        continue;
                    }

                    distances[neighbor.Row, neighbor.Col] = distances[current.Row, current.Col] + 1;
                    neighbor.PrevTile = current;
                    frontier.Enqueue(neighbor);

                    if(neighbor != end)
                    {
                        outSteps.Add(new PushTileInFrontierStep(neighbor, distances[neighbor.Row, neighbor.Col]));
                    }
                }
            }

            List<Tile> path = new List<Tile>();
            Tile pathTile = end;

            while(pathTile != null && pathTile != start)
            {
                path.Add(pathTile);
                pathTile = pathTile.PrevTile;
            }

            if(pathTile == start)
            {
                path.Add(start);
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
            }
            else
            {
                // No path found
                path.Clear();
            }

            return path;
        }
    }
}
