using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPScript : MonoBehaviour
{
    GameObject empShot;
    GameObject empShotEffect;
    Transform t_player;

    Vector3 abilityLocalPos;
    Quaternion abilityLocalRot;

    Vector3 empEffectLocalScale;

    [SerializeField] float empSpeed;
    [SerializeField] float empRadius;
    [SerializeField] float empCooldownPeriod;
    [SerializeField] float empEffectSpeed;

    bool empHitWall = false;

    List<GameObject> survCameras = new List<GameObject> ();

    int timesAbilityUsed = -1;
    void Awake()
    {      
        empShot = this.transform.Find("EMPShot").gameObject;
        empShotEffect = this.transform.Find("EMPEffect").gameObject;       
        empShot.SetActive(false);
        empShotEffect.SetActive(false); 

    }

    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public IEnumerator ShootEMP()
    {
        this.transform.SetParent(t_player.transform.parent);

        while (!empHitWall)
        {
            this.transform.position += transform.forward * empSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    void OnEnable()
    {
        empHitWall = false;
        t_player = this.transform.parent;
        abilityLocalPos = this.transform.localPosition;
        empEffectLocalScale = empShotEffect.transform.localScale;

        timesAbilityUsed++;
        if (timesAbilityUsed > 0)
        {
            InitiateEMP();
        }
    }

    public void InitiateEMP()
    {
        empShot.SetActive(true);
        StartCoroutine(ShootEMP());
    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.CompareTag("WeakWall")||coll.gameObject.CompareTag("StrongWall"))
        {
            //StopCoroutine(ShootEMP());  

            if (!empHitWall)
            {
                empHitWall = true;
                empShotEffect.SetActive(true);
                empShot.SetActive(false);

                Collider[] objectsInRadius = Physics.OverlapSphere(this.transform.position, empRadius);
              
             
                StartCoroutine(PortalEfect(objectsInRadius));
            }
        }
    }

    IEnumerator PortalEfect(Collider[] _objectsInRadius)
    {
        print("Portal effet called");
        while(empShotEffect.transform.localScale.z < empRadius*2)
        {
        //    Debug.Log("size increase");
            empShotEffect.transform.localScale += Vector3.one * empEffectSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        empShotEffect.SetActive(false);
        FindCamerasinVicinity(_objectsInRadius);
        yield return new WaitForEndOfFrame();   

    }

    void FindCamerasinVicinity(Collider[] _objectsInRadius)
    {
        foreach(Collider c in _objectsInRadius)
        {
            if(c.gameObject.CompareTag("survCam"))
            {
                survCameras.Add(c.gameObject);
            }
        }

        //Debug.Log(survCameras.Count);
        StartCoroutine(DisableCameras());
    }

    IEnumerator DisableCameras()
    {
        if (survCameras.Count > 0)
        {
            print("Cameras founds");
            foreach (GameObject cam in survCameras)
            {
                GameObject visCone = cam.transform.Find("Swivel").transform.Find("visionCone").gameObject;
                Debug.Log(cam.gameObject.name);
                visCone.SetActive(false);
                cam.GetComponent<Animator>().speed = 0;
            }
            yield return new WaitForSeconds(empCooldownPeriod);
            EnableCameras();
        }
        else
        {
            print(" no cameras found");
            survCameras.Clear();
            ResetEMP();
            yield return new WaitForEndOfFrame();
        }
    }

    public void EnableCameras()
    {
        foreach (GameObject cam in survCameras)
        {
            GameObject visCone = cam.transform.Find("Swivel").transform.Find("visionCone").gameObject;
            Debug.Log(cam.gameObject.name);
            visCone.SetActive(true);
            cam.GetComponent<Animator>().speed = 1;
        }

        survCameras.Clear();
        ResetEMP();
        
    }
       
    public void ResetEMP()
    {
        StartCoroutine(t_player.GetComponent<PlayerScript>().AbilityCooldown());
        this.transform.SetParent(t_player);
        this.transform.localPosition = abilityLocalPos;
        this.transform.localRotation = abilityLocalRot;
        empShotEffect.transform.localScale = empEffectLocalScale;
        empHitWall = false;
    }
    
}
