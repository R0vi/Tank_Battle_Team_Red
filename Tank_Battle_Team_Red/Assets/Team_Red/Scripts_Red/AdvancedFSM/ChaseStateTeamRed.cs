using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;

public class ChaseStateTeamRed : FSMStateTeamRed
{
    public ChaseStateTeamRed() 
    {
        stateIdTeamRed = FSMStateIDTeamRed.Chasing;

        curRotSpeed = 1.0f;
        curSpeed = 100.0f;
    }

    public override void ReasonTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        float closestTankDistance = float.MaxValue;

        if (!enemyTanks.Any())
        {
            redTank.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.LostPlayer);
        }

        foreach (var enemyTank in enemyTanks)
        {
            var distanceToEnemyTank = Vector3.Distance(redTank.position, enemyTank.position);

            if (distanceToEnemyTank < closestTankDistance)
            {
                closestTankDistance = distanceToEnemyTank;
                
                if(closestTankDistance <= DataTeamRed.AttackingRange)
                {
                    //chase to attack
                    Debug.Log("Switch to Attack State");
                    redTank.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.ReachPlayer);
                    redTank.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    redTank.gameObject.GetComponent<NavMeshAgent>().ResetPath();
                    break;
                }
            }
        }
    }
    
    public override void ActTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        var closestTank = redTank;
        var closestDist = float.MaxValue;

        foreach(var enemyTank in enemyTanks)
        {
            var distance = Vector3.Distance(redTank.position, enemyTank.position);

            if (distance < closestDist)
            {
                closestDist = distance;
                closestTank = enemyTank;
            }
        }

        destPos = closestTank.position;

        redTank.gameObject.GetComponent<NavMeshAgent>().SetDestination(destPos);
    }
}
