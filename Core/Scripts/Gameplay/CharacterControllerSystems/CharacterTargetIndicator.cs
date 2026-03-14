using UnityEngine;

namespace MultiplayerARPG
{
    public class CharacterTargetIndicator : MonoBehaviour
    {
        public enum FollowTargetMode
        {
            FollowPosition,
            FollowTopBound,
        }
        public GameObject indicatorPrefab;
        public FollowTargetMode followTargetMode;
        public Vector3 offsets;
        public bool followTargetRotation;

        private GameObject indicatorObject;
        private BaseGameEntity currentTarget;
        private Collider2D[] colliders2D;
        private Collider[] colliders;

        private void Start()
        {
            indicatorObject = Instantiate(indicatorPrefab);
        }

        private void OnDestroy()
        {
            indicatorPrefab = null;
            indicatorObject = null;
            currentTarget = null;
            colliders2D.Nullify();
            colliders.Nullify();
        }

        private void LateUpdate()
        {
            if (currentTarget != GameInstance.PlayingCharacterEntity.GetTargetEntity())
            {
                currentTarget = GameInstance.PlayingCharacterEntity.GetTargetEntity();
                if (currentTarget != null)
                {
                    colliders2D = currentTarget.GetComponentsInChildren<Collider2D>();
                    colliders = currentTarget.GetComponentsInChildren<Collider>();
                }
            }

            if (currentTarget != null)
            {
                if (currentTarget is DamageableEntity damageableEntity && damageableEntity.IsDead())
                {
                    indicatorObject.gameObject.SetActive(false);
                    return;
                }
                float xPosition = currentTarget.transform.position.x;
                float yPosition = currentTarget.transform.position.y;
                float zPosition = currentTarget.transform.position.z;
                if (followTargetMode == FollowTargetMode.FollowTopBound)
                {
                    if (GameInstance.Singleton.DimensionType == DimensionType.Dimension2D)
                    {
                        if (colliders2D != null && colliders2D.Length > 0)
                        {
                            for (int i = 0; i < colliders2D.Length; ++i)
                            {
                                if (yPosition < colliders2D[i].bounds.center.y + colliders2D[i].bounds.extents.y)
                                    yPosition = colliders2D[i].bounds.center.y + colliders2D[i].bounds.extents.y;
                            }
                        }
                    }
                    else
                    {
                        if (colliders != null && colliders.Length > 0)
                        {
                            for (int i = 0; i < colliders.Length; ++i)
                            {
                                if (yPosition < colliders[i].bounds.center.y + colliders[i].bounds.extents.y)
                                    yPosition = colliders[i].bounds.center.y + colliders[i].bounds.extents.y;
                            }
                        }
                    }
                }
                indicatorObject.transform.position = new Vector3(xPosition, yPosition, zPosition) + offsets;
                if (followTargetRotation)
                    indicatorObject.transform.rotation = currentTarget.transform.rotation;
                indicatorObject.gameObject.SetActive(true);
            }
            else
            {
                indicatorObject.gameObject.SetActive(false);
            }
        }
    }
}
