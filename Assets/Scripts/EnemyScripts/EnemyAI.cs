using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    NavMeshAgent navAgent;
    Animator anim;
    public Transform[] players;
    [HideInInspector] public EnemyStateSystem currentState;
    public List<GameObject> waypoints;

    private void Start()
    {
        navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentState = new Idle(this.gameObject, navAgent, anim, players, waypoints);
    }

    private void Update()
    {
        currentState = currentState.Process();
    }
}
