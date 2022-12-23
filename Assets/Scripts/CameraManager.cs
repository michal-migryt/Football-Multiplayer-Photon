using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform ballTransform;
    [SerializeField] private float cameraHeight;
    private Vector3 ballPos;

    public void FollowBall()
    {
        ballPos = ballTransform.position;
        // if(Mathf.Abs(ballPos.x - cameraTransform.position.x) > 0.01)
        //     cameraTransform.position -= new Vector3(cameraTransform.position.x - ballPos.x, 0, 0)*Time.deltaTime;
        float tempZ = ballPos.z - cameraTransform.position.z;
        // if(tempZ > 1.94)
        // {
        //     cameraTransform.position += new Vector3(0, 0, Mathf.Abs(tempZ) )*Time.deltaTime;
        //     return;
        // }
        // if(tempZ < 1.935)
        //     cameraTransform.position -= new Vector3(0, 0, Mathf.Abs(tempZ) )*Time.deltaTime;
        Vector3 lerpVector =  Vector3.Lerp(cameraTransform.position, ballPos - new Vector3(0, 0, tempZ), 0.1f);
        cameraTransform.position = new Vector3(lerpVector.x, cameraTransform.position.y, lerpVector.z);
    }
}
