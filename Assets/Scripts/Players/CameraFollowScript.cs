using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CameraFollowScript : MonoBehaviour
{
    PhotonView pv;
    public Transform target_Player;
    [SerializeField] float followSpeed;
    [SerializeField] float lookSpeed;
    [SerializeField] Vector3 offset;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
    }
    private void LateUpdate()
    {
        LookAtTarget();
        SmoothFollowTarget();
    
    }
    void LookAtTarget()
    {
          Vector3 lookDirection = target_Player.position - this.transform.position;
          Quaternion rot = Quaternion.LookRotation(lookDirection, Vector3.up);
          transform.rotation = Quaternion.Lerp(transform.rotation, rot, lookSpeed * Time.deltaTime);

    }

    void SmoothFollowTarget()
    {
        Vector3 targetPos = target_Player.position
                            + target_Player.forward * offset.z
                            + target_Player.right * offset.x
                            + target_Player.up * offset.y;

        this.transform.position = Vector3.Lerp(this.transform.position, targetPos, followSpeed * Time.deltaTime);
    }
}
