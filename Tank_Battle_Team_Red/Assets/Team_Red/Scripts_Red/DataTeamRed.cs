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

    public float TurretRotationSpeed = 1.5f;
    public float TankRotationSpeed = 2.0f;
    public float MaxVelocity = 150f;
    public float MaxVelocityOnLowHealth = 75f;

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
    public float StrafeDistance = 10;
    public float StrafeDistanceErrorMargin = 2;

    public float UseCommonDestinationForSeconds = 20f;
    public float UseEmergentBehaviourForSeconds = 120f;
    public float CommonDestinationRadius = 600f;
    public Vector3? CommonDestination = null;
}
