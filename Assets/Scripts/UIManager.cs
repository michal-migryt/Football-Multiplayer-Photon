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
    [SerializeField] private GameObject waitPanel, menuPanel;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private ShotSlider shotSlider;
    private Camera mainCamera;
    public static UIManager instance;
    private void Awake() {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    public void SetupUI()
    {
        timerText.text = "00:00";
        scoreLineText.text = "0:0";
        mainCamera = Camera.main;
        shotSlider.InitializeShotSlider();

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
    public void EnableShotSlider(Vector3 playerPos)
    {
        Vector3 sliderNewPos = CalculateUIPosition(playerPos, new Vector3(0.1f, 0, 0));
        Debug.Log("Check 1");
        shotSlider.EnableShotSlider(sliderNewPos);
    }

    public void UpdateShotSlider(Vector3 playerPos, float shotPower)
    {
        Vector3 sliderNewPos = CalculateUIPosition(playerPos, new Vector3(0.1f, 0, 0));
        shotSlider.UpdateShotSlider(sliderNewPos, shotPower);
    }
    public void DisableShotSlider()
    {
        shotSlider.DisableShotSlider();
    }
    // Calculates ui element position relatively to object pos
    public Vector3 CalculateUIPosition(Vector3 objectPos, Vector3 displacement)
    {
        var screen = mainCamera.WorldToScreenPoint(objectPos + displacement);
        screen.z = (canvas.transform.position - uiCamera.transform.position).magnitude;
        var uiObjectPosition = uiCamera.ScreenToWorldPoint(screen);
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
    public void OnExitGame()
    {
        Application.Quit();
    }
}
