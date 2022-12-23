using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum Team { TEAMRED, TEAMBLUE };
public class PlayerController : MonoBehaviourPunCallbacks
{
    private BallController ballController;
    private GameController gameController;
    private float horizontal, vertical;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private Vector3 movement;
    private PhotonView _photonView;
    private Team myTeam;
    private PlayerShooting playerShooting;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        ballController = FindObjectOfType<BallController>();
        gameController = GameController.instance;
        _photonView = GetComponent<PhotonView>();
        playerShooting = GetComponent<PlayerShooting>();
        if (_photonView.AmOwner)
            OnSpawn();
    }
    // Update is called once per frame

    private void FixedUpdate()
    {
        if (!_photonView.AmOwner)
            return;
        ManageMovement();
    }
    private void ManageMovement()
    {
        if (gameController.IsGamePlayed)
        {
            horizontal = Input.GetAxis("Horizontal") * Time.deltaTime;
            vertical = Input.GetAxis("Vertical") * Time.deltaTime;
            movement = new Vector3(horizontal, 0, vertical) * 1.5f;
            if (playerShooting.IsShooting())
                movement *= 0.75f;
            rb.AddForce(movement, ForceMode.VelocityChange);
        }
    }
    public void ResetPlayerPosition()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }
        if (myTeam == Team.TEAMRED)
            transform.position = new Vector3(Random.Range(-1, -0.2f), 0.21f, Random.Range(-1, -0.2f));
        else
            transform.position = new Vector3(Random.Range(0.2f, 1), 0.21f, Random.Range(0.2f, 1));
        rb.isKinematic = false;
    }
    [PunRPC]
    public void RPC_ResetPlayerPosition()
    {
        if (_photonView != null && _photonView.IsMine)
        {
            ResetPlayerPosition();
        }
    }
    private void OnSpawn()
    {
        GameController.instance.OnPlayerSpawn(_photonView.ViewID);
    }

    public Team GetTeam()
    {
        return myTeam;
    }
    public void SetTeam(Team team)
    {
        myTeam = team;
    }
    public PhotonView GetPhotonView()
    {
        return _photonView;
    }
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    public CapsuleCollider GetCollider()
    {
        return capsuleCollider;
    }

}
