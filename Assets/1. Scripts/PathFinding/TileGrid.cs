using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PathFinding
{
    public class TileGrid : MonoBehaviour
    {
        public TMP_Text algorithmName;

        public int Rows;
        public int Cols;
        public int seed;
        public int numberOfObstacles;
        public int numberOfExpensiveTiles;
        public Vector2 startPosition;
        public Vector2 endPosition;

        public GameObject TilePrefab;

        public Color TileColor_Default = new Color(0.86f, 0.83f, 0.83f);
        public Color TileColor_Expensive = new Color(0.19f, 0.65f, 0.43f);
        public Color TileColor_Infinity = new Color(0.37f, 0.37f, 0.37f);
        public Color TileColor_Start = Color.green;
        public Color TileColor_End = Color.red;
        public Color TileColor_Path = new Color(0.73f, 0.0f, 1.0f);
        public Color TileColor_Visited = new Color(0.75f, 0.55f, 0.38f);
        public Color TileColor_Frontier = new Color(0.4f, 0.53f, 0.8f);

        public Tile[] Tiles { get; private set; }

        private IEnumerator _pathRoutine;

        private void Awake()
        {
            // Initialize random generator with seed
            System.Random random = new System.Random(seed);

            // Initialize the tile grid
            Tiles = new Tile[Rows * Cols];
            for(int r = 0; r < Rows; r++)
            {
                for(int c = 0; c < Cols; c++)
                {
                    Tile tile = new Tile(this, r, c, TileWeights.Default);
                    tile.InitGameObject(transform, TilePrefab);

                    int index = GetTileIndex(r, c);
                    Tiles[index] = tile;
                }
            }

            // Generate start and end positions
            startPosition = GenerateRandomPosition(random);
            endPosition = GenerateRandomPosition(random);

            // Ensure the start and end positions are not the same
            while(startPosition == endPosition)
            {
                endPosition = GenerateRandomPosition(random);
            }

            // Generate obstacles and expensive tiles
            CreateObstacles(random, numberOfObstacles); // Adjust the number of obstacles as needed
            CreateExpensiveTiles(random, numberOfExpensiveTiles); // Adjust the number of expensive tiles as needed

            ResetGrid();
        }

        private void Update()
        {
            Tile start = GetTile((int)startPosition.x, (int)startPosition.y);
            Tile end = GetTile((int)endPosition.x, (int)endPosition.y);

            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                algorithmName.text = "Breadth First Search";
                FindPath(start, end, BFS.FindPath);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                algorithmName.text = "Dijkstra";
                FindPath(start, end, Dijkstra.FindPath);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                algorithmName.text = "AStar";
                FindPath(start, end, AStar.FindPath);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha4))
            {
                algorithmName.text = "Greedy Best First Search";
                FindPath(start, end, GreedyBestFirstSearch.FindPath);
            }
            else if(Input.GetKeyDown(KeyCode.Escape))
            {
                ResetGrid();
                start.SetColor(TileColor_Start);
                end.SetColor(TileColor_End);
            }
        }

        private void StopPathCoroutine()
        {
            if(_pathRoutine != null)
            {
                StopCoroutine(_pathRoutine);
                _pathRoutine = null;
            }
        }

        private void CreateObstacles(System.Random random, int obstacleCount)
        {
            for(int i = 0; i < obstacleCount; i++)
            {
                Vector2 position = GenerateRandomPosition(random);
                Tile tile = GetTile((int)position.x, (int)position.y);

                if(tile != null && position != startPosition && position != endPosition)
                {
                    tile.Weight = TileWeights.Infinity;
                }
            }
        }

        private void CreateExpensiveTiles(System.Random random, int expensiveTileCount)
        {
            for(int i = 0; i < expensiveTileCount; i++)
            {
                Vector2 position = GenerateRandomPosition(random);
                Tile tile = GetTile((int)position.x, (int)position.y);

                if(tile != null && tile.Weight != TileWeights.Infinity && position != startPosition && position != endPosition)
                {
                    tile.Weight = TileWeights.Expensive;
                }
            }
        }

        private Vector2 GenerateRandomPosition(System.Random random)
        {
            int row = random.Next(0, Rows);
            int col = random.Next(0, Cols);
            return new Vector2(col, row);
        }

        private void ResetGrid()
        {
            foreach(var tile in Tiles)
            {
                tile.Cost = 0;
                tile.PrevTile = null;
                tile.SetText("");

                switch(tile.Weight)
                {
                    case TileWeights.Default:
                        tile.SetColor(TileColor_Default);
                        break;
                    case TileWeights.Expensive:
                        tile.SetColor(TileColor_Expensive);
                        break;
                    case TileWeights.Infinity:
                        tile.SetColor(TileColor_Infinity);
                        break;
                }
            }

            GetTile((int)startPosition.x, (int)startPosition.y)?.SetColor(TileColor_Start);
            GetTile((int)endPosition.x, (int)endPosition.y)?.SetColor(TileColor_End);
        }

        private void FindPath(Tile start, Tile end, Func<TileGrid, Tile, Tile, List<IVisualStep>, List<Tile>> pathFindingFunc)
        {
            ResetGrid();

            List<IVisualStep> steps = new List<IVisualStep>();
            pathFindingFunc(this, start, end, steps);

            foreach(var step in steps)
            {
                step.Execute();
            }
        }

        public Tile GetTile(int row, int col)
        {
            if(!IsInBounds(row, col))
            {
                return null;
            }

            return Tiles[GetTileIndex(row, col)];
        }

        public IEnumerable<Tile> GetNeighbors(Tile tile)
        {
            Tile right = GetTile(tile.Row, tile.Col + 1);
            if(right != null)
            {
                yield return right;
            }

            Tile up = GetTile(tile.Row - 1, tile.Col);
            if(up != null)
            {
                yield return up;
            }

            Tile left = GetTile(tile.Row, tile.Col - 1);
            if(left != null)
            {
                yield return left;
            }

            Tile down = GetTile(tile.Row + 1, tile.Col);
            if(down != null)
            {
                yield return down;
            }
        }

        private bool IsInBounds(int row, int col)
        {
            bool rowInRange = row >= 0 && row < Rows;
            bool colInRange = col >= 0 && col < Cols;
            return rowInRange && colInRange;
        }

        private int GetTileIndex(int row, int col)
        {
            return row * Cols + col;
        }
    }
}
