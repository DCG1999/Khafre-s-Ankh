using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    EnemyAI[] t_enemyList;
    float nearestDist;
    EnemyAI nearestEnemy = null;

    public Vector3 lastknownPlayerPos;

    void Awake()
    {
        nearestDist = Mathf.Infinity;
        t_enemyList = FindObjectsOfType<EnemyAI>();
    }

    public void AlertNearestEnemy(GameObject _lastknownPlayerPos)
    {
        for(int i =0; i< t_enemyList.Length; i++)
        {
            float distanceToPlayer = Vector3.Distance(_lastknownPlayerPos.transform.position, t_enemyList[i].transform.position);
            if ( distanceToPlayer < nearestDist)
            {
                nearestDist = distanceToPlayer;
                nearestEnemy = t_enemyList[i];
            }
        }
        lastknownPlayerPos = _lastknownPlayerPos.transform.position;
        nearestEnemy.currentState.AlertedByCameras();

    }
}
