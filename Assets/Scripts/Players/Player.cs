using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Player
{
    protected PhotonView pv;
    protected CharacterController charController;
    protected float moveSpeed;
    protected float rotSpeed;
    protected Transform t_player;
    protected Animator animator;

    public Player(PhotonView _pv, CharacterController _charController, float _moveSpeed, float _rotSpeed, Transform _t_player, Animator _animator)
    {
        pv = _pv;
        charController = _charController;
        moveSpeed = _moveSpeed;
        t_player = _t_player;
        rotSpeed = _rotSpeed;
        animator = _animator;
    }

    public virtual void MovePlayer()
    {
        if (pv.IsMine)
        {
            float horInput = Input.GetAxis("Horizontal");
            float verInput = Input.GetAxis("Vertical");

            if (verInput != 0) // for moving the player
            {
                SetAnimTrigger("Running");
                charController.Move(verInput * moveSpeed * Time.deltaTime * t_player.forward);
            }

            if (horInput != 0) // for rotating the player
            {
                
                SetAnimTrigger("Running");
                t_player.RotateAround(t_player.transform.position, t_player.up, horInput * rotSpeed * Time.deltaTime);
            }

            if(verInput == 0 && horInput ==0)
            {
                SetAnimTrigger("Idle");
            }

        }
    }


    public virtual void FindStunnedPlayers(float unStunDist, Vector3 playerPos, GameObject unStunDistViz) 
    {
        Collider[] playersNearby = Physics.OverlapSphere(playerPos, unStunDist);
        List<GameObject> PlayersNearbyList = new List<GameObject>();
        foreach(Collider c in playersNearby)
        {
            if(c.gameObject.CompareTag("Player"))
            {
                PlayersNearbyList.Add(c.gameObject);
            }
           
        }
        Debug.Log(PlayersNearbyList.Count);
        if (PlayersNearbyList.Count == 1)
        {
            unStunDistViz.GetComponent<MeshRenderer>().material.color = Color.red;
            Debug.Log("No Stunned player Found"); 
            return;
        }
        else if(PlayersNearbyList.Count > 1)
        {
            List<PlayerScript> playersStunned = new List<PlayerScript>();
            foreach (GameObject p in PlayersNearbyList)
            {
                
                playersStunned.Add(p.transform.GetComponent<PlayerScript>());
               
            }
            foreach (PlayerScript playerStunned in playersStunned)
            {
                if (playerStunned.isStunned)
                {
                    unStunDistViz.GetComponent<MeshRenderer>().material.color = Color.green;
                    Debug.Log("Stunned player Found");
                    playerStunned.UnStunPlayers();
                }
                else
                {
                    Debug.Log("PlayerNearby but not stunned");
                }
            }
            
            playersStunned.Clear();
            PlayersNearbyList.Clear();
        }
    }

    public virtual void SetAnimTrigger(string trigger)
    {
        animator.ResetTrigger("Ability");
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Stunned");
        animator.ResetTrigger("Running");

        animator.SetTrigger(trigger);
    }
}

public class WizardScript : Player
{
    public WizardScript(PhotonView _pv, CharacterController _charController, float _moveSpeed, float _rotSpeed, Transform _t_player, Animator _animator) : base(_pv, _charController, _moveSpeed, _rotSpeed, _t_player, _animator)
    {

    }

    public override void MovePlayer()
    {
        if (pv.IsMine)
        {
            base.MovePlayer();
        }
    }
      

    public override void FindStunnedPlayers(float unStunDist, Vector3 playerPos, GameObject unStunDistviz)
    {
        base.FindStunnedPlayers(unStunDist, playerPos, unStunDistviz);
    }
}

public class TechieScript : Player
{
    public TechieScript(PhotonView _pv, CharacterController _charController, float _moveSpeed, float _rotSpeed, Transform _t_player, Animator _animator) : base(_pv, _charController, _moveSpeed, _rotSpeed, _t_player, _animator)
    {

    }
    public override void MovePlayer()
    {
        if (pv.IsMine)
            base.MovePlayer();
    }

    public override void FindStunnedPlayers(float unStunDist, Vector3 playerPos , GameObject unStunDistviz)
    {
        base.FindStunnedPlayers(unStunDist, playerPos, unStunDistviz);
    }
}

public class IllusionistScript : Player
{
    public IllusionistScript(PhotonView _pv, CharacterController _charController, float _moveSpeed, float _rotSpeed, Transform _t_player, Animator _animator) : base(_pv, _charController, _moveSpeed, _rotSpeed, _t_player, _animator)
    {

    }
    public override void MovePlayer()
    {
        if (pv.IsMine)
            base.MovePlayer();
    }

    public override void FindStunnedPlayers(float unStunDist, Vector3 playerPos, GameObject unStunDistviz)
    {
        base.FindStunnedPlayers(unStunDist, playerPos, unStunDistviz);
    }

}

public class ThiefScript : Player
{
    public ThiefScript(PhotonView _pv, CharacterController _charController, float _moveSpeed, float _rotSpeed, Transform _t_player, Animator _animator) : base(_pv, _charController, _moveSpeed, _rotSpeed, _t_player, _animator)
    {

    }
    public override void MovePlayer()
    {
        if (pv.IsMine)
            base.MovePlayer();
    }

    public override void FindStunnedPlayers(float unStunDist, Vector3 playerPos, GameObject unStunDistviz)
    {
        base.FindStunnedPlayers(unStunDist, playerPos, unStunDistviz);
    }

}
