using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : Enemy
{
    //////////////////////////////
    // PUBLIC VARIABLES
    //////////////////////////////

    [Header("Ranged Variables")]

    //Objects and components
    
    public List<GameObject> projectiles;

    //Variables


    //////////////////////////////
    // COMPONENTS AND OBJECTS
    //////////////////////////////




    //////////////////////////////
    // POSITIONS AND DISTANCES
    //////////////////////////////



    //////////////////////////////
    // BOOLS
    //////////////////////////////



    //////////////////////////////
    // AUDIO CLIPS
    //////////////////////////////

    public string shootSoundsObjectName;

    AudioList shootSoundList;

    ////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////

    public override void Initialize()
    {
        Physics.IgnoreLayerCollision( 9, 12 ); //ignore bodyparts

        // Get objects

        player = GameObject.Find( "Player" );
        codingTrash = GameObject.Find( "Coding_Trash" ).transform;
        pointsParent = GameObject.Find( "Points" ).transform;
        psBlood = transform.Find( "ps_blood" ).GetComponent<ParticleSystem>();

        // Get components

        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        deathSoundList = GameObject.Find( deathSoundsObjectName ).GetComponent<AudioList>();
        shootSoundList = GameObject.Find( shootSoundsObjectName ).GetComponent<AudioList>();

        //get animations
        for ( int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++ )
        {
            if ( animator.runtimeAnimatorController.animationClips[ i ].name.Contains( "Attack" ) )
            {
                attackAnimations.Add( animator.runtimeAnimatorController.animationClips[ i ].name );
            }
        }

        // Set AI destination
        distanceFromPlayer = Vector3.Distance( transform.TransformPoint( new Vector3( 0.0f, 1.0f, 0.0f ) ), player.transform.position );

        if ( distanceFromPlayer < detectRange )
        {
            isMoving = true;
            navAgent.SetDestination( player.transform.position );
        }
        else
        {
            isMoving = true;
            GoPos = pointsParent.GetChild( pointIndex ).position;
            GoPos.y = transform.position.y;
            navAgent.SetDestination( GoPos );
        }
    }

    ////////////////////////////////////////////////////////////
    //
    //                  UPDATE FUNCTIONS
    //
    ////////////////////////////////////////////////////////////

    void Update()
    {
        if ( scriptEnabled == false )
            return;

        animator.SetBool( "IsMoving", isMoving );

        //////////////////////////////////////////
        // SUCCESSFULLY WALKED TO POINT
        //////////////////////////////////////////

        if ( Vector3.Distance( transform.position, GoPos ) < 1.0f )
        {
            //Increase point index

            pointIndex++;
            if ( pointIndex >= pointsParent.childCount )
                pointIndex = 0;

            Vector3 pointPos = pointsParent.GetChild( pointIndex ).position;

            // Go towards new point

            GoPos = pointsParent.GetChild( pointIndex ).position;
            GoPos.y = transform.position.y;
            navAgent.SetDestination( GoPos );
        }

        //////////////////////////////////////////
        // PLAYER IS NEAR?
        //////////////////////////////////////////

        if ( isAttacking == false )
        {
            if ( distanceFromPlayer < detectRange && distanceFromPlayer > attackRange )
            {
                isMoving = true;
                navAgent.SetDestination( player.transform.position );
            }
            else if ( distanceFromPlayer < attackRange )
            {
                isMoving = false;

                if ( isAttacking == false )
                {
                    transform.rotation = Quaternion.Slerp( transform.rotation, Quaternion.LookRotation( player.transform.position - transform.position ), turnSpeed * Time.deltaTime );
                }

                Vector3 enemyToPlayer = player.transform.position - transform.position;
                enemyToPlayer = enemyToPlayer.normalized;

                float dotprod = Vector3.Dot( transform.forward, enemyToPlayer );
                if ( dotprod > minAttackAngle )
                {
                    animator.Play( attackAnimations[ Random.Range( 0, attackAnimations.Count ) ] );
                }

                navAgent.SetDestination( transform.position );
            }
            else
            {
                isMoving = true;
                navAgent.SetDestination( GoPos );
            }
        }
    }

    ////////////////////////////////////////////////////////////

    private void FixedUpdate()
    {
        distanceFromPlayer = Vector3.Distance( transform.position, player.transform.position );

        if ( ableToBeDamaged == true )
            GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    ////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////
    //
    //                  SHOOT FUNCTIONS
    //
    ////////////////////////////////////////////////////////////

    public void ShootBullet( int _index )
    {
        //////////////////////////////
        // MAKE BULLET READY
        //////////////////////////////

        Vector3 pos = Vector3.forward * 2;
        Vector3 newPos = transform.TransformPoint( pos );

        //////////////////////////////
        // SHOOT BULLET
        //////////////////////////////

        GameObject bullet = Instantiate( projectiles[ _index ], codingTrash  );
        bullet.transform.position = newPos;
        bullet.transform.rotation = Quaternion.LookRotation( player.transform.position - transform.position );
        bullet.GetComponent<Basic_Enemy_Musketball>().Activate();
        audioSource.clip = shootSoundList.GetRandomSound();
        audioSource.Play();
    }

    ////////////////////////////////////////////////////////////

    public void SummonProjectileOnPlayer( int _index )
    {
        //////////////////////////////
        // MAKE BULLET READY
        //////////////////////////////

        Vector3 pos = player.transform.position;

        //////////////////////////////
        // SHOOT BULLET
        //////////////////////////////

        GameObject bullet = Instantiate( projectiles[ _index ], codingTrash );
        bullet.transform.position = pos;
        audioSource.clip = shootSoundList.GetRandomSound();
        audioSource.Play();
    }

    ////////////////////////////////////////////////////////////

    public void SummonProjectileOnSelf( int _index )
    {
        //////////////////////////////
        // MAKE BULLET READY
        //////////////////////////////

        Vector3 pos = transform.position;

        //////////////////////////////
        // SHOOT BULLET
        //////////////////////////////

        GameObject projectile = Instantiate( projectiles[ _index ], codingTrash );
        projectile.transform.position = pos;
        projectile.transform.LookAt( player.transform );
        audioSource.clip = shootSoundList.GetRandomSound();
        audioSource.Play();
    }

    ////////////////////////////////////////////////////////////
    //
    //                   CHANGE STATES
    //
    ////////////////////////////////////////////////////////////



    ////////////////////////////////////////////////////////////
    //
    //                      ENUMERATORS
    //
    ////////////////////////////////////////////////////////////


}
