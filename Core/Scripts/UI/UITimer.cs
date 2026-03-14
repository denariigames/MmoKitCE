using Cysharp.Text;
using UnityEngine;

public class UITimer : MonoBehaviour
{
    public TextWrapper textHours;
    public TextWrapper textMinutes;
    public TextWrapper textSeconds;
    public TextWrapper textMilliseconds;
    public TextWrapper textAll;
    public string allFormat = "{0}:{1}:{2}.{3}";
    public float changedStep = 1f;
    protected float _prevSeconds = -1f;

    private void OnEnable()
    {
        if (textHours != null)
            textHours.enabled = true;

        if (textMinutes != null)
            textMinutes.enabled = true;

        if (textSeconds != null)
            textSeconds.enabled = true;

        if (textMilliseconds != null)
            textMilliseconds.enabled = true;

        if (textAll != null)
            textAll.enabled = true;
    }

    private void OnDisable()
    {
        if (textHours != null)
            textHours.enabled = false;

        if (textMinutes != null)
            textMinutes.enabled = false;

        if (textSeconds != null)
            textSeconds.enabled = false;

        if (textMilliseconds != null)
            textMilliseconds.enabled = false;

        if (textAll != null)
            textAll.enabled = false;
    }

    public void UpdateTime(float seconds)
    {
        if (Mathf.Abs(_prevSeconds - seconds) < changedStep)
            return;
        _prevSeconds = seconds;

        float hrs = Mathf.FloorToInt(seconds / 60f / 60f);
        float remainsSecFromHrs = seconds - (hrs * 60f * 60f);
        float min = Mathf.FloorToInt(remainsSecFromHrs / 60f);
        float secWithMilli = seconds % 60f;
        float sec = Mathf.FloorToInt(secWithMilli);
        float milli = (secWithMilli - sec) * 100;

        if (textHours != null)
            textHours.text = ZString.Format("{0:00}", hrs);

        if (textMinutes != null)
            textMinutes.text = ZString.Format("{0:00}", min);

        if (textSeconds != null)
            textSeconds.text = ZString.Format("{0:00}", sec);

        if (textMilliseconds != null)
            textMilliseconds.text = ZString.Format("{0:00}", milli);

        if (textAll != null)
            textAll.text = ZString.Format(allFormat, ZString.Format("{0:00}", hrs), ZString.Format("{0:00}", min), ZString.Format("{0:00}", sec), ZString.Format("{0:00}", milli));
    }
}
