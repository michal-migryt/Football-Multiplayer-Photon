using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class GameUIVariables : MonoBehaviour, IPunObservable
{
    public int TeamLeftGoals {get;private set;}
    public int TeamRightGoals {get;private set;}
    public int GameTime {get; private set;}
    void Start()
    {
        TeamLeftGoals=0;
        TeamRightGoals=0;
        GameTime=0;
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
