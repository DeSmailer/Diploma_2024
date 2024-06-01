using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

namespace PathFinding
{
    public delegate List<Tile> PathFindingFunc(TileGrid grid, Tile start, Tile end, List<IVisualStep> outSteps, out long executionTime, out int nodesVisited, out int pathLength, out long memoryUsage);

    public class TileGrid : MonoBehaviour
    {
        public bool instantly = true;
        public bool visualize = true;
        public TMP_Text algorithmNameText;
        public TMP_Text executionTimeText;
        public TMP_Text nodesVisitedText;
        public TMP_Text pathLengthText;
        public TMP_Text memoryUsageText;

        public int Rows;
        public int Cols;
        public int seed;
        public int numberOfObstacles;
        public int numberOfExpensiveTiles;
        private Vector2 startPosition;
        private Vector2 endPosition;

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

        private int[] gridSizes = { 5, 10, 25, 50, 75, /*100, 200*/ };

        private IEnumerator _pathRoutine;

        private void Awake()
        {
            CreateMap();
        }

        public void CreateMap()
        {
            DeleteAllChildren();
            // Initialize random generator with seed
            System.Random random = new System.Random(seed);

            // Initialize the tile grid
            Tiles = new Tile[Rows * Cols];
            for(int r = 0; r < Rows; r++)
            {
                for(int c = 0; c < Cols; c++)
                {
                    Tile tile = new Tile(this, r, c, TileWeights.Default);
                    if(visualize)
                    {
                        tile.InitGameObject(transform, TilePrefab);
                    }

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
            numberOfObstacles = CalculateNumberOfObstacles(seed, Rows, Cols);
            numberOfExpensiveTiles = CalculateNumberOfExpensiveTiles(seed, Rows, Cols);
            CreateObstacles(random, numberOfObstacles);
            CreateExpensiveTiles(random, numberOfExpensiveTiles);

            ResetGrid();
        }

        public void DeleteAllChildren()
        {
            foreach(Transform child in transform)
            {
                Destroy(child.gameObject);
            }
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
            if(Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(RunAllAlgorithmsCoroutine());
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

        //private void FindPathInstantly(Tile start, Tile end, Func<TileGrid, Tile, Tile, List<IVisualStep>, List<Tile>> pathFindingFunc)
        //{
        //    ResetGrid();

        //    List<IVisualStep> steps = new List<IVisualStep>();
        //    pathFindingFunc(this, start, end, steps);

        //    foreach(var step in steps)
        //    {
        //        step.Execute();
        //    }
        //}

        //private IEnumerator FindPathSmoothly(Tile start, Tile end, Func<TileGrid, Tile, Tile, List<IVisualStep>, List<Tile>> pathFindingFunc)
        //{
        //    ResetGrid();

        //    List<IVisualStep> steps = new List<IVisualStep>();
        //    pathFindingFunc(this, start, end, steps);

        //    foreach(var step in steps)
        //    {
        //        step.Execute();
        //        yield return new WaitForFixedUpdate();
        //    }
        //}

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
            string filePath = "PathfindingResults.csv";

            bool fileExists = File.Exists(filePath);

            using(StreamWriter writer = new StreamWriter(filePath, true))
            {
                if(!fileExists)
                {
                    writer.WriteLine("Algorithm,Execution Time (ms),Nodes Visited,Path Length,Memory Usage (bytes),Grid Size,Seed,Start Position,End Position");
                }

                writer.WriteLine($"{algorithmName},{executionTime},{nodesVisited},{pathLength},{memoryUsage},{rows}x{cols},{seed},{startPosition.x},{startPosition.y},{endPosition.x},{endPosition.y}");
            }
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

            if(visualize)
            {
                foreach(var step in steps)
                {
                    step.Execute();
                }
            }


            ShowInfo(algorithmName, executionTime, nodesVisited, pathLength, memoryUsage);

            WriteResultsToExcel(algorithmName, executionTime, nodesVisited, pathLength, memoryUsage, Rows, Cols, seed, startPosition, endPosition);
        }

        private void VisualizeInstantly()
        {
            Tile start = GetTile((int)startPosition.x, (int)startPosition.y);
            Tile end = GetTile((int)endPosition.x, (int)endPosition.y);

            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                FindPathInstantly(start, end, BFS.FindPath, "Breadth First Search");
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                FindPathInstantly(start, end, Dijkstra.FindPath, "Dijkstra");
            }
            else if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                FindPathInstantly(start, end, AStar.FindPath, "AStar");
            }
            else if(Input.GetKeyDown(KeyCode.Alpha4))
            {
                FindPathInstantly(start, end, GreedyBestFirstSearch.FindPath, "Greedy Best First Search");
            }
            else if(Input.GetKeyDown(KeyCode.Alpha5))
            {
                FindPathInstantly(start, end, DFS.FindPath, "Depth First Search");
            }
            else if(Input.GetKeyDown(KeyCode.Alpha6))
            {
                FindPathInstantly(start, end, BidirectionalSearch.FindPath, "Bidirectional Search");
            }
            else if(Input.GetKeyDown(KeyCode.Alpha7))
            {
                FindPathInstantly(start, end, LeeAlgorithm.FindPath, "Lee Algorithm");
            }
            else if(Input.GetKeyDown(KeyCode.Alpha8))
            {
                FindPathInstantly(start, end, DynamicProgrammingMaze.FindPath, "Dynamic Programming Maze");
            }

            else if(Input.GetKeyDown(KeyCode.Escape))
            {
                ResetGrid();
                start.SetColor(TileColor_Start);
                end.SetColor(TileColor_End);
            }
        }

        private IEnumerator FindPathSmoothly(Tile start, Tile end, PathFindingFunc pathFindingFunc, string algorithmName)
        {
            ResetGrid();

            List<IVisualStep> steps = new List<IVisualStep>();
            long executionTime;
            int nodesVisited;
            int pathLength;
            long memoryUsage;

            pathFindingFunc(this, start, end, steps, out executionTime, out nodesVisited, out pathLength, out memoryUsage);

            if(visualize)
            {
                foreach(var step in steps)
                {
                    step.Execute();
                    yield return new WaitForFixedUpdate();
                }
            }


            ShowInfo(algorithmName, executionTime, nodesVisited, pathLength, memoryUsage);

            WriteResultsToExcel(algorithmName, executionTime, nodesVisited, pathLength, memoryUsage, Rows, Cols, seed, startPosition, endPosition);
        }

        private void ShowInfo(string algorithmName, long executionTime, int nodesVisited, int pathLength, long memoryUsage)
        {
            algorithmNameText.text = $"Algorithm: {algorithmName}";
            executionTimeText.text = $"Execution Time: {executionTime} ms";
            nodesVisitedText.text = $"Nodes Visited: {nodesVisited}";
            pathLengthText.text = $"Path Length: {pathLength}";
            memoryUsageText.text = $"Memory Usage: {memoryUsage} bytes";

            Debug.Log($"Algorithm: {algorithmName}");
            Debug.Log($"Execution Time: {executionTime} ms");
            Debug.Log($"Nodes Visited: {nodesVisited}");
            Debug.Log($"Path Length: {pathLength}");
            Debug.Log($"Memory Usage: {memoryUsage} bytes");
        }

        private void VisualizeSmoothly()
        {
            Tile start = GetTile((int)startPosition.x, (int)startPosition.y);
            Tile end = GetTile((int)endPosition.x, (int)endPosition.y);

            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                StopPathCoroutine();
                _pathRoutine = FindPathSmoothly(start, end, BFS.FindPath, "Breadth First Search");
                StartCoroutine(_pathRoutine);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                StopPathCoroutine();
                _pathRoutine = FindPathSmoothly(start, end, Dijkstra.FindPath, "Dijkstra");
                StartCoroutine(_pathRoutine);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                StopPathCoroutine();
                _pathRoutine = FindPathSmoothly(start, end, AStar.FindPath, "AStar");
                StartCoroutine(_pathRoutine);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha4))
            {
                StopPathCoroutine();
                _pathRoutine = FindPathSmoothly(start, end, GreedyBestFirstSearch.FindPath, "Greedy Best First Search");
                StartCoroutine(_pathRoutine);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha5))
            {
                StopPathCoroutine();
                _pathRoutine = FindPathSmoothly(start, end, DFS.FindPath, "Depth First Search");
                StartCoroutine(_pathRoutine);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha6))
            {
                StopPathCoroutine();
                _pathRoutine = FindPathSmoothly(start, end, BidirectionalSearch.FindPath, "Bidirectional Search");
                StartCoroutine(_pathRoutine);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha7))
            {
                StopPathCoroutine();
                _pathRoutine = FindPathSmoothly(start, end, LeeAlgorithm.FindPath, "Lee Algorithm");
                StartCoroutine(_pathRoutine);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha8))
            {
                StopPathCoroutine();
                _pathRoutine = FindPathSmoothly(start, end, DynamicProgrammingMaze.FindPath, "Dynamic Programming Maze");
                StartCoroutine(_pathRoutine);
            }

            else if(Input.GetKeyDown(KeyCode.Escape))
            {
                StopPathCoroutine();
                ResetGrid();
                start.SetColor(TileColor_Start);
                end.SetColor(TileColor_End);
            }
        }

        private int CalculateNumberOfObstacles(int seed, int rows, int cols)
        {
            return (int)(0.1 * rows * cols) + (seed % 10);
        }

        private int CalculateNumberOfExpensiveTiles(int seed, int rows, int cols)
        {
            return (int)(0.05 * rows * cols) + (seed % 5);
        }

        public IEnumerator RunAllAlgorithmsCoroutine()
        {
            PathFindingFunc[] algorithms = {
                BFS.FindPath,
                Dijkstra.FindPath,
                AStar.FindPath,
                GreedyBestFirstSearch.FindPath,
                DFS.FindPath,
                BidirectionalSearch.FindPath,
                LeeAlgorithm.FindPath,
                DynamicProgrammingMaze.FindPath
            };
            string[] algorithmNames = {
                "Breadth First Search",
                "Dijkstra",
                "AStar",
                "Greedy Best First Search",
                "Depth First Search",
                "Bidirectional Search",
                "Lee Algorithm",
                "Dynamic Programming Maze"
            };

            for(int seed = 0; seed <= 15; seed++)
            {
                for(int i = 0; i < gridSizes.Length; i++)
                {
                    Rows = gridSizes[i];
                    Cols = gridSizes[i];
                    this.seed = seed;
                    CreateMap();

                    Tile start = GetTile((int)startPosition.x, (int)startPosition.y);
                    Tile end = GetTile((int)endPosition.x, (int)endPosition.y);

                    for(int j = 0; j < algorithms.Length; j++)
                    {
                        FindPathInstantly(start, end, algorithms[j], algorithmNames[j]);
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
        }

        public void RunAllAlgorithms()
        {
            PathFindingFunc[] algorithms = {
                BFS.FindPath,
                Dijkstra.FindPath,
                AStar.FindPath,
                GreedyBestFirstSearch.FindPath,
                DFS.FindPath,
                BidirectionalSearch.FindPath,
                LeeAlgorithm.FindPath,
                DynamicProgrammingMaze.FindPath
            };
            string[] algorithmNames = {
                "Breadth First Search",
                "Dijkstra",
                "AStar",
                "Greedy Best First Search",
                "Depth First Search",
                "Bidirectional Search",
                "Lee Algorithm",
                "Dynamic Programming Maze"
            };

            for(int seed = 0; seed <= 15; seed++)
            {
                for(int i = 0; i < gridSizes.Length; i++)
                {
                    Rows = gridSizes[i];
                    Cols = gridSizes[i];
                    this.seed = seed;
                    CreateMap();

                    Tile start = GetTile((int)startPosition.x, (int)startPosition.y);
                    Tile end = GetTile((int)endPosition.x, (int)endPosition.y);

                    for(int j = 0; j < algorithms.Length; j++)
                    {
                        FindPathInstantly(start, end, algorithms[j], algorithmNames[j]);
                    }
                }
            }
        }
    }
}
