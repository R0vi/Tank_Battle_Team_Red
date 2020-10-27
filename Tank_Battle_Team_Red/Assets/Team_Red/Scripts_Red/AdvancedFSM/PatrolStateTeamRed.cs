using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Random = UnityEngine.Random;

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

    float RandomBinomial()
    {
        return Random.Range(0f, 1f) - Random.Range(0f, 1f);
    }

    public override void ActTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        var directionVector = Combine(redTank, platoonRedTanks);

        destPos = redTank.position + directionVector.normalized * dataTeamRed.LookAheadDistance;

        Debug.Log($"cohesion radius: {dataTeamRed.RadiusCohesion}");

        /*redTank.gameObject.GetComponent<NavMeshAgent>().destination = destPos;*/

        var succeeded = redTank.gameObject.GetComponent<NavMeshAgent>().SetDestination(destPos);

        Debug.Log("stopped: " + redTank.gameObject.GetComponent<NavMeshAgent>().isStopped);
        Debug.Log($"{redTank.name} destPos: {destPos}, succeeded: {succeeded}");

        //Rotate to the target point
        /*Quaternion targetRotation = Quaternion.LookRotation(destPos - redTank.position);
        redTank.rotation = Quaternion.Slerp(redTank.rotation, targetRotation, Time.deltaTime * curRotSpeed);

        //Go Forward
        redTank.Translate(Vector3.forward * Time.deltaTime * curSpeed);*/
    }

    Vector3 Wander()
    {
        var jitter = dataTeamRed.WanderJitter * Time.deltaTime;
        
        return new Vector3(RandomBinomial() * jitter, 0, RandomBinomial() * jitter);
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

            if (Vector3.Distance(tank.position, curTank.position) <= dataTeamRed.RadiusCohesion)
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

            if (Vector3.Distance(tank.position, curTank.position) <= dataTeamRed.RadiusSeparation)
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

                if (Vector3.Distance(tank.position, curTank.position) <= dataTeamRed.RadiusAlignment)
                {
                    destPoint += tank.forward;
                }
        }

        return destPoint.normalized;
    }

    Vector3 Combine(Transform curTank, IList<Transform> platoonTanks)
    {
        return dataTeamRed.WeightCohesion * Cohesion(curTank, platoonTanks) +
               dataTeamRed.WeightSeparation * Separation(curTank, platoonTanks) +
               dataTeamRed.WeightAlignment * Alignment(curTank, platoonTanks) +
                dataTeamRed.WeightWander * Wander();
    }
}