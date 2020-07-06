using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSettingsFunctions : MonoBehaviour
{
    public PlayerController player;

    [Header("Input Reference")]
    public Slider FovSlider;

    [Space]
    public Slider SensXSlider;
    public Slider SensYSlider;
    public Slider MultiXSlider;
    public Slider MultiYSlider;

    [Space]
    public Toggle ADSToggle;
    public Toggle CrouchToggle;

    [Space]
    public TMPro.TextMeshProUGUI FovDisplay;
    public TMPro.TextMeshProUGUI SensXDisplay;
    public TMPro.TextMeshProUGUI SensYDisplay;
    public TMPro.TextMeshProUGUI MultiXDisplay;
    public TMPro.TextMeshProUGUI MultiYDisplay;

    public void UpdateFOV(Slider inFloat)
    {
        player.FOV = inFloat.value;
        FovDisplay.text = inFloat.value.ToString("F0");
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
}
