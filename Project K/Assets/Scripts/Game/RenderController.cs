using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderController : MonoBehaviour
{
    [SerializeField]
    RenderTexture texture;

    public Camera FPSCamera;
    public Camera EnvCamera;
    public Camera MainCamera;

    bool Started = false;

    void Start()
    {
        if(!Started)
        {
            //texture = new RenderTexture(Screen.width, Screen.height, 24);
            texture = new RenderTexture(1920, 1080, 24);
            RenderTexture.active = texture;
            if (texture.Create())
            {
                //Debug.Log("RenderTexture");
            }

            FPSCamera.targetTexture = texture;
            EnvCamera.targetTexture = texture;

            Started = true;
        }
    }

    void Update()
    {
    }

    public void UpdateResolution(float scale)
    {
        int width = Mathf.RoundToInt(1920 * scale);
        int height = Mathf.RoundToInt(1080 * scale);

        RenderTexture.Destroy(texture);

        texture = new RenderTexture(width, height, 24);
        RenderTexture.active = texture;
        if (texture.Create())
        {
            //Debug.Log("RenderTexture1");
        }

        FPSCamera.targetTexture = texture;
        EnvCamera.targetTexture = texture;

        Started = true;
    }

    void OnPreRender()
    {
        //FPSCamera.gameObject.SetActive(true);
        //EnvCamera.gameObject.SetActive(true);
        //MainCamera.gameObject.SetActive(false);

        //FPSCamera.targetTexture = texture;
        //EnvCamera.targetTexture = texture;

        //RenderTexture.active = texture;
    }

    void OnPostRender()
    {
        //FPSCamera.gameObject.SetActive(false);
        //EnvCamera.gameObject.SetActive(false);
        //MainCamera.gameObject.SetActive(true);

        //MainCamera.targetTexture = null;
        //RenderTexture.active = null;

        //Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);
        Graphics.Blit(texture, null as RenderTexture);

        //Debug.Log("OnPostRender");
    }
}
