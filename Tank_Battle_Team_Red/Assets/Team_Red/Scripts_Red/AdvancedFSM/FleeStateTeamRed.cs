using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeStateTeamRed : FSMStateTeamRed
{
    public FleeStateTeamRed()
    {
        stateIdTeamRed = FSMStateIDTeamRed.Fleeing;
    }

    public override void ActTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        throw new System.NotImplementedException();
        //Moet een positie vinden die de andere kant op is dan waar de vijand is
        //Richting die positie rijden
    }

    public override void ReasonTeamRed(Transform redTank, IList<Transform> platoonRedTanks, IList<Transform> enemyTanks)
    {
        throw new System.NotImplementedException();
        //Als de afstand tussen het vijand platoon en de eigen tank platoon ver genoeg is, terug naar patrol state
    }
}
