using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerScript : MonoBehaviour
{
    PhotonView pv;
    Player player;
    PlayerSpawner playerSpawner;

    CharacterController charController;

    [SerializeField] float moveSpeed;
    [SerializeField] float rotSpeed;


    [SerializeField] float abilityCooldowntime;
    [SerializeField] float unStunCooldownTime;

    [SerializeField] float unStunDistace;

    GameObject ability;

    public bool isStunned = false;
    private bool canUseAbility = true;
    private bool canUnstun = true;

    GameObject UnStunDistViz = null;

    Animator animator;

    bool playerInitiated = false;

    // attack trigger not proper
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        charController = this.GetComponent<CharacterController>();
        ability = transform.Find("Ability").gameObject;
        animator = GetComponentInChildren<Animator>();  
        charController.detectCollisions = false;
}

    private void Start()
    {
        playerSpawner = GameObject.Find("PlayerSpawner").GetComponent<PlayerSpawner>();

        if(pv.IsMine)
        {
            if(playerSpawner.characterName == "Wizard")
            {
                player = new WizardScript(pv, charController, moveSpeed, rotSpeed, this.transform, animator);
            }
            else if (playerSpawner.characterName == "Techie")
            {
                player = new TechieScript(pv, charController, moveSpeed, rotSpeed, this.transform, animator);
            }
            else if (playerSpawner.characterName == "Illusionist")
            {
                player = new IllusionistScript(pv, charController, moveSpeed, rotSpeed, this.transform, animator);
            }
            else if(playerSpawner.characterName == "Thief")
            {
                player = new ThiefScript(pv, charController, moveSpeed, rotSpeed, this.transform, animator);
            }
            StartCoroutine(InitiatePlayer());
            playerInitiated = true;
        }
    }

    IEnumerator InitiatePlayer()
    {
        while (true)
        {
            if (!isStunned)
            {
                player.MovePlayer();

                if (Input.GetKey(KeyCode.Q) && canUseAbility)
                {
                    pv.RPC("ChangeAbilityStatus", RpcTarget.AllBuffered, true);
                    canUseAbility = false;

                }

                if (Input.GetKey(KeyCode.E) && canUnstun)
                {
                    VisualizeUnStunDistance();
                    player.FindStunnedPlayers(unStunDistace, this.transform.position, UnStunDistViz);
                    canUnstun = false;
                    StartCoroutine(CooldownPeriod(unStunCooldownTime));

                }

                yield return new WaitForEndOfFrame();
            }
            else
            {
                animator.SetTrigger("Stunned");
                yield return new WaitForEndOfFrame();
            }
        }
    }


    void Update()
    {
       /* if (playerInitiated)
        {
            if (!isStunned)
            {
                player.MovePlayer();
                print(player.isIdle);
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
                {
                    animator.speed = 1f;
                    pv.RPC("ChangeRootMotionStatus", RpcTarget.AllBuffered, false);
                    print("in here");
                    animator.ResetTrigger("Ability");
                    animator.ResetTrigger("Idle");
                    animator.SetTrigger("Running");

                }
                else if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
                {
                    Debug.Log("key up");
                    if (player.isIdle)
                    {
                        animator.speed = 1f;
                        pv.RPC("ChangeRootMotionStatus", RpcTarget.AllBuffered, false);
                        print("is idle");
                        animator.ResetTrigger("Ability");
                        animator.ResetTrigger("Running");
                        animator.SetTrigger("Idle");
                    }
                }




                if (Input.GetKey(KeyCode.Q) && canUseAbility)
                {
                   // animator.speed = 0;
                   // canMove = false;                   
                    animator.ResetTrigger("Idle");
                    animator.ResetTrigger("Running");
                    animator.speed = 3.5f;
                    pv.RPC("ChangeRootMotionStatus", RpcTarget.AllBuffered, true);
                    animator.SetTrigger("Ability");                                    
                    player.isIdle = false;
                    pv.RPC("ChangeAbilityStatus", RpcTarget.AllBuffered, true);
                    canUseAbility = false;

                }



                if (Input.GetKey(KeyCode.E) && canUnstun)
                {
                    VisualizeUnStunDistance();
                    player.FindStunnedPlayers(unStunDistace, this.transform.position, UnStunDistViz);
                    canUnstun = false;
                    StartCoroutine(CooldownPeriod(unStunCooldownTime));

                }

            }
        }*/
    }
  /*  IEnumerator InitiatePlayer()
    {
        while (true)
        {
            if (!isStunned)
            {
                player.MovePlayer();              

                if (Input.GetKey(KeyCode.Q) && canUseAbility)
                {
                    animator.SetTrigger("Ability");
                    pv.RPC("ChangeAbilityStatus", RpcTarget.AllBuffered, true);
                    canUseAbility = false;
                    
                }

                if (Input.GetKey(KeyCode.E) && canUnstun)
                {
                    VisualizeUnStunDistance();
                    player.FindStunnedPlayers(unStunDistace, this.transform.position, UnStunDistViz);
                    canUnstun = false;
                    StartCoroutine(CooldownPeriod(unStunCooldownTime));

                }
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForEndOfFrame();
        }
    }*/

    public IEnumerator AbilityCooldown()
    {
        if (pv.IsMine)
        {
            StartCoroutine(GameObject.FindObjectOfType<HUDScript>().StartCountdown(abilityCooldowntime));
            print("called");
            yield return new WaitForSeconds(abilityCooldowntime);
            canUseAbility = true;
            pv.RPC("ChangeAbilityStatus", RpcTarget.AllBuffered, false);
            print("can use ability");
        }
    }

    [PunRPC]
    void ChangeAbilityStatus(bool abilityRequested)
    {
        ability.SetActive(abilityRequested);
    }


    void VisualizeUnStunDistance()
    {

        UnStunDistViz = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        UnStunDistViz.GetComponent<Collider>().enabled = false;
        UnStunDistViz.name = "StunDistViz";
        UnStunDistViz.transform.position = new Vector3(this.transform.position.x, 0.1f, this.transform.position.z);
        UnStunDistViz.transform.parent = this.transform;     
        UnStunDistViz.transform.localScale = new Vector3(unStunDistace*2, 0.1f, unStunDistace*2);

    }


    IEnumerator CooldownPeriod(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);

        Debug.Log("after cooldown");
        canUnstun = true;
        GameObject.Destroy(UnStunDistViz);

    }

    public void Stun()
    {
       // player.isIdle = false;
       // canMove = false;
        pv.RPC("ChangeStunStatus", RpcTarget.AllBuffered, true);
        animator.ResetTrigger("Running");
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Ability");
        //animator.applyRootMotion = true;
        animator.SetTrigger("Stunned");

        StartCoroutine(GameObject.FindObjectOfType<GameManager>().CheckStunnedPlayers());
        
    }

    public void UnStunPlayers()
    {
        animator.ResetTrigger("Stunned");
        animator.SetTrigger("Idle");
        pv.RPC("ChangeStunStatus", RpcTarget.AllBuffered, false);
     
    }

    [PunRPC]
    void ChangeStunStatus(bool stunStatus)
    {
        isStunned = stunStatus;
    }
    
}
