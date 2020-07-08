using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TestRender : MonoBehaviour
{
    public new Camera camera;
    [Range(0.01F, 1.0F)]
    public float renderScale = 0.4F;
    public FilterMode filterMode = FilterMode.Bilinear;

    private Rect originalRect;
    private Rect scaledRect;

    void OnDestroy()
    {
        camera.rect = originalRect;
    }

    void OnPreRender()
    {
        originalRect = camera.rect;
        scaledRect.Set(originalRect.x, originalRect.y, originalRect.width * renderScale, originalRect.height * renderScale);
        camera.rect = scaledRect;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        camera.rect = originalRect;
        src.filterMode = filterMode;
        Graphics.Blit(src, dest);
        Debug.Log("Happening");
    }
}
