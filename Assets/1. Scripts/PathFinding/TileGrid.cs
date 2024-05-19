using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
//using OfficeOpenXml;  // Додайте цей using для EPPlus
//using OfficeOpenXml.Style;

namespace PathFinding
{
    public delegate List<Tile> PathFindingFunc(TileGrid grid, Tile start, Tile end, List<IVisualStep> outSteps, out long executionTime, out int nodesVisited, out int pathLength, out long memoryUsage);
   
    public class TileGrid : MonoBehaviour
    {
        public bool instantly = true;
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
            CreatMap();
        }

        public void CreatMap()
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
            if(instantly)
            {
                VisualizeInstantly();
            }
            else
            {
                VisualizeSmoothly();
            }
        }

        //private void VisualizeInstantly()
        //{
        //    Tile start = GetTile((int)startPosition.x, (int)startPosition.y);
        //    Tile end = GetTile((int)endPosition.x, (int)endPosition.y);

        //    if(Input.GetKeyDown(KeyCode.Alpha1))
        //    {
        //        algorithmName.text = "Breadth First Search";
        //        FindPathInstantly(start, end, BFS.FindPath, "Breadth First Search");
        //    }
        //    else if(Input.GetKeyDown(KeyCode.Alpha2))
        //    {
        //        algorithmName.text = "Dijkstra";
        //        FindPathInstantly(start, end, Dijkstra.FindPath, "Dijkstra");
        //    }
        //    else if(Input.GetKeyDown(KeyCode.Alpha3))
        //    {
        //        algorithmName.text = "AStar";
        //        FindPathInstantly(start, end, AStar.FindPath, "AStar");
        //    }
        //    else if(Input.GetKeyDown(KeyCode.Alpha4))
        //    {
        //        algorithmName.text = "Greedy Best First Search";
        //        FindPathInstantly(start, end, GreedyBestFirstSearch.FindPath, "Greedy Best First Search");
        //    }
        //    else if(Input.GetKeyDown(KeyCode.Alpha5))
        //    {
        //        algorithmName.text = "Depth First Search";
        //        FindPathInstantly(start, end, DFS.FindPath, "Depth First Search");
        //    }
        //    else if(Input.GetKeyDown(KeyCode.Alpha6))
        //    {
        //        algorithmName.text = "Bidirectional Search";
        //        FindPathInstantly(start, end, BidirectionalSearch.FindPath, "Bidirectional Search");
        //    }
        //    else if(Input.GetKeyDown(KeyCode.Alpha7))
        //    {
        //        algorithmName.text = "Lee Algorithm";
        //        FindPathInstantly(start, end, LeeAlgorithm.FindPath, "Lee Algorithm");
        //    }
        //    else if(Input.GetKeyDown(KeyCode.Alpha8))
        //    {
        //        algorithmName.text = "Dynamic Programming Maze";
        //        FindPathInstantly(start, end, DynamicProgrammingMaze.FindPath(this, start, end, null), "Dynamic Programming Maze");
        //    }
        //    else if(Input.GetKeyDown(KeyCode.Escape))
        //    {
        //        ResetGrid();
        //        start.SetColor(TileColor_Start);
        //        end.SetColor(TileColor_End);
        //    }
        //}

        //private void VisualizeSmoothly()
        //{
        //    Tile start = GetTile((int)startPosition.x, (int)startPosition.y);
        //    Tile end = GetTile((int)endPosition.x, (int)endPosition.y);

        //    if(Input.GetKeyDown(KeyCode.Alpha1))
        //    {
        //        algorithmName.text = "Breadth First Search";
        //        StopPathCoroutine();
        //        _pathRoutine = FindPathSmoothly(start, end, BFS.FindPath);
        //        StartCoroutine(_pathRoutine);
        //    }
        //    else if(Input.GetKeyDown(KeyCode.Alpha2))
        //    {
        //        algorithmName.text = "Dijkstra";
        //        StopPathCoroutine();
        //        _pathRoutine = FindPathSmoothly(start, end, Dijkstra.FindPath);
        //        StartCoroutine(_pathRoutine);
        //    }
        //    else if(Input.GetKeyDown(KeyCode.Alpha3))
        //    {
        //        algorithmName.text = "AStar";
        //        StopPathCoroutine();
        //        _pathRoutine = FindPathSmoothly(start, end, AStar.FindPath);
        //        StartCoroutine(_pathRoutine);
        //    }
        //    else if(Input.GetKeyDown(KeyCode.Alpha4))
        //    {
        //        algorithmName.text = "Greedy Best First Search";
        //        StopPathCoroutine();
        //        _pathRoutine = FindPathSmoothly(start, end, GreedyBestFirstSearch.FindPath);
        //        StartCoroutine(_pathRoutine);
        //    }
        //    else if(Input.GetKeyDown(KeyCode.Alpha5))
        //    {
        //        algorithmName.text = "Depth First Search";
        //        StopPathCoroutine();
        //        _pathRoutine = FindPathSmoothly(start, end, DFS.FindPath);
        //        StartCoroutine(_pathRoutine);
        //    }
        //    else if(Input.GetKeyDown(KeyCode.Alpha6))
        //    {
        //        algorithmName.text = "Bidirectional Search";
        //        StopPathCoroutine();
        //        _pathRoutine = FindPathSmoothly(start, end, BidirectionalSearch.FindPath);
        //        StartCoroutine(_pathRoutine);
        //    }
        //    else if(Input.GetKeyDown(KeyCode.Alpha7))
        //    {
        //        algorithmName.text = "Lee Algorithm";
        //        StopPathCoroutine();
        //        _pathRoutine = FindPathSmoothly(start, end, LeeAlgorithm.FindPath);
        //        StartCoroutine(_pathRoutine);
        //    }
        //    else if(Input.GetKeyDown(KeyCode.Alpha8))
        //    {
        //        algorithmName.text = "Dynamic Programming Maze";
        //        StopPathCoroutine();
        //        _pathRoutine = FindPathSmoothly(start, end, DynamicProgrammingMaze.FindPath);
        //        StartCoroutine(_pathRoutine);
        //    }

        //    else if(Input.GetKeyDown(KeyCode.Escape))
        //    {
        //        StopPathCoroutine();
        //        ResetGrid();
        //        start.SetColor(TileColor_Start);
        //        end.SetColor(TileColor_End);
        //    }
        //}

        private void StopPathCoroutine()
        {
            if(_pathRoutine != null)
            {
                StopCoroutine(_pathRoutine);
                _pathRoutine = null;
            }
        }

        public bool IsWalkable(int row, int col)
        {
            if(!IsInBounds(row, col))
            {
                return false;
            }

            Tile tile = GetTile(row, col);
            return tile.Weight != TileWeights.Infinity;
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

        private void FindPathInstantly(Tile start, Tile end, Func<TileGrid, Tile, Tile, List<IVisualStep>, List<Tile>> pathFindingFunc)
        {
            ResetGrid();

            List<IVisualStep> steps = new List<IVisualStep>();
            pathFindingFunc(this, start, end, steps);

            foreach(var step in steps)
            {
                step.Execute();
            }
        }

        private IEnumerator FindPathSmoothly(Tile start, Tile end, Func<TileGrid, Tile, Tile, List<IVisualStep>, List<Tile>> pathFindingFunc)
        {
            ResetGrid();

            List<IVisualStep> steps = new List<IVisualStep>();
            pathFindingFunc(this, start, end, steps);

            foreach(var step in steps)
            {
                step.Execute();
                yield return new WaitForFixedUpdate();
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

        public bool IsInBounds(int row, int col)
        {
            bool rowInRange = row >= 0 && row < Rows;
            bool colInRange = col >= 0 && col < Cols;
            return rowInRange && colInRange;
        }

        private int GetTileIndex(int row, int col)
        {
            return row * Cols + col;
        }


        private void WriteResultsToExcel(string algorithmName, long executionTime, int nodesVisited, int pathLength, long memoryUsage, int rows, int cols, int seed, Vector2 startPosition, Vector2 endPosition)
        {
            string filePath = "PathfindingResults.xlsx";

            FileInfo fileInfo = new FileInfo(filePath);

            //using(ExcelPackage package = new ExcelPackage(fileInfo))
            //{
            //    ExcelWorksheet worksheet = package.Workbook.Worksheets.Count > 0 ? package.Workbook.Worksheets[0] : package.Workbook.Worksheets.Add("Results");

            //    int row = worksheet.Dimension?.Rows + 1 ?? 1;

            //    if(row == 1)
            //    {
            //        worksheet.Cells[row, 1].Value = "Algorithm";
            //        worksheet.Cells[row, 2].Value = "Execution Time (ms)";
            //        worksheet.Cells[row, 3].Value = "Nodes Visited";
            //        worksheet.Cells[row, 4].Value = "Path Length";
            //        worksheet.Cells[row, 5].Value = "Memory Usage (bytes)";
            //        worksheet.Cells[row, 6].Value = "Grid Size";
            //        worksheet.Cells[row, 7].Value = "Seed";
            //        worksheet.Cells[row, 8].Value = "Start Position";
            //        worksheet.Cells[row, 9].Value = "End Position";
            //        row++;
            //    }

            //    worksheet.Cells[row, 1].Value = algorithmName;
            //    worksheet.Cells[row, 2].Value = executionTime;
            //    worksheet.Cells[row, 3].Value = nodesVisited;
            //    worksheet.Cells[row, 4].Value = pathLength;
            //    worksheet.Cells[row, 5].Value = memoryUsage;
            //    worksheet.Cells[row, 6].Value = $"{rows}x{cols}";
            //    worksheet.Cells[row, 7].Value = seed;
            //    worksheet.Cells[row, 8].Value = $"{startPosition.x}, {startPosition.y}";
            //    worksheet.Cells[row, 9].Value = $"{endPosition.x}, {endPosition.y}";

            //    package.Save();
            //}
        }

        private void FindPathInstantly(Tile start, Tile end, PathFindingFunc pathFindingFunc, string algorithmName)
        {
            ResetGrid();

            List<IVisualStep> steps = new List<IVisualStep>();
            long executionTime;
            int nodesVisited;
            int pathLength;
            long memoryUsage;

            pathFindingFunc(this, start, end, steps, out executionTime, out nodesVisited, out pathLength, out memoryUsage);

            foreach(var step in steps)
            {
                step.Execute();
            }

            Debug.Log($"Execution Time: {executionTime} ms");
            Debug.Log($"Nodes Visited: {nodesVisited}");
            Debug.Log($"Path Length: {pathLength}");
            Debug.Log($"Memory Usage: {memoryUsage} bytes");

            WriteResultsToExcel(algorithmName, executionTime, nodesVisited, pathLength, memoryUsage, Rows, Cols, seed, startPosition, endPosition);
        }

        private void VisualizeInstantly()
        {
            Tile start = GetTile((int)startPosition.x, (int)startPosition.y);
            Tile end = GetTile((int)endPosition.x, (int)endPosition.y);

            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                algorithmName.text = "Breadth First Search";
                FindPathInstantly(start, end, BFS.FindPath, "Breadth First Search");
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                algorithmName.text = "Dijkstra";
                FindPathInstantly(start, end, Dijkstra.FindPath, "Dijkstra");
            }
            else if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                algorithmName.text = "AStar";
                FindPathInstantly(start, end, AStar.FindPath, "AStar");
            }
            else if(Input.GetKeyDown(KeyCode.Alpha4))
            {
                algorithmName.text = "Greedy Best First Search";
                FindPathInstantly(start, end, GreedyBestFirstSearch.FindPath, "Greedy Best First Search");
            }
            else if(Input.GetKeyDown(KeyCode.Alpha5))
            {
                algorithmName.text = "Depth First Search";
                FindPathInstantly(start, end, DFS.FindPath, "Depth First Search");
            }
            else if(Input.GetKeyDown(KeyCode.Alpha6))
            {
                algorithmName.text = "Bidirectional Search";
                FindPathInstantly(start, end, BidirectionalSearch.FindPath, "Bidirectional Search");
            }
            else if(Input.GetKeyDown(KeyCode.Alpha7))
            {
                algorithmName.text = "Lee Algorithm";
                FindPathInstantly(start, end, LeeAlgorithm.FindPath, "Lee Algorithm");
            }
            else if(Input.GetKeyDown(KeyCode.Alpha8))
            {
                algorithmName.text = "Dynamic Programming Maze";
                FindPathInstantly(start, end, DynamicProgrammingMaze.FindPath, "Dynamic Programming Maze");
            }

            else if(Input.GetKeyDown(KeyCode.Escape))
            {
                ResetGrid();
                start.SetColor(TileColor_Start);
                end.SetColor(TileColor_End);
            }
        }

        private void VisualizeSmoothly()
        {
            Tile start = GetTile((int)startPosition.x, (int)startPosition.y);
            Tile end = GetTile((int)endPosition.x, (int)endPosition.y);

            //if(Input.GetKeyDown(KeyCode.Alpha1))
            //{
            //    algorithmName.text = "Breadth First Search";
            //    StopPathCoroutine();
            //    _pathRoutine = FindPathSmoothly(start, end, BFS.FindPath, "Breadth First Search");
            //    StartCoroutine(_pathRoutine);
            //}
            //else if(Input.GetKeyDown(KeyCode.Alpha2))
            //{
            //    algorithmName.text = "Dijkstra";
            //    StopPathCoroutine();
            //    _pathRoutine = FindPathSmoothly(start, end, Dijkstra.FindPath, "Dijkstra");
            //    StartCoroutine(_pathRoutine);
            //}
            //else if(Input.GetKeyDown(KeyCode.Alpha3))
            //{
            //    algorithmName.text = "AStar";
            //    StopPathCoroutine();
            //    _pathRoutine = FindPathSmoothly(start, end, AStar.FindPath, "AStar");
            //    StartCoroutine(_pathRoutine);
            //}
            //else if(Input.GetKeyDown(KeyCode.Alpha4))
            //{
            //    algorithmName.text = "Greedy Best First Search";
            //    StopPathCoroutine();
            //    _pathRoutine = FindPathSmoothly(start, end, GreedyBestFirstSearch.FindPath, "Greedy Best First Search");
            //    StartCoroutine(_pathRoutine);
            //}
            //else if(Input.GetKeyDown(KeyCode.Alpha5))
            //{
            //    algorithmName.text = "Depth First Search";
            //    StopPathCoroutine();
            //    _pathRoutine = FindPathSmoothly(start, end, DFS.FindPath, "Depth First Search");
            //    StartCoroutine(_pathRoutine);
            //}
            //else if(Input.GetKeyDown(KeyCode.Alpha6))
            //{
            //    algorithmName.text = "Bidirectional Search";
            //    StopPathCoroutine();
            //    _pathRoutine = FindPathSmoothly(start, end, BidirectionalSearch.FindPath, "Bidirectional Search");
            //    StartCoroutine(_pathRoutine);
            //}
            //else if(Input.GetKeyDown(KeyCode.Alpha7))
            //{
            //    algorithmName.text = "Lee Algorithm";
            //    StopPathCoroutine();
            //    _pathRoutine = FindPathSmoothly(start, end, LeeAlgorithm.FindPath, "Lee Algorithm");
            //    StartCoroutine(_pathRoutine);
            //}
            //else if(Input.GetKeyDown(KeyCode.Alpha8))
            //{
            //    algorithmName.text = "Dynamic Programming Maze";
            //    StopPathCoroutine();
            //    _pathRoutine = FindPathSmoothly(start, end, DynamicProgrammingMaze.FindPath, "Dynamic Programming Maze");
            //    StartCoroutine(_pathRoutine);
            //}
            //else if(Input.GetKeyDown(KeyCode.Alpha9))
            //{
            //    algorithmName.text = "Jump Point Search";
            //    StopPathCoroutine();
            //    _pathRoutine = FindPathSmoothly(start, end, JumpPointSearch.FindPath, "Jump Point Search");
            //    StartCoroutine(_pathRoutine);
            //}
            //else if(Input.GetKeyDown(KeyCode.Escape))
            //{
            //    StopPathCoroutine();
            //    ResetGrid();
            //    start.SetColor(TileColor_Start);
            //    end.SetColor(TileColor_End);
            //}
        }
    }
}
