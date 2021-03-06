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

        if (!enemyTanks.Any())
        {
            redTank.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.LostPlayer);
        }

        foreach (var enemyTank in enemyTanks)
        {
            var distanceToEnemyTank = Vector3.Distance(redTank.position, enemyTank.position);
           
            if (distanceToEnemyTank > DataTeamRed.AttackingRange)
            {
                Debug.Log("Switch to Chase State");
                redTank.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.SawPlayer);
                redTank.gameObject.GetComponent<NavMeshAgent>().isStopped = false;
                return;
            }
            if (Vector3.Distance(redTank.position, enemyTank.position) <= dataTeamRed.EvadingRange)
            {
                redTank.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                redTank.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.EnemyTooClose);
                break;
            }
        }

        if (platoonRedTanks.All(x => x.gameObject.GetComponent<NPCTankControllerTeamRed>().HasShotInAttackState))
        {
            Debug.Log("Switch to Strafe State");
            npcTankController.SetTransition(Transition.GoToStrafe);
            redTank.gameObject.GetComponent<NavMeshAgent>().isStopped = false;
            return;
        }

        if (time == 0)
        {
            time = Time.time;
            oldHealth = npcTankController.GetHealth();
        }
        if (Time.time - time > dataTeamRed.TimeToCheckForDamage)
        {
            if (oldHealth - npcTankController.GetHealth() >= dataTeamRed.MaxDamageInTime)
            {
                redTank.GetComponent<NavMeshAgent>().isStopped = false;
                npcTankController.SetTransition(Transition.GoToFlee);
                return;
            }
            time = Time.time;
            oldHealth = npcTankController.GetHealth();
        }
    }

    public override void ActTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
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

        var npcTankController = redTank.gameObject.GetComponent<NPCTankControllerTeamRed>();

        //turn turret towards closest enemy tank
        var turret = npcTankController.turret;

        var turretRotation = Quaternion.LookRotation(closestTank.position - turret.position);
        turret.rotation = Quaternion.Slerp(turret.rotation, turretRotation, Time.deltaTime * dataTeamRed.TurretRotationSpeed);

        if (Quaternion.Angle(turretRotation, turret.rotation) <= 20)
        {
            npcTankController.ShootBullet();
            npcTankController.HasShotInAttackState = true;
        }
    }
}
