using UnityEngine;
using UnityEngine.Tilemaps;

namespace MultiplayerARPG
{
    public class BuildingMaterialBuildModeHandler : MonoBehaviour
    {
        private BuildingMaterial buildingMaterial;

        public void Setup(BuildingMaterial buildingMaterial)
        {
            this.buildingMaterial = buildingMaterial;
        }

        private void OnDestroy()
        {
            buildingMaterial = null;
        }

        private void OnTriggerStay(Collider other)
        {
            AddTriggered(other.gameObject, other, null);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            AddTriggered(other.gameObject, null, other);
        }

        private void AddTriggered(GameObject other, Collider collider, Collider2D collider2D)
        {
            if (!ValidateTriggerLayer(other) || SameBuildingAreaTransform(other))
                return;

            if (buildingMaterial.BuildingEntity.AddTriggeredComponent(other.GetComponent<NoConstructionArea>()))
                return;

            if (buildingMaterial.BuildingEntity.AddTriggeredComponent(other.GetComponent<TilemapCollider2D>()))
                return;

            BuildingArea buildingArea = other.GetComponent<BuildingArea>();
            if (buildingArea != null)
                return;

            bool foundMaterial = other.GetComponent<BuildingMaterial>() != null;
            IGameEntity gameEntity = other.GetComponent<IGameEntity>();
            if (!foundMaterial && !gameEntity.IsNull())
            {
                buildingMaterial.BuildingEntity.AddTriggeredEntity(gameEntity.Entity);
                return;
            }

            bool isMaterialOrNonTrigger = false;
            if (!isMaterialOrNonTrigger && collider != null && !collider.isTrigger && buildingMaterial.CacheCollider.ColliderIntersect(collider, buildingMaterial.boundsSizeRateWhilePlacing))
                isMaterialOrNonTrigger = true;
            if (!isMaterialOrNonTrigger && collider2D != null && !collider2D.isTrigger && buildingMaterial.CacheCollider2D.ColliderIntersect(collider2D, buildingMaterial.boundsSizeRateWhilePlacing))
                isMaterialOrNonTrigger = true;

            if (isMaterialOrNonTrigger)
                buildingMaterial.BuildingEntity.AddTriggeredGameObject(other);
        }

        private bool SameBuildingAreaTransform(GameObject other)
        {
            return buildingMaterial.BuildingEntity.BuildingArea != null && buildingMaterial.BuildingEntity.BuildingArea.transform == other.transform;
        }

        public bool ValidateTriggerLayer(GameObject gameObject)
        {
            return gameObject.layer != PhysicLayers.TransparentFX;
        }
    }
}
