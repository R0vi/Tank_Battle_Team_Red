using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
        //Check the distance with player tank
        //When the distance is near, transition to chase state
        foreach (var enemyTank in enemyTanks)
        {
            if (Vector3.Distance(redTank.position, enemyTank.position) <= dataTeamRed.EvadingRange)
            {
                redTank.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.EnemyTooClose);
                break;
            }
            if (Vector3.Distance(redTank.position, enemyTank.position) <= DataTeamRed.SpottingRange)
            {
                redTank.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.SawPlayer);
                break;
            }
        }
    }

    private bool usingCommonDestination = false;
    private bool startingCommonDestination = true;
    private float startedCommonDestinationTime;
    private float startedEmergentBehaviourTime;
    private NavMeshAgent _navMeshAgent;

    public override void ActTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        if (_navMeshAgent == null)
        {
            _navMeshAgent = redTank.gameObject.GetComponent<NavMeshAgent>();
        }

        if (startingCommonDestination)
        {
            startingCommonDestination = false;
            usingCommonDestination = true;
            startedCommonDestinationTime = Time.time;

            if (!dataTeamRed.CommonDestination.HasValue)
            {
                dataTeamRed.CommonDestination = GetRandomNavmeshLocation(redTank.position,
                    dataTeamRed.CommonDestinationRadius);
            }

            _navMeshAgent.SetDestination(dataTeamRed.CommonDestination.Value);
        }

        if (usingCommonDestination)
        {
            var currentTime = Time.time;

            if (currentTime - startedCommonDestinationTime >= dataTeamRed.UseCommonDestinationForSeconds)
            {
                usingCommonDestination = false;
                dataTeamRed.CommonDestination = null;
                startedEmergentBehaviourTime = Time.time;
            }
        }
        else
        {
            var currentTime = Time.time;

            if (currentTime - startedEmergentBehaviourTime >= dataTeamRed.UseEmergentBehaviourForSeconds)
            {
                startingCommonDestination = true;
            }

            var directionVector = Combine(redTank, platoonRedTanks);

            destPos = redTank.position + directionVector.normalized * dataTeamRed.LookAheadDistance;

            var succeeded = _navMeshAgent.SetDestination(destPos);
        }
    }

    public Vector3 GetRandomNavmeshLocation(Vector3 position, float radius)
    {
        var randomDirection = Random.insideUnitSphere * radius;
        randomDirection += position;
    
        var finalPosition = Vector3.zero;

        if (NavMesh.SamplePosition(randomDirection, out var hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        
        return finalPosition;
    }

    Vector3 Wander()
    {
        var jitter = dataTeamRed.WanderJitter * Time.deltaTime;

        return new Vector3(RandomBinomial() * jitter, 0, RandomBinomial() * jitter);
    }

    float RandomBinomial()
    {
        return Random.Range(0f, 1f) - Random.Range(0f, 1f);
    }

    Vector3 Cohesion(Transform curTank, IList<Transform> platoonTanks)
    {
        var destPoint = new Vector3();

        if (platoonTanks.Count == 0)
            return destPoint;

        var tankCounter = 0;

        foreach (var tank in platoonTanks)
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
        var destPoint = new Vector3();

        if (platoonTanks.Count == 0)
            return destPoint;

        foreach (var tank in platoonTanks)
        {
            if (tank == curTank)
            {
                continue;
            }

            if (Vector3.Distance(tank.position, curTank.position) <= dataTeamRed.RadiusSeparation)
            {
                var inverseAgentDirection = curTank.position - tank.position;

                if (inverseAgentDirection.magnitude != 0)
                    destPoint += inverseAgentDirection.normalized / inverseAgentDirection.magnitude;
            }
        }

        return destPoint.normalized;
    }

    Vector3 Alignment(Transform curTank, IList<Transform> platoonTanks)
    {
        var destPoint = new Vector3();

        if (platoonTanks.Count == 0)
            return destPoint;

        foreach (var tank in platoonTanks)
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