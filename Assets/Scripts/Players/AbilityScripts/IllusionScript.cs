using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class IllusionScript : MonoBehaviour
{
    PhotonView pv;
    Animator animator;
    GameObject illusion;
 
    Vector3 abilityLocalPos;
    Quaternion abilityLocalRot;

    [SerializeField] float illusionSpeed;
    [SerializeField] float illusionCooldownPeriod;

    Transform t_player;

    int timesAbilityUsed =-1;

    public bool isStunned = false;
    void Awake()
    {
        pv = GetComponent<PhotonView>();    
        illusion = this.transform.Find("Illusion").gameObject;
        animator = illusion.GetComponent<Animator>();      
        illusion.SetActive(false);   
    }

    void Start()
    {
        this.gameObject.SetActive(false);
    }

    IEnumerator ShootIllusion()
    {
        this.transform.SetParent(t_player.transform.parent);
        animator.SetTrigger("Run");
        animator.ResetTrigger("Stun");
        StartCoroutine(DestroyIllusion());

        while (!isStunned)
        {
            this.transform.position += transform.forward * illusionSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        

    }

    void OnEnable()
    {
        isStunned = false;
        t_player = this.transform.parent;
        abilityLocalPos = this.transform.localPosition;
        abilityLocalRot = this.transform.localRotation;

        timesAbilityUsed++;
        if(timesAbilityUsed > 0)
        {
            InitiateIllusion();
        }
    }
    public void InitiateIllusion()
    {

        illusion.SetActive(true);
        StartCoroutine(ShootIllusion());
    }

    public void StunIllusion()
    {
        isStunned = true;
        StartCoroutine(InitiateStun());    
    }

    IEnumerator InitiateStun()
    {
        StopCoroutine(DestroyIllusion());
        animator.SetTrigger("Stun");
        animator.ResetTrigger("Run");
        yield return new WaitForSeconds(illusionCooldownPeriod);
        illusion.SetActive(false);
        ResetAbility();
    }

    IEnumerator DestroyIllusion()
    {
        yield return new WaitForSeconds(illusionCooldownPeriod*2);       
        illusion.SetActive(false);
        isStunned = true; //to stop the illusion moving
        ResetAbility();
    }


    void ResetAbility()
    {
        StartCoroutine(t_player.GetComponent<PlayerScript>().AbilityCooldown());
        this.transform.SetParent(t_player);
        this.transform.localPosition = abilityLocalPos;
        this.transform.localRotation = abilityLocalRot;

    }
}
