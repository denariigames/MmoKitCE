using UnityEngine;
using UnityEngine.UI;

namespace UtilsComponents
{
    public class RTCameraCapturer : MonoBehaviour
    {
        public Camera srcCamera;
        public RawImage dstImage;

        private int _dirtyWidth;
        private int _dirtyHeight;
        private RenderTexture _rt;

        private void Update()
        {
            if (_dirtyWidth != Screen.width ||
                _dirtyHeight != Screen.height)
            {
                _dirtyWidth = Screen.width;
                _dirtyHeight = Screen.height;
                if (srcCamera != null && dstImage != null)
                {
                    var oldRt = _rt;
                    _rt = new RenderTexture(_dirtyWidth, _dirtyHeight, 16, RenderTextureFormat.ARGB32);
                    _rt.name = "__rt";
                    _rt.Create();

                    srcCamera.targetTexture = _rt;
                    dstImage.texture = _rt;
                    if (oldRt != null)
                        DestroyImmediate(oldRt);
                }
            }
        }
    }
}
