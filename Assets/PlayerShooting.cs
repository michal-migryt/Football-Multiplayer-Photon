using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerShooting : MonoBehaviour
{
    private BallController ballController;
    private Rigidbody rb;
    private PlayerController playerController;
    private bool isShooting = false;
    private float shotPower = 10;
    private ShotType shotType = ShotType.NORMAL;
    private ShootingIndicator shootingIndicator;
    private PhotonView _photonView;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ballController = FindObjectOfType<BallController>();
        shootingIndicator = GetComponentInChildren<ShootingIndicator>();
        playerController = GetComponent<PlayerController>();
        _photonView = GetComponent<PhotonView>();
        shootingIndicator.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_photonView.AmOwner)
            return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartShooting();
        }
        if(isShooting)
        {
        if (Input.GetKey(KeyCode.Space))
            HandleShotPower();
        if (Input.GetKeyUp(KeyCode.Space))
        {
            TryToKick();
        }
        if(Input.GetKeyDown(KeyCode.S))
            ResetShooting();
        }
    }
    private void StartShooting()
    {
        // uiManager.EnableShotSlider(transform.position);
        shootingIndicator.Enable();
        isShooting = true;
    }
    private void HandleShotPower()
    {
        if(Input.GetKey(KeyCode.F))
            shotType = ShotType.CURVED;
        if(shotPower < 100)
            shotPower += 50 * Time.deltaTime;
        shootingIndicator.UpdateColor(shotPower);
    }
    // TODO: Possibly refractor into PlayerShooting class
    private void TryToKick()
    { 
        ballController.TryToKick(playerController, shotPower, shotType);
        ResetShooting();
    }
    private void ResetShooting()
    {
        isShooting = false;
        shootingIndicator.Disable();
        shotPower = 10;
        shotType = ShotType.NORMAL;
    }
    public bool IsShooting()
    {
        return isShooting;
    }
    
}
