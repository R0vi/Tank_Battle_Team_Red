using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatrolStateTeamRed : FSMStateTeamRed
{
    public PatrolStateTeamRed() : base()
    {
        stateIdTeamRed = FSMStateIDTeamRed.Patrolling;

        curRotSpeed = 1.0f;
        curSpeed = 100.0f;
    }

    public override void ReasonTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        //Check the distance with player tank
        //When the distance is near, transition to chase state
        foreach (Transform enemyTank in enemyTanks)
        {
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
        destPos = Combine(redTank, platoonRedTanks);

        //Rotate to the target point
        Quaternion targetRotation = Quaternion.LookRotation(destPos - redTank.position);
        redTank.rotation = Quaternion.Slerp(redTank.rotation, targetRotation, Time.deltaTime * curRotSpeed);

        //Go Forward
        redTank.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }

    Vector3 Cohesion(Transform curTank, IList<Transform> platoonTanks)
    {
        Vector3 destPoint = new Vector3();

        if (platoonTanks.Count == 0)
            return destPoint;
        int tankCount = 0;
        foreach (Transform tank in platoonTanks)
        {
            if (Vector3.Distance(tank.position, curTank.position) <= dataTeamRed.radiusCohesion)
            {
                destPoint += tank.position;
                tankCount++;
            }
        }

        destPoint /= tankCount;
        destPoint -= curTank.position;

        return destPoint.normalized;
    }

    Vector3 Separation(Transform curTank, IList<Transform> platoonTanks)
    {
        Vector3 destPoint = new Vector3();

        if (platoonTanks.Count == 0)
            return destPoint;

        foreach (Transform tank in platoonTanks)
        {
            if (Vector3.Distance(tank.position, curTank.position) <= dataTeamRed.radiusSeparation)
            {
                Vector3 inverseAgentDirection = curTank.position - tank.position;

                if (inverseAgentDirection.magnitude != 0)
                    destPoint += inverseAgentDirection.normalized / inverseAgentDirection.magnitude;
            }
        }

        return destPoint.normalized;
    }

    Vector3 Alignment(Transform curTank, IList<Transform> platoonTanks)
    {
        Vector3 destPoint = new Vector3();

        if (platoonTanks.Count == 0)
            return destPoint;

        foreach (Transform tank in platoonTanks)
        {
            if (Vector3.Distance(tank.position, curTank.position) <= dataTeamRed.radiusAlignment)
            {
                destPoint += tank.forward;
            }
        }

        return destPoint.normalized;
    }

    Vector3 Combine(Transform curTank, IList<Transform> platoonTanks)
    {
        return dataTeamRed.weightCohesion * Cohesion(curTank, platoonTanks) + 
            dataTeamRed.weightSeparation * Separation(curTank, platoonTanks) + 
            dataTeamRed.weightAlignment * Alignment(curTank, platoonTanks);
    }
}