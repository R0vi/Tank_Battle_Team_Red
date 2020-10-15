using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeadStateTeamRed : FSMStateTeamRed
{
    public DeadStateTeamRed() 
    {
        stateIdTeamRed = FSMStateIDTeamRed.Dead;
    }

    public override void ReasonTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {

    }

    public override void ActTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        //Do Nothing for the dead state
    }
}
