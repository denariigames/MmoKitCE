using Cysharp.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class ExternalTextureManager
{
    private static ConcurrentDictionary<string, object> s_loadingTextures = new ConcurrentDictionary<string, object>();
    private static ConcurrentDictionary<string, Texture2D> s_loadedTextures = new ConcurrentDictionary<string, Texture2D>();

    public static void Clean(string url)
    {
        s_loadingTextures.TryRemove(url, out _);
        s_loadedTextures.TryRemove(url, out Texture2D texture);
        if (texture != null)
            Object.Destroy(texture);
    }

    public static void CleanAll()
    {
        List<string> keys = new List<string>(s_loadedTextures.Keys);
        foreach (string key in keys)
        {
            Clean(key);
        }
    }

    public static async UniTask<Texture2D> Load(string url, System.Action<float> onProgressUpdate = null)
    {
        if (s_loadingTextures.ContainsKey(url))
        {
            do
            {
                await UniTask.WaitForSeconds(0.1f);
            } while (s_loadingTextures.ContainsKey(url));
        }

        if (s_loadedTextures.TryGetValue(url, out Texture2D loadedTexture) && loadedTexture != null)
        {
            return loadedTexture;
        }

        loadedTexture = null;
        s_loadingTextures.TryAdd(url, new object());

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        UnityWebRequestAsyncOperation asyncOp = request.SendWebRequest();
        onProgressUpdate?.Invoke(0f);
        do
        {
            onProgressUpdate?.Invoke(asyncOp.progress);
            await UniTask.NextFrame();
        } while (!asyncOp.isDone);

        // Error occuring O_O
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error occuring when load texture from {url}");
            s_loadingTextures.TryRemove(url, out _);
            return null;
        }
        onProgressUpdate?.Invoke(1f);

        // Get texture data
        Texture2D originalTexture = DownloadHandlerTexture.GetContent(asyncOp.webRequest);
        request.Dispose();

        // Create a new texture with a lower bit depth format (e.g., RGB565)
        loadedTexture = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.RGBA32, false);

        // Copy the pixels from the original texture to the new texture
        loadedTexture.SetPixels(originalTexture.GetPixels());
        loadedTexture.requestedMipmapLevel = 0;
        loadedTexture.Compress(false); // Compress the texture to save memory
        loadedTexture.Apply(updateMipmaps: false, makeNoLongerReadable: true); // Make it non-readable to free up memory

        // Optionally, you can destroy the original texture to free up memory
        Object.Destroy(originalTexture);

        // Store loaded texture to collection
        s_loadedTextures[url] = loadedTexture;
        s_loadingTextures.TryRemove(url, out _);

        return loadedTexture;
    }
}
