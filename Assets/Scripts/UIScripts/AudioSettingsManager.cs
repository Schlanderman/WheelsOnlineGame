using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [Header("UI Elemente")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private TMP_InputField masterInput;
    [SerializeField] private Slider uiSlider;
    [SerializeField] private TMP_InputField uiInput;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_InputField sfxInput;

    //Variablen für PlayerPrefs
    private const string MASTER_VOLUME_KEY = "MasterVolume";
    private const string UI_VOLUME_KEY = "UIVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    private void Start()
    {
        //Initialisieren mit gespeicherten Werten oder Standardwert 100
        masterSlider.value = PlayerPrefs.GetInt(MASTER_VOLUME_KEY, 100);
        uiSlider.value = PlayerPrefs.GetInt(UI_VOLUME_KEY, 100);
        sfxSlider.value = PlayerPrefs.GetInt(SFX_VOLUME_KEY, 100);

        //Werte in den InputFields setzen
        UpdateInputFields();

        //Events hinzufügen
        masterSlider.onValueChanged.AddListener(value => UpdateVolume(MASTER_VOLUME_KEY, value, masterInput));
        masterInput.onEndEdit.AddListener(value => UpdateFromInputField(MASTER_VOLUME_KEY, value, masterSlider));

        uiSlider.onValueChanged.AddListener(value => UpdateVolume(UI_VOLUME_KEY, value, uiInput));
        uiInput.onEndEdit.AddListener(value => UpdateFromInputField(UI_VOLUME_KEY, value, uiSlider));

        sfxSlider.onValueChanged.AddListener(value => UpdateVolume(SFX_VOLUME_KEY, value, sfxInput));
        sfxInput.onEndEdit.AddListener(value => UpdateFromInputField(SFX_VOLUME_KEY, value, sfxSlider));
    }

    private void UpdateVolume(string key, float value, TMP_InputField inputField)
    {
        int intValue = Mathf.RoundToInt(value);
        inputField.text = intValue.ToString();  //InputField aktualisieren
        PlayerPrefs.SetInt(key, intValue);
        PlayerPrefs.Save();
        ApplyVolumeSettings();
    }

    private void UpdateFromInputField(string key, string input, Slider slider)
    {
        if (int.TryParse(input, out int intValue))
        {
            intValue = Mathf.Clamp(intValue, 0, 100);   //Sicherstellen, dass der Wert im gewünschten Bereich ist
            slider.value = intValue;    //Slider-Wert aktualisieren
        }
    }

    private void UpdateInputFields()
    {
        masterInput.text = masterSlider.value.ToString();
        uiInput.text = uiSlider.value.ToString();
        sfxInput.text = sfxSlider.value.ToString();
    }

    private void ApplyVolumeSettings()
    {
        float masterVolume = (PlayerPrefs.GetInt(MASTER_VOLUME_KEY, 100) / 100f) + 0.0001f;
        float uiVolume = (PlayerPrefs.GetInt(UI_VOLUME_KEY, 100) / 100f) + 0.0001f;
        float sfxVolume = (PlayerPrefs.GetInt(SFX_VOLUME_KEY, 100) / 100f) + 0.0001f;

        //Lautstäre im Mixer anpassen
        AudioManager.Instance.SetVolume("ExposedMasterVolume", masterVolume);
        AudioManager.Instance.SetVolume("ExposedUIVolume", uiVolume);
        AudioManager.Instance.SetVolume("ExposedSFXVolume", sfxVolume);
    }
}
