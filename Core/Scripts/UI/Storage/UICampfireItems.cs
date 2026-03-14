using Cysharp.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public class UICampfireItems : UIStorageItems
    {
        public TextWrapper uiTextTurnOnElapsed;
        [Tooltip("These game objects will be activated while campfire turn on")]
        public GameObject[] turnOnObjects;
        [Tooltip("These game objects will be activated while campfire turn off")]
        public GameObject[] turnOffObjects;

        private CampFireEntity campFireEntity;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (TargetEntity)
                campFireEntity = TargetEntity as CampFireEntity;
        }

        protected override void Update()
        {
            base.Update();
            if (uiTextTurnOnElapsed)
            {
                System.TimeSpan time = System.TimeSpan.FromSeconds(campFireEntity.TurnOnElapsed);
                uiTextTurnOnElapsed.text = ZString.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
            }

            if (turnOnObjects != null && turnOnObjects.Length > 0)
            {
                foreach (GameObject obj in turnOnObjects)
                {
                    obj.SetActive(campFireEntity.IsTurnOn);
                }
            }

            if (turnOffObjects != null && turnOffObjects.Length > 0)
            {
                foreach (GameObject obj in turnOffObjects)
                {
                    obj.SetActive(!campFireEntity.IsTurnOn);
                }
            }
        }

        public void OnClickTurnOn()
        {
            GameInstance.PlayingCharacterEntity.BuildingComponent.CallCmdTurnOnCampFire(TargetEntity.ObjectId);
        }

        public void OnClickTurnOff()
        {
            GameInstance.PlayingCharacterEntity.BuildingComponent.CallCmdTurnOffCampFire(TargetEntity.ObjectId);
        }
    }
}
