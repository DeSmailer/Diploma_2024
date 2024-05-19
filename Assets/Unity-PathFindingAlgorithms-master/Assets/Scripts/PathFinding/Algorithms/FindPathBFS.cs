using System.Collections.Generic;

namespace PathFinding
{
    public static class BFS
    {
        public static List<Tile> FindPath(TileGrid grid, Tile start, Tile end, List<IVisualStep> outSteps)
        {
            outSteps.Add(new MarkStartTileStep(start));
            outSteps.Add(new MarkEndTileStep(end));

            HashSet<Tile> visited = new HashSet<Tile>() { start };

            Queue<Tile> frontier = new Queue<Tile>();
            frontier.Enqueue(start);

            start.PrevTile = null;

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
                    if(!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        frontier.Enqueue(neighbor);

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