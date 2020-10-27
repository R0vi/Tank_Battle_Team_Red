using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AttackStateTeamRed : FSMStateTeamRed
{
    public AttackStateTeamRed() 
    {
        stateIdTeamRed = FSMStateIDTeamRed.Attacking;
        curRotSpeed = 1.0f;
        curSpeed = 100.0f;
    }

    public override void ReasonTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        if (platoonRedTanks.All(x => x.gameObject.GetComponent<NPCTankControllerTeamRed>().HasShotInAttackState))
        {
            redTank.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.GoToStrafe);
        }
    }

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
