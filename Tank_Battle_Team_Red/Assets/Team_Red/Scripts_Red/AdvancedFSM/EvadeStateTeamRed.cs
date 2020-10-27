using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadeStateTeamRed : FSMStateTeamRed
{
    private bool evadeRight;
    private Quaternion evadeRotation;
    private bool isDone;
    public EvadeStateTeamRed()
    {
        stateIdTeamRed = FSMStateIDTeamRed.Evade;

        curRotSpeed = 1.0f;
        curSpeed = 100.0f;

        evadeRight = true;
        isDone = false;
    }

    public override void ReasonTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        if (isDone)
        {
            redTank.GetComponent<NPCTankControllerTeamRed>().SetTransition(Transition.LostPlayer);
        }
    }

    public override void ActTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        if (evadeRight)
        {
            Debug.Log("Right rotation");
            evadeRotation = Quaternion.LookRotation(redTank.right);

            redTank.rotation = Quaternion.Slerp(redTank.rotation, evadeRotation, Time.deltaTime * curRotSpeed);
            Debug.Log($"Rotation {redTank.rotation}, {redTank.gameObject.name}");
            redTank.Translate(Vector3.forward * Time.deltaTime * curSpeed);

            float angle = Quaternion.Angle(evadeRotation, redTank.rotation);

            if (angle <= 10f)
            {
                Debug.Log("Left rotation");
                evadeRight = false;
            }
        }
        if (evadeRight == false)
        {
            evadeRotation = Quaternion.LookRotation(-redTank.right);

            redTank.rotation = Quaternion.Slerp(redTank.rotation, evadeRotation, Time.deltaTime * curRotSpeed);

            redTank.Translate(Vector3.forward * Time.deltaTime * curSpeed);

            float angle = Quaternion.Angle(evadeRotation, redTank.rotation);

            if(angle <= 10f)
            {
                isDone = true;
            }
        }

    }
}
