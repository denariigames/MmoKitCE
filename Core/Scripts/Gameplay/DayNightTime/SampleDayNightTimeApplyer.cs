using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [ExecuteInEditMode]
    public class SampleDayNightTimeApplyer : MonoBehaviour
    {
        [Header("Color Settings")]
        [Tooltip("Gradient from time at midnight to another midnight")]
        public Gradient ambientColor;
        [Tooltip("Gradient from time at midnight to another midnight")]
        public Gradient directionalColor;

        [Header("Required Components")]
        public Light directionalLight;

        [Header("Debugging")]
        [Range(0f, 1f)]
        public float timeOfDayPercent = 0.5f;

        private void Update()
        {
            // Update time of day percent while network active only
            if (Application.isPlaying && BaseGameNetworkManager.Singleton.IsNetworkActive)
                timeOfDayPercent = GameInstance.Singleton.DayNightTimeUpdater.TimeOfDay / 24f;

            // Set ambient light
            RenderSettings.ambientLight = ambientColor.Evaluate(timeOfDayPercent);

            // Set directional light and rotate it to changes shadow direction
            if (directionalLight != null)
            {
                directionalLight.color = directionalColor.Evaluate(timeOfDayPercent);
                directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timeOfDayPercent * 360f) - 90f, 170f, 0));
            }
        }
    }
}
