using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : Enemy
{
    //////////////////////////////
    // VARIABLES
    //////////////////////////////

    [Header("Melee Variables")]

    //Variables
    public int attackDamage;
    public float chargeSpeed;
    public float chargeLength;
    public float chargeYOffset = 2.0f;

    //////////////////////////////
    // COMPONENTS AND OBJECTS
    //////////////////////////////


    //////////////////////////////
    // POSITIONS AND DISTANCES
    //////////////////////////////


    //////////////////////////////
    // BOOLS
    //////////////////////////////

    bool isCharging = false;

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

        //get animations
        for ( int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++ )
        {
            if ( animator.runtimeAnimatorController.animationClips[ i ].name.Contains( "Attack" ) )
            {
                attackAnimations.Add( animator.runtimeAnimatorController.animationClips[ i ].name );
                attackAnimationTimers.Add( animator.runtimeAnimatorController.animationClips[ i ].length );
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

    private void OnCollisionEnter( Collision collision )
    {
        if ( isCharging )
        {
            if ( collision.gameObject.name.Contains( "Player" ) )
            {
                collision.gameObject.GetComponent<PlayerController>().Damage( attackDamage );
                isCharging = false;
                animator.SetBool( "IsCharging", false );
            }
            else if ( Physics.Raycast( new Vector3( transform.position.x, transform.position.y, transform.position.z ), transform.forward, 2.0f )
                && collision.gameObject.name.Contains( "&&" ) == false )
            {
                isCharging = false;
                animator.SetBool( "IsCharging", false );
            }
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

        if ( isAttacking == false && isCharging == false )
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
                    int animationIndex = Random.Range( 0, attackAnimations.Count );
                    animator.Play( attackAnimations[ animationIndex ] );
                    attackAnimationTimer = attackAnimationTimers[ animationIndex ];
                }

                navAgent.SetDestination( transform.position );
            }
            else
            {
                isMoving = true;
                navAgent.SetDestination( GoPos );
            }
        }
        else if ( isCharging == true )
        {
            transform.position += transform.forward * chargeSpeed * Time.deltaTime;
        }
        else
        {
            attackAnimationTimer -= Time.deltaTime;
            if ( attackAnimationTimer < 0.0f )
                isAttacking = false;
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
    //
    //                MELEE FUNCTIONS
    //
    ////////////////////////////////////////////////////////////

    public void DamagePlayer()
    {
        if ( distanceFromPlayer < attackRange )
        {
            Vector3 enemyToPlayer = player.transform.position - transform.position;
            enemyToPlayer = enemyToPlayer.normalized;

            float dotprod = Vector3.Dot( transform.forward, enemyToPlayer );

            if ( dotprod > minAttackAngle )
            {
                player.GetComponent<PlayerController>().Damage( attackDamage );
            }
        }
    }

    ////////////////////////////////////////////////////////////
    //
    //                   CHANGE STATES
    //
    ////////////////////////////////////////////////////////////

    public void ChangeChargingState( int state )
    {
        if ( state == 1 )
        {
            StartCoroutine( ChargeLengthEnumerator() );
            isCharging = true;
        }
        else
            isCharging = false;
    }

    ////////////////////////////////////////////////////////////
    //
    //                      ENUMERATORS
    //
    ////////////////////////////////////////////////////////////

    IEnumerator ChargeLengthEnumerator()
    {
        yield return new WaitForSeconds( chargeLength );
        isCharging = false;
    }
}
