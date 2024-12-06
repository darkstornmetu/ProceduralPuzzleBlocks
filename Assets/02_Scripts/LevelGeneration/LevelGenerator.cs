using System;
using System.Collections.Generic;
using System.Linq;
using ProceduralBlocks.Data;
using ProceduralBlocks.Data.Process;
using ProceduralBlocks.Grid;
using UnityEngine;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;


/// <summary>
/// Manages the generation of puzzle pieces within a grid-based level.
/// Handles grid initialization, piece generation based on difficulty and traversal methods,
/// and ensures all triangles are appropriately assigned to pieces.
/// </summary>
public class LevelGenerator : MonoBehaviour
{
    // List of triangles that have not yet been assigned to any piece.
    private readonly List<TriangleData> _unassignedTriangles = new ();

    // List of triangles that were unable to be assigned to a piece initially.
    private readonly List<TriangleData> _leftoverTriangles = new();

    // List of all generated puzzle pieces.
    private readonly List<PieceData> _pieces = new();
    
    // The grid structure containing all grid cells and their associated data.
    private Grid<GridCellData> _grid;
    
    // Constant defining the size of each grid cell.
    private const float _CELL_SIZE = 1;

    // Current number of pieces generated.
    private int _pieceCount;

    // Size of the grid (number of cells along one axis).
    private int _gridSize;
    
    // Minimum and maximum number of pieces based on difficulty level.
    private int _minPieceCount;
    private int _maxPieceCount;

    // Minimum number of triangles required for a piece.
    private readonly int _minPieceSize = 2;

    // Current difficulty level of the game.
    private DifficultyLevel _difficultyLevel;

    // The traversal method used for generating pieces.
    private TraversalMethod _traversalMethod;

    /// <summary>
    /// Initiates the level generation process with the specified difficulty level and traversal method.
    /// </summary>
    /// <param name="difficultyLevel">The difficulty level for the level generation (e.g., Easy, Medium, Hard).</param>
    /// <param name="method">The traversal method to use for generating pieces (e.g., Spiral, Horizontal, Vertical).</param>
    [Button]
    public void GenerateLevel(DifficultyLevel difficultyLevel, TraversalMethod method)
    {
        _difficultyLevel = difficultyLevel;
        _traversalMethod = method;
        
        InitializeParameters();
        CreateGrid();
        GeneratePieces();
        SaveLevel();
    }
    
    /// <summary>
    /// Initializes grid size and piece count parameters based on the selected difficulty level.
    /// </summary>
    private void InitializeParameters()
    {
        switch (_difficultyLevel)
        {
            case DifficultyLevel.Easy:
                _gridSize = 4; 
                _minPieceCount = 5;
                _maxPieceCount = 7;
                break;
            case DifficultyLevel.Medium:
                _gridSize = 5;
                _minPieceCount = 6;
                _maxPieceCount = 9;
                break;
            case DifficultyLevel.Hard:
                _gridSize = 6;
                _minPieceCount = 8;
                _maxPieceCount = 13;
                break;
        }
    }

    /// <summary>
    /// Creates the grid based on the initialized grid size.
    /// Populates the grid with grid cells and collects all triangles as unassigned.
    /// </summary>
    private void CreateGrid()
    {
        _grid = new Grid<GridCellData>(_gridSize, _gridSize);
        _unassignedTriangles.Clear();

        for (int x = 0; x < _gridSize; x++)
        {
            for (int y = 0; y < _gridSize; y++)
            {
                Vector2Int coordinates = new Vector2Int(x, y);
                GridCellData cellData = new GridCellData(coordinates);
                
                _grid.SetItem(coordinates, cellData);
                _unassignedTriangles.AddRange(cellData.Triangles);
            }
        }
    }

    /// <summary>
    /// Generates puzzle pieces based on the grid and difficulty settings.
    /// Determines the number and size of each piece and initiates their creation.
    /// </summary>
    private void GeneratePieces()
    {
        _pieces.Clear();

        // Calculate total number of triangles assuming 4 triangles per grid cell.
        int totalTriangles = _gridSize * _gridSize * 4;
        
        // Determine the desired number of pieces within the specified range.
        int desiredPieceCount = Random.Range(_minPieceCount, _maxPieceCount);
        
        // Calculate the base size for each piece and distribute any remainder.
        int basePieceSize = totalTriangles / desiredPieceCount;
        int remainder = totalTriangles % desiredPieceCount;
        
        List<int> pieceSizes = new List<int>();
        for (int i = 0; i < desiredPieceCount; i++)
        {
            int pieceSize = basePieceSize;
            if (i < remainder)
            {
                pieceSize += 1; // Distribute the remainder among the first few pieces.
            }
            pieceSizes.Add(pieceSize);
        }
        
        // Proceed to generate pieces using the selected traversal method.
        GeneratePiecesWithTraversal(pieceSizes);
    }
    
    /// <summary>
    /// Generates puzzle pieces by traversing the grid and assigning triangles to each piece.
    /// Ensures that each piece is connected and meets the minimum piece size requirement.
    /// </summary>
    /// <param name="pieceSizes">A list indicating the number of triangles each piece should contain.</param>
    private void GeneratePiecesWithTraversal(List<int> pieceSizes)
    {
        // Retrieve the appropriate traversal method instance.
        var traversalMethod = GetTraversalMethod(_traversalMethod);
        
        // Select a random starting corner for the traversal.
        var startingCorner = GetRandomStartingCorner();
        
        // Get an enumerator to iterate through unassigned triangles in traversal order.
        var unassignedTrianglesEnumerator = traversalMethod.GetUnassignedTrianglesInOrder(startingCorner).
            GetEnumerator();

        // Define the maximum number of attempts to generate each piece to prevent infinite loops.
        int maxAttempt = 5;

        // Iterate through each desired piece size.
        foreach (int pieceSize in pieceSizes.ToList())
        {
            int attempts = 0;
            bool pieceGenerated = false;
            
            // Attempt to generate the piece within the allowed number of attempts.
            while (attempts < maxAttempt && !pieceGenerated)
            {
                // Create a new piece with a unique ID and color.
                PieceData piece = new PieceData(_pieces.Count, GetNextPieceColor(_pieces.Count));
                
                // Try generating a connected piece of the specified size.
                if (TryGenerateConnectedPiece(pieceSize, unassignedTrianglesEnumerator, out var pieceTriangles))
                {
                    // Finalize the piece's data by localizing its triangles.
                    CreatePieceFromConnectedTriangles(piece, pieceTriangles);
                    
                    // Add the successfully generated piece to the list of pieces.
                    _pieces.Add(piece);
                    pieceGenerated = true;
                    
                    // If the generated piece is smaller than desired, redistribute the deficit.
                    if (pieceTriangles.Count < pieceSize)
                    {
                        int deficit = pieceSize - pieceTriangles.Count;
                        int remainingPieces = pieceSizes.Count - _pieces.Count - 1;

                        if (remainingPieces > 0)
                        {
                            int adjustmentPerPiece = deficit / remainingPieces;
                            for (int i = _pieces.Count + 1; i < pieceSizes.Count; i++)
                            {
                                pieceSizes[i] += adjustmentPerPiece;
                            }
                        }
                    }
                }
                else
                {
                    // Increment the attempt counter if piece generation failed.
                    attempts++;
                }
            }
            
        }
        
        // Assign any remaining triangles that couldn't be allocated during piece generation.
        if (_leftoverTriangles.Count > 0 || _unassignedTriangles.Count > 0)
        {
            AssignRemainingTriangles();
        }
    }

    /// <summary>
    /// Attempts to generate a connected puzzle piece of the specified size by traversing through unassigned triangles.
    /// </summary>
    /// <param name="pieceSize">The desired number of triangles for the piece.</param>
    /// <param name="unassignedTrianglesEnumerator">Enumerator to iterate through unassigned triangles in traversal order.</param>
    /// <param name="generatedTriangles">Outputs the list of triangles assigned to the generated piece.</param>
    /// <returns>True if a connected piece meeting the minimum size was generated; otherwise, false.</returns>
    private bool TryGenerateConnectedPiece(int pieceSize,
        IEnumerator<TriangleData> unassignedTrianglesEnumerator, out List<TriangleData> generatedTriangles)
    {
        generatedTriangles = new();
        Queue<TriangleData> searchQueue = new();
        
        // Find the next unassigned triangle to start building the piece.
        TriangleData startingTriangle = null;
        while (unassignedTrianglesEnumerator.MoveNext())
        {
            startingTriangle = unassignedTrianglesEnumerator.Current;
            if (_unassignedTriangles.Contains(startingTriangle))
                break;
            else
                startingTriangle = null;
        }

        // Return false if no starting triangle is found.
        if (startingTriangle == null)
            return false;
        
        // Assign the starting triangle to the piece and enqueue it for traversal.
        startingTriangle.IsAssigned = true;
        _unassignedTriangles.Remove(startingTriangle);
        searchQueue.Enqueue(startingTriangle);
        generatedTriangles.Add(startingTriangle);
        
        int trianglesAdded = 1;

        // Perform a breadth-first search to assign connected triangles to the piece.
        while (trianglesAdded < pieceSize && searchQueue.Count > 0)
        {
            var currentTriangle = searchQueue.Dequeue();

            var neighbours = GetAllPossibleTriangleNeighbours(currentTriangle);

            // Shuffle the list of neighbors to introduce randomness in piece formation.
            neighbours = ShuffleList(neighbours);

            foreach (var triangle in neighbours)
            {
                if (trianglesAdded >= pieceSize) break;
                
                if (triangle.IsAssigned) continue;

                // Assign the neighbor triangle to the piece and enqueue it.
                triangle.IsAssigned = true;
                _unassignedTriangles.Remove(triangle);
                searchQueue.Enqueue(triangle);
                generatedTriangles.Add(triangle);
                trianglesAdded++;
            }
        }

        // Check if the generated piece meets the minimum size requirement.
        if (generatedTriangles.Count >= _minPieceSize)
            return true;
        
        // If not, add the generated triangles to leftover triangles for reassignment.
        _leftoverTriangles.AddRange(generatedTriangles);
        return false;
    }
    
    /// <summary>
    /// Finalizes the data for a newly generated piece by calculating its position offset and localizing its triangles.
    /// </summary>
    /// <param name="piece">The puzzle piece to be created.</param>
    /// <param name="pieceTriangles">The list of triangles assigned to this piece.</param>
    private void CreatePieceFromConnectedTriangles(PieceData piece, List<TriangleData> pieceTriangles)
    {
        // Determine the minimum X and Y coordinates to calculate the piece's offset.
        int minX = pieceTriangles.Min(t => t.Coordinates.x);
        int minY = pieceTriangles.Min(t => t.Coordinates.y);
        Vector2Int offset = new Vector2Int(minX, minY);

        // Set the piece's offset based on the calculated minimum coordinates.
        piece.Offset = offset;
        List<TriangleData> localizedTriangles = new();

        // Localize each triangle's coordinates relative to the piece's offset.
        foreach (var triangle in pieceTriangles)
        {
            TriangleData localizedTriangle =
                new TriangleData(
                    triangle.Coordinates - offset,
                    triangle.TriangleIndex,
                    triangle.IsAssigned);

            // Assign the piece ID to both original and localized triangles.
            triangle.PieceID = localizedTriangle.PieceID = piece.PieceID;
            localizedTriangles.Add(localizedTriangle);
        }
        
        // Assign the list of localized triangles to the piece.
        piece.Triangles = localizedTriangles;
    }
    
    /// <summary>
    /// Assigns any remaining triangles that couldn't be allocated during the initial piece generation process.
    /// </summary>
    private void AssignRemainingTriangles()
    {
        // Reset the assignment status of leftover triangles.
        _leftoverTriangles.ForEach(t => t.IsAssigned = false);
        
        // Add all unassigned triangles to the leftover list for reassignment.
        _leftoverTriangles.AddRange(_unassignedTriangles);
        _unassignedTriangles.Clear();
        
        // Iterate through the leftover triangles and assign them to adjacent pieces.
        while (_leftoverTriangles.Count > 0)
        {
            foreach (var triangle in _leftoverTriangles.ToList())
            {
                // Find all adjacent triangles that are already assigned to pieces.
                List<TriangleData> adjacentAssignedTriangles = 
                    GetAllPossibleTriangleNeighbours(triangle).Where(t => t.IsAssigned).ToList();

                if (adjacentAssignedTriangles.Count <= 0) continue;
                
                // Collect unique piece IDs from the adjacent assigned triangles.
                var pieceIDs = 
                    adjacentAssignedTriangles.Select(t => t.PieceID).Distinct().ToList();
                
                // Select the piece with the lowest ID for assignment.
                int targetPieceID = pieceIDs.Min();
                
                // Retrieve the target piece based on the selected piece ID.
                PieceData targetPiece = _pieces.First(p => p.PieceID == targetPieceID);
                
                // Create a new localized triangle and assign it to the target piece.
                TriangleData newLocalizedTriangle = new TriangleData
                (
                    coordinates: triangle.Coordinates - targetPiece.Offset,
                    triangleIndex: triangle.TriangleIndex,
                    isAssigned: true
                )
                {
                    PieceID = targetPiece.PieceID
                };

                // Update the triangle's assignment status and associate it with the piece.
                triangle.IsAssigned = true;
                triangle.PieceID = targetPiece.PieceID;
                targetPiece.Triangles.Add(newLocalizedTriangle);
                
                // Remove the triangle from the leftover list as it's now assigned.
                _leftoverTriangles.Remove(triangle);
            }
        }
    }

    /// <summary>
    /// Retrieves all neighboring triangles of a given triangle, both within the same grid cell and in adjacent cells.
    /// </summary>
    /// <param name="triangle">The triangle for which neighbors are to be found.</param>
    /// <returns>A list of neighboring TriangleData objects.</returns>
    private List<TriangleData> GetAllPossibleTriangleNeighbours(TriangleData triangle)
    {
        List<TriangleData> neighbours = new List<TriangleData>();

        // Retrieve the current grid cell containing the triangle.
        var currentCell = _grid.GetItem(triangle.Coordinates);
        
        // Add neighbors within the same cell based on TriangleIndex.
        neighbours.AddRange(GetNeighboursInsideCell(triangle));
        
        // Attempt to find a neighbor in an adjacent cell.
        if (TryGetNeighbourOutsideCell(triangle, out var outsideNeighbour))
            neighbours.Add(outsideNeighbour);

        return neighbours;
        
        // Local function to get neighboring triangles within the same grid cell.
        List<TriangleData> GetNeighboursInsideCell(TriangleData data)
        {
            return new List<TriangleData>
            {
                currentCell.Triangles[Mod(data.TriangleIndex - 1, 4)],
                currentCell.Triangles[Mod(data.TriangleIndex + 1, 4)]
            };
        }

        // Local function to attempt retrieving a neighboring triangle from an adjacent grid cell.
        bool TryGetNeighbourOutsideCell(TriangleData data, out TriangleData outside)
        {
            outside = default;
            Vector2Int cellDirection = GetDirectionFromIndex(data.TriangleIndex);
                
            // Check if the adjacent cell exists in the grid.
            if (_grid.TryGetItem(data.Coordinates + cellDirection, out GridCellData cell))
            {
                // Retrieve the adjacent triangle based on the current TriangleIndex.
                outside = cell.Triangles[GetAdjacentIndex(data.TriangleIndex)];
                return true;
            }

            return false;

            // Determines the direction vector based on the TriangleIndex.
            Vector2Int GetDirectionFromIndex(int index)
            {
                return index switch
                {
                    0 => Vector2Int.left,
                    1 => Vector2Int.up,
                    2 => Vector2Int.right,
                    3 => Vector2Int.down,
                    _ => Vector2Int.zero
                };
            }

            // Calculates the adjacent TriangleIndex for neighboring triangles.
            int GetAdjacentIndex(int index)
            {
                return Mod(index + 2, 4);
            }
        }
    }

    /// <summary>
    /// Selects a random corner of the grid to serve as the starting point for traversal.
    /// </summary>
    /// <returns>A Vector2Int representing the coordinates of the selected corner.</returns>
    private Vector2Int GetRandomStartingCorner()
    {
        int gridSize = _gridSize - 1;
        int cornerIndex = Random.Range(0, 4);
        return cornerIndex switch
        {
            0 => new Vector2Int(0, 0),            // Top-left
            1 => new Vector2Int(gridSize, 0),     // Top-right
            2 => new Vector2Int(0, gridSize),     // Bottom-left
            3 => new Vector2Int(gridSize, gridSize), // Bottom-right
            _ => new Vector2Int(0, 0),
        };
    }
    
    /// <summary>
    /// Retrieves the appropriate traversal method instance based on the specified TraversalMethod enumeration.
    /// </summary>
    /// <param name="method">The traversal method to be used.</param>
    /// <returns>An instance of TriangleTraversal corresponding to the specified method.</returns>
    private TriangleTraversal GetTraversalMethod(TraversalMethod method)
    {
        return method switch
        { 
           TraversalMethod.ClockwiseSpiral => new ClockwiseSpiralTraversal(_grid, _gridSize, _unassignedTriangles),
           TraversalMethod.CounterClockwiseSpiral => new CounterClockwiseSpiralTraversal(_grid, _gridSize, _unassignedTriangles),
           TraversalMethod.Horizontal => new HorizontalTraversal(_grid, _gridSize, _unassignedTriangles),
           TraversalMethod.Vertical => new VerticalTraversal(_grid, _gridSize, _unassignedTriangles),
            _ => new ClockwiseSpiralTraversal(_grid, _gridSize, _unassignedTriangles)
        };
    }
    
    /// <summary>
    /// Determines the color to assign to a puzzle piece based on its index.
    /// Cycles through available colors to ensure diversity.
    /// </summary>
    /// <param name="pieceIndex">The index of the piece for which a color is being assigned.</param>
    /// <returns>A PieceColor enumeration value representing the assigned color.</returns>
    private PieceColor GetNextPieceColor(int pieceIndex)
    {
       return (PieceColor)Mod(pieceIndex, Enum.GetValues(typeof(PieceColor)).Length);
    }
    
    /// <summary>
    /// Saves the generated level data by serializing it and storing it using the LevelSerializer.
    /// </summary>
    private void SaveLevel()
    {
        LevelData levelData = new LevelData(
            difficultyLevel: _difficultyLevel,
            gridSize: _gridSize,
            cellSize: _CELL_SIZE,
            pieces: _pieces);
        
        LevelSerializer.SaveLevel(levelData);
    }
    
    /// <summary>
    /// Randomizes the order of elements in a list using the Fisher-Yates shuffle algorithm.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list to be shuffled.</param>
    /// <returns>A new list with elements shuffled in random order.</returns>
    private List<T> ShuffleList<T>(List<T> list)
    {
        // Implementing Fisher-Yates shuffle algorithm
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
        return list;
    }

    /// <summary>
    /// Calculates the modulus of two integers, ensuring a non-negative result.
    /// Useful for wrapping indices within a specified range.
    /// </summary>
    /// <param name="x">The dividend.</param>
    /// <param name="m">The divisor.</param>
    /// <returns>The modulus result.</returns>
    private int Mod(int x, int m) 
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }
}
