using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerTriggers : MonoBehaviour
{
    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.CompareTag("Switch"))
        {
            if (hit.gameObject.GetComponent<SwitchScript>().objectToOpen.CompareTag("Door"))
            {
                GameObject door = hit.gameObject.GetComponent<SwitchScript>().objectToOpen;
                door.GetComponent<DoorManager>().OpenDoor();
            }
            else if (hit.gameObject.GetComponent<SwitchScript>().objectToOpen.CompareTag("LaserDoor"))
            {
                GameObject laserDoor = hit.gameObject.GetComponent<SwitchScript>().objectToOpen;
                laserDoor.GetComponent<DoorManager>().OpenLaserDoorPermanently();
            }

        }

        if (hit.gameObject.CompareTag("PressurePlate"))
        {
            if (hit.gameObject.CompareTag("PressurePlate") && hit.gameObject.GetComponent<SwitchScript>().objectToOpen != null)
            {
                GameObject laserDoor = hit.gameObject.GetComponent<SwitchScript>().objectToOpen;
                laserDoor.GetComponent<DoorManager>().OpenLaserDoorTemporarily();
            }
        }

        if(hit.gameObject.CompareTag("CameraCone"))
        {
            EnemyManager enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
            enemyManager.AlertNearestEnemy(this.transform.parent.gameObject);
            Debug.Log("Seen by camera");
            hit.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            hit.gameObject.transform.parent.parent.parent.GetComponent<Animator>().speed = 0;
        }

        if(hit.gameObject.CompareTag("VisibleLoot"))
        {
            hit.transform.parent.parent.gameObject.SetActive(false);
            GameObject.FindObjectOfType<ScoreManager>().GetComponent<ScoreManager>().UpdateScore();
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("PressurePlate") && other.gameObject.GetComponent<SwitchScript>().objectToOpen != null)
        {
            GameObject laserDoor = other.gameObject.GetComponent<SwitchScript>().objectToOpen;
            laserDoor.GetComponent<DoorManager>().CloseLaserDoor();
        }
        if(other.gameObject.CompareTag("CameraCone"))
        {
            other.gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
            other.gameObject.transform.parent.parent.parent.GetComponent<Animator>().speed = 1;
        }


    }

   private void OnCollisionEnter(Collision collision)
   {
        if (collision.gameObject.CompareTag("Portal"))
        {
            Vector3 portalStartPos = collision.gameObject.GetComponent<PortalScript>().t_portalStartPos.position;
            Vector3 portalEndPos = collision.gameObject.GetComponent<PortalScript>().t_portalEndPos.position;

            if (Vector3.Distance(this.transform.position, portalStartPos) < Vector3.Distance(this.transform.position, portalEndPos))
            {
                this.transform.parent.transform.position = new Vector3(portalEndPos.x, 0.14f, portalEndPos.z);
            }
            else
            {
                 this.transform.parent.transform.position = new Vector3(portalStartPos.x, 0.14f, portalStartPos.z);
            }
        }

        if(collision.gameObject.CompareTag("FinalLoot"))
        {
            GameObject.FindObjectOfType<ScoreManager>().UpdateScore();
            GameObject.FindObjectOfType<NavScript>().SyncedLoadLevel("End");
        }



    }

}
