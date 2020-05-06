using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    //////////////////////////////
    // PUBLIC VARIABLES
    //////////////////////////////

    [Header("Variables")]

    //Objects and components
    
    public GameObject head;

    //Variables
    public float detectRange;
    public float attackRange;
    public float minAttackAngle;
    public float turnSpeed;

    public int health;
    public int healthItemDropChance;
    public float damageShieldTime = 0.1f;

    public Enemy_Types enemyType;

    //////////////////////////////
    // COMPONENTS AND OBJECTS
    //////////////////////////////

    protected GameObject player;
    protected Transform pointsParent;
    protected Transform codingTrash;

    protected NavMeshAgent navAgent;
    protected ParticleSystem psBlood;
    protected Rigidbody rb;

    protected Animator animator;
    protected List<string> attackAnimations = new List<string>();
    protected List<float> attackAnimationTimers = new List<float>();
    protected float attackAnimationTimer = 0.0f;

    //////////////////////////////
    // POSITIONS AND DISTANCES
    //////////////////////////////

    protected int pointIndex = 0;
    protected Vector3 GoPos;

    protected float distanceFromPlayer;

    //////////////////////////////
    // BOOLS
    //////////////////////////////

    public bool isAttacking = false;
    protected bool isMoving = false;
    protected bool scriptEnabled = true;
    [HideInInspector]
    public bool ableToBeDamaged = true;

    //////////////////////////////
    // AUDIO CLIPS
    //////////////////////////////

    [Header("Audio")]

    public string deathSoundsObjectName = "Enemy_Human_Death_Sounds";

    protected AudioSource audioSource;
    protected AudioList deathSoundList;

    ////////////////////////////////////////////////////////////
    
    public virtual void Initialize()
    {

        Physics.IgnoreLayerCollision( 9, 12 ); //ignore bodyparts

        // Get objects

        player = GameObject.Find( "Player" );
        codingTrash = GameObject.Find( "Coding_Trash" ).transform;
        pointsParent = GameObject.Find( "Points" ).transform;
        psBlood = transform.Find( "ps_blood" ).GetComponent<ParticleSystem>();

        // Get components

        navAgent = GetComponent<NavMeshAgent>();
        if ( navAgent.isOnNavMesh == false )
        {
            Debug.Log( "Error: Spawned enemy not on navmesh" );
            Destroy( gameObject );
        }

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
    //
    //                  UPDATE FUNCTIONS
    //
    ////////////////////////////////////////////////////////////

    private void Update()
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

    public void Damage( int damage, float knockback )
    {
        if ( ableToBeDamaged == true )
        {
            animator.Play( "Hit_001" );

            //ableToBeDamaged = false;
            health -= damage;

            if ( health <= 0 )
            {
                head.GetComponent<AudioSource>().clip = deathSoundList.GetRandomSound();
                head.GetComponent<AudioSource>().Play();
                psBlood.Play();
                scriptEnabled = false;

                if ( Random.Range( 0, 100 ) <= 1 )
                    GameObject.Find( "GameManager" ).GetComponent<GameManager>().SpawnHealthItem( transform.position );
            }
            else
            {
                GetComponent<Rigidbody>().AddForce( -transform.forward * knockback, ForceMode.Impulse );
                //GetComponent<Rigidbody>().AddForce( transform.up * knockback / 2.0f, ForceMode.Impulse );

                StartCoroutine( damageShield() );
            }
        }
    }

    ////////////////////////////////////////////////////////////
    //
    //                   CHANGE STATES
    //
    ////////////////////////////////////////////////////////////

    public void ChangeAttackState( int state )
    {
        if ( state == 1 )
            isAttacking = true;
        else
        {
            navAgent.SetDestination( player.transform.position );
            isAttacking = false;
        }
    }

    ////////////////////////////////////////////////////////////

    public void ChangeMovingState( int state )
    {
        if ( state == 1 )
            isMoving = true;
        else
            isMoving = false;
    }

    ////////////////////////////////////////////////////////////
    //
    //                      ENUMERATORS
    //
    ////////////////////////////////////////////////////////////

    protected IEnumerator damageShield()
    {
        yield return new WaitForSeconds( damageShieldTime );
        ableToBeDamaged = true;
    }

}
