using System.IO;
using System;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// TODO: APPLY SETTINGS OR REVERT THEM
public class SettingsManager : MonoBehaviour
{
    enum ButtonType{NONE, SHOOT, FLAT, CURVE, CHIP, STOP};
    public static SettingsManager instance;
    private KeycodeManager keycodeManager;
    private CustomKeycodes customKeycodes, customKeycodesCopy;
    [SerializeField] private Slider brightnessSlider, volumeSlider;
    [SerializeField] private Button shootButton, flatButton, curveButton, chippedButton, stopButton;
    ButtonType chosenButtonType = ButtonType.NONE;
    private Button chosenButton;
    [SerializeField]private int animationTime, defaultBrightness, defaultVolume;
    public Settings settings;
    public int brightness{get;set;}
    public int volume{get;set;}
    public bool isChangingSettings{get;set;}
    public delegate void UpdateSettingsDelegate();
    public UpdateSettingsDelegate updateSettingsDelegate;
    //TODO: possibly check if user made change and make a popup appear if he will try to exit settings without applying
    private bool isMappingKey = false, applied = false, madeChanges = false;
    private int brightnessCopy, volumeCopy;
    private WaitForSeconds waitForSeconds = new WaitForSeconds(0.01f);

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
        {
            instance = this;
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        ReadFiles();
        InitializeSettings();
        updateSettingsDelegate.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(isMappingKey);
        if (!isMappingKey)
            return;
        TryToGetKey();
    }

    private void TryToGetKey()
    {
        foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                if(keyCode == KeyCode.Escape)
                {
                    StartCoroutine(DelayForEscape());
                    break;
                }
                MapButton(keyCode);
            }
        }
    }
    private void ReadFiles()
    {
        
        keycodeManager = KeycodeManager.CreateFromJSON(FileManager.instance.ReadFromPlayerInputFile());
        FileManager.instance.SaveToPlayerInputFile(keycodeManager);
        string settingsJsonString = FileManager.instance.ReadFromSettingsFile();
        if(settingsJsonString != "")
            settings = JsonUtility.FromJson<Settings>(settingsJsonString);
        else
            settings = new Settings(defaultVolume, defaultBrightness);
        volume = settings.volume;
        brightness = settings.brightness;
    }
    private void SaveFiles()
    {
        FileManager.instance.SaveToPlayerInputFile(keycodeManager);
        FileManager.instance.SaveToSettingsFile(settings);
    }
    private void InitializeSettings()
    {
        brightnessSlider.value = brightness;
        brightnessSlider.onValueChanged.AddListener(UpdateBrightness);
        volumeSlider.value = volume;
        volumeSlider.onValueChanged.AddListener(UpdateVolume);
        shootButton.onClick.AddListener(() => {SelectButton(shootButton, ButtonType.SHOOT);});
        shootButton.GetComponentInChildren<TextMeshProUGUI>().text = keycodeManager.ShootKeyCode.ToString();
        flatButton.onClick.AddListener(() => {SelectButton(flatButton, ButtonType.FLAT);});
        flatButton.GetComponentInChildren<TextMeshProUGUI>().text = keycodeManager.FlatKeyCode.ToString();
        curveButton.onClick.AddListener(() => {SelectButton(curveButton, ButtonType.CURVE);});
        curveButton.GetComponentInChildren<TextMeshProUGUI>().text = keycodeManager.CurveKeyCode.ToString();
        chippedButton.onClick.AddListener(() => {SelectButton(chippedButton, ButtonType.CHIP);});
        chippedButton.GetComponentInChildren<TextMeshProUGUI>().text = keycodeManager.ChipKeyCode.ToString();
        stopButton.onClick.AddListener(() => {SelectButton(stopButton, ButtonType.STOP);});
        stopButton.GetComponentInChildren<TextMeshProUGUI>().text = keycodeManager.StopShootingKeyCode.ToString();
    }
    private void UpdateButtons()
    {
        shootButton.GetComponentInChildren<TextMeshProUGUI>().text = keycodeManager.ShootKeyCode.ToString();
        flatButton.GetComponentInChildren<TextMeshProUGUI>().text = keycodeManager.FlatKeyCode.ToString();
        curveButton.GetComponentInChildren<TextMeshProUGUI>().text = keycodeManager.CurveKeyCode.ToString();
        chippedButton.GetComponentInChildren<TextMeshProUGUI>().text = keycodeManager.ChipKeyCode.ToString();
        stopButton.GetComponentInChildren<TextMeshProUGUI>().text = keycodeManager.StopShootingKeyCode.ToString();
    }
    private void UpdateSliders()
    {
        brightnessSlider.value = brightness;
        volumeSlider.value = volume;
    }
    private void MapButton(KeyCode keyCode)
    {
        KeyCode oldKeycode;
        switch(chosenButtonType){

            case ButtonType.SHOOT:
            oldKeycode = keycodeManager.ShootKeyCode;
            keycodeManager.ShootKeyCode = keyCode;
            shootButton.GetComponentInChildren<TextMeshProUGUI>().text = keyCode.ToString();
            break;

            case ButtonType.FLAT:
            oldKeycode = keycodeManager.FlatKeyCode;
            keycodeManager.FlatKeyCode = keyCode;
            flatButton.GetComponentInChildren<TextMeshProUGUI>().text = keyCode.ToString();
            break;

            case ButtonType.CURVE:
            oldKeycode =  keycodeManager.CurveKeyCode;
            keycodeManager.CurveKeyCode = keyCode;
            curveButton.GetComponentInChildren<TextMeshProUGUI>().text = keyCode.ToString();
            break;

            case ButtonType.CHIP:
            oldKeycode = keycodeManager.ChipKeyCode;
            keycodeManager.ChipKeyCode = keyCode;
            chippedButton.GetComponentInChildren<TextMeshProUGUI>().text = keyCode.ToString();
            break;

            case ButtonType.STOP:
            oldKeycode = keycodeManager.StopShootingKeyCode;
            keycodeManager.StopShootingKeyCode = keyCode;
            stopButton.GetComponentInChildren<TextMeshProUGUI>().text = keyCode.ToString();
            break;
            
            default:
            oldKeycode = KeyCode.None;
            break;
        }
        if(oldKeycode != keyCode)
            madeChanges = true;
        isMappingKey = false;
    }
    public void OnRevertSettings()
    {
        keycodeManager.UpdateKeyCodes(customKeycodesCopy);
        UpdateButtons();
        brightness = brightnessCopy;
        volume = volumeCopy;
        UpdateSliders();
        updateSettingsDelegate.Invoke();
    }
    public KeycodeManager GetKeycodeManager()
    {
        return keycodeManager;
    }
    public void OnApplyButton()
    {
        applied = true;
        settings.brightness = brightness;
        settings.volume = volume;
        
        SaveFiles();
    }

    public void OnExitSettings()
    {
        if(applied)
            applied = false;
        else{
            OnRevertSettings();
        }
        isChangingSettings = false;

    }
    public void OnOpenSettings()
    {
        brightnessCopy = brightness;
        volumeCopy = volume;
        customKeycodesCopy = keycodeManager.GetCustomKeycodes();
    }
    public void UpdateBrightness(float value)
    {
        madeChanges = true;
        brightness = (int)value;
        updateSettingsDelegate.Invoke();
    }
    public void UpdateVolume(float value)
    {
        madeChanges = true;
        volume = (int)value;
        updateSettingsDelegate.Invoke();
    }
    private void ResetButton()
    {

        ColorBlock colorBlock = chosenButton.colors;
        colorBlock.normalColor = Color.white;
        colorBlock.selectedColor = Color.white;
        chosenButton.colors = colorBlock;
        chosenButton = null;
        chosenButtonType = ButtonType.NONE;
    }
    private void SelectButton(Button button, ButtonType buttonType)
    {
        chosenButton = button;
        chosenButtonType = buttonType;
        isMappingKey = true;
        StartCoroutine(PlayAnimation());
        
    }
    private IEnumerator PlayAnimation()
    {
        // Debug.Log(chosenButton.gameObject.name);
        ColorBlock colorBlock = chosenButton.colors;
        // colorBlock.normalColor = Color.black;
        chosenButton.colors = colorBlock;
        int direction = -1;
        yield return waitForSeconds;
        while (isMappingKey)
        {
            colorBlock.normalColor = new Color(colorBlock.normalColor.r, colorBlock.normalColor.g + direction/127f, colorBlock.normalColor.b + direction/127f);
            colorBlock.selectedColor = new Color(colorBlock.selectedColor.r, colorBlock.selectedColor.g + direction/127f, colorBlock.selectedColor.b + direction/127f);
            chosenButton.colors = colorBlock;
            if (colorBlock.selectedColor.g <= 0.6f || colorBlock.selectedColor.g >= 0.999f)
                direction = -direction;
            yield return waitForSeconds;
        }
        ResetButton();
    }
    private IEnumerator DelayForEscape()
    {
        yield return waitForSeconds;
        isMappingKey = false;
    }
    public bool CanBeClosed()
    {
        return !isMappingKey;
    }
}
