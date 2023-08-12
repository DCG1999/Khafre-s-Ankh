using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LootPulseScript : MonoBehaviour
{
    PhotonView pv;
    GameObject Pulse;
    Transform t_player;

    [SerializeField] float pulseRadius;
    [SerializeField] float pulseSpeed;
   // [SerializeField] float pulseActiveTime;

    Vector3 localAbilityScale;


    private int timesAbilityUsed = -1; 

    List<GameObject> lootFound= new List<GameObject> ();
    
    void Awake()
    {
        pv = GetComponent<PhotonView>();
       
        Pulse = this.transform.Find("Pulse").gameObject;
        Pulse.SetActive(false);

    }
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        localAbilityScale = this.transform.localScale;
        t_player = this.transform.parent;

        timesAbilityUsed++;

        if (timesAbilityUsed > 0)
        {
            InitiatePulse();
        }

    }

    void InitiatePulse()
    {
        Pulse.SetActive(true);
        StartCoroutine("ShootPulse");
    }

    IEnumerator ShootPulse()
    {
        Collider[] objectsInVicinity = Physics.OverlapSphere(this.transform.position, pulseRadius); 

        foreach(Collider c in objectsInVicinity)
        {
            if(c.gameObject.CompareTag("Loot"))
            {
                lootFound.Add(c.transform.Find("LootMesh").gameObject);
            }
        }

        while(this.transform.localScale.z < pulseRadius*2)
        {
            this.transform.localScale += new Vector3(1, 0, 1) * pulseSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }       
        ShowFoundLoot();
        ResetPulse();
        yield return null;
    }

    void ShowFoundLoot()
    {
        foreach(GameObject loot in lootFound)
        {
            ActivateLoot(loot);
        }
        lootFound.Clear();
    }

    void ActivateLoot(GameObject loot)
    {
        loot.gameObject.SetActive(true);
    }

    void ResetPulse()
    {
        StartCoroutine(t_player.GetComponent<PlayerScript>().AbilityCooldown());
        Pulse.SetActive(false);
        this.transform.localScale = localAbilityScale;

    }
}
