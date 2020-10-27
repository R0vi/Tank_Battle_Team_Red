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
        Transform closestTank = null;
        float closestTankDistance = float.MaxValue;

        foreach (var enemyTank in enemyTanks)
        {
            var distanceToEnemyTank = Vector3.Distance(redTank.position, enemyTank.position);

            if (distanceToEnemyTank < closestTankDistance)
            {
                closestTank = enemyTank;

                closestTankDistance = distanceToEnemyTank;
                
                if(closestTankDistance <= 150)
                {
                    //chase to attack
                    Debug.Log("Switch to Attack State");
                    redTank.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.ReachPlayer);
                    redTank.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    redTank.gameObject.GetComponent<NavMeshAgent>().ResetPath();
                    break;
                }
                if(closestTankDistance > 300)
                {
                    //chase to patrol
                    redTank.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.LostPlayer);
                    break;
                }
            }
        }

        //var closestTank = enemyTanks.Min(x => Vector3.Distance(redTank.position, x.position)

        ////Set the target position as the player position
        //destPos = player.position;

        ////Check the distance with player tank
        ////When the distance is near, transition to attack state
        //float dist = Vector3.Distance(npc.position, destPos);
        //if (dist <= 200.0f)
        //{
        //    Debug.Log("Switch to Attack state");
        //    npc.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.ReachPlayer);
        //}
        ////Go back to patrol is it become too far
        //else if (dist >= 300.0f)
        //{
        //    Debug.Log("Switch to Patrol state");
        //    npc.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.LostPlayer);
        //}
    }
    
    public override void ActTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        
        Transform closestTank = redTank;
        float closestDist = float.MaxValue;
        foreach(var enemyTank in enemyTanks)
        {
            if(Vector3.Distance(redTank.position, enemyTank.position) < closestDist)
            {
                closestTank = enemyTank;
            }
        }

        destPos = closestTank.position;

        redTank.gameObject.GetComponent<NavMeshAgent>().SetDestination(destPos);

        ////Rotate to the target point
        //Quaternion targetRotation = Quaternion.LookRotation(destPos - redTank.position);
        //redTank.rotation = Quaternion.Slerp(redTank.rotation, targetRotation, Time.deltaTime * curRotSpeed);

        ////Go Forward
        //redTank.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }
}
