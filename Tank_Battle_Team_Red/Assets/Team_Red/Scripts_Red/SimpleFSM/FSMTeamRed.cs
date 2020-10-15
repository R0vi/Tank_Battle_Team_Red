using UnityEngine;
using System.Collections;

public class FSMTeamRed : MonoBehaviour 
{
    //Player Transform
    protected Transform playerTransform;

    //Next destination position of the NPC Tank
    protected Vector3 destPos;

    //List of points for patrolling
    protected GameObject[] pointList;

    //Bullet shooting rate
    protected float shootRate;
    protected float elapsedTime;

    //Tank Turret
    public Transform turret { get; set; }
    public Transform bulletSpawnPoint { get; set; }

    protected virtual void InitializeTeamRed() { }
    protected virtual void FSMUpdateTeamRed() { }
    protected virtual void FSMFixedUpdate() { }

	// Use this for initialization
	void Start () 
    {
        InitializeTeamRed();
	}
	
	// Update is called once per frame
	void Update () 
    {
        FSMUpdateTeamRed();
	}

    void FixedUpdate()
    {
        FSMFixedUpdate();
    }    
}
