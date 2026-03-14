using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class IgnoreColliderManager
    {
        protected readonly List<IgnoreColliderInfo> _ignoreColliderInfos = new List<IgnoreColliderInfo>();
        protected readonly List<IgnoreColliderInfo2D> _ignoreColliderInfo2Ds = new List<IgnoreColliderInfo2D>();
        protected Collider[] _colliders;
        protected Collider2D[] _collider2Ds;

        public IgnoreColliderManager(Collider[] colliders, Collider2D[] collider2Ds)
        {
            _colliders = colliders;
            _collider2Ds = collider2Ds;
        }

        public void ResetAndSetIgnoreColliders(EntityInfo instigator)
        {
            ResetIgnoreColliders();
            if (_colliders != null && _colliders.Length > 0 && instigator.TryGetEntity(out BaseGameEntity entity))
                SetIgnoreColliders(entity.EntityGameObject.GetComponentsInChildren<Collider>());
        }

        public void ResetAndSetIgnoreCollider2Ds(EntityInfo instigator)
        {
            ResetIgnoreCollider2Ds();
            if (_collider2Ds != null && _collider2Ds.Length > 0 && instigator.TryGetEntity(out BaseGameEntity entity))
                SetIgnoreColliders2D(entity.EntityGameObject.GetComponentsInChildren<Collider2D>());
        }

        public void SetIgnoreColliders(Collider[] colliders)
        {
            for (int i = 0; i < colliders.Length; ++i)
            {
                for (int j = 0; j < _colliders.Length; ++j)
                {
                    _ignoreColliderInfos.Add(new IgnoreColliderInfo()
                    {
                        defaultIgnoreStatus = Physics.GetIgnoreCollision(colliders[i], _colliders[j]),
                        colliderA = colliders[i],
                        colliderB = _colliders[j],
                    });
                    Physics.IgnoreCollision(colliders[i], _colliders[j], true);
                }
            }
        }

        public void SetIgnoreColliders2D(Collider2D[] colliders)
        {
            for (int i = 0; i < colliders.Length; ++i)
            {
                for (int j = 0; j < _collider2Ds.Length; ++j)
                {
                    _ignoreColliderInfo2Ds.Add(new IgnoreColliderInfo2D()
                    {
                        defaultIgnoreStatus = Physics2D.GetIgnoreCollision(colliders[i], _collider2Ds[j]),
                        colliderA = colliders[i],
                        colliderB = _collider2Ds[j],
                    });
                    Physics2D.IgnoreCollision(colliders[i], _collider2Ds[j], true);
                }
            }
        }

        public void ResetIgnoreColliders()
        {
            for (int i = 0; i < _ignoreColliderInfos.Count; ++i)
            {
                if (_ignoreColliderInfos[i].colliderA == null || _ignoreColliderInfos[i].colliderB == null)
                    continue;
                Physics.IgnoreCollision(_ignoreColliderInfos[i].colliderA, _ignoreColliderInfos[i].colliderB, _ignoreColliderInfos[i].defaultIgnoreStatus);
            }
            _ignoreColliderInfos.Clear();
        }

        public void ResetIgnoreCollider2Ds()
        {
            for (int i = 0; i < _ignoreColliderInfo2Ds.Count; ++i)
            {
                if (_ignoreColliderInfo2Ds[i].colliderA == null || _ignoreColliderInfo2Ds[i].colliderB == null)
                    continue;
                Physics2D.IgnoreCollision(_ignoreColliderInfo2Ds[i].colliderA, _ignoreColliderInfo2Ds[i].colliderB, _ignoreColliderInfo2Ds[i].defaultIgnoreStatus);
            }
            _ignoreColliderInfo2Ds.Clear();
        }
    }
}
