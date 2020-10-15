using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadeStateTeamRed : FSMStateTeamRed
{
    public EvadeStateTeamRed()
    {
        stateIdTeamRed = FSMStateIDTeamRed.Evade;
    }
    public override void ReasonTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {

    }

    public override void ActTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        //Do Nothing for the evade state
    }
}
