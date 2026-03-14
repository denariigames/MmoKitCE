using UnityEngine;

namespace UtilsComponents
{
    public class SimpleFade : MonoBehaviour
    {
        public enum FadeState
        {
            None,
            FadeIn,
            FadeOut,
        }
        public float awakeAlpha = 1f;
        public FadeState currentFadeState = FadeState.None;
        public float fadeSpeed = 0.2f;
        private Texture2D blackTexture;
        private float alpha;

        void Awake()
        {
            alpha = awakeAlpha;
            blackTexture = new Texture2D(1, 1);
            blackTexture.SetPixel(0, 0, new Color(0, 0, 0, alpha));
            blackTexture.Apply();
        }

        void OnGUI()
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blackTexture);
        }

        void Update()
        {
            if (currentFadeState == FadeState.FadeOut)
            {
                if (alpha > 0)
                {
                    alpha -= Time.deltaTime * fadeSpeed;
                    if (alpha < 0) alpha = 0f;
                    blackTexture.SetPixel(0, 0, new Color(0, 0, 0, alpha));
                    blackTexture.Apply();
                }
            }
            if (currentFadeState == FadeState.FadeIn)
            {
                if (alpha < 1)
                {
                    alpha += Time.deltaTime * fadeSpeed;
                    if (alpha > 1) alpha = 1f;
                    blackTexture.SetPixel(0, 0, new Color(0, 0, 0, alpha));
                    blackTexture.Apply();
                }
            }
        }

        public void FadeIn()
        {
            PrepareToFadeIn();
            currentFadeState = FadeState.FadeIn;
        }

        public void FadeOut()
        {
            PrepareToFadeOut();
            currentFadeState = FadeState.FadeOut;
        }

        public void PrepareToFadeIn()
        {
            alpha = 0;
            currentFadeState = FadeState.None;
        }

        public void PrepareToFadeOut()
        {
            alpha = 1;
            currentFadeState = FadeState.None;
        }
    }
}
