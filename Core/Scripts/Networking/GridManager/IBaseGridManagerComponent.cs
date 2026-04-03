using UnityEngine;

namespace MultiplayerARPG
{
    public interface IBaseGridManagerComponenent
    {
        bool IsDisabled { get; }
        ushort CellSize { get; }
        int GridSize { get; }
        void SetupDynamicGrid();

        byte GetCellId(Vector3 pos);

        void GetCell(byte id, out GridCell gridCell);

        Vector3 GetCellLocalPosition(byte cellId, Vector3 position);

        Vector3 GetWorldPosition(byte cellId, Vector3 position);
    }
    static class DefaultGridManagerComponentExtensions
    {
        public static IBaseGridManagerComponenent GetGridManager()
        {
            return BaseGameNetworkManager.Singleton.GridManager;
        }
        public static void GetCell(this IBaseGridManagerComponenent gridManager, byte id, out GridCell gridCell)
        {
            gridManager.GetCell(id, out gridCell);
        }

        public static Vector3 GetCellLocalPosition(this IBaseGridManagerComponenent gridManager, byte cellId, Vector3 position)
        {
           return BaseGameNetworkManager.Singleton.GridManager.GetCellLocalPosition(cellId, position);
        }

    }
}

