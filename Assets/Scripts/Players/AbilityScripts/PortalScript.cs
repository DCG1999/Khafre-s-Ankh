using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PortalScript : MonoBehaviour
{
    GameObject PortalShot;
    GameObject Portal;
    Transform t_player;
    public Transform t_portalStartPos;
    public Transform t_portalEndPos;

    Vector3 localAbilityPos;
    Quaternion localAbilityRot;
    bool portalHitWall;
    public float portalSpeed = 2f;

    [SerializeField]float portalDuration;


    private int timesAbilityUsed = -1;
    // Start is called before the first frame update
    void Awake()
    {
        Portal = this.transform.Find("FinalPortal").gameObject;
        PortalShot = this.transform.Find("PortalShot").gameObject;
        Portal.SetActive(false);
        PortalShot.SetActive(false);
    }

    private void Start()
    {
        this.gameObject.SetActive(false);
    }
    
    public void OnEnable()
    {
        this.GetComponent<Collider>().enabled = true;
        portalHitWall = false;
        t_player = this.transform.parent;
        localAbilityPos = this.transform.localPosition;
        localAbilityRot = this.transform.localRotation;

        timesAbilityUsed++;

        if(timesAbilityUsed > 0)
        {
            InitiatePortal();
        }

    }
    public IEnumerator ShootPortal()
    {
        this.transform.SetParent(t_player.transform.parent);
        while(!portalHitWall)
        {
            this.transform.position += transform.forward * portalSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return null;

    }
    public void InitiatePortal()
    {
        PortalShot.SetActive(true);
        StartCoroutine(ShootPortal());
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("WeakWall") && !portalHitWall)
        {
            portalHitWall = true;
            PortalShot.SetActive(false);
            Portal.SetActive(true);

            this.transform.position = new Vector3(collision.GetContact(0).point.x, 2.5f, collision.GetContact(0).point.z) + this.transform.forward*collision.transform.localScale.x/2;
            this.transform.rotation = collision.gameObject.transform.rotation;

            StartCoroutine(ActivePortalCooldown());
        }

        if(collision.gameObject.CompareTag("StrongWall") && !portalHitWall)
        {
            PortalShot.SetActive(false);
            portalHitWall = true;
            StopCoroutine(ShootPortal());
            ResetPortal();
        }
    }

    IEnumerator ActivePortalCooldown()
    {
        yield return new WaitForSeconds(portalDuration);
        ResetPortal();
    }
    void ResetPortal()
    {
        Portal.SetActive(false);
        PortalShot.SetActive(false);
        this.GetComponent<Collider>().enabled = false;
        this.transform.SetParent(t_player);
        this.transform.localPosition = localAbilityPos;
        this.transform.localRotation = localAbilityRot;

        StartCoroutine(t_player.GetComponent<PlayerScript>().AbilityCooldown());
      

    }
}
