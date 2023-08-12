using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientMusicscript : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        //this.GetComponent<AudioSource>().clip;
        DontDestroyOnLoad(this.gameObject);
    }

}
