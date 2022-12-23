using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;
public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField inputField;
    public TextMeshProUGUI versionText;
    private void Start() {
        versionText.text = "v" + Application.version;
    }
    public void OnCreateButton()
    {
        PhotonNetwork.CreateRoom(inputField.text);
    }
    public void OnJoinButton()
    {
        PhotonNetwork.JoinRoom(inputField.text);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(2);
    }
    public override void OnLeftRoom()
    {

    }

    
}
