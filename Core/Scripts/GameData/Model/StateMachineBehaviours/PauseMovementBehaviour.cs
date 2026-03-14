using UnityEngine;

namespace MultiplayerARPG
{
    public class PauseMovementBehaviour : StateMachineBehaviour
    {
        public float minNormalizedTime = 0.15f;
        private BaseGameEntity _entity = null;
        private float _updatedNormalizedTime = 0f;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_entity == null)
                _entity = animator.GetComponentInParent<BaseGameEntity>();
            _entity.onCanMoveValidated += _entity_onCanMoveValidated;
            _updatedNormalizedTime = stateInfo.normalizedTime;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _updatedNormalizedTime = stateInfo.normalizedTime;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _entity.onCanMoveValidated -= _entity_onCanMoveValidated;
        }

        private void _entity_onCanMoveValidated(ref bool canMove)
        {
            if (_updatedNormalizedTime >= minNormalizedTime)
                canMove = false;
        }
    }
}