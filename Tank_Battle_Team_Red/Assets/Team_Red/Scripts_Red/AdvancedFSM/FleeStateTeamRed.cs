using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FleeStateTeamRed : FSMStateTeamRed
{
    public FleeStateTeamRed()
    {
        stateIdTeamRed = FSMStateIDTeamRed.Fleeing;
    }

    public override void ActTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        Debug.Log("Flee state active");
        var firstEnemyPosition = enemyTanks.First().position;
        redTank.LookAt(firstEnemyPosition);
        Quaternion.Slerp(redTank.rotation, Quaternion.Euler(0, 180, 0), 1 * Time.deltaTime);
        redTank.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }

    public override void ReasonTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        var nearestEnemyTank = enemyTanks.Min(enemyTank => Vector3.Distance(enemyTank.position, redTank.position));
        var patrolDistance = 300f;
        if(nearestEnemyTank > patrolDistance)
        {
            Debug.Log("Switch to Patrol State");
            redTank.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.LostPlayer);
        }
    }
}
