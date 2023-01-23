using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerSpawner : MonoBehaviour
{
    [Tooltip("Player gameobject, that he will be controlling.")]
    [SerializeField] private GameObject playerPrefab;
    void Start()
    {
        Vector3 randomPos = new Vector3(50,0,0); 
        // if(GameController.instance.teamOne.Count < GameController.instance.teamTwo.Count)
        //     randomPos = new Vector3(Random.Range(-1,-0.2f), 0.21f, Random.Range(-1, -0.2f));
        // else
        //     randomPos = new Vector3(Random.Range(0.2f, 1), 0.21f, Random.Range(0.2f, 1));
        PhotonNetwork.Instantiate(playerPrefab.name, randomPos, Quaternion.identity);
        //GameController.instance.OnPlayerSpawn(go);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

}
