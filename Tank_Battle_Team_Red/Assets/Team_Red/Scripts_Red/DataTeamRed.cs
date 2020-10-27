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

    public float RadiusCohesion = 300;
    public float RadiusSeparation = 56.3f;
    public float RadiusAlignment = 26.9f;
    public float WeightCohesion = 136.5f;
    public float WeightSeparation = 113;
    public float WeightAlignment = 62;
    public float WeightWander = 66f;

    public float TimeToCheckForDamage = 2;
    public float MaxDamageInTime = 50;
    public float LookAheadDistance = 5.35f;
    public float WanderJitter = 3.1f;
   
}
