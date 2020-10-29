using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EvadeStateTeamRed : FSMStateTeamRed
{
    private bool firstRotationDone;
    private Quaternion evadeRotation;
    private bool isDone;
    private bool isStarted;

    public EvadeStateTeamRed()
    {
        stateIdTeamRed = FSMStateIDTeamRed.Evade;

        curRotSpeed = 1.0f;
        curSpeed = 100.0f;
    }

    public override void ReasonTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        if (isDone)
        {
            redTank.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.LostPlayer);
            firstRotationDone = false;
            isDone = false;
            isStarted = false;
        }
    }

    public override void ActTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        if (!isStarted)
        {
            // Initially rotate to the right
            evadeRotation = evadeRotation = Quaternion.LookRotation(redTank.right);
            isStarted = true;
        }

        redTank.rotation = Quaternion.Slerp(redTank.rotation, evadeRotation, Time.deltaTime * dataTeamRed.TankRotationSpeed);

        redTank.Translate(Vector3.forward * Time.deltaTime * dataTeamRed.MaxEvadeVelocity);

        var angleToDesiredRotation = Quaternion.Angle(evadeRotation, redTank.rotation);

        if (angleToDesiredRotation <= 20f)
        {
            if (firstRotationDone)
            {
                isDone = true;
            }
            else
            {
                evadeRotation = Quaternion.LookRotation(-redTank.right);
                firstRotationDone = true;
            }
        }
    }
}
