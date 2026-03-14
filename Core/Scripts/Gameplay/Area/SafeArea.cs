using UnityEngine;

namespace MultiplayerARPG
{
    public class SafeArea : MonoBehaviour, IUnHittable
    {
        public enum DetectionMode
        {
            ByComponent,
            ByInterface,
        }

        public DetectionMode detectionMode = DetectionMode.ByComponent;

        private void Awake()
        {
            gameObject.layer = PhysicLayers.IgnoreRaycast;
        }

        private void OnTriggerEnter(Collider other)
        {
            TriggerEnter(other.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TriggerEnter(other.gameObject);
        }

        private void TriggerEnter(GameObject other)
        {
            IDamageableEntity gameEntity = null;
            switch (detectionMode)
            {
                case DetectionMode.ByInterface:
                    gameEntity = other.GetComponent<IDamageableEntity>();
                    break;
                default:
                    gameEntity = other.GetComponent<DamageableEntity>();
                    break;
            }
            if (gameEntity.IsNull())
            {
                // Interface is not null but Unity's object can be null, so it have to be checked like this
                return;
            }
            gameEntity.SafeArea = this;
        }

        private void OnTriggerExit(Collider other)
        {
            TriggerExit(other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            TriggerExit(other.gameObject);
        }

        private void TriggerExit(GameObject other)
        {
            IDamageableEntity gameEntity = null;
            switch (detectionMode)
            {
                case DetectionMode.ByInterface:
                    gameEntity = other.GetComponent<IDamageableEntity>();
                    break;
                default:
                    gameEntity = other.GetComponent<DamageableEntity>();
                    break;
            }
            if (gameEntity.IsNull())
            {
                // Interface is not null but Unity's object can be null, so it have to be checked like this
                return;
            }
            if (gameEntity.SafeArea == this)
                gameEntity.SafeArea = null;
        }
    }
}
