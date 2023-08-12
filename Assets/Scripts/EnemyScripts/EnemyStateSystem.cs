using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateSystem
{
    public enum STATE
    {
        IDLE, PATROL, CHASE, INSPECT
    };

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    };

    public STATE stateName;
    protected EVENT eventStage;
    protected EnemyStateSystem nextState;
    protected Transform[] players;
    protected GameObject npc;
    protected Animator anim;
    protected NavMeshAgent navAgent;
    protected List<GameObject> waypoints;

    protected bool isAlertedByCams = false;

    public EnemyStateSystem(GameObject _npc, NavMeshAgent _navAgent, Animator _anim, Transform[] _players, List<GameObject> _waypoints)
    {
        npc = _npc;
        navAgent = _navAgent;
        anim = _anim;
        players = _players;
        waypoints = _waypoints;
        eventStage = EVENT.ENTER;
    }

    public virtual void Enter()
    {
        eventStage = EVENT.UPDATE;
    }

    public virtual void Update()
    {
        eventStage = EVENT.UPDATE;
    }

    public virtual void Exit()
    {
        eventStage = EVENT.EXIT;
    }

    public EnemyStateSystem Process()
    {
        if (eventStage == EVENT.ENTER)
            Enter();

        if (eventStage == EVENT.UPDATE)
            Update();

        if (eventStage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }
        return this;
    }

    public bool CanSeePlayer()
    {
        if (npc.GetComponent<EnemyFOV>().detectedPlayer)
        {
            return npc.GetComponent<EnemyFOV>().detectedPlayer;
        }

        if (npc.GetComponent<EnemyFOV>().detectedIllusion)
        {
            return npc.GetComponent<EnemyFOV>().detectedIllusion;
        }
        else
        {
            return false;
        }
    }

    public void AlertedByCameras()
    {
        isAlertedByCams = true;
    }

    public virtual void SetAnimTrigger(string trigger)
    {
        anim.ResetTrigger("Patrol");
        anim.ResetTrigger("Idle");
        anim.ResetTrigger("Chase");
        anim.ResetTrigger("Attack");

        anim.SetTrigger(trigger);
    }

}


public class Idle : EnemyStateSystem
{
    public Idle(GameObject _npc, NavMeshAgent _navAgent, Animator _anim, Transform[] _players, List<GameObject> _waypoints) : base(_npc, _navAgent, _anim, _players, _waypoints)
    {
        stateName = STATE.IDLE;
        navAgent.speed = 0;
        navAgent.acceleration = 0;
        navAgent.angularSpeed = 0;
    }

    public override void Enter()
    {       
        base.SetAnimTrigger("Idle");
        // add idle animation;
        base.Enter();
    }

    public override void Update()
    {
        if (CanSeePlayer())
        {
            nextState = new Chase(npc, navAgent, anim, players, waypoints);
            eventStage = EVENT.EXIT;

        }

        if(isAlertedByCams)
        {
            nextState = new Inspect(npc, navAgent, anim, players, waypoints);
            eventStage = EVENT.EXIT;
            isAlertedByCams=false;
        }

        else if (Random.Range(0, 100) < 10 && !CanSeePlayer())
        {
            nextState = new Patrol(npc, navAgent, anim, players, waypoints);
            eventStage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class Patrol : EnemyStateSystem
{
    int currentIndex = -1;
    float waitTime = 4;
    float timeWaited = 0;

    public Patrol(GameObject _npc, NavMeshAgent _navAgent, Animator _anim, Transform[] _players, List<GameObject> _waypoints) : base(_npc, _navAgent, _anim, _players, _waypoints)
    {
        stateName = STATE.PATROL;
        navAgent.speed = 5;
        //navAgent.isStopped = false;
        navAgent.acceleration = 5;
        navAgent.angularSpeed = 500;
    }

    public override void Enter()
    {
        base.SetAnimTrigger("Patrol");
        timeWaited = waitTime;
        float lastDist = Mathf.Infinity;
        for (int i = 0; i < waypoints.Count; i++)
        {
            GameObject currentWP = waypoints[i];
            float distance = Vector3.Distance(npc.transform.position, currentWP.transform.position);
            if (distance < lastDist)
            {
                currentIndex = i - 1;
                lastDist = distance;
            }
        }
        // start the wa;lking anim trig
        base.Enter();
    }

    public override void Update()
    {

        if (CanSeePlayer())
        {
            nextState = new Chase(npc, navAgent, anim, players, waypoints);
            eventStage = EVENT.EXIT;
        }

        else if(isAlertedByCams)
        {
            nextState = new Inspect(npc, navAgent, anim, players, waypoints);
            eventStage = EVENT.EXIT;
            isAlertedByCams = false;
        }

        else
        {
            if (navAgent.remainingDistance < 1f)
            {
                if (timeWaited > 0) // for waiting 
                {
                    base.SetAnimTrigger("Idle");
                    timeWaited -= Time.deltaTime;
                    // code for rotation can be implemented here with extra cut off for time
                }

                else if (timeWaited < 0)
                {
                    base.SetAnimTrigger("Patrol");
                    timeWaited = waitTime;

                    if (currentIndex >= waypoints.Count - 1)
                    {
                        currentIndex = 0;
                    }
                    else
                    {
                        currentIndex++;
                    }

                    navAgent.SetDestination(waypoints[currentIndex].transform.position);

                }
            }
        }
    }

    public override void Exit()
    {
        // reset walking anim trig
        base.Exit();

    }

    IEnumerator Inspect()
    {
        yield return new WaitForSeconds(1f);
    }

}

public class Chase : EnemyStateSystem
{
    public Chase(GameObject _npc, NavMeshAgent _navAgent, Animator _anim, Transform[] _players, List<GameObject> _waypoints) : base(_npc, _navAgent, _anim, _players, _waypoints)
    {
        stateName = STATE.CHASE;
        navAgent.speed = 15;
        navAgent.acceleration = 5;
        navAgent.angularSpeed = 500;
        // navAgent.stoppingDistance = 3f;
    }

    public override void Enter()
    {
        base.SetAnimTrigger("Chase");
        Debug.Log("player seen");

        if(npc.GetComponent<EnemyFOV>().detectedPlayer)
            navAgent.SetDestination(npc.GetComponent<EnemyFOV>().playerDetected.position);
        else if(npc.GetComponent<EnemyFOV>().detectedIllusion)
            navAgent.SetDestination(npc.GetComponent<EnemyFOV>().illusionDetected.position);

        base.Enter();
    }

    public override void Update()
    {
        if (!CanSeePlayer())
        {
            if (Random.Range(0, 300) < 2)
            {
                base.SetAnimTrigger("Idle");
                nextState = new Idle(npc, navAgent, anim, players, waypoints);
                eventStage = EVENT.EXIT;
            }
        }
        else
        {
            Transform playerDetected = null;

            if (npc.GetComponent<EnemyFOV>().detectedPlayer)
                playerDetected = npc.GetComponent<EnemyFOV>().playerDetected;

            else if(npc.GetComponent<EnemyFOV>().detectedIllusion)
                playerDetected = npc.GetComponent<EnemyFOV>().illusionDetected;

            if (Vector3.Distance(npc.transform.position, playerDetected.transform.position) < 2f)
            {
                base.SetAnimTrigger("Attack");

                if (npc.GetComponent<EnemyFOV>().detectedPlayer)
                {
                    Debug.Log("enemy found player");
                    npc.GetComponent<EnemyFOV>().playerDetected.parent.GetComponent<PlayerScript>().Stun();
                }


                else if (npc.GetComponent<EnemyFOV>().detectedIllusion)
                    npc.GetComponent<EnemyFOV>().illusionDetected.GetComponent<IllusionScript>().StunIllusion();

                navAgent.SetDestination(npc.transform.position);
                Debug.Log("Code Reached");
                navAgent.speed = 0;
                //navAgent.isStopped = true;
                navAgent.acceleration = 0;           
            }
            else
            {
                navAgent.SetDestination(playerDetected.position);
            }


        }
    }

    public override void Exit()
    {
        base.Exit();
    }


}

public class Inspect : EnemyStateSystem
{
    Vector3 lastKnownPlayerPos;
    public Inspect(GameObject _npc, NavMeshAgent _navAgent, Animator _anim, Transform[] _players, List<GameObject> _waypoints) : base(_npc, _navAgent, _anim, _players, _waypoints)
    {
        stateName = STATE.INSPECT;
        navAgent.speed = 15;
        navAgent.acceleration = 5;
        navAgent.angularSpeed = 500;
        
    }

    public override void Enter()
    {
        base.SetAnimTrigger("Chase");
        lastKnownPlayerPos = GameObject.FindObjectOfType<EnemyManager>().lastknownPlayerPos;
        navAgent.SetDestination(lastKnownPlayerPos);
        base.Enter();
    }

    public override void Update()
    {
        if (CanSeePlayer())
        {
            nextState = new Chase(npc, navAgent, anim, players, waypoints);
            eventStage = EVENT.EXIT;
        }
        else if (!CanSeePlayer() && Vector3.Distance(npc.transform.position, lastKnownPlayerPos) < 1f)
        {
            base.SetAnimTrigger("Idle");
            navAgent.speed = 0;
            nextState = new Idle(npc, navAgent, anim, players, waypoints);
            eventStage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

}

