using System.Collections.Generic;

namespace PathFinding
{
    public static class BFS
    {
        public static List<Tile> FindPath(TileGrid grid, Tile start, Tile end, List<IVisualStep> outSteps)
        {
            // Visual stuff
            outSteps.Add(new MarkStartTileStep(start));
            outSteps.Add(new MarkEndTileStep(end));
            // ~Visual stuff

            HashSet<Tile> visited = new HashSet<Tile>();
            visited.Add(start);

            Queue<Tile> frontier = new Queue<Tile>();
            frontier.Enqueue(start);

            start.PrevTile = null;

            while(frontier.Count > 0)
            {
                Tile current = frontier.Dequeue();

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
                        visited.Add(neighbor);
                        frontier.Enqueue(neighbor);

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