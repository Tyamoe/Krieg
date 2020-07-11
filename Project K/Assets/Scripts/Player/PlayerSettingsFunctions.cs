using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerSettingsFunctions : MonoBehaviour
{
    public PlayerController player;

    [Header("General")]
    public GameObject SaveSettingsButton;

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

    [Header("Ref")]
    public RenderController renderCtrl;

    struct Settings
    {
        // Gameplay
        public float fov;
        public float brightness;

        public float SensitivityX;
        public float SensitivityY;
        public float ADSMultiplierX;
        public float ADSMultiplierY;

        public bool toggleADS;
        public bool toggleCrouch;

        // Audio
        public float masterVolume;
        public float uiVolume;

        // Performance
        public float resolutionScale;
        public bool toggleVSync;

        public float renderDistance;
        public int modelQuality;
        public int textureQuality;
        public int shadows;
        public int shadowQuality;
        public int MSAA;
        public int anisotropicFiltering;
        public float shadowDistance;
        public int shadowCascades;

        public bool ambientOcclusion;
        public bool DOF;
    }

    Settings settings;

    bool Started = false;

    private void Awake()
    {
        settings = new Settings();
    }

    void Start()
    {
        LoadSaved();
        GameplayTab();
        Started = true;
    }

    private void OnDisable()
    {
        Started = false;
        LoadSaved();
        Started = true;
    }

    private void OnEnable()
    {
        Vector3 v = GameplayTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position;
        GameplayTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position = new Vector3(v.x, 0.0f, v.z);

        v = AudioTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position;
        AudioTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position = new Vector3(v.x, 0.0f, v.z);

        v = KeybindTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position;
        KeybindTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position = new Vector3(v.x, 0.0f, v.z);

        v = PerformanceTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position;
        PerformanceTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position = new Vector3(v.x, 0.0f, v.z);

        GameplayTab();
    }

    void LoadSaved()
    {
        // Gameplay
        settings.fov = PlayerPrefs.GetFloat("fov", 75);
        settings.brightness = PlayerPrefs.GetFloat("brightness", 70);

        settings.SensitivityX = PlayerPrefs.GetFloat("SensitivityX", 1.0f);
        settings.SensitivityY = PlayerPrefs.GetFloat("SensitivityY", 1.0f);
        settings.ADSMultiplierX = PlayerPrefs.GetFloat("ADSMultiplierX", 1.0f);
        settings.ADSMultiplierY = PlayerPrefs.GetFloat("ADSMultiplierY", 1.0f);

        settings.toggleADS = PlayerPrefs.GetInt("toggleADS", 0) == 0 ? false : true;
        settings.toggleCrouch = PlayerPrefs.GetInt("toggleCrouch", 0) == 0 ? false : true;

        // Set UI Elements
        FovSlider.value = settings.fov;
        BrightnessSlider.value = settings.brightness;

        SensXSlider.value = settings.SensitivityX;
        SensYSlider.value = settings.SensitivityY;
        MultiXSlider.value = settings.ADSMultiplierX;
        MultiYSlider.value = settings.ADSMultiplierY;

        ADSToggle.isOn = settings.toggleADS;
        CrouchToggle.isOn = settings.toggleCrouch;

        //-----------------------------------------------------------------//
        //*****************************************************************//
        //-----------------------------------------------------------------//

        // Audio
        settings.masterVolume = PlayerPrefs.GetFloat("masterVolume", 0.6f);
        settings.uiVolume = PlayerPrefs.GetFloat("uiVolume", 1.0f);

        // Set UI Elements
        MasterSlider.value = settings.masterVolume;
        UISlider.value = settings.uiVolume;

        //-----------------------------------------------------------------//
        //*****************************************************************//
        //-----------------------------------------------------------------//

        // Performance
        settings.resolutionScale = PlayerPrefs.GetFloat("resolutionScale", 8.0f);
        settings.toggleVSync = PlayerPrefs.GetInt("toggleVSync", 1) == 0 ? false : true;
        settings.renderDistance = PlayerPrefs.GetFloat("renderDistance", 600.0f);

        settings.modelQuality = PlayerPrefs.GetInt("modelQuality", 1);
        settings.textureQuality = PlayerPrefs.GetInt("textureQuality", 2);
        settings.shadows = PlayerPrefs.GetInt("shadows", 1);
        settings.shadowQuality = PlayerPrefs.GetInt("shadowQuality", 1);
        settings.MSAA = PlayerPrefs.GetInt("MSAA", 2);
        settings.anisotropicFiltering = PlayerPrefs.GetInt("anisotropicFiltering", 1);
        settings.shadowDistance = PlayerPrefs.GetFloat("shadowDistance", 400.0f);
        settings.shadowCascades = PlayerPrefs.GetInt("shadowCascades", 4);

        settings.ambientOcclusion = PlayerPrefs.GetInt("ambientOcclusion", 0) == 0 ? false : true;
        settings.DOF = PlayerPrefs.GetInt("DOF", 0) == 0 ? false : true;

        // Set UI Elements
        ResolutionSlider.value = settings.resolutionScale;
        VSyncToggle.isOn = settings.toggleVSync;
        RenderSlider.value = settings.renderDistance;

        ChangeModelQuality(settings.modelQuality);
        ChangeTextureQuality(settings.textureQuality);
        ChangeShadowQuality(settings.shadows);
        ChangeShadowQuality2(settings.shadowQuality);
        ChangeMSAA(settings.MSAA);
        ChangeAnis(settings.anisotropicFiltering);

        ShadowSlider.value = settings.shadowDistance;

        ChangeShadowCascades(settings.shadowCascades);

        AmbientOcToggle.isOn = settings.ambientOcclusion;
        DepthOfFieldToggle.isOn = settings.DOF;

        //-----------------------------------------------------------------//
        //*****************************************************************//
        //-----------------------------------------------------------------//

        SaveSettingsButton.GetComponent<Image>().fillCenter = false;
        SaveSettingsButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
    }

    public void SaveSettings()
    {
        // Gameplay
        PlayerPrefs.SetFloat("fov", settings.fov);
        PlayerPrefs.SetFloat("brightness", settings.brightness);

        PlayerPrefs.SetFloat("SensitivityX", settings.SensitivityX);
        PlayerPrefs.SetFloat("SensitivityY", settings.SensitivityY);
        PlayerPrefs.SetFloat("ADSMultiplierX", settings.ADSMultiplierX);
        PlayerPrefs.SetFloat("ADSMultiplierY", settings.ADSMultiplierY);

        PlayerPrefs.SetInt("toggleADS", settings.toggleADS ? 1 : 0);
        PlayerPrefs.SetInt("toggleCrouch", settings.toggleCrouch ? 1 : 0);

        // Audio
        PlayerPrefs.SetFloat("masterVolume", settings.masterVolume);
        PlayerPrefs.SetFloat("uiVolume", settings.uiVolume);

        // Performance
        PlayerPrefs.SetFloat("resolutionScale", settings.resolutionScale);
        PlayerPrefs.SetInt("VSync", settings.toggleVSync ? 1 : 0);
        PlayerPrefs.SetFloat("renderDistance", settings.renderDistance);

        PlayerPrefs.SetInt("modelQuality", settings.modelQuality);
        PlayerPrefs.SetInt("textureQuality", settings.textureQuality);
        PlayerPrefs.SetInt("shadows", settings.shadows);
        PlayerPrefs.SetInt("shadowQuality", settings.shadowQuality);
        PlayerPrefs.SetInt("MSAA", settings.MSAA);
        PlayerPrefs.SetInt("anisotropicFiltering", settings.anisotropicFiltering);
        PlayerPrefs.SetFloat("shadowDistance", settings.shadowDistance);
        PlayerPrefs.SetInt("shadowCascades", settings.shadowCascades);

        PlayerPrefs.SetInt("ambientOcclusion", settings.ambientOcclusion ? 1 : 0);
        PlayerPrefs.SetInt("DOF", settings.DOF ? 1 : 0);

        SaveSettingsButton.GetComponent<Image>().fillCenter = false;
        SaveSettingsButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
    }

    void SettingsChanged()
    {
        SaveSettingsButton.GetComponent<Image>().fillCenter = true;
        SaveSettingsButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
    }

    IEnumerator DoSomething(GameObject OBJ)
    {
        yield return new WaitForSeconds(0.05f);

        Vector3 v = OBJ.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position;

        if(Mathf.Abs(v.y) > 0.1f)
        {
            Debug.Log(OBJ.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position);
            OBJ.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position = new Vector3(v.x, 0.0f, v.z);
        }
    }

    #region GameplayTab

    public void GameplayTab()
    {
        GameplayTabPanel.SetActive(true);
        /*Vector3 v = GameplayTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position;

        Debug.Log(GameplayTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position);
        GameplayTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position = new Vector3(v.x, 0.0f, v.z);*/

        StartCoroutine(DoSomething(GameplayTabPanel));

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
        if(settings.fov != inFloat.value || !Started)
        {
            if (player)
                player.FOV = inFloat.value;
            FovDisplay.text = inFloat.value.ToString("F0");

            settings.fov = inFloat.value;
            SettingsChanged();
        }
    }

    public void UpdateBrightness(Slider inFloat)
    {
        if (settings.brightness != inFloat.value || !Started)
        {
            float brightness = inFloat.value;
            //Screen.brightness = brightness / 100.0f;
            BrightnessDisplay.text = inFloat.value.ToString("F2");

            RenderSettings.ambientLight = new Color(brightness / 100.0f, brightness / 100.0f, brightness / 100.0f, 1.0f);
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            //RenderSettings.
            settings.brightness = inFloat.value;
            SettingsChanged();
        }
    }

    public void UpdateSensitivityX(Slider inFloat)
    {
        if (settings.SensitivityX != inFloat.value || !Started)
        {
            if (player)
                player.SensitivityX = inFloat.value;
            SensXDisplay.text = inFloat.value.ToString("F2");

            settings.SensitivityX = inFloat.value;
            SettingsChanged();
        }
    }

    public void UpdateSensitivityY(Slider inFloat)
    {
        if (settings.SensitivityY != inFloat.value || !Started)
        {
            if (player)
                player.SensitivityY = inFloat.value;
            SensYDisplay.text = inFloat.value.ToString("F2");

            settings.SensitivityY = inFloat.value;
            SettingsChanged();
        }
    }

    public void UpdateADSSensitivityX(Slider inFloat)
    {
        if (settings.ADSMultiplierX != inFloat.value || !Started)
        {
            if (player)
                player.ADSMultiplierX = inFloat.value;
            MultiXDisplay.text = inFloat.value.ToString("F2");

            settings.ADSMultiplierX = inFloat.value;
            SettingsChanged();
        }
    }

    public void UpdateADSSensitivityY(Slider inFloat)
    {
        if (settings.ADSMultiplierY != inFloat.value || !Started)
        {
            if (player)
                player.ADSMultiplierY = inFloat.value;
            MultiYDisplay.text = inFloat.value.ToString("F2");

            settings.ADSMultiplierY = inFloat.value;
            SettingsChanged();
        }
    }

    public void UpdateADSTgl(Toggle inBool)
    {
        if (player)
            player.toggleADS = inBool.isOn;

        settings.toggleADS = inBool.isOn;
        SettingsChanged();
    }

    public void UpdateCrouchTgl(Toggle inBool)
    {
        if(player)
            player.toggleCrouch = inBool.isOn;

        settings.toggleCrouch = inBool.isOn;
        SettingsChanged();
    }

    #endregion

    #region AudioTab

    public void AudioTab()
    {
        AudioTabPanel.SetActive(true);

        /*Vector3 v = AudioTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position;
        AudioTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position = new Vector3(v.x, 0.0f, v.z);*/

        StartCoroutine(DoSomething(AudioTabPanel));

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
        if (player)
            player.MasterVolume = inFloat.value;
        MasterDisplay.text = inFloat.value.ToString("F3");

        settings.masterVolume = inFloat.value;
        SettingsChanged();
    }

    public void UpdateUIVolume(Slider inFloat)
    {
        if (player)
            player.UIVolume = inFloat.value;
        UIDisplay.text = inFloat.value.ToString("F3");

        settings.uiVolume = inFloat.value;
        SettingsChanged();
    }

    #endregion

    #region KeybindTab

    public void KeybindTab()
    {
        KeybindTabPanel.SetActive(true);

        /*Vector3 v = KeybindTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position;
        KeybindTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position = new Vector3(v.x, 0.0f, v.z);*/

        StartCoroutine(DoSomething(KeybindTabPanel));

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

        /*Vector3 v = PerformanceTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position;
        PerformanceTabPanel.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>().position = new Vector3(v.x, 0.0f, v.z);*/

        StartCoroutine(DoSomething(PerformanceTabPanel));

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

        float resolution = mapToRange(res, 0.0f, 8.0f, 0.0f, 1.0f) + mapToRange(x, 0.0f, 2.0f, 0.0f, 0.5f);

        //int width = Mathf.RoundToInt(Screen.currentResolution.width * resolution);
        // int height = Mathf.RoundToInt(Screen.currentResolution.height * resolution);

        //Screen.SetResolution(width, height, true);

        if (renderCtrl)
            renderCtrl.UpdateResolution(resolution);
        ResolutionDisplay.text = inFloat.value.ToString("F2");

        //Debug.Log(QualitySettings.resolutionScalingFixedDPIFactor);

        //QualitySettings.resolutionScalingFixedDPIFactor = resolution;

        //Debug.Log(QualitySettings.resolutionScalingFixedDPIFactor);

        settings.resolutionScale = inFloat.value;
        SettingsChanged();
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

        settings.toggleVSync = inBool.isOn;
        SettingsChanged();
    }

    public void UpdateRenderDistance(Slider inFloat)
    {
        //player.FPSCamera.farClipPlane = inFloat.value;
        if (player)
            player.EnvCamera.farClipPlane = inFloat.value;
        RenderDisplay.text = inFloat.value.ToString("F2");

        settings.renderDistance = inFloat.value;
        SettingsChanged();
    }

    public void ChangeModelQuality(int selection)
    {
        if(settings.modelQuality != selection || !Started)
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

            settings.modelQuality = selection;
            SettingsChanged();
        }
    }

    public void ChangeTextureQuality(int selection)
    {
        if (settings.textureQuality != selection || !Started)
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

            QualitySettings.masterTextureLimit = selection;

            settings.textureQuality = selection;
            SettingsChanged();
        }
    }

    public void ChangeShadowQuality(int selection)
    {
        if (settings.shadows != selection || !Started)
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

            settings.shadows = selection;
            SettingsChanged();
        }
    }

    public void ChangeShadowQuality2(int selection)
    {
        if (settings.shadowQuality != selection || !Started)
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

            settings.shadowQuality = selection;
            SettingsChanged();
        }
    }

    public void ChangeMSAA(int selection)
    {
        if (settings.MSAA != selection || !Started)
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

            QualitySettings.antiAliasing = selection;

            settings.MSAA = selection;
            SettingsChanged();
        }
    }

    public void ChangeAnis(int selection)
    {
        if (settings.anisotropicFiltering != selection || !Started)
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

            settings.anisotropicFiltering = selection;
            SettingsChanged();
        }
    }

    public void UpdateShadowDistance(Slider inFloat)
    {
        QualitySettings.shadowDistance = inFloat.value;
        ShadowDisplay.text = inFloat.value.ToString("F2");

        settings.shadowDistance = inFloat.value;
        SettingsChanged();
    }

    public void ChangeShadowCascades(int selection)
    {
        if (selection != settings.shadowCascades || !Started)
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

            settings.shadowCascades = selection;
            SettingsChanged();
        }
    }

    public void UpdateAmbientOc(Toggle inBool)
    {
        if (inBool.isOn != settings.ambientOcclusion || !Started)
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

                settings.ambientOcclusion = inBool.isOn;
                SettingsChanged();
            }
            catch
            {
                Debug.Log("No AO post processing found");
                return;
            }
        }
    }

    public void UpdateDOF(Toggle inBool)
    {
        if(inBool.isOn != settings.DOF || !Started)
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

                settings.DOF = inBool.isOn;
                SettingsChanged();
            }
            catch
            {
                Debug.Log("No DOF post processing found");
                return;
            }
        }
    }

    #endregion
}
