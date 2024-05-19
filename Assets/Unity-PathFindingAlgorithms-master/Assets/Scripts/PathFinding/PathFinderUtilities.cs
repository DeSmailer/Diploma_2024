﻿using System.Collections.Generic;

namespace PathFinding
{
    public static class PathFinderUtilities
    {
        public static float GetEuclideanHeuristicCost(Tile current, Tile end)
        {
            float heuristicCost = (current.ToVector2() - end.ToVector2()).magnitude;
            return heuristicCost;
        }

        public static List<Tile> BacktrackToPath(Tile end)
        {
            Tile current = end;
            List<Tile> path = new List<Tile>();

            while(current != null)
            {
                path.Add(current);
                current = current.PrevTile;
            }

            path.Reverse();

            return path;
        }
    }

    public interface IVisualStep
    {
        void Execute();
    }

    public abstract class VisualStep : IVisualStep
    {
        protected Tile _tile;

        public VisualStep(Tile tile)
        {
            _tile = tile;
        }

        public abstract void Execute();
    }

    public class MarkStartTileStep : VisualStep
    {
        public MarkStartTileStep(Tile tile) : base(tile)
        {
        }

        public override void Execute()
        {
            _tile.SetColor(_tile.Grid.TileColor_Start);
        }
    }

    public class MarkEndTileStep : VisualStep
    {
        public MarkEndTileStep(Tile tile) : base(tile)
        {
        }

        public override void Execute()
        {
            _tile.SetColor(_tile.Grid.TileColor_End);
        }
    }

    public class MarkPathTileStep : VisualStep
    {
        public MarkPathTileStep(Tile tile) : base(tile)
        {
        }

        public override void Execute()
        {
            _tile.SetColor(_tile.Grid.TileColor_Path);
        }
    }

    public class PushTileInFrontierStep : VisualStep
    {
        private int _cost;

        public PushTileInFrontierStep(Tile tile, int cost) : base(tile)
        {
            _cost = cost;
        }

        public override void Execute()
        {
            _tile.SetColor(_tile.Grid.TileColor_Frontier);
            _tile.SetText(_cost != 0 ? _cost.ToString() : "");
        }
    }

    public class VisitTileStep : VisualStep
    {
        public VisitTileStep(Tile tile) : base(tile)
        {
        }

        public override void Execute()
        {
            _tile.SetColor(_tile.Grid.TileColor_Visited);
        }
    }
}
