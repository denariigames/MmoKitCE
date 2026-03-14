using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MultiplayerARPG
{
    [CustomEditor(typeof(AnimationClip2D))]
    public class AnimationClip2DEditor : Editor
    {
        private float nextFrameTime = 0;
        private int currentFrame = 0;
        
        private void OnEnable()
        {
#if UNITY_EDITOR
            EditorApplication.update += EditorUpdate;
#endif
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
#endif
        }

        void EditorUpdate()
        {
            if (!Application.isPlaying)
            {
            }
        }

        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Repaint();
        }

        public override void DrawPreview(Rect previewArea)
        {
            Rect drawRect = previewArea;
            AnimationClip2D clip = (AnimationClip2D)target;
            Sprite sprite = currentFrame < clip.frames.Length ? clip.frames[currentFrame] : null;
            if (sprite != null)
            {
                float aspectRatio = sprite.rect.width / sprite.rect.height;
                Rect uv = new Rect() { position = Vector2.Scale(sprite.rect.position, sprite.texture.texelSize), size = Vector2.Scale(sprite.rect.size, sprite.texture.texelSize) };
                if (drawRect.width > drawRect.height)
                {
                    drawRect.width = drawRect.height / aspectRatio;
                }
                else
                {
                    drawRect.height = drawRect.width / aspectRatio;
                }
                drawRect.x = (previewArea.width - drawRect.width) / 2;
                drawRect.y += (previewArea.height - drawRect.height) / 2;
                if (clip.flipX)
                {
                    drawRect.x += drawRect.width;
                    drawRect.width *= -1f;
                }
                if (clip.flipY)
                {
                    drawRect.y += drawRect.height;
                    drawRect.height *= -1f;
                }
                GUI.DrawTextureWithTexCoords(drawRect, sprite.texture, uv);
            }

            // Is is time to play next frame?
            float time = Time.realtimeSinceStartup;
            if (time < nextFrameTime)
                return;
            
            // Play next frame
            currentFrame++;
            if (currentFrame >= clip.frames.Length)
            {
                // Looping
                currentFrame = 0;
            }
            nextFrameTime = time + (1 / clip.framesPerSec);
        }
    }
}
