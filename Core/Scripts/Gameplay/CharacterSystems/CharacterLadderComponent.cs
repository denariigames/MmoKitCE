using Cysharp.Threading.Tasks;
using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    [DisallowMultipleComponent]
    public class CharacterLadderComponent : BaseNetworkedGameEntityComponent<BaseCharacterEntity>
    {
        protected LadderEntrance _triggeredLadderEntry;
        protected int _lastTriggeredLadderEntryFrame;
        /// <summary>
        /// Triggered ladder entry, will decide to enter the ladder or not later
        /// </summary>
        public LadderEntrance TriggeredLadderEntry
        {
            get
            {
                if (Time.frameCount > _lastTriggeredLadderEntryFrame)
                    _triggeredLadderEntry = null;
                return _triggeredLadderEntry;
            }
            set
            {
                _lastTriggeredLadderEntryFrame = Time.frameCount;
                _triggeredLadderEntry = value;
            }
        }
        /// <summary>
        /// The ladder which the entity is climbing
        /// </summary>
        public Ladder ClimbingLadder { get; set; } = null;
        public Vector3 EnterOrExitFromPosition { get; set; }
        public Vector3 EnterOrExitToPosition { get; set; }
        public EnterExitState EnterExitState { get; set; }
        public float EnterOrExitTime { get; set; }
        public float EnterOrExitDuration { get; set; }
        public float EnterOrExitEndTime => EnterOrExitTime + EnterOrExitDuration;

        #region Enter Ladder Functions
        public void CallCmdEnterLadder()
        {
            RPC(CmdEnterLadder);
        }

        [ServerRpc]
        protected void CmdEnterLadder()
        {
            EnterLadder();
        }

        public void EnterLadder()
        {
            if (!IsServer)
            {
                Logging.LogWarning(LogTag, "Only server can perform ladder entering");
                return;
            }
            TriggeredLadderEntry = LadderEntrance.FindNearest(Entity.EntityTransform.position);
            if (ClimbingLadder && ClimbingLadder == TriggeredLadderEntry.ladder)
            {
                // Already climbing, do not enter
                return;
            }
            CallRpcConfirmEnterLadder(TriggeredLadderEntry.type);
        }

        public void CallRpcConfirmEnterLadder(LadderEntranceType entranceType)
        {
            RPC(RpcConfirmEnterLadder, entranceType);
        }

        [AllRpc]
        protected void RpcConfirmEnterLadder(LadderEntranceType entranceType)
        {
            ConfirmEnterLadderTask(entranceType);
        }

        protected virtual async void ConfirmEnterLadderTask(LadderEntranceType entranceType)
        {
            TriggeredLadderEntry = LadderEntrance.FindNearest(Entity.EntityTransform.position);
            ClimbingLadder = TriggeredLadderEntry.ladder;
            await PlayEnterLadderAnimation(entranceType);
        }

        public virtual async UniTask PlayEnterLadderAnimation(LadderEntranceType entranceType)
        {
            EnterExitState = EnterExitState.Enter;
            EnterOrExitFromPosition = Entity.EntityTransform.position;
            EnterOrExitToPosition = ClimbingLadder.ClosestPointOnLadderSegment(EnterOrExitFromPosition, Entity.Movement.GetMovementBounds().extents.z, out _);
            EnterOrExitTime = Time.unscaledTime;
            EnterOrExitDuration = 0f;
            if (Entity.Model is ILadderEnterExitModel ladderEnterExitModel)
            {
                EnterOrExitDuration = ladderEnterExitModel.GetEnterLadderAnimationDuration(entranceType);
                if (EnterOrExitDuration > 0f)
                {
                    ladderEnterExitModel.PlayEnterLadderAnimation(entranceType);
                }
            }
            if (EnterOrExitDuration > 0f)
            {
                await UniTask.WaitForSeconds(EnterOrExitDuration);
            }
            await UniTask.DelayFrame(2);
            EnterExitState = EnterExitState.None;
        }
        #endregion

        #region Exit Ladder Functions
        public void CallCmdExitLadder(LadderEntranceType entranceType)
        {
            RPC(CmdExitLadder, entranceType);
        }

        [ServerRpc]
        protected void CmdExitLadder(LadderEntranceType entranceType)
        {
            ExitLadder(entranceType);
        }

        public void ExitLadder(LadderEntranceType entranceType)
        {
            if (!IsServer)
            {
                Logging.LogWarning(LogTag, "Only server can perform ladder exiting");
                return;
            }
            if (!ClimbingLadder)
            {
                // Not climbing yet, do not exit
                return;
            }
            CallRpcConfirmExitLadder(entranceType);
        }

        public void CallRpcConfirmExitLadder(LadderEntranceType entranceType)
        {
            RPC(RpcConfirmExitLadder, entranceType);
        }

        [AllRpc]
        protected void RpcConfirmExitLadder(LadderEntranceType entranceType)
        {
            ConfirmExitLadderTask(entranceType);
        }

        protected virtual async void ConfirmExitLadderTask(LadderEntranceType entranceType)
        {
            await PlayExitLadderAnimation(entranceType);
            ClimbingLadder = null;
        }

        public virtual async UniTask PlayExitLadderAnimation(LadderEntranceType entranceType)
        {
            EnterExitState = EnterExitState.Exit;
            EnterOrExitFromPosition = Entity.EntityTransform.position;
            switch (entranceType)
            {
                case LadderEntranceType.Bottom:
                    EnterOrExitToPosition = ClimbingLadder.bottomExitTransform.position;
                    break;
                case LadderEntranceType.Top:
                    EnterOrExitToPosition = ClimbingLadder.topExitTransform.position;
                    break;
            }
            EnterOrExitTime = Time.unscaledTime;
            EnterOrExitDuration = 0f;
            if (Entity.Model is ILadderEnterExitModel ladderEnterExitModel)
            {
                EnterOrExitDuration = ladderEnterExitModel.GetExitLadderAnimationDuration(entranceType);
                if (EnterOrExitDuration > 0f)
                {
                    ladderEnterExitModel.PlayExitLadderAnimation(entranceType);
                }
            }
            if (EnterOrExitDuration > 0f)
            {
                await UniTask.WaitForSeconds(EnterOrExitDuration);
            }
            await UniTask.DelayFrame(2);
            EnterExitState = EnterExitState.None;
        }
        #endregion
    }
}