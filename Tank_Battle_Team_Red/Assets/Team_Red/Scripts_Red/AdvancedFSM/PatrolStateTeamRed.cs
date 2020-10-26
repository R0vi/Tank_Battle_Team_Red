using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatrolStateTeamRed : FSMStateTeamRed
{
    public PatrolStateTeamRed(Transform[] wp) 
    { 
        waypoints = wp;
        stateIdTeamRed = FSMStateIDTeamRed.Patrolling;

        curRotSpeed = 1.0f;
        curSpeed = 100.0f;
    }

    public override void ReasonTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        Debug.Log("In patrol");
        //Check the distance with player tank
        //When the distance is near, transition to chase state
        foreach (Transform enemyTank in enemyTanks)
        {
            if(Vector3.Distance(redTank.position, enemyTank.position) <= 50.0f)
            {
                Debug.Log("Switch to Evade State");
                redTank.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.EnemyTooClose);
                break;
            }
            if (Vector3.Distance(redTank.position, enemyTank.position) <= 300.0f)
            {
                Debug.Log("Switch to Chase State");
                redTank.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.SawPlayer);
                break;
            }
        }
    }

    public override void ActTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        ////Find another random patrol point if the current point is reached
		
        //if (Vector3.Distance(npc.position, destPos) <= 100.0f)
        //{
        //    Debug.Log("Reached to the destination point\ncalculating the next point");
        //    FindNextPointTeamRed();
        //}

        ////Rotate to the target point
        //Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
        //npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);

        ////Go Forward
        //npc.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }

    Vector3 Cohesion(Transform curTank, List<Transform> platoonTanks)
    {
        Vector3 steerVector = new Vector3();

        if (platoonTanks.Count == 0)
            return steerVector;

        foreach (Transform tank in platoonTanks)
            steerVector += tank.position;

        steerVector /= platoonTanks.Count;
        steerVector -= curTank.position;

        return steerVector.normalized;
    }

    Vector3 Separation(Transform curTank, List<Transform> platoonTanks)
    {
        Vector3 steerVector = new Vector3();

        if (platoonTanks.Count == 0)
            return steerVector;

        foreach (Transform tank in platoonTanks)
        {
            Vector3 inverseAgentDirection = curTank.position - tank.position;

            if (inverseAgentDirection.magnitude != 0)
                steerVector += inverseAgentDirection.normalized / inverseAgentDirection.magnitude;
        }

        return steerVector.normalized;
    }

    Vector3 Alignment(Transform curTank, List<Transform> platoonTanks)
    {
        Vector3 steerVector = new Vector3();

        if (platoonTanks.Count == 0)
            return steerVector;

        //foreach (Transform tank in platoonTanks)
        //    steerVector += tank.velocity;

        return steerVector.normalized;
    }
}