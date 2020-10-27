using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;

public class StrafeStateTeamRed : FSMStateTeamRed
{
    public StrafeStateTeamRed() 
    { 
        stateIdTeamRed = FSMStateIDTeamRed.Strafing;
        curRotSpeed = 1.0f;
        curSpeed = 100.0f;
    }

    public override void ReasonTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        if (platoonRedTanks.All(x => x.gameObject.GetComponent<NPCTankControllerTeamRed>().HasFinishedStrafe))
        {
            redTank.GetComponent<NavMeshAgent>().isStopped = true;
            redTank.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.ReachPlayer);
        }
    }
	
    public override void ActTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        Debug.Log("strafing");
        var closestTank = redTank;
        var closestTankDistance = float.MaxValue;

        var cumulativeEnemyPosition = Vector3.zero;

        foreach (var enemyTank in enemyTanks)
        {
            var distanceToEnemyTank = Vector3.Distance(redTank.position, enemyTank.position);
            
            if (distanceToEnemyTank < closestTankDistance)
            {
                closestTank = enemyTank;

                closestTankDistance = distanceToEnemyTank;
            }

            cumulativeEnemyPosition += enemyTank.position;
        }

        destPos = cumulativeEnemyPosition / enemyTanks.Count;

        var npcTankController = redTank.gameObject.GetComponent<NPCTankControllerTeamRed>();
		
		float rotateDegrees = Random.Range(60.0f, 120.0f);
		
		npcTankController.transform.RotateAround(closestTank.transform.position, Vector3.right, rotateDegrees);
        //turn turret towards closest enemy tank
        var turret = npcTankController.turret;

       /*Transform turret = npc.GetComponent<NPCTankControllerTeamRed>().turret;*/
        var turretRotation = Quaternion.LookRotation(destPos - turret.position);
        turret.rotation = Quaternion.Slerp(turret.rotation, turretRotation, Time.deltaTime * curRotSpeed);

        npcTankController.ShootBullet();
		
		platoonRedTanks.All(x => x.gameObject.GetComponent<NPCTankControllerTeamRed>().HasFinishedStrafe = true);
    }
}
