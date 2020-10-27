using UnityEngine;

[CreateAssetMenu]
public class DataTeamRed : ScriptableObject
{
    public const float SpottingRange = 300;

    public const float AttackingRange = 150;

    public float EvadingRange = 50;

    public const float MaximumSlopeAngle = 45;

    public int ShootTimesDuringStrafing = 2;

    public bool Strafing = false;

    public float radiusCohesion = 20;
    public float radiusSeparation = 6;
    public float radiusAlignment = 10;
    public float weightCohesion = 60;
    public float weightSeparation = 90;
    public float weightAlignment = 90;

    public float TimeToCheckForDemage = 2;
    public float MaxDemageInTime = 50;
}
