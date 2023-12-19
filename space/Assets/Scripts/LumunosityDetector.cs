using UnityEngine.Rendering;
using UnityEngine;

public class LuminosityDetector : MonoBehaviour
{
    public RenderTexture targetTexture;
    private Texture2D tempTexture;

    void Start()
    {
        tempTexture = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.RGBA32, false);
        InvokeRepeating("CheckLuminosity", 2.0f, 0.3f); // Adjust time as needed
    }

    void CheckLuminosity()
    {
        AsyncGPUReadback.Request(targetTexture, 0, OnCompleteReadback);
    }

    void OnCompleteReadback(AsyncGPUReadbackRequest request)
    {
        if (request.hasError)
        {
            Debug.LogError("GPU readback error detected.");
            return;
        }

        tempTexture.LoadRawTextureData(request.GetData<uint>());
        tempTexture.Apply();

        Color32[] pixels = tempTexture.GetPixels32();
        float luminanceSum = 0;

        foreach (Color32 pixel in pixels)
        {
            luminanceSum += (0.2126f * pixel.r + 0.7152f * pixel.g + 0.0722f * pixel.b) / 255.0f;
        }

        float averageLuminance = luminanceSum / pixels.Length;
        Debug.Log("Average Luminance: " + averageLuminance);
    }

}
