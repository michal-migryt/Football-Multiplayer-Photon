using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class GameController : MonoBehaviourPunCallbacks, IPunObservable
{
    
    public int TeamLeftGoals {get;private set;}
    public int TeamRightGoals {get;private set;}
    public List<PlayerController> teamOne = new(), teamTwo = new();
    public int GameTime {get; private set;}
    public bool IsGamePlayed {get; private set;}
    
    public static GameController instance;
    private UIManager uiManager;
    private CameraManager cameraManager;
    [Header("Times")]
    [SerializeField] private int maxGameTime=600;
    [Tooltip("Time in which scoring team is supposed to celebrate. After that players and ball will be reseted.")]
    public float celebrationTime;
    private WaitForSeconds uiUpdateTime = new WaitForSeconds(1f);
    [Header("")]
    [SerializeField] private Materials materials;
    private PhotonView _photonView;
    private void Awake() {
        if(instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    void Start()
    {
        StartCoroutine(ManageGameTime());
        cameraManager = FindObjectOfType<CameraManager>();
        _photonView = GetComponent<PhotonView>();
        uiManager = FindObjectOfType<UIManager>();
        if(photonView.AmOwner){
        InitializeGame();
        IsGamePlayed = false;
        uiManager.SetStartButtonVisible(true);
        uiManager.SetWaitPanelVisible(false);
        }
        else{
            uiManager.SetStartButtonVisible(false);
            uiManager.SetWaitPanelVisible(true);
        }
        uiManager.SetupUI();
        //PhotonNetwork.InstantiateRoomObject(ballPrefab.name, new Vector3(0,0,0), Quaternion.identity);
    }
    void Update()
    {
        cameraManager.FollowBall();
        if(Input.GetKeyDown(KeyCode.Escape))
            uiManager.OnEscapeInput();
    }

    void InitializeGame()
    {
        GameTime = 0;
        TeamLeftGoals = 0;
        TeamRightGoals = 0;
        //IsGamePlayed = true;
    }
    public void StartGame()
    {
        uiManager.SetStartButtonVisible(false);
        photonView.RPC("RPC_GameStart", RpcTarget.AllBuffered);
    }

    // TODO: Find player controller associated with leaving player
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Happens");
        PlayerController[] playerControllers = FindObjectsOfType<PlayerController>();
        Debug.Log(playerControllers.Length);
        List<PlayerController> newTeamOne = new(), newTeamTwo = new();
        for(int i=0;i<playerControllers.Length;i++)
        {
            if(teamOne.Contains(playerControllers[i]))
            {
                newTeamOne.Add(playerControllers[i]);
                continue;
            }
            if(teamTwo.Contains(playerControllers[i]))
            {
                newTeamTwo.Add(playerControllers[i]);
            }
        }
        teamOne.Clear();
        teamTwo.Clear();
        newTeamOne.ForEach((x) => teamOne.Add(x));
        newTeamTwo.ForEach((x) => teamTwo.Add(x));
    }
    public IEnumerator ManageGameTime()
    {
        while(true){
            if(GameTime >= maxGameTime)
                break;  
            yield return uiUpdateTime;
            if(IsGamePlayed)
            {
            uiManager.UpdateUI(GameTime, TeamLeftGoals, TeamRightGoals);
            GameTime += 1;
            }
            
        }
        //Time.timeScale = 0;
    }
    public void OnPlayerSpawn(int photonId)
    {
        //int photonId = playerGameObject.GetComponent<_photonView>().ViewID;
        //ERROR: doesn't see game object need to make it pass viewId or something
        _photonView.RPC("RPC_OnPlayerSpawn", RpcTarget.MasterClient, photonId);
        
        
    }
    public void OnGoalScored(GoalType goalType)
    {
        if(_photonView.IsMine){
        if (goalType == GoalType.LEFTGOAL)
        {
            TeamRightGoals += 1;     
        }
        else
        {
            TeamLeftGoals +=1;
        }
        //int temp = (int) goalType;
        //_photonView.RPC("RPC_GoalScored",RpcTarget.OthersBuffered, TeamLeftGoals, TeamRightGoals);
        }
    }
    public void OnGoalScoredWithProperties(GoalType goalType)
    {
        if(_photonView.IsMine)
        {
            if (goalType == GoalType.LEFTGOAL)
        {
            TeamRightGoals += 1;     
        }
        else
        {
            TeamLeftGoals +=1;
        }
            Hashtable hash = new Hashtable();
            hash.Add("leftGoals", TeamLeftGoals);
            hash.Add("rightGoals", TeamRightGoals);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }
    public void ResetPlayerPositions()
    {
        int i;
        for(i=0;i<teamOne.Count;i++)
            {
                teamOne[i].GetPhotonView().RPC("RPC_ResetPlayerPosition", RpcTarget.All);
            }
            for(i=0;i<teamTwo.Count;i++)
            {
                teamTwo[i].GetPhotonView().RPC("RPC_ResetPlayerPosition", RpcTarget.All);
            }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(!_photonView.IsMine && targetPlayer == _photonView.Owner)
        {
            TeamRightGoals = (int)changedProps["rightGoals"];
            TeamLeftGoals = (int)changedProps["leftGoals"];
        }
    }
    [PunRPC] 
    public void RPC_GoalScored(int leftGoals, int rightGoals)
    {
        /*
        if(goalType == (int) GoalType.LEFTGOAL)
            TeamRightGoals += 1;
        else
            TeamLeftGoals += 1;
        */
        TeamRightGoals = rightGoals;
        TeamLeftGoals = leftGoals;
        
    }
    public void OnPlayerLeft(int viewId)
    {
        photonView.RPC("RPC_PlayerLeft", RpcTarget.AllBuffered, viewId);
    }
    public bool IsMenuOpen()
    {
        return uiManager.IsMenuOpen();
    }
    [PunRPC]
    public void RPC_PlayerLeft(int viewId)
    {
        PhotonView _photonView = PhotonNetwork.GetPhotonView(viewId);
        PlayerController playerController = _photonView.GetComponent<PlayerController>();
        if(teamOne.Contains(playerController))
            teamOne.Remove(playerController);
        else
            teamTwo.Remove(playerController);
    }
    [PunRPC]
    public void RPC_GameStart()
    {
        IsGamePlayed = true;
        uiManager.SetWaitPanelVisible(false);
    }
    [PunRPC]
    // TODO: fix spawn error when all player components aren't properly initialized
    public void RPC_OnPlayerSpawn(int viewId)
    {
        //GameObject playerGO = PhotonNetwork.GetPhotonView(viewId).GetComponent<MeshRenderer>();
        PhotonView _photonView = PhotonNetwork.GetPhotonView(viewId);
        MeshRenderer meshRenderer = _photonView.GetComponent<MeshRenderer>();
        PlayerController playerController = _photonView.GetComponent<PlayerController>();
        if(teamOne.Count <= teamTwo.Count)
        {
            playerController.SetTeam(Team.TEAMRED);
            meshRenderer.material = materials.red;
            teamOne.Add(playerController);
            this._photonView.RPC("RPC_OnPlayerSpawnOthers", RpcTarget.OthersBuffered, viewId, 0);
            
        }
        else{
            playerController.SetTeam(Team.TEAMBLUE);
            meshRenderer.material = materials.blue;
            teamTwo.Add(playerController);
            this._photonView.RPC("RPC_OnPlayerSpawnOthers", RpcTarget.OthersBuffered ,viewId, 1);
        }
        //playerController.ResetPlayerPosition();
        // TODO: this is bugged
        _photonView.RPC("RPC_ResetPlayerPosition", RpcTarget.All);
        
    }
    [PunRPC]
    public void RPC_OnPlayerSpawnOthers(int viewId, int teamAssigned)
    {
        PhotonView _photonView = PhotonNetwork.GetPhotonView(viewId);
        MeshRenderer meshRenderer = _photonView.GetComponent<MeshRenderer>();
        PlayerController playerController = _photonView.GetComponent<PlayerController>();
        if(teamAssigned == 0)
        {
            playerController.SetTeam(Team.TEAMRED);
            meshRenderer.material = materials.red;
            teamOne.Add(playerController);
        }
        else{
            playerController.SetTeam(Team.TEAMBLUE);
            meshRenderer.material = materials.blue;
            teamTwo.Add(playerController);
        }
    }
    [PunRPC]
    public void RPC_TimePassed()
    {
        GameTime += 1;
    }
    [PunRPC]
    public void RPC_UpdateTime(int time)
    {
        GameTime = time;
    }
    void IPunObservable.OnPhotonSerializeView(Photon.Pun.PhotonStream stream, Photon.Pun.PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(TeamLeftGoals);
            stream.SendNext(TeamRightGoals);
            stream.SendNext(GameTime);
        }
        if(stream.IsReading)
        {
            TeamLeftGoals = (int) stream.ReceiveNext();
            TeamRightGoals = (int) stream.ReceiveNext();
            GameTime = (int) stream.ReceiveNext();
        }
    }

}
