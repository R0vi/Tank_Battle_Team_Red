using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
            //redTank.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.GoToAttack);
        }
    }
	/*
    public override void ReasonTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        if (platoonRedTanks.All(x => x.gameObject.GetComponent<NPCTankControllerTeamRed>().HasFinishedStrafe))
        {
            redTank.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.GoToAttack);
        }
        ////Check the distance with the player tank
        //float dist = Vector3.Distance(npc.position, player.position);
        //if (dist >= 200.0f && dist < 300.0f)
        //{
        //    //Rotate to the target point
        //    Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
        //    npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);

        //    //Go Forward
        //    npc.Translate(Vector3.forward * Time.deltaTime * curSpeed);

        //    Debug.Log("Switch to Chase State");
        //    npc.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.SawPlayer);
        //}
        ////Transition to patrol is the tank become too far
        //else if (dist >= 300.0f)
        //{
        //    Debug.Log("Switch to Patrol State");
        //    npc.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.LostPlayer);
        //}  
    }
*/
    public override void ActTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        Transform closestTank = null;
        float closestTankDistance = float.MaxValue;

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
