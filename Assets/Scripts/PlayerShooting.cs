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
    private KeycodeManager keycodeManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ballController = FindObjectOfType<BallController>();
        shootingIndicator = GetComponentInChildren<ShootingIndicator>();
        playerController = GetComponent<PlayerController>();
        _photonView = GetComponent<PhotonView>();
        shootingIndicator.Init();
    }

    void Update()
    {
        if (!_photonView.AmOwner || keycodeManager == null)
            return;
        ManageShooting();
    }

    private void ManageShooting()
    {
        if (Input.GetKeyDown(keycodeManager.ShootKeyCode))
            StartShooting();
        if (isShooting)
        {
            if (Input.GetKey(keycodeManager.ShootKeyCode))
                HandleShot();
            if (Input.GetKeyUp(keycodeManager.ShootKeyCode))
                TryToKick();
            if (Input.GetKeyDown(keycodeManager.StopShootingKeyCode))
                ResetShooting();
        }
    }

    private void StartShooting()
    {
        shootingIndicator.Enable();
        isShooting = true;
    }

    private void HandleShot()
    {
        if (Input.GetKey(keycodeManager.CurveKeyCode))
            shotType = ShotType.CURVED;
        if (Input.GetKey(keycodeManager.ChipKeyCode))
            shotType = ShotType.CHIPPED;
        if (Input.GetKey(keycodeManager.FlatKeyCode))
            shotType = ShotType.FLAT;
        if (shotPower < 100)
            shotPower += 50 * Time.deltaTime;
        shootingIndicator.UpdateColor(shotPower);
    }

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
    public void SetKeyCodeManager(KeycodeManager keycodeManager)
    {
        this.keycodeManager = keycodeManager;
    }

}
