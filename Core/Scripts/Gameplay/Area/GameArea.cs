using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using Insthync.UnityEditorUtils;
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    public enum GameAreaType
    {
        Radius,
        Square,
    }

    public enum GameAreaGroundFindingType
    {
        NavMesh,
        Raycast,
    }

    public enum GameAreaRandomPositionMode
    {
        ByOrder,
        FullyRandom,
    }

    public class GameArea : MonoBehaviour
    {
        protected static System.Random randomizer = new System.Random();
        protected static readonly RaycastHit[] s_findGroundRaycastHits = new RaycastHit[10];
#if UNITY_EDITOR
        [Header("Generic")]
        public Color gizmosColor = Color.magenta;
        public Color positionGizmosColor = Color.cyan;
        public float positionGizmosRadius = 0.1f;
        public DimensionType gizmosDimentionType = DimensionType.Dimension3D;
#endif
        public LayerMask groundLayerMask = -1;
        public GameAreaType type;
        [Header("Radius Area")]
        public float randomRadius = 5f;
        [Header("Square Area")]
        public float squareSizeX = 10f;
        public float squareSizeZ = 10f;
        public bool enableRotation = true;
        public GameAreaGroundFindingType groundFindingType = GameAreaGroundFindingType.Raycast;
        public float groundDetectionOffsets = 100f;
        public float findGroundUpOffsetsRate = 1f;
        public bool stillUseRandomedPositionIfGroundNotFound = false;
        public int randomPositionAmount = 20;
        public int randomPositionSeed = 123456;
        public float randomSpacing = 0.5f;
        public List<Vector3> randomedPosition3Ds = new List<Vector3>();
        public List<Vector3> randomedPosition2Ds = new List<Vector3>();
        public bool excludeFromAllAreaBaking = false;
        public GameAreaRandomPositionMode randomPositionMode = GameAreaRandomPositionMode.ByOrder;
#if UNITY_EDITOR
        [InspectorButton(nameof(BakeRandomPositions), "Bake Random Positions")]
        public bool btnBakeRandomPositions;
        [InspectorButton(nameof(BakeAllRandomPositions), "Bake Random Positions (All Areas)")]
        public bool btnBakeAllRandomPositions;
        [Header("Test tools")]
        public GameObject testSpawnPrefab;
        [InspectorButton(nameof(TestSpawn), "Test Spawn")]
        public bool btnTestSpawn;
#endif

        protected GameInstance CurrentGameInstance { get { return GameInstance.Singleton; } }

        protected IPhysicFunctions _physicFunctions;
        protected int _indexOfRandomPosition = 0;

        public List<Vector3> GetWorldRandomPositions()
        {
            return GetWorldRandomPositions(GameInstance.Singleton.DimensionType);
        }

        public List<Vector3> GetWorldRandomPositions(DimensionType type)
        {
            List<Vector3> result = new List<Vector3>();
            switch (type)
            {
                case DimensionType.Dimension2D:
                    foreach (Vector3 position in randomedPosition2Ds)
                    {
                        result.Add(LocalToWorldPosition(position));
                    }
                    break;
                default:
                    foreach (Vector3 position in randomedPosition3Ds)
                    {
                        result.Add(LocalToWorldPosition(position));
                    }
                    break;
            }
            return result;
        }

        public virtual bool GetRandomPosition(out Vector3 randomedPosition)
        {
            switch (GameInstance.Singleton.DimensionType)
            {
                case DimensionType.Dimension2D:
                    if (randomedPosition2Ds != null && randomedPosition2Ds.Count > 0)
                    {
                        int index = _indexOfRandomPosition;
                        if (randomPositionMode == GameAreaRandomPositionMode.FullyRandom)
                            index = Random.Range(0, randomedPosition2Ds.Count);
                        randomedPosition = GetRandomPosition2D(index);
                        if (randomPositionMode == GameAreaRandomPositionMode.ByOrder)
                        {
                            _indexOfRandomPosition++;
                            if (_indexOfRandomPosition >= randomedPosition2Ds.Count)
                                _indexOfRandomPosition = 0;
                        }
                        return true;
                    }
                    if (GetRandomedPosition2D(randomizer, out randomedPosition))
                    {
                        randomedPosition = LocalToWorldPosition(randomedPosition);
                        return true;
                    }
                    break;
                default:
                    if (randomedPosition3Ds != null && randomedPosition3Ds.Count > 0)
                    {
                        int index = _indexOfRandomPosition;
                        if (randomPositionMode == GameAreaRandomPositionMode.FullyRandom)
                            index = Random.Range(0, randomedPosition3Ds.Count);
                        randomedPosition = GetRandomPosition3D(index);
                        if (randomPositionMode == GameAreaRandomPositionMode.ByOrder)
                        {
                            _indexOfRandomPosition++;
                            if (_indexOfRandomPosition >= randomedPosition3Ds.Count)
                                _indexOfRandomPosition = 0;
                        }
                        return true;
                    }
                    if (GetRandomedPosition3D(randomizer, out randomedPosition, stillUseRandomedPositionIfGroundNotFound))
                    {
                        randomedPosition = LocalToWorldPosition(randomedPosition);
                        return true;
                    }
                    break;
            }
            randomedPosition = transform.position;
            return false;
        }

        public Vector3 GetRandomPosition2D(int index)
        {
            return LocalToWorldPosition(randomedPosition2Ds[index]);
        }

        public Vector3 GetRandomPosition3D(int index)
        {
            return LocalToWorldPosition(randomedPosition3Ds[index]);
        }

        public Vector3 LocalToWorldPosition(Vector3 localPosition)
        {
            if (enableRotation)
                return transform.TransformPoint(localPosition);
            else
                return transform.position + localPosition;
        }

        public virtual Quaternion GetRandomRotation()
        {
            if (GameInstance.Singleton.DimensionType == DimensionType.Dimension3D)
                return Quaternion.Euler(Vector3.up * Random.Range(0, 360));
            return Quaternion.identity;
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            if (enableRotation)
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            else
                Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one);
            Color handleCol = Handles.color;
            Color gizmosCol = Gizmos.color;
            Handles.color = gizmosColor;
            Gizmos.color = gizmosColor;
            switch (type)
            {
                case GameAreaType.Radius:
                    Vector3 upOrigin = Vector3.down * (1f - findGroundUpOffsetsRate) * groundDetectionOffsets;
                    Vector3 upDestination = Vector3.up * findGroundUpOffsetsRate * groundDetectionOffsets;
                    Handles.DrawWireDisc(transform.position + upOrigin, Vector3.up, randomRadius);
                    Handles.DrawWireDisc(transform.position + upDestination, Vector3.up, randomRadius);
                    Gizmos.DrawLine((Vector3.left * randomRadius) + upOrigin, (Vector3.left * randomRadius) + upDestination);
                    Gizmos.DrawLine((Vector3.right * randomRadius) + upOrigin, (Vector3.right * randomRadius) + upDestination);
                    Gizmos.DrawLine((Vector3.forward * randomRadius) + upOrigin, (Vector3.forward * randomRadius) + upDestination);
                    Gizmos.DrawLine((Vector3.back * randomRadius) + upOrigin, (Vector3.back * randomRadius) + upDestination);
                    break;
                case GameAreaType.Square:
                    Gizmos.DrawWireCube(Vector3.up * findGroundUpOffsetsRate * groundDetectionOffsets * 0.5f, new Vector3(squareSizeX, groundDetectionOffsets, squareSizeZ));
                    break;
            }
            Handles.color = handleCol;
            Gizmos.color = gizmosCol;
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (enableRotation)
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            else
                Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one);
            Color gizmosCol = Gizmos.color;
            Gizmos.color = positionGizmosColor;
            switch (gizmosDimentionType)
            {
                case DimensionType.Dimension3D:
                    for (int i = 0; i < randomedPosition3Ds.Count; ++i)
                    {
                        Gizmos.DrawWireSphere(randomedPosition3Ds[i], positionGizmosRadius);
                    }
                    break;
                case DimensionType.Dimension2D:
                    for (int i = 0; i < randomedPosition2Ds.Count; ++i)
                    {
                        Gizmos.DrawWireSphere(randomedPosition2Ds[i], positionGizmosRadius);
                    }
                    break;
            }
            Gizmos.color = gizmosCol;
        }
#endif

        public bool IsRandomingPositionEligible(List<Vector3> randomedPositions, Vector3 randomedPosition, float spacing)
        {
            if (spacing <= 0f)
                return true;
            for (int i = 0; i < randomedPositions.Count; ++i)
            {
                if (Vector3.Distance(randomedPositions[i], randomedPosition) < spacing)
                    return false;
            }
            return true;
        }

#if UNITY_EDITOR
        [ContextMenu("Bake Random Positions")]
        public void BakeRandomPositions()
        {
            System.Random random = new System.Random(randomPositionSeed);
            int i;
            Vector3 randomingPosition;
            randomedPosition3Ds.Clear();
            i = 0;
            int failedCount = 0;
            while (i < randomPositionAmount)
            {
                if (GetRandomedPosition3D(random, out randomingPosition, stillUseRandomedPositionIfGroundNotFound) && IsRandomingPositionEligible(randomedPosition3Ds, randomingPosition, randomSpacing))
                {
                    randomedPosition3Ds.Add(randomingPosition);
                    ++i;
                    continue;
                }
                ++failedCount;
                if (failedCount > 100)
                {
                    Debug.LogError("Unable to find grounded position (3D)", gameObject);
                    break;
                }
            }
            randomedPosition2Ds.Clear();
            i = 0;
            while (i < randomPositionAmount)
            {
                if (GetRandomedPosition2D(random, out randomingPosition) && IsRandomingPositionEligible(randomedPosition2Ds, randomingPosition, randomSpacing))
                {
                    randomedPosition2Ds.Add(randomingPosition);
                    ++i;
                    continue;
                }
                ++failedCount;
                if (failedCount > 100)
                {
                    Debug.LogError("Unable to find grounded position (2D)", gameObject);
                    break;
                }
            }
            EditorUtility.SetDirty(this);
        }

        [ContextMenu("Bake Random Positions (All Areas)")]
        public void BakeAllRandomPositions()
        {
            GameArea[] areas = FindObjectsOfType<GameArea>();
            for (int i = 0; i < areas.Length; ++i)
            {
                if (areas[i].excludeFromAllAreaBaking)
                    continue;
                areas[i].BakeRandomPositions();
            }
        }

        [ContextMenu("Test Spawn")]
        public void TestSpawn()
        {
            if (testSpawnPrefab == null)
                return;

            for (int i = 0; i < randomedPosition2Ds.Count; ++i)
            {
                Instantiate(testSpawnPrefab, GetRandomPosition2D(i), Quaternion.identity);
            }

            for (int i = 0; i < randomedPosition3Ds.Count; ++i)
            {
                Instantiate(testSpawnPrefab, GetRandomPosition3D(i), Quaternion.identity);
            }
        }
#endif

        public bool GetRandomedPosition3D(System.Random random, out Vector3 randomedPosition, bool stillUseRandomedPositionIfGroundNotFound = false)
        {
            Vector3 randomingPosition = Vector3.zero;
            switch (type)
            {
                case GameAreaType.Radius:
                    var preState = Random.state;
                    Random.InitState(random.Next());
                    Vector2 circle = Random.insideUnitCircle * randomRadius;
                    randomingPosition += new Vector3(circle.x, 0f, circle.y);
                    Random.state = preState;
                    break;
                case GameAreaType.Square:
                    randomingPosition += new Vector3(random.RandomFloat(-0.5f, 0.5f) * squareSizeX, 0f, random.RandomFloat(-0.5f, 0.5f) * squareSizeZ);
                    break;
            }
            if (FindGroundedPosition(enableRotation ? transform.TransformPoint(randomingPosition) : (randomingPosition + transform.position), groundDetectionOffsets, out randomedPosition))
            {
                if (enableRotation)
                    randomedPosition = transform.InverseTransformPoint(randomedPosition);
                else
                    randomedPosition -= transform.position.GetXZ();
                return true;
            }
            if (!stillUseRandomedPositionIfGroundNotFound)
                return false;
            randomedPosition = randomingPosition;
            return true;
        }

        public bool GetRandomedPosition2D(System.Random random, out Vector3 randomedPosition)
        {
            Vector3 randomingPosition = Vector3.zero;
            switch (type)
            {
                case GameAreaType.Radius:
                    var preState = Random.state;
                    Random.InitState(random.Next());
                    Vector2 circle = Random.insideUnitCircle * randomRadius;
                    randomingPosition += new Vector3(circle.x, circle.y, 0f);
                    Random.state = preState;
                    break;
                case GameAreaType.Square:
                    randomingPosition += new Vector3(random.RandomFloat(-0.5f, 0.5f) * squareSizeX, random.RandomFloat(-0.5f, 0.5f) * squareSizeZ, 0f);
                    break;
            }
            randomedPosition = randomingPosition;
            return true;
        }

        public bool FindGroundedPosition(Vector3 fromPosition, out Vector3 result)
        {
            return FindGroundedPosition(groundFindingType, GroundLayerMask, fromPosition, groundDetectionOffsets, out result, null, findGroundUpOffsetsRate);
        }

        public bool FindGroundedPosition(Vector3 fromPosition, float findDistance, out Vector3 result)
        {
            return FindGroundedPosition(groundFindingType, GroundLayerMask, fromPosition, findDistance, out result, null, findGroundUpOffsetsRate);
        }

        public static bool FindGroundedPosition(GameAreaGroundFindingType groundFindingType, int groundLayerMask, Vector3 fromPosition, float findDistance, out Vector3 result, Transform excludingObject = null, float findGroundUpOffsetsRate = 0.5f)
        {
            result = fromPosition;
            switch (groundFindingType)
            {
                case GameAreaGroundFindingType.NavMesh:
                    if (NavMesh.SamplePosition(fromPosition, out NavMeshHit navHit, findDistance, NavMesh.AllAreas))
                    {
                        result = navHit.position;
                        return true;
                    }
                    return false;
                default:
                    return PhysicUtils.FindGroundedPosition(fromPosition, s_findGroundRaycastHits, findDistance, groundLayerMask, out result, excludingObject, findGroundUpOffsetsRate);
            }
        }

        public virtual int GroundLayerMask { get { return groundLayerMask; } }
    }
}
