using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChaseStateTeamRed : FSMStateTeamRed
{
    public ChaseStateTeamRed(Transform[] wp) 
    { 
        waypoints = wp;
        stateIdTeamRed = FSMStateIDTeamRed.Chasing;

        curRotSpeed = 1.0f;
        curSpeed = 100.0f;

        //find next Waypoint position
        FindNextPointTeamRed();
    }

    public override void ReasonTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
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
        ////Rotate to the target point
        //destPos = player.position;

        //Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
        //npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);

        ////Go Forward
        //npc.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }
}
