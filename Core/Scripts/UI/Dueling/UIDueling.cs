using System.Collections.Generic;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace MultiplayerARPG
{
    public partial class UIDueling : UISelectionEntry<BasePlayerCharacterEntity>
    {
        [System.Serializable]
        public class CountDownEvent : UnityEvent<int> { }

        [Header("String Formats")]
        [Tooltip("Format => {0} = {Number}")]
        public UILocaleKeySetting formatCountDown = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);

        [Header("UI Elements")]
        public UITimer uiTimerCountDown;
        public UITimer uiTimerDuelingTime;
        public UICharacter uiAnotherCharacter;

        [Header("Other Settings")]
        public float delayBeforeHideAfterEnd = 3f;

        [Header("UI Events")]
        public CountDownEvent onCountDown = new CountDownEvent();
        public UnityEvent onStart = new UnityEvent();
        public UnityEvent onWin = new UnityEvent();
        public UnityEvent onLose = new UnityEvent();

        public float CountDown { get; set; }
        public float DuelingTime { get; set; }
        private int _dirtyCountDown = -1;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTimerCountDown = null;
            uiTimerDuelingTime = null;
            uiAnotherCharacter = null;
            onCountDown?.RemoveAllListeners();
            onCountDown = null;
            onStart?.RemoveAllListeners();
            onStart = null;
            onWin?.RemoveAllListeners();
            onWin = null;
            onLose?.RemoveAllListeners();
            onLose = null;
            _data = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (uiTimerCountDown != null)
                uiTimerCountDown.gameObject.SetActive(false);
            if (uiTimerDuelingTime != null)
                uiTimerDuelingTime.gameObject.SetActive(false);
            _dirtyCountDown = -1;
            if (!GameInstance.PlayingCharacterEntity) return;
            GameInstance.PlayingCharacterEntity.DuelingComponent.onEndDueling += EndDueling;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (!GameInstance.PlayingCharacterEntity) return;
            GameInstance.PlayingCharacterEntity.DuelingComponent.onEndDueling -= EndDueling;
        }

        public override void ManagedUpdate()
        {
            base.ManagedUpdate();

            if (CountDown > 0f)
            {
                CountDown -= Time.unscaledDeltaTime;
                if (CountDown < 0f)
                    CountDown = 0f;
                if (uiTimerCountDown != null)
                {
                    uiTimerCountDown.UpdateTime(CountDown);
                    uiTimerCountDown.gameObject.SetActive(true);
                }
                int intCountDown = Mathf.CeilToInt(CountDown);
                if (_dirtyCountDown != intCountDown)
                {
                    _dirtyCountDown = intCountDown;
                    onCountDown.Invoke(_dirtyCountDown);
                }
                if (CountDown <= 0f)
                    onStart.Invoke();
                return;
            }

            if (DuelingTime > 0f)
            {
                DuelingTime -= Time.unscaledDeltaTime;
                if (DuelingTime < 0f)
                    DuelingTime = 0f;
                if (uiTimerDuelingTime != null)
                {
                    uiTimerDuelingTime.UpdateTime(DuelingTime);
                    uiTimerDuelingTime.gameObject.SetActive(true);
                }
                return;
            }
        }

        protected override void UpdateData()
        {
            BasePlayerCharacterEntity anotherCharacter = Data;

            if (uiAnotherCharacter != null)
            {
                uiAnotherCharacter.NotForOwningCharacter = true;
                uiAnotherCharacter.Data = anotherCharacter;
            }
        }

        private async void EndDueling(BasePlayerCharacterEntity loser)
        {
            CountDown = -1f;
            DuelingTime = -1f;

            if (loser != null && loser.ObjectId != GameInstance.PlayingCharacterEntity.ObjectId)
                onWin.Invoke();

            if (loser != null && loser.ObjectId == GameInstance.PlayingCharacterEntity.ObjectId)
                onLose.Invoke();

            if (loser == null)
            {
                if (uiTimerCountDown != null)
                {
                    uiTimerCountDown.UpdateTime(0f);
                    uiTimerCountDown.gameObject.SetActive(true);
                }
            }

            await UniTask.Delay(Mathf.CeilToInt(delayBeforeHideAfterEnd * 1000));
            Hide();
        }

    }
}
