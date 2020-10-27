using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

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
        Debug.Log("In patrol");
        //Check the distance with player tank
        //When the distance is near, transition to chase state
        foreach (var enemyTank in enemyTanks)
        {
            if (Vector3.Distance(redTank.position, enemyTank.position) <= dataTeamRed.EvadingRange)
            {
                Debug.Log("Switch to Evade State");
                redTank.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.EnemyTooClose);
                break;
            }
            if (Vector3.Distance(redTank.position, enemyTank.position) <= DataTeamRed.SpottingRange)
            {
                Debug.Log("Switch to Chase State");
                redTank.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.SawPlayer);
                break;
            }
        }
    }

    public override void ActTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        var directionVector = Combine(redTank, platoonRedTanks);

        destPos = redTank.position + directionVector;

        redTank.gameObject.GetComponent<NavMeshAgent>().SetDestination(destPos);
        Debug.Log("stopped: " + redTank.gameObject.GetComponent<NavMeshAgent>().isStopped);
        Debug.Log(destPos);

        //Rotate to the target point
        /*Quaternion targetRotation = Quaternion.LookRotation(destPos - redTank.position);
        redTank.rotation = Quaternion.Slerp(redTank.rotation, targetRotation, Time.deltaTime * curRotSpeed);

        //Go Forward
        redTank.Translate(Vector3.forward * Time.deltaTime * curSpeed);*/
    }

    Vector3 Cohesion(Transform curTank, IList<Transform> platoonTanks)
    {
        Vector3 destPoint = new Vector3();

        if (platoonTanks.Count == 0)
            return destPoint;

        var tankCounter = 0;

        foreach (Transform tank in platoonTanks)
        {
            if (tank == curTank)
            {
                continue;
            }

            tankCounter++;

            if (Vector3.Distance(tank.position, curTank.position) <= dataTeamRed.radiusCohesion)
            {
                destPoint += tank.position;
            }
        }

        destPoint /= tankCounter;

        var directionVector = (destPoint - curTank.position).normalized;

        return directionVector;
    }

    Vector3 Separation(Transform curTank, IList<Transform> platoonTanks)
    {
        Vector3 destPoint = new Vector3();

        if (platoonTanks.Count == 0)
            return destPoint;

        foreach (Transform tank in platoonTanks)
        {
            if (tank == curTank)
            {
                continue;
            }

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
            if (curTank == tank)
            {
                continue;
            }

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