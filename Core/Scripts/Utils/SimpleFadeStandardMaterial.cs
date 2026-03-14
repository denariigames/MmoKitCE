using UnityEngine;

namespace UtilsComponents
{
    public class SimpleFadeStandardMaterial : MonoBehaviour
    {
        public enum FadeState
        {
            None,
            FadeIn,
            FadeOut,
        }
        public FadeState currentFadeState = FadeState.None;
        public float fadeSpeed = 0.2f;
        private Texture2D blackTexture;
        private float alpha;

        private Renderer[] cacheRenderers;
        public Renderer[] CacheRenderers
        {
            get
            {
                if (cacheRenderers == null)
                    cacheRenderers = GetComponentsInChildren<Renderer>();
                return cacheRenderers;
            }
        }

        void Awake()
        {
            alpha = 1;
            SetMaterialsType();
            SetMaterialsAlpha(alpha);
        }

        void Update()
        {
            if (currentFadeState == FadeState.FadeOut)
            {
                if (alpha > 0)
                {
                    alpha -= Time.deltaTime * fadeSpeed;
                    if (alpha < 0) alpha = 0f;
                    SetMaterialsType();
                    SetMaterialsAlpha(alpha);
                }
            }
            if (currentFadeState == FadeState.FadeIn)
            {
                if (alpha < 1)
                {
                    alpha += Time.deltaTime * fadeSpeed;
                    if (alpha > 1) alpha = 1f;
                    SetMaterialsType();
                    SetMaterialsAlpha(alpha);
                }
            }
        }

        void SetMaterialsAlpha(float alpha)
        {
            foreach (Renderer renderer in CacheRenderers)
            {
                foreach (Material material in renderer.materials)
                {
                    material.color = new Color(material.color.r, material.color.g, material.color.b, alpha);
                }
            }
        }

        void SetMaterialsType()
        {
            if (alpha >= 1)
                SetMeterialsToOpaque();
            else
                SetMeterialsToFade();
        }

        void SetMeterialsToOpaque()
        {
            foreach (Renderer renderer in CacheRenderers)
            {
                foreach (Material material in renderer.materials)
                {
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                }
            }
        }

        void SetMeterialsToFade()
        {
            foreach (Renderer renderer in CacheRenderers)
            {
                foreach (Material material in renderer.materials)
                {
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                }
            }
        }

        public void FadeIn()
        {
            currentFadeState = FadeState.FadeIn;
        }

        public void FadeOut()
        {
            currentFadeState = FadeState.FadeOut;
        }
    }
}
