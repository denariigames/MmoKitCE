using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ToggleActivator : MonoBehaviour
{
    public Toggle toggle;
    [Tooltip("These objects will be activated while toggle is the On, otherwise it will deactivated")]
    public GameObject[] turnOnObjects;
    [Tooltip("These objects will be activated while toggle is the off, otherwise it will deactivated")]
    public GameObject[] turnOffObjects;

    private void OnEnable()
    {
        toggle.onValueChanged.AddListener(OnToggle);
    }

    private void OnDisable()
    {
        toggle.onValueChanged.RemoveListener(OnToggle);
    }

    private void OnToggle(bool isOn)
    {
        foreach (GameObject obj in turnOnObjects)
        {
            obj.SetActive(isOn);
        }
        foreach (GameObject obj in turnOffObjects)
        {
            obj.SetActive(!isOn);
        }
    }
}
