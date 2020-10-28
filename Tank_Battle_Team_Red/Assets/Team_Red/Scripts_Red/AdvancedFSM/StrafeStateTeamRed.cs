using System;
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
            _started = false;
            _firstGoalMet = false;

            redTank.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.ReachPlayer);
        }
    }

    private bool _started = false;
    private bool _firstGoalMet = false;
    private Vector3 _goal = Vector3.zero;

    public override void ActTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        var npcTankController = redTank.gameObject.GetComponent<NPCTankControllerTeamRed>();

        if (!_started)
        {
            _goal = redTank.position + redTank.forward * dataTeamRed.StrafeDistance;

            redTank.gameObject.GetComponent<NavMeshAgent>().SetDestination(_goal);

            _started = true;
        }

        if (Vector3.Distance(_goal, redTank.position) < dataTeamRed.StrafeDistanceErrorMargin)
        {
            if (_firstGoalMet)
            {
                npcTankController.HasFinishedStrafe = true;
            }
            else
            {
                _firstGoalMet = true;
                _goal = redTank.position - redTank.forward * dataTeamRed.StrafeDistance;
                redTank.gameObject.GetComponent<NavMeshAgent>().SetDestination(_goal);
            }
        }

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

        var turret = npcTankController.turret;

        var turretRotation = Quaternion.LookRotation(closestTank.position - turret.position);
        turret.rotation = Quaternion.Slerp(turret.rotation, turretRotation, Time.deltaTime * dataTeamRed.TurretRotationSpeed);

        npcTankController.ShootBullet();
    }
}
