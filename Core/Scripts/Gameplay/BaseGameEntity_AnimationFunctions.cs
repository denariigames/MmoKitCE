using LiteNetLib;
using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial class BaseGameEntity
    {
        #region Play Jump Animation
        public void CallRpcPlayJumpAnimation()
        {
            RPC(RpcPlayJumpAnimation, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered);
        }

        [AllRpc]
        protected void RpcPlayJumpAnimation()
        {
            PlayJumpAnimation();
        }

        public virtual void PlayJumpAnimation()
        {
            if (Model is IJumppableModel jumppableModel)
                jumppableModel.PlayJumpAnimation();
        }
        #endregion

        #region Play Pickup Animation
        public void CallRpcPlayPickupAnimation()
        {
            RPC(RpcPlayPickupAnimation, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered);
        }

        [AllRpc]
        protected void RpcPlayPickupAnimation()
        {
            PlayPickupAnimation();
        }

        public virtual void PlayPickupAnimation()
        {
            if (Model is IPickupableModel pickupableModel)
                pickupableModel.PlayPickupAnimation();
        }
        #endregion

        #region Play Custom Animation
        public void CallRpcPlayCustomAnimation(int id, bool loop)
        {
            RPC(RpcPlayCustomAnimation, id, loop);
        }

        [AllRpc]
        protected virtual void RpcPlayCustomAnimation(int id, bool loop)
        {
            PlayCustomAnimation(id, loop);
        }

        public virtual void PlayCustomAnimation(int id, bool loop)
        {
            if (Model is ICustomAnimationModel customAnimationModel)
                customAnimationModel.PlayCustomAnimation(id, loop);
        }
        #endregion

        #region Stop Custom Animation
        public void CallRpcStopCustomAnimation()
        {
            RPC(RpcStopCustomAnimation);
        }

        [AllRpc]
        protected virtual void RpcStopCustomAnimation()
        {
            StopCustomAnimation();
        }

        public virtual void StopCustomAnimation()
        {
            if (Model is ICustomAnimationModel customAnimationModel)
                customAnimationModel.StopCustomAnimation();
        }
        #endregion
    }
}
