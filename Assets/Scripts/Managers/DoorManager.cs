using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public void OpenDoor()
    {
        this.gameObject.SetActive(false);
    }

    public void OpenLaserDoorPermanently()
    {
        this.gameObject.SetActive(false);
    }

    public void OpenLaserDoorTemporarily()
    {
        this.gameObject.SetActive(false);
    }

    public void CloseLaserDoor()
    {
        this.gameObject.SetActive(true);
    }


}
