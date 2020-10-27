using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;

public class AttackStateTeamRed : FSMStateTeamRed
{
    private float time = 0;
    private float oldHealth;

    public AttackStateTeamRed() : base()
    {
        stateIdTeamRed = FSMStateIDTeamRed.Attacking;
        curRotSpeed = 1.0f;
        curSpeed = 100.0f;
    }

    public override void ReasonTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        var npcTankController = redTank.gameObject.GetComponent<NPCTankControllerTeamRed>();

        var closestTank = redTank;
        var closestTankDistance = float.MaxValue;

        foreach (var enemyTank in enemyTanks)
        {
            var distanceToEnemyTank = Vector3.Distance(redTank.position, enemyTank.position);
            if (distanceToEnemyTank < closestTankDistance)
            {
                closestTank = enemyTank;
                closestTankDistance = distanceToEnemyTank;
            }
            if (closestTankDistance > 150)
            {
                Debug.Log("Switch to Chase State");
                redTank.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.SawPlayer);
                redTank.gameObject.GetComponent<NavMeshAgent>().isStopped = false;
                break;
            }
        }

        if (platoonRedTanks.All(x => x.gameObject.GetComponent<NPCTankControllerTeamRed>().HasShotInAttackState))
        {
            //Debug.Log("Switch to Strafe State");
            //npcTankController.SetTransition(Transition.GoToStrafe);
        }

        if (time == 0)
        {
            time = Time.time;
            oldHealth = npcTankController.GetHealth();
        }
        if (Time.time - time > dataTeamRed.TimeToCheckForDemage)
        {
            if (oldHealth - npcTankController.GetHealth() >= dataTeamRed.MaxDemageInTime)
            {
                npcTankController.SetTransition(Transition.GoToFlee);
            }
            time = Time.time;
            oldHealth = npcTankController.GetHealth();
        }
    }

    public override void ActTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        Debug.Log("attacking");

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

        /*destPos = cumulativeEnemyPosition / enemyTanks.Count;

        Debug.Log($"destinationPosition {destPos}");

        redTank.gameObject.GetComponent<NavMeshAgent>().SetDestination(destPos);*/

        var npcTankController = redTank.gameObject.GetComponent<NPCTankControllerTeamRed>();

        //turn turret towards closest enemy tank
        var turret = npcTankController.turret;

        var turretRotation = Quaternion.LookRotation(closestTank.position - turret.position);
        turret.rotation = Quaternion.Slerp(turret.rotation, turretRotation, Time.deltaTime * curRotSpeed);

        if (Quaternion.Angle(turretRotation, turret.rotation) <= 20)
        {
            npcTankController.ShootBullet();
            npcTankController.HasShotInAttackState = true;
        }
    }
}
