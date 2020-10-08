using UnityEngine;
using System.Collections;

public class DeadStateTeamRedTeamRed : FSMStateTeamRed
{
    public DeadStateTeamRedTeamRed() 
    {
        stateIdTeamRed = FSMStateIDTeamRed.Dead;
    }

    public override void ReasonTeamRed(Transform player, Transform npc)
    {

    }

    public override void ActTeamRed(Transform player, Transform npc)
    {
        //Do Nothing for the dead state
    }
}
