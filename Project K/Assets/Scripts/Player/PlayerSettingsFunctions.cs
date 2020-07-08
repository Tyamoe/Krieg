using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSettingsFunctions : MonoBehaviour
{
    public PlayerController player;

    [Header("Tabs")]
    public GameObject GameplayTabButton;
    public GameObject AudioTabButton;
    public GameObject KeybindTabButton;
    public GameObject PerformanceTabButton;

    [Space]
    public GameObject GameplayTabPanel;
    public GameObject AudioTabPanel;
    public GameObject KeybindTabPanel;
    public GameObject PerformanceTabPanel;

    [Header("Gameplay Tab")]
    public Slider FovSlider;
    public Slider BrightnessSlider;

    [Space]
    public Slider SensXSlider;
    public Slider SensYSlider;
    public Slider MultiXSlider;
    public Slider MultiYSlider;

    [Space]
    public Toggle ADSToggle;
    public Toggle CrouchToggle;

    [Space]
    public TextMeshProUGUI FovDisplay;
    public TextMeshProUGUI BrightnessDisplay;
    public TextMeshProUGUI SensXDisplay;
    public TextMeshProUGUI SensYDisplay;
    public TextMeshProUGUI MultiXDisplay;
    public TextMeshProUGUI MultiYDisplay;

    [Header("Audio Tab")]
    public Slider MasterSlider;
    public Slider UISlider;
    public TextMeshProUGUI MasterDisplay;
    public TextMeshProUGUI UIDisplay;

    [Header("Keybinds Tab")]
    public TextMeshProUGUI JumpInput;

    [Header("Performance Tab")]
    public Slider ResolutionSlider;
    public Toggle VSyncToggle;
    public Slider RenderSlider;
    public GameObject ModelQuality;
    public GameObject TextureQuality;
    public GameObject Shadows;
    public GameObject ShadowQual;
    public GameObject MSAAQuality;
    public GameObject ANISQuality;
    public Slider ShadowSlider;
    public GameObject ShadowCascades;
    public Toggle AmbientOcToggle;
    public Toggle DepthOfFieldToggle;

    [Space]
    public TextMeshProUGUI ResolutionDisplay;
    public TextMeshProUGUI RenderDisplay;
    public TextMeshProUGUI ShadowDisplay;

    //
    private float brightness = 0.0f;
    private float resolution = 0.0f;

    private int modelQuality = -1;
    private int textureQuality = -1;
    private int MSAA = -1;
    private int Anis = -1;

    void Start()
    {
        LoadSaved();
        GameplayTab();
    }

    void LoadSaved()
    {

    }

    #region GameplayTab

    public void GameplayTab()
    {
        GameplayTabPanel.SetActive(true);
        AudioTabPanel.SetActive(false);
        KeybindTabPanel.SetActive(false);
        PerformanceTabPanel.SetActive(false);

        GameplayTabButton.GetComponent<Image>().fillCenter = true;
        GameplayTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

        AudioTabButton.GetComponent<Image>().fillCenter = false;
        AudioTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        KeybindTabButton.GetComponent<Image>().fillCenter = false;
        KeybindTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        PerformanceTabButton.GetComponent<Image>().fillCenter = false;
        PerformanceTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
    }

    public void UpdateFOV(Slider inFloat)
    {
        player.FOV = inFloat.value;
        FovDisplay.text = inFloat.value.ToString("F0");
    }

    public void UpdateBrightness(Slider inFloat)
    {
        brightness = inFloat.value;
        Screen.brightness = brightness / 100.0f;
        BrightnessDisplay.text = inFloat.value.ToString("F2");
    }

    public void UpdateSensitivityX(Slider inFloat)
    {
        player.SensitivityX = inFloat.value;
        SensXDisplay.text = inFloat.value.ToString("F2");
    }

    public void UpdateSensitivityY(Slider inFloat)
    {
        player.SensitivityY = inFloat.value;
        SensYDisplay.text = inFloat.value.ToString("F2");
    }

    public void UpdateADSSensitivityX(Slider inFloat)
    {
        player.ADSMultiplierX = inFloat.value;
        MultiXDisplay.text = inFloat.value.ToString("F2");
    }

    public void UpdateADSSensitivityY(Slider inFloat)
    {
        player.ADSMultiplierY = inFloat.value;
        MultiYDisplay.text = inFloat.value.ToString("F2");
    }

    public void UpdateADSTgl(Toggle inBool)
    {
        player.toggleADS = inBool.isOn;
    }

    public void UpdateCrouchTgl(Toggle inBool)
    {
        player.toggleCrouch = inBool.isOn;
    }

    #endregion

    #region AudioTab

    public void AudioTab()
    {
        AudioTabPanel.SetActive(true);
        KeybindTabPanel.SetActive(false);
        PerformanceTabPanel.SetActive(false);
        GameplayTabPanel.SetActive(false);

        AudioTabButton.GetComponent<Image>().fillCenter = true;
        AudioTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

        GameplayTabButton.GetComponent<Image>().fillCenter = false;
        GameplayTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        KeybindTabButton.GetComponent<Image>().fillCenter = false;
        KeybindTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        PerformanceTabButton.GetComponent<Image>().fillCenter = false;
        PerformanceTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
    }

    public void UpdateMasterVolume(Slider inFloat)
    {
        player.MasterVolume = inFloat.value;
        MasterDisplay.text = inFloat.value.ToString("F3");
    }

    public void UpdateUIVolume(Slider inFloat)
    {
        player.UIVolume = inFloat.value;
        UIDisplay.text = inFloat.value.ToString("F3");
    }

    #endregion

    #region KeybindTab

    public void KeybindTab()
    {
        KeybindTabPanel.SetActive(true);
        PerformanceTabPanel.SetActive(false);
        GameplayTabPanel.SetActive(false);
        AudioTabPanel.SetActive(false);

        KeybindTabButton.GetComponent<Image>().fillCenter = true;
        KeybindTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

        GameplayTabButton.GetComponent<Image>().fillCenter = false;
        GameplayTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        AudioTabButton.GetComponent<Image>().fillCenter = false;
        AudioTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        PerformanceTabButton.GetComponent<Image>().fillCenter = false;
        PerformanceTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
    }

    #endregion

    #region PerformanceTab

    public void PerformanceTab()
    {
        PerformanceTabPanel.SetActive(true);
        GameplayTabPanel.SetActive(false);
        AudioTabPanel.SetActive(false);
        KeybindTabPanel.SetActive(false);

        PerformanceTabButton.GetComponent<Image>().fillCenter = true;
        PerformanceTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

        GameplayTabButton.GetComponent<Image>().fillCenter = false;
        GameplayTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        AudioTabButton.GetComponent<Image>().fillCenter = false;
        AudioTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        KeybindTabButton.GetComponent<Image>().fillCenter = false;
        KeybindTabButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
    }

    float mapToRange(float val, float r1s, float r1e, float r2s, float r2e)
    {
        return (val - r1s) / (r1e - r1s) * (r2e - r2s) + r2s;
    }

    public void UpdateResolutionQuality(Slider inFloat)
    {
        float res = Mathf.Clamp(inFloat.value, 0.0f, 8.0f);
        float x = Mathf.Clamp(inFloat.value - 8.0f, 0.0f, 2.0f);

        resolution = mapToRange(res, 0.0f, 8.0f, 0.0f, 1.0f) + mapToRange(x, 0.0f, 2.0f, 0.0f, 0.5f);

        int width = Mathf.RoundToInt(Screen.currentResolution.width * resolution);
        int height = Mathf.RoundToInt(Screen.currentResolution.height * resolution);

        Screen.SetResolution(width, height, true);
        ResolutionDisplay.text = inFloat.value.ToString("F2");

        //Debug.Log(QualitySettings.resolutionScalingFixedDPIFactor);

        //QualitySettings.resolutionScalingFixedDPIFactor = resolution;

        //Debug.Log(QualitySettings.resolutionScalingFixedDPIFactor);
    }

    public void UpdateVSync(Toggle inBool)
    {
        if(inBool.isOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
    }

    public void UpdateRenderDistance(Slider inFloat)
    {
        //player.FPSCamera.farClipPlane = inFloat.value;
        player.EnvCamera.farClipPlane = inFloat.value;
        RenderDisplay.text = inFloat.value.ToString("F2");
    }

    public void ChangeModelQuality(int selection)
    {
        if(modelQuality != selection)
        {
            Image[] img = ModelQuality.GetComponentsInChildren<Image>();
            TextMeshProUGUI[] txt = ModelQuality.GetComponentsInChildren<TextMeshProUGUI>();

            img[0].fillCenter = false;
            img[1].fillCenter = false;
            img[2].fillCenter = false;

            txt[1].color = Color.white;
            txt[2].color = Color.white;
            txt[3].color = Color.white;

            img[selection].fillCenter = true;
            txt[selection + 1].color = Color.black;

            switch (selection)
            {
                case 0:
                    QualitySettings.lodBias = 0.3f;
                    break;
                case 1:
                    QualitySettings.lodBias = 1.0f;
                    break;
                case 2:
                    QualitySettings.lodBias = 2.0f;
                    break;
            }
            modelQuality = selection;
        }
    }

    public void ChangeTextureQuality(int selection)
    {
        if (textureQuality != selection)
        {
            Image[] img = TextureQuality.GetComponentsInChildren<Image>();
            TextMeshProUGUI[] txt = TextureQuality.GetComponentsInChildren<TextMeshProUGUI>();

            img[0].fillCenter = false;
            img[1].fillCenter = false;
            img[2].fillCenter = false;

            txt[1].color = Color.white;
            txt[2].color = Color.white;//////////////////////////////////
            txt[3].color = Color.white;

            img[2 - selection].fillCenter = true;
            txt[2 - selection + 1].color = Color.black;

            QualitySettings.masterTextureLimit = textureQuality = selection;
        }
    }

    public void ChangeShadowQuality(int selection)
    {
        Image[] img = Shadows.GetComponentsInChildren<Image>();
        TextMeshProUGUI[] txt = Shadows.GetComponentsInChildren<TextMeshProUGUI>();

        img[0].fillCenter = false;
        img[1].fillCenter = false;
        img[2].fillCenter = false;

        txt[1].color = Color.white;
        txt[2].color = Color.white;
        txt[3].color = Color.white;

        img[selection].fillCenter = true;
        txt[selection + 1].color = Color.black;

        switch (selection)
        {
            case 0:
                QualitySettings.shadows = ShadowQuality.Disable;
                break;
            case 1:
                QualitySettings.shadows = ShadowQuality.HardOnly;
                break;
            case 2:
                QualitySettings.shadows = ShadowQuality.All;
                break;
        }
    }

    public void ChangeShadowQuality2(int selection)
    {
        Image[] img = ShadowQual.GetComponentsInChildren<Image>();
        TextMeshProUGUI[] txt = ShadowQual.GetComponentsInChildren<TextMeshProUGUI>();

        img[0].fillCenter = false;
        img[1].fillCenter = false;
        img[2].fillCenter = false;

        txt[1].color = Color.white;
        txt[2].color = Color.white;
        txt[3].color = Color.white;

        img[selection].fillCenter = true;
        txt[selection + 1].color = Color.black;

        switch (selection)
        {
            case 0:
                QualitySettings.shadowResolution = ShadowResolution.Low;
                break;
            case 1:
                QualitySettings.shadowResolution = ShadowResolution.Medium;
                break;
            case 2:
                QualitySettings.shadowResolution = ShadowResolution.High;
                break;
        }
    }

    public void ChangeMSAA(int selection)
    {
        if (MSAA != selection)
        {
            Image[] img = MSAAQuality.GetComponentsInChildren<Image>();
            TextMeshProUGUI[] txt = MSAAQuality.GetComponentsInChildren<TextMeshProUGUI>();

            int s = Mathf.RoundToInt(Mathf.Sqrt(selection));

            img[0].fillCenter = false;
            img[1].fillCenter = false;
            img[2].fillCenter = false;
            img[3].fillCenter = false;

            txt[1].color = Color.white;
            txt[2].color = Color.white;
            txt[3].color = Color.white;
            txt[4].color = Color.white;

            img[s].fillCenter = true;
            txt[s + 1].color = Color.black;

            QualitySettings.antiAliasing = MSAA = selection;
        }
    }

    public void ChangeAnis(int selection)
    {
        if (Anis != selection)
        {
            Image[] img = ANISQuality.GetComponentsInChildren<Image>();
            TextMeshProUGUI[] txt = ANISQuality.GetComponentsInChildren<TextMeshProUGUI>();

            img[0].fillCenter = false;
            img[1].fillCenter = false;
            img[2].fillCenter = false;

            txt[1].color = Color.white;
            txt[2].color = Color.white;
            txt[3].color = Color.white;

            img[selection].fillCenter = true;
            txt[selection + 1].color = Color.black;

            switch (selection)
            {
                case 0:
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                    break;
                case 1:
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                    break;
                case 2:
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                    break;
            }
            Anis = selection;
        }
    }

    public void UpdateShadowDistance(Slider inFloat)
    {
        QualitySettings.shadowDistance = inFloat.value;
        ShadowDisplay.text = inFloat.value.ToString("F2");
    }

    public void ChangeShadowCascades(int selection)
    {
        Image[] img = ShadowCascades.GetComponentsInChildren<Image>();
        TextMeshProUGUI[] txt = ShadowCascades.GetComponentsInChildren<TextMeshProUGUI>();

        int s = selection / 2;

        img[0].fillCenter = false;
        img[1].fillCenter = false;
        img[2].fillCenter = false;

        txt[1].color = Color.white;
        txt[2].color = Color.white;
        txt[3].color = Color.white;

        img[s].fillCenter = true;
        txt[s + 1].color = Color.black;

        QualitySettings.shadowCascades = selection;
    }

    public void UpdateAmbientOc(Toggle inBool)
    {
        try
        {
            MonoBehaviour tempScript = (MonoBehaviour)player.EnvCamera.GetComponent("AmbientOcclusion");
            if (inBool.isOn)
            {
                tempScript.enabled = true;
            }
            else
            {
                tempScript.enabled = false;
            }
        }
        catch
        {
            Debug.Log("No AO post processing found");
            return;
        }
    }

    public void UpdateDOF(Toggle inBool)
    {
        try
        {
            MonoBehaviour tempScript = (MonoBehaviour)player.EnvCamera.GetComponent("DepthOfField");
            if (inBool.isOn)
            {
                tempScript.enabled = true;
            }
            else
            {
                tempScript.enabled = false;
            }
        }
        catch
        {
            Debug.Log("No DOF post processing found");
            return;
        }
    }

    #endregion
}
