using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorScript : MonoBehaviour
{
    [SerializeField] int rotSpeed;
    void FixedUpdate()
    {
        this.transform.Rotate(0,rotSpeed * Time.deltaTime, 0 , Space.Self);
    }
}
