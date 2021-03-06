using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCTankControllerTeamRed : AdvancedFSMTeamRed
{
    public GameObject Bullet;
    private int health;
    public bool HasShotInAttackState;
	public bool HasFinishedStrafe;


    //Initialize the Finite state machine for the NPC tank
    protected override void InitializeTeamRed()
    {
        health = 100;

        elapsedTime = 0.0f;
        shootRate = 2.0f;

        //Get the turret of the tank
        turret = gameObject.transform.GetChild(0).transform;
        bulletSpawnPoint = turret.GetChild(0).transform;

        //Start Doing the Finite State Machine
        ConstructFSM();
    }

    //Update each frame
    protected override void FSMUpdateTeamRed()
    {
        //Check for health
        Debug.Log($"{gameObject.name} state: {CurrentStateTeamRed}");
        elapsedTime += Time.deltaTime;
    }

    protected override void FSMFixedUpdate()
    {
        var platoonRedTanks = GetPlatoonRedTanks();
        var enemyTanks = GetEnemyTanks(platoonRedTanks);
        CurrentStateTeamRed.ReasonTeamRed(transform, platoonRedTanks, enemyTanks);
        CurrentStateTeamRed.ActTeamRed(transform, platoonRedTanks, enemyTanks);
    }

    private IList<Transform> GetPlatoonRedTanks()
    {
        return GameObject.FindGameObjectsWithTag("RedTank")
            .Select(redTank => redTank.GetComponent<Transform>()).ToList();
    }

    private IList<Transform> GetEnemyTanks(IList<Transform> platoonRedTanks)
    {
        var position = Vector3.zero;

        foreach (var platoonRedTank in platoonRedTanks)
        {
            position += platoonRedTank.position;
        }

        var averagePosition = position / platoonRedTanks.Count;

        var enemyTanks = new List<Transform>();
        var enemyTags = new List<string>() { "PurpleTank", "PinkTank", "GreenTank", "BlackTank" };
        foreach (var tag in enemyTags)
        {
            var enemies = GameObject.FindGameObjectsWithTag(tag);
            foreach (var enemyTank in enemies)
            {
                var enemyTransform = enemyTank.GetComponent<Transform>();

                if (Vector3.Distance(averagePosition, enemyTransform.position) < DataTeamRed.SpottingRange)
                {
                    enemyTanks.Add(enemyTransform);
                }
            }
        }

        return enemyTanks;
    }

    public void SetTransition(Transition t) 
    { 
        PerformTransitionTeamRed(t); 
    }

    private void ConstructFSM()
    {
        var patrol = new PatrolStateTeamRed();
        patrol.AddTransitionTeamRed(Transition.SawPlayer, FSMStateIDTeamRed.Chasing);
        patrol.AddTransitionTeamRed(Transition.NoHealth, FSMStateIDTeamRed.Dead);
        patrol.AddTransitionTeamRed(Transition.EnemyTooClose, FSMStateIDTeamRed.Evade);

        var chase = new ChaseStateTeamRed();
        chase.AddTransitionTeamRed(Transition.LostPlayer, FSMStateIDTeamRed.Patrolling);
        chase.AddTransitionTeamRed(Transition.ReachPlayer, FSMStateIDTeamRed.Attacking);
        chase.AddTransitionTeamRed(Transition.NoHealth, FSMStateIDTeamRed.Dead);
        chase.AddTransitionTeamRed(Transition.EnemyTooClose, FSMStateIDTeamRed.Evade);

        var attack = new AttackStateTeamRed();
        attack.AddTransitionTeamRed(Transition.LostPlayer, FSMStateIDTeamRed.Patrolling);
        attack.AddTransitionTeamRed(Transition.GoToStrafe, FSMStateIDTeamRed.Strafing);
        attack.AddTransitionTeamRed(Transition.SawPlayer, FSMStateIDTeamRed.Chasing);
        attack.AddTransitionTeamRed(Transition.NoHealth, FSMStateIDTeamRed.Dead);
        attack.AddTransitionTeamRed(Transition.GoToFlee, FSMStateIDTeamRed.Fleeing);
        attack.AddTransitionTeamRed(Transition.EnemyTooClose, FSMStateIDTeamRed.Evade);

        var dead = new DeadStateTeamRed();
        dead.AddTransitionTeamRed(Transition.NoHealth, FSMStateIDTeamRed.Dead);
		
		var strafe = new StrafeStateTeamRed();
		strafe.AddTransitionTeamRed(Transition.ReachPlayer, FSMStateIDTeamRed.Attacking);
        strafe.AddTransitionTeamRed(Transition.SawPlayer, FSMStateIDTeamRed.Chasing);
        strafe.AddTransitionTeamRed(Transition.EnemyTooClose, FSMStateIDTeamRed.Evade);

        var evade = new EvadeStateTeamRed();
        evade.AddTransitionTeamRed(Transition.LostPlayer, FSMStateIDTeamRed.Patrolling);

        var flee = new FleeStateTeamRed();
        flee.AddTransitionTeamRed(Transition.LostPlayer, FSMStateIDTeamRed.Patrolling);
        flee.AddTransitionTeamRed(Transition.EnemyTooClose, FSMStateIDTeamRed.Evade);

        AddFSMStateTeamRed(patrol);
        AddFSMStateTeamRed(chase);
        AddFSMStateTeamRed(attack);
        AddFSMStateTeamRed(dead);
		AddFSMStateTeamRed(strafe);
        AddFSMStateTeamRed(evade);
        AddFSMStateTeamRed(flee);
    }

    /// <summary>
    /// Check the collision with the bullet
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        //Reduce health
        if (collision.gameObject.tag == "Bullet")
        {
            health -= 25;

            if (health <= 0)
            {
                Debug.Log("Switch to Dead State");
                SetTransition(Transition.NoHealth);
                Explode();
            }
        }
    }

    protected void Explode()
    {
        float rndX = Random.Range(10.0f, 30.0f);
        float rndZ = Random.Range(10.0f, 30.0f);
        for (int i = 0; i < 3; i++)
        {
            GetComponent<Rigidbody>().AddExplosionForce(10000.0f, transform.position - new Vector3(rndX, 10.0f, rndZ), 40.0f, 10.0f);
            GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(rndX, 20.0f, rndZ));
        }

        Destroy(gameObject, 1.5f);
    }

    /// <summary>
    /// Shoot the bullet from the turret
    /// </summary>
    public void ShootBullet()
    {
        if (elapsedTime >= shootRate)
        {
            Instantiate(Bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            elapsedTime = 0.0f;
        }
    }

    public int GetHealth()
    {
        return health;
    }
}
