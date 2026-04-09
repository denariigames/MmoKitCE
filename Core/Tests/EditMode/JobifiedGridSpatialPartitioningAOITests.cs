using Insthync.SpatialPartitioningSystems;
using LiteNetLibManager;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MultiplayerARPG.Tests.EditMode
{
    [TestFixture]
    public class JobifiedGridSpatialPartitioningAOITests
    {
        private readonly List<GameObject> _createdObjects = new List<GameObject>();
        private GameInstance _previousGameInstance;

        [SetUp]
        public void SetUp()
        {
            _previousGameInstance = GameInstance.Singleton;
            SetGameInstanceSingleton(null);
            SetGameInstanceSingleton(CreateGameInstance(DimensionType.Dimension3D));
        }

        [TearDown]
        public void TearDown()
        {
            SetGameInstanceSingleton(_previousGameInstance);
            for (int i = _createdObjects.Count - 1; i >= 0; --i)
            {
                if (_createdObjects[i] != null)
                    UnityEngine.Object.DestroyImmediate(_createdObjects[i]);
            }
            _createdObjects.Clear();
        }

        [Test]
        public void PrepareSystem_CreatesSystem_WhenColliderExists()
        {
            (JobifiedGridSpatialPartitioningAOI aoi, _) = CreateAoiWithManager(
                true,
                new ServerSceneInfo
                {
                    isAddressable = false,
                    sceneName = "TestScene",
                });
            CreateColliderObject();

            aoi.PrepareSystem();

            Assert.That(aoi.IsSystemReady, Is.True);
        }

        [Test]
        public void PrepareSystem_DisposesExistingSystem_WhenSceneInfoMissing()
        {
            (JobifiedGridSpatialPartitioningAOI aoi, LiteNetLibGameManager manager) = CreateAoiWithManager(
                true,
                new ServerSceneInfo
                {
                    isAddressable = false,
                    sceneName = "TestScene",
                });
            CreateColliderObject();
            aoi.PrepareSystem();
            JobifiedGridSpatialPartitioningSystem oldSystem = GetPrivateField<JobifiedGridSpatialPartitioningSystem>(aoi, "_system");

            SetAutoPropertyBackingField(
                manager,
                typeof(LiteNetLibGameManager),
                "ServerSceneInfo",
                (ServerSceneInfo?)null);
            aoi.PrepareSystem();

            Assert.That(aoi.IsSystemReady, Is.False);
            Assert.That(IsCellToObjectsCreated(oldSystem), Is.False);
        }

        [Test]
        public void OnLoadSceneFinish_ReinitializesOnlyForAdditiveOnlineLoads()
        {
            (JobifiedGridSpatialPartitioningAOI aoi, _) = CreateAoiWithManager(
                true,
                new ServerSceneInfo
                {
                    isAddressable = false,
                    sceneName = "TestScene",
                });
            GameObject colliderObject = CreateColliderObject();
            aoi.PrepareSystem();
            Assert.That(aoi.IsSystemReady, Is.True);
            UnityEngine.Object.DestroyImmediate(colliderObject);

            InvokeOnLoadSceneFinish(aoi, false, true);
            Assert.That(aoi.IsSystemReady, Is.True, "Non-additive load should not reinitialize grid AOI.");

            InvokeOnLoadSceneFinish(aoi, true, true);
            Assert.That(aoi.IsSystemReady, Is.False, "Additive online load should reinitialize AOI and reflect missing bounds.");
        }

        [Test]
        public void PrepareSystem_MissingSystemWarningFlag_DeduplicatesAndResets()
        {
            (JobifiedGridSpatialPartitioningAOI aoi, _) = CreateAoiWithManager(
                true,
                new ServerSceneInfo
                {
                    isAddressable = false,
                    sceneName = "TestScene",
                });

            aoi.PrepareSystem();
            Assert.That(aoi.IsSystemReady, Is.False);
            Assert.That(GetPrivateField<bool>(aoi, "_didLogMissingSystemWarning"), Is.True);

            aoi.PrepareSystem();
            Assert.That(GetPrivateField<bool>(aoi, "_didLogMissingSystemWarning"), Is.True);

            CreateColliderObject();
            aoi.PrepareSystem();
            Assert.That(aoi.IsSystemReady, Is.True);
            Assert.That(GetPrivateField<bool>(aoi, "_didLogMissingSystemWarning"), Is.False);
        }

        [Test]
        public void IsWithinBox_UsesDimensionSpecificAxes()
        {
            Vector3 point = new Vector3(0.5f, 50f, 0.5f);
            Vector3 center = Vector3.zero;
            Vector3 extents = Vector3.one;

            bool result3D = InvokeIsWithinBox(point, center, extents, DimensionType.Dimension3D);
            bool result2D = InvokeIsWithinBox(point, center, extents, DimensionType.Dimension2D);

            Assert.That(result3D, Is.True);
            Assert.That(result2D, Is.False);
        }

        [Test]
        public void IsWithinSphere_UsesDimensionSpecificAxes()
        {
            Vector3 point = new Vector3(0.6f, 50f, 0.6f);
            Vector3 center = Vector3.zero;

            bool result3D = InvokeIsWithinSphere(point, center, 1f, DimensionType.Dimension3D);
            bool result2D = InvokeIsWithinSphere(point, center, 1f, DimensionType.Dimension2D);

            Assert.That(result3D, Is.True);
            Assert.That(result2D, Is.False);
        }

        private (JobifiedGridSpatialPartitioningAOI, LiteNetLibGameManager) CreateAoiWithManager(bool isServer, ServerSceneInfo? sceneInfo)
        {
            GameObject managerObject = new GameObject("TestNetworkManager");
            _createdObjects.Add(managerObject);
            LiteNetLibGameManager manager = managerObject.AddComponent<LiteNetLibGameManager>();
            SetAutoPropertyBackingField(manager, typeof(LiteNetLibManager.LiteNetLibManager), "IsServer", isServer);
            SetAutoPropertyBackingField(manager, typeof(LiteNetLibGameManager), "ServerSceneInfo", sceneInfo);

            GameObject aoiObject = new GameObject("TestAOI");
            _createdObjects.Add(aoiObject);
            JobifiedGridSpatialPartitioningAOI aoi = aoiObject.AddComponent<JobifiedGridSpatialPartitioningAOI>();
            SetAutoPropertyBackingField(aoi, typeof(BaseInterestManager), "Manager", manager);
            return (aoi, manager);
        }

        private GameObject CreateColliderObject()
        {
            GameObject colliderObject = new GameObject("TestCollider");
            _createdObjects.Add(colliderObject);
            colliderObject.AddComponent<BoxCollider>();
            return colliderObject;
        }

        private GameInstance CreateGameInstance(DimensionType dimensionType)
        {
            GameObject gameInstanceObject = new GameObject("TestGameInstance");
            _createdObjects.Add(gameInstanceObject);
            GameInstance gameInstance = gameInstanceObject.AddComponent<GameInstance>();
            SetPrivateField(gameInstance, typeof(GameInstance), "dimensionType", dimensionType);
            return gameInstance;
        }

        private static void SetGameInstanceSingleton(GameInstance value)
        {
            SetStaticAutoPropertyBackingField(typeof(GameInstance), "Singleton", value);
        }

        private static void InvokeOnLoadSceneFinish(JobifiedGridSpatialPartitioningAOI aoi, bool isAdditive, bool isOnline)
        {
            MethodInfo method = typeof(JobifiedGridSpatialPartitioningAOI).GetMethod(
                "OnLoadSceneFinish",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(method);
            method.Invoke(aoi, new object[] { "TestScene", isAdditive, isOnline, 1f });
        }

        private static bool InvokeIsWithinBox(Vector3 objectPosition, Vector3 center, Vector3 extents, DimensionType dimensionType)
        {
            MethodInfo method = typeof(JobifiedGridSpatialPartitioningAOI).GetMethod(
                "IsWithinBox",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new[] { typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(DimensionType) },
                null);
            Assert.IsNotNull(method);
            return (bool)method.Invoke(null, new object[] { objectPosition, center, extents, dimensionType });
        }

        private static bool InvokeIsWithinSphere(Vector3 objectPosition, Vector3 center, float radius, DimensionType dimensionType)
        {
            MethodInfo method = typeof(JobifiedGridSpatialPartitioningAOI).GetMethod(
                "IsWithinSphere",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new[] { typeof(Vector3), typeof(Vector3), typeof(float), typeof(DimensionType) },
                null);
            Assert.IsNotNull(method);
            return (bool)method.Invoke(null, new object[] { objectPosition, center, radius, dimensionType });
        }

        private static bool IsCellToObjectsCreated(JobifiedGridSpatialPartitioningSystem system)
        {
            FieldInfo fieldInfo = typeof(JobifiedGridSpatialPartitioningSystem).GetField(
                "_cellToObjects",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(fieldInfo);
            object map = fieldInfo.GetValue(system);
            PropertyInfo propertyInfo = map.GetType().GetProperty("IsCreated", BindingFlags.Instance | BindingFlags.Public);
            Assert.IsNotNull(propertyInfo);
            return (bool)propertyInfo.GetValue(map);
        }

        private static T GetPrivateField<T>(object target, string fieldName)
        {
            FieldInfo fieldInfo = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(fieldInfo, $"Unable to find field `{fieldName}` on `{target.GetType().Name}`.");
            return (T)fieldInfo.GetValue(target);
        }

        private static void SetPrivateField(object target, Type declaringType, string fieldName, object value)
        {
            FieldInfo fieldInfo = declaringType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(fieldInfo, $"Unable to find field `{fieldName}`.");
            fieldInfo.SetValue(target, value);
        }

        private static void SetAutoPropertyBackingField(object target, Type declaringType, string propertyName, object value)
        {
            FieldInfo fieldInfo = declaringType.GetField($"<{propertyName}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(fieldInfo, $"Unable to find auto-property backing field for `{propertyName}`.");
            fieldInfo.SetValue(target, value);
        }

        private static void SetStaticAutoPropertyBackingField(Type declaringType, string propertyName, object value)
        {
            FieldInfo fieldInfo = declaringType.GetField($"<{propertyName}>k__BackingField", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.IsNotNull(fieldInfo, $"Unable to find static auto-property backing field for `{propertyName}`.");
            fieldInfo.SetValue(null, value);
        }
    }
}
