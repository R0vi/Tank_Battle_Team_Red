using UnityEngine;
using System.Collections;

public class DeadStateTeamRed : FSMStateTeamRed
{
    public DeadStateTeamRed() 
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
