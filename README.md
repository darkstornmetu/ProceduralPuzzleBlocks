# Procedural Puzzle Blocks Generator

![PuzzleBlock](https://github.com/user-attachments/assets/42ac92f7-b75c-4fae-8c53-17e530768f7c)

This project implements a procedural generation algorithm to create unique and engaging puzzle levels in a grid-based game. It ensures variability and complexity using flexible traversal strategies, randomized patterns, and contiguous piece generation. The procedural generation algorithm is managed by the `LevelGenerator` class, responsible for creating puzzle pieces within a grid-based level. The process begins with initializing parameters like grid size and piece count, determined by the selected difficulty level. 

### Key Features:
- Uses several traversal methods like **Clockwise Spiral**, **Horizontal**, and **Vertical** to generate diverse puzzle shapes.
- Balances triangle distribution across pieces while ensuring contiguity using **Breadth-First Search (BFS)**.
- Ensures randomness, solveability and unique configurations.

# High-Level Constructs

### 1. Grid System
- **Grid Structure**: Represents the play area using a `Grid<GridCellData>` structure.
- **GridCellData**: Encapsulates cell data, including coordinates and contained triangles.

### 2. Triangle and Piece Data
- **TriangleData**: Represents individual triangles, holding attributes like coordinates, assignment status, and piece ID.
- **PieceData**: Defines a puzzle piece, containing its ID, color, position offset, and localized triangles.

### 3. Traversal Methods
Implements several traversal strategies:
- **Clockwise Spiral**
- **Counterclockwise Spiral**
- **Horizontal**
- **Vertical**

These methods provide flexibility in how triangles are assigned to pieces.

# Algorithm Workflow

### 1. Initialization (`InitializeParameters`)
- Configures parameters based on difficulty level (Easy, Medium, Hard).
- Defines grid size and piece count range.

### 2. Grid Creation (`CreateGrid`)
- Constructs the grid by populating it with `GridCellData` objects.
- Initializes triangles as unassigned.

### 3. Piece Determination (`GeneratePieces`)
- Calculates total triangles and evenly distributes them across pieces.
- Handles remainders by incrementally adjusting piece sizes.

### 4. Piece Generation (`GeneratePiecesWithTraversal`)
- Selects a random starting corner for traversal.
- Generates connected pieces using the selected traversal method.
- Ensures contiguity via **BFS** in `TryGenerateConnectedPiece`.

### 5. Handling Leftover Triangles (`AssignRemainingTriangles`)
- Reassigns unassigned triangles to adjacent pieces, ensuring no orphaned triangles.

### 6. Saving and Serialization (`SaveLevel`)
- Compiles level data, including grid size, difficulty level, and piece configurations.
- Saves the level using the `LevelSerializer`.

# Example Visuals
### Gameplay
https://github.com/user-attachments/assets/a52be82c-5c42-4b58-b5ff-d051bc856f09
