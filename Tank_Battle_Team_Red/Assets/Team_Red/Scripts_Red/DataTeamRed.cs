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
}
