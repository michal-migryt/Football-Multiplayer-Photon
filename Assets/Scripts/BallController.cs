using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum ShotType{NORMAL, CURVED, CHIPPED, FLAT}
public class BallController : MonoBehaviour
{
    [SerializeField] private Vector3 defaultPosition;
    [SerializeField] private float shootingDistance;
    [SerializeField] private int shotForceDivider;
    [SerializeField] private float curveModifier;
    private SphereCollider sphereCollider;
    private Rigidbody rb;
    private bool isInNet = false;
    private WaitForSeconds waitTime;
    private Vector3 shotForce;
    private PhotonView photonView;
    [SerializeField] private AudioSource kickAudioSource, hitBarAudioSource;
    // Start is called before the first frame update
    private void Awake() {
        sphereCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        
        photonView = GetComponent<PhotonView>();
        rb.maxAngularVelocity = 100f;
        kickAudioSource = GetComponent<AudioSource>();
        
    }
    void Start()
    {
        UpdateAudioSource();
        SettingsManager.instance.updateSettingsDelegate += UpdateAudioSource;
        waitTime = new WaitForSeconds(GameController.instance.celebrationTime);
    }

    public void TryToKick(PlayerController playerController, float shotPower, ShotType shotType)
    {
        Vector3 playerPosition = playerController.GetPosition();
        // its missing player collider size but works anyway so should i bother ?
        float distanceToBall = Vector3.Distance(transform.position, playerPosition) - sphereCollider.radius*transform.localScale.x;

        // is in range to shoot
        if(distanceToBall <= shootingDistance)
        {
            Vector3 tempShotForce = new Vector3((transform.position.x - playerPosition.x), (playerPosition.y - transform.position.y), (transform.position.z - playerPosition.z));
            tempShotForce.Normalize();
            shotForce = tempShotForce*shotPower/shotForceDivider;
            
            if(shotType == ShotType.NORMAL)
            {
                photonView.RPC("RPC_StrikeBall", RpcTarget.MasterClient, shotForce);
                return;
            }
            if(shotType == ShotType.CURVED)
            {
                shotForce *= 0.7f;
                photonView.RPC("RPC_CurveBall", RpcTarget.MasterClient, shotForce);
                return;
            }
            if(shotType == ShotType.CHIPPED)
            {
                shotForce = new Vector3(shotForce.x * 0.3f, shotForce.y * 2f, shotForce.z * 0.3f);
                photonView.RPC("RPC_StrikeBall", RpcTarget.MasterClient, shotForce);
                return;
            }
            if(shotType == ShotType.FLAT)
            {
                shotForce = new Vector3(shotForce.x, 0, shotForce.z);
                photonView.RPC("RPC_StrikeBall", RpcTarget.MasterClient, shotForce);
            }
        }
    }
    private void UpdateAudioSource()
    {
        // divided by 100 because volume is measured in int 0-100, while AudioSource volume is measured in float 0-1
        float volume = SettingsManager.instance.volume/100f;
        kickAudioSource.volume = volume;
        hitBarAudioSource.volume = volume;
    }
    [PunRPC]
    void RPC_StrikeBall(Vector3 shotForce)
    {
        rb.AddForce(shotForce, ForceMode.VelocityChange);
        kickAudioSource.Play();
    }
    // Scale curve with shotforce
    [PunRPC]
    void RPC_CurveBall(Vector3 shotForce)
    {
        float curveX = shotForce.z > 0 ? -100 : 100;
        rb.AddForce(shotForce, ForceMode.VelocityChange);
        rb.AddTorque(new Vector3(curveX, 0, 0), ForceMode.VelocityChange);
        kickAudioSource.Play();
    }
    private void OnTriggerEnter(Collider other) {
        if(photonView.AmOwner)
        {
        Goal goal = other.GetComponent<Goal>();
        if(goal && !isInNet)
        {
            goal.OnGoalScored();
            StartCoroutine(AfterGoalScored());
        }
        }
    }
    private void OnCollisionEnter(Collision other) {
         if(photonView.AmOwner){
        if (other.gameObject.tag.Equals("Wall"))
                {
                    //Vector3 collisionPoint = other.contacts[0].point;
                    Vector3 normal = other.GetContact(0).normal;
                    rb.velocity =  Vector3.Reflect(rb.velocity, normal);
                }
        if(other.gameObject.tag.Equals("Net"))
        {
                //rb.velocity = Vector3.zero;
                rb.velocity /= 95;
        }
        if(other.gameObject.tag.Equals("Bar"))
        {
            hitBarAudioSource.Play();
        }
         }
    }
    private IEnumerator AfterGoalScored()
    {
        isInNet = true;
        yield return waitTime;
        GameController.instance.ResetPlayerPositions();
        ResetPosition();
    }
    public void ResetPosition()
    {
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        rb.isKinematic = false;
        transform.position = defaultPosition;
        isInNet = false;
    }
    private void OnDrawGizmos() {
        Gizmos.DrawSphere(transform.position, shootingDistance + sphereCollider.radius*transform.localScale.x);
    }
}
