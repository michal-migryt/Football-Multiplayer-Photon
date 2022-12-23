using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class ConnectToServer : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    public override void OnConnectedToMaster()
    {
        
        PhotonNetwork.JoinLobby();
        //SceneManager.LoadScene(1);
    }
    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene(1);
        //PhotonNetwork.CreateRoom("room");
    }

    

}
