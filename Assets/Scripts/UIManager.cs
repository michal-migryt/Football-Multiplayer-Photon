using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
public class UIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI timerText, scoreLineText;
    [SerializeField] private Button startButton;
    [SerializeField] private GameObject waitPanel, menuPanel, settingsPanel;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private RectTransform canvas;
    private Camera mainCamera;

    public void SetupUI()
    {
        timerText.text = "00:00";
        scoreLineText.text = "0:0";
        mainCamera = Camera.main;
    }

    // Update is called once per frame

    public void UpdateUI(int time, int goalsLeft, int goalsRight)
    {
        // format seconds into timespan
        TimeSpan result = TimeSpan.FromSeconds(time);
        timerText.text = result.ToString("mm':'ss");
        scoreLineText.text = goalsLeft.ToString()+":"+goalsRight.ToString();
    }
    public void SetStartButtonVisible(bool state)
    {
        startButton.gameObject.SetActive(state);
    }
    public void SetWaitPanelVisible(bool state)
    {
        waitPanel.SetActive(state);
    }
    public void OnStartButton()
    {
        GameController.instance.StartGame();
    }
    // Calculates ui element position relatively to object pos
    public Vector3 CalculateUIPosition(Vector3 objectPos, Vector3 displacement)
    {
        Vector3 screen = mainCamera.WorldToScreenPoint(objectPos + displacement);
        screen.z = (canvas.transform.position - uiCamera.transform.position).magnitude;
        Vector3 uiObjectPosition = uiCamera.ScreenToWorldPoint(screen);
        return uiObjectPosition;
    }
    public void OnExitToMenuButton()
    {
        //TODO: Make OnPlayerExit method that will remove player from team he was in
        //GameController.instance.OnPlayerExit();
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(1);
    }
    public void ChangeMenuVisibility()
    {
        menuPanel.SetActive(!menuPanel.activeSelf);
    }
    // when player gets Escape key down
    public void OnEscapeInput()
    {
        Debug.Log(SettingsManager.instance.CanBeClosed());
        if(settingsPanel.activeSelf && SettingsManager.instance.CanBeClosed())
        {
            settingsPanel.SetActive(false);
            SettingsManager.instance.OnExitSettings();
            menuPanel.SetActive(true);
            return;
        }
        if(menuPanel.activeSelf)
            menuPanel.SetActive(false);
        else
            menuPanel.SetActive(true);
        
    }
    public void OnSettingsButton()
    {
        settingsPanel.SetActive(true);
        menuPanel.SetActive(false);
        SettingsManager.instance.OnOpenSettings();
    }
    public void OnExitGame()
    {
        Application.Quit();
    }
    public bool IsMenuOpen()
    {
        return menuPanel.activeSelf || settingsPanel.activeSelf;
    }
}
