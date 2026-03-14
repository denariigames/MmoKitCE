using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class LadderEntrance : MonoBehaviour
    {
        private static HashSet<LadderEntrance> s_entrances = new HashSet<LadderEntrance>();

        public Ladder ladder;
        public LadderEntranceType type = LadderEntranceType.Bottom;

        public Transform TipTransform
        {
            get
            {
                switch (type)
                {
                    case LadderEntranceType.Bottom:
                        return ladder.bottomTransform;
                    case LadderEntranceType.Top:
                        return ladder.topTransform;
                }
                return null;
            }
        }

        private void Awake()
        {
            if (ladder == null)
                ladder = GetComponentInParent<Ladder>();
            s_entrances.Add(this);
        }

        private void OnDestroy()
        {
            s_entrances.Remove(this);
        }

        public static LadderEntrance FindNearest(Vector3 origin)
        {
            float nearestDist = float.MaxValue;
            LadderEntrance nearestEntrance = null;
            foreach (LadderEntrance entrance in s_entrances)
            {
                float dist = Vector3.Distance(origin, entrance.transform.position);
                if (nearestEntrance == null || dist < nearestDist)
                {
                    nearestDist = dist;
                    nearestEntrance = entrance;
                }
            }
            return nearestEntrance;
        }

        private void OnTriggerStay(Collider other)
        {
            OnTrigger(other);
        }

        private void OnTrigger(Collider other)
        {
            if (!other.transform.root.TryGetComponent(out BaseCharacterEntity characterEntity) || characterEntity.LadderComponent == null)
            {
                return;
            }
            characterEntity.LadderComponent.TriggeredLadderEntry = this;
        }
    }
}