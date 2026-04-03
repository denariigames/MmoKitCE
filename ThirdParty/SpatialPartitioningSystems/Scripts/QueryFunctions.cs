using Unity.Mathematics;

namespace Insthync.SpatialPartitioningSystems
{
    public static class QueryFunctions
    {
        public static int3 GetCellIndex(float3 position, float3 worldMin, float cellSize,
            bool disableXAxis, bool disableYAxis, bool disableZAxis)
        {
            float3 relative = position - worldMin;
            return new int3(
                disableXAxis ? 0 : (int)(relative.x / cellSize),
                disableYAxis ? 0 : (int)(relative.y / cellSize),
                disableZAxis ? 0 : (int)(relative.z / cellSize)
            );
        }

        public static int GetFlatIndex(int3 index, int gridSizeX, int gridSizeY)
        {
            // Converts a 3D grid index (x, y, z) into a 1D array index.
            // This is useful for flattening 3D data into a 1D array.
            //
            // Example:
            // Grid size: 3 x 3 x 3 (X: width, Y: height, Z: depth)
            //
            // Imagine the 3D grid as layers (z-layers) of 2D grids:
            //
            //     ------------------------------------------------
            //     |    z  =  0    |    z  =  1   |    z  =  2    |
            //     ------------------------------------------------
            //     |  x0  x1  x2   |  x0  x1  x2  |  x0  x1  x2   |
            //     |   |   |   |   |   |   |   |  |   |   |   |   |
            // y0 -|   0   1   2   |   9  10  11  |  18  19  20   |
            // y1 -|   3   4   5   |  12  13  14  |  21  22  23   |
            // y2 -|   6   7   8   |  15  16  17  |  24  25  26   |
            //     ------------------------------------------------
            //
            // To get the flat index of position (x=2, y=1, z=2):
            //
            // index.x = 2
            // index.y = 1
            // index.z = 2
            //
            // Calculation:
            // flatIndex = x + gridSizeX * (y + gridSizeY * z)
            //           = 2 + 3 * (1 + 3 * 2)
            //           = 2 + 3 * (1 + 6)
            //           = 2 + 3 * 7
            //           = 2 + 21
            //           = 23
            //
            // So (2, 1, 2) maps to index 23 in the 1D array.
            return index.x + gridSizeX * (index.y + gridSizeY * index.z);
        }
    }
}