using UnityEngine;

namespace MultiplayerARPG
{
    public class NoConstructionArea : MonoBehaviour, IUnHittable
    {
        private void Awake()
        {
            gameObject.layer = PhysicLayers.IgnoreRaycast;
        }
    }
}
