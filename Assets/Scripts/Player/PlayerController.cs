using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct PlayerController_InitValues
{
    public int movementSpeed;
    public int fallSpeed;
}

enum AudioSources
{
    AUDIO_SHOOT_FIRST,
    AUDIO_SHOOT_SECOND,
    AUDIO_SHOOT_THIRD,
    AUDIO_WALKING,
    AUDIO_OTHER
}

public class PlayerController : MonoBehaviour
{

    //playercontroller from Room shooter
    //able to walk, jump, crouch
    //a bit updated

    //////////////////////////////
    // VARIABLES
    //////////////////////////////

    //Public variables
    [Header("Variables")]
    public int health = 100;
    public float movementSpeed = 10.0f;

    public float dashingSpeed = 10.0f;
    public int dashDamage = 40;
    public float dashKnockback = 20.0f;

    public float fallSpeed = 1.0f;
    public float shootForceUp = 200.0f;
    public float shootForceBackwards = 50.0f;

    //public List<string> cannonball_amounts_strings;

    //Private variables

    int healthDefault;
    //int ammoIndex = 0;
    int cannonballIndex = 0;
    float dashDamageTimer = 0.0f;

    Vector3 dashDirection;

    //////////////////////////////////////////
    // PUBLIC COMPONENTS AND OBJECTS
    //////////////////////////////////////////

    [Header("Game Objects")]
    public GameObject playerPerspectiveCamera;
    public GameObject hurtEffect;
    public GameObject lose_screen;
    public Text healthText;

    public GameObject treasureChest;
    public GameObject cannon;
    public GameObject aimAssist;
    public GameObject shootPos;
    public List<Player_Ammunition> cannonballPrefabs = new List<Player_Ammunition>();
    public ParticleSystem psShootCloud;

    //////////////////////////////
    // BOOLS
    //////////////////////////////

    bool active = true;
    bool flying = true;
    bool ableToShoot = true;

    bool ableToBeDamaged = true;
    bool ableToBeBalanced = true;
    bool ableToDash = false;

    [HideInInspector]
    public bool isDashing = false;
    bool autoshoot = false;
    [HideInInspector]
    public bool moving = false;

    bool isChildOfPlayer = false;
    [HideInInspector]
    public bool hasTreasureChest = false;

    //////////////////////////////
    // COMPONENTS AND OBJECTS
    //////////////////////////////

    TrashCollector trashCollector;
    GameManager gameManager;

    //scripts, components
    BoxCollider boxCol;
    Plane plane = new Plane(Vector3.up, Vector3.zero);

    //List<GameObject> cannonBalls = new List<GameObject>();
    Text cannonballCounter;
    RawImage cannonballImage;

    Animator animator;

    //////////////////////////////
    // AUDIO
    //////////////////////////////

    AudioSource[] audioSources;
    int audioIndex = 0;

    public AudioClip healedSound;

    public void Initialize()
    {
        Physics.IgnoreLayerCollision( 9, 12 );

        if ( GameObject.Find( "DataManager" ) )
        {
            DataManager dataManager = GameObject.Find( "DataManager" ).GetComponent<DataManager>();
            health = Mathf.RoundToInt( health * dataManager.GetGameValue( EGame_Values_Keys.PLAYER_HEALTH_MODIFIER ) );
        }

        healthDefault = health;

        cannonballCounter = GameObject.Find( "Cannonball_Counter" ).GetComponent<Text>();
        cannonballImage = GameObject.Find( "Cannonball_Image" ).GetComponent<RawImage>();

        trashCollector = GameObject.Find( "Coding_Trash" ).GetComponent<TrashCollector>();
        gameManager = GameObject.Find( "GameManager" ).GetComponent<GameManager>();

        boxCol = GetComponent<BoxCollider>();
        healthText = GameObject.Find( "health_text" ).GetComponent<Text>();
        healthText.text = "Health: " + health.ToString();

        audioSources = GetComponents<AudioSource>();
        animator = GetComponent<Animator>();

        if( PlayerPrefs.HasKey( "AutoShoot" ) )
            if ( PlayerPrefs.GetInt( "Autoshoot" ) == 1 )
                autoshoot = true;

        if( PlayerPrefs.HasKey( "AimAssist" ) )
            if ( PlayerPrefs.GetInt( "AimAssist" ) == 1 )
                aimAssist.SetActive( true );
            else
                aimAssist.SetActive( false );

        if ( PlayerPrefs.HasKey( "Dash_Upgrade" ) == true )
            if ( PlayerPrefs.GetInt( "Dash_Upgrade" ) == 1 )
                ableToDash = true;

        int count = 0;
        foreach ( Player_Ammunition ammo in cannonballPrefabs )
        {
            if ( PlayerPrefs.HasKey( ammo.amountPrefKey ) == true )
                if ( PlayerPrefs.GetInt( ammo.amountPrefKey ) > 0 )
                {
                    cannonballIndex = count;
                    break;
                }
            count++;
        }

        cannonballCounter.text = PlayerPrefs.GetInt( cannonballPrefabs[ cannonballIndex ].amountPrefKey ).ToString();

    }

    void Update()
    {
        if (active)
        {
            Input_ThirdPerson();
        }

        if ( Input.GetKeyDown( KeyCode.Escape ) )
        {
            gameManager.EndGame();
        }
    }

    private void OnCollisionEnter( Collision collision )
    {
        //////////////////////////////
        // HIT ENEMY
        //////////////////////////////

        if ( collision.gameObject.tag == "Enemy" && isDashing && dashDamageTimer > 0.07f )
        {
            if ( collision.gameObject.GetComponent<Enemy>().health - dashDamage <= 0
                && collision.gameObject.GetComponent<Enemy>().ableToBeDamaged == true )
            {
                collision.gameObject.GetComponent<Enemy>().Damage( dashDamage, dashKnockback );
                collision.gameObject.tag = "Untagged";
                Destroy( collision.gameObject.GetComponent<Enemy>() );
                Destroy( collision.gameObject.GetComponent<Animator>() );
                Destroy( collision.gameObject.GetComponent<BoxCollider>() );
                Destroy( collision.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>() );
                KillParent( collision.transform );
                trashCollector.AddToTrash( collision.gameObject );

                ChangeDashingState( 0 );
            }
            else
                collision.gameObject.GetComponent<Enemy>().Damage( dashDamage, dashKnockback );
        }
    }

    void Input_ThirdPerson()
    {
        //////////////////////////////
        // DIRECTIONAL MOVEMENT
        //////////////////////////////

        if( moving && audioSources[ (int)AudioSources.AUDIO_WALKING ].isPlaying == false )
        {
            audioSources[ (int)AudioSources.AUDIO_WALKING ].Play();
        }

        if( isDashing == false )
        {
            if ( Input.GetKey( KeyCode.W ) )
            {
                transform.position += transform.forward * movementSpeed * Time.deltaTime;
                moving = true;
                animator.SetBool( "walking", true );

                if ( Input.GetKeyDown( KeyCode.LeftShift ) 
                    && ableToShoot == true 
                    && Input.GetKey( KeyCode.A ) == false 
                    && Input.GetKey( KeyCode.D ) == false
                    && ableToDash == true )
                {
                    dashDirection = transform.forward;
                    isDashing = true;
                    PlayAnimation( "Dash_Forward" );
                }
            }
            else if ( Input.GetKey( KeyCode.S ) )
            {
                transform.position -= transform.forward * movementSpeed * Time.deltaTime;
                moving = true;
                animator.SetBool( "walking", true );

                if ( Input.GetKeyDown( KeyCode.LeftShift ) 
                    && ableToShoot == true 
                    && Input.GetKey( KeyCode.A ) == false 
                    && Input.GetKey( KeyCode.D ) == false
                    && ableToDash == true )
                {
                    dashDirection = -transform.forward;
                    isDashing = true;
                    PlayAnimation( "Dash_Back" );
                }
            }
            if ( Input.GetKey( KeyCode.D ) )
            {
                transform.position += transform.right * movementSpeed * Time.deltaTime;
                moving = true;
                animator.SetBool( "walking", true ); 

                if ( Input.GetKeyDown( KeyCode.LeftShift ) 
                    && ableToShoot == true 
                    && Input.GetKey( KeyCode.S ) == false
                    && Input.GetKey( KeyCode.W ) == false
                    && ableToDash == true )
                {
                    dashDirection = transform.right;
                    isDashing = true;
                    PlayAnimation( "Dash_Right" );
                }
            }
            else if ( Input.GetKey( KeyCode.A ) )
            {
                transform.position -= transform.right * movementSpeed * Time.deltaTime;
                moving = true;
                animator.SetBool( "walking", true ); 

                if ( Input.GetKeyDown( KeyCode.LeftShift ) 
                    && ableToShoot == true 
                    && Input.GetKey( KeyCode.S ) == false 
                    && Input.GetKey( KeyCode.W ) == false 
                    && ableToDash == true )
                {
                    dashDirection = -transform.right;
                    PlayAnimation( "Dash_Left" );
                    isDashing = true;
                }
            }

            //////////////////////////////
            // IS IDLE
            //////////////////////////////

            if ( Input.GetKeyUp( KeyCode.W ) || Input.GetKeyUp( KeyCode.S ) || Input.GetKeyUp( KeyCode.D ) || Input.GetKeyUp( KeyCode.A ) )
            {
                StartCoroutine( NotMoving() );
            }
        }
        else if ( isDashing == true )
        {
            dashDamageTimer += Time.deltaTime;

            RaycastHit hit = new RaycastHit();
            Ray ray = new Ray( new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), dashDirection );
            Physics.Raycast( ray, out hit, 0.5f );
            if ( hit.collider != null )
            {
                FindPlayerParent( hit.collider.transform );
                if ( hit.collider.name.Contains( "&&" ) == false && isChildOfPlayer == false )
                {
                    ChangeDashingState( 0 );
                }
                else
                    isChildOfPlayer = false;
            }
            else
            {
                transform.position += dashDirection * dashingSpeed * Time.deltaTime;
            }
        }

        //////////////////////////////
        // VIBE CHECK!
        //////////////////////////////

        flying = true;
        if ( Physics.Raycast( transform.position, transform.TransformDirection( Vector3.down ), 1.0f ) )
        {
            flying = false;
        }

        if ( Physics.Raycast( transform.position, transform.TransformDirection( Vector3.down ), 1.0f ) 
            && ableToBeBalanced == true 
            && flying == false 
            && transform.position.y > -4.0f)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        //////////////////////////////
        // CANNONBALL FIRE
        //////////////////////////////

        if ( Input.GetKeyDown( KeyCode.F ) && PlayerPrefs.GetInt( cannonballPrefabs[ cannonballIndex ].amountPrefKey ) > 0 
            || autoshoot == true && PlayerPrefs.GetInt( cannonballPrefabs[ cannonballIndex ].amountPrefKey ) > 0 )
        {
            if ( ableToShoot == true )
            {
                ableToShoot = false;
                PlayAnimation( "Attack001" );
                Shoot();
            }
        }

        //////////////////////////////
        // SWITCH CANNONBALL
        //////////////////////////////

        if ( Input.GetKeyDown( KeyCode.Alpha2 ) )
            ChangeCannonballIndex( 0 );

        else if ( Input.GetKeyDown( KeyCode.Alpha1 ) )
            ChangeCannonballIndex( 1 );

        //if (transform.position.y <= 0 )
        //{
        //    transform.position = new Vector3( transform.position.x, 0, transform.position.z );
        //}
    }

    //private void LateUpdate()
    //{
    //    string m_ClipName;
    //    AnimatorClipInfo[] m_CurrentClipInfo = animator.GetCurrentAnimatorClipInfo(0);

    //    if ( m_CurrentClipInfo[ 0 ].clip.name == "Idle" && ableToShoot == false )
    //        ableToShoot = true;
    //}

    //damage player
    public void Damage(int amount)
    {
        if ( ableToBeDamaged && active == true )
        {
            //////////////////////////////
            // DAMAGE PLAYER
            //////////////////////////////

            StartCoroutine( NotBalanced() );
            GetComponent<Rigidbody>().AddForce( -transform.forward * shootForceBackwards, ForceMode.Impulse );
            GetComponent<Rigidbody>().AddForce( transform.up * shootForceUp, ForceMode.Impulse );
            //ableToBeDamaged = false;
            health -= amount;
            healthText.text = "Health: " + health.ToString();
            //hurtEffect.GetComponent<Animator>().Play("PlayerHurt");

            //////////////////////////////
            // PLAYER IS DEAD
            //////////////////////////////

            if ( health <= 0 )
            {
                Camera.main.GetComponent<CameraController>().ChangeState( false );
                healthText.text = "Health: DEAD";
                lose_screen.SetActive(true);
                PlayAnimation( "Dying" );

                if( DataManager.singleton != null )
                    DataManager.singleton.AddLog( "Joe was brutally torn apart and died" );

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                active = false;
            }
        }
    }

    public void Shoot()
    {
        isDashing = false;

        PlayerPrefs.SetInt( cannonballPrefabs[ cannonballIndex ].amountPrefKey
            , PlayerPrefs.GetInt( cannonballPrefabs[ cannonballIndex ].amountPrefKey ) - 1 );

        //ammoIndex++;
        //if ( ammoIndex >= 20 )
        //    ammoIndex = 0;

        cannonballCounter.text = PlayerPrefs.GetInt( cannonballPrefabs[ cannonballIndex ].amountPrefKey ).ToString();

        //////////////////////////////
        // MAKE CANNONBALL READY
        //////////////////////////////

        Vector3 pos = Vector3.zero;
        Vector3 newPos = shootPos.transform.TransformPoint( pos );
        //List<Transform> children = new List<Transform>();
        //foreach (Transform trans in cannonBalls[ ammoIndex ].transform )
        //    children.Add( trans );
        //foreach ( Transform trans in children )
        //    trashCollector.AddToTrash( trans.gameObject );

        //foreach ( Transform trans in children )
        //{
        //    if ( trans.GetComponent<Collider>() )
        //    {
        //        int colliderCount = trans.GetComponents<Collider>().Length;
        //        for ( int colliderIndex = 0; colliderIndex < colliderCount; colliderIndex++ )
        //            trans.GetComponent<Collider>().enabled = false;
        //    }

        //    if ( trans.GetComponent<Rigidbody>() )
        //        trans.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        //}

        //////////////////////////////
        // SHOOT CANNONBALL
        //////////////////////////////

        GameObject newCannonball = Instantiate( cannonballPrefabs[ cannonballIndex ].prefab, newPos, cannon.transform.rotation );
        trashCollector.AddToTrash( newCannonball );
        //newCannonball.GetComponent<Cannonball>().Activate();

        StartCoroutine( NotBalanced() );

        //////////////////////////////
        // ADD EFFECTS TO PLAYER
        //////////////////////////////

        GetComponent<Rigidbody>().AddForce( -transform.forward * 500.0f, ForceMode.Impulse );
        if( cannon.transform.rotation.x < 45 )
            GetComponent<Rigidbody>().AddForce( transform.up * 200.0f, ForceMode.Impulse );
        if ( cannon.transform.rotation.x > 45 )
            GetComponent<Rigidbody>().AddForce( transform.up * 5000.0f, ForceMode.Impulse );

        psShootCloud.Play();

        audioSources[ audioIndex ].Play();
        audioIndex++;
        if ( audioIndex >= (int)AudioSources.AUDIO_SHOOT_THIRD )
            audioIndex = 0;
    }
    public void Heal( int healAmount )
    {
        if( health + healAmount > healthDefault )
            health = healthDefault;
        else
            health += healAmount;

        healthText.text = "Health: " + health.ToString();
        audioSources[ (int)AudioSources.AUDIO_OTHER ].clip = healedSound;
        audioSources[ (int)AudioSources.AUDIO_OTHER ].Play();
    }

    void ChangeCannonballIndex( int index )
    {
        cannonballIndex = index;
        cannonballImage.texture = cannonballPrefabs[ cannonballIndex ].texture;
        cannonballCounter.text = PlayerPrefs.GetInt( cannonballPrefabs[ cannonballIndex ].amountPrefKey ).ToString();
    }

    public void ChangeState(bool newState)
    {
        active = newState;
    }
    public void ChangeDashingState( int state )
    {
        if ( state == 1 )
        {
            dashDamageTimer = 0.0f;
            isDashing = false;
            ableToShoot = true;
            PlayAnimation( "Idle" );
        }
        else
        {
            dashDamageTimer = 0.0f;
            isDashing = false;
            ableToShoot = true;
        }
    }
    public void ChangeAbleToAttackState( int state )
    {
        if ( state == 1 )
            ableToShoot = true;
        else
            ableToShoot = false;
    }

    public void ChangeIsHoldingTreasure( int state )
    {
        if ( state == 1 )
        {
            treasureChest.SetActive( true );
            PlayAnimation( "Pickup_TreasureChest" );
            hasTreasureChest = true;
            isDashing = false;
            ableToShoot = false;
        }
        else
        {
            hasTreasureChest = false;
            treasureChest.SetActive( false );
        }
    }

    void FindPlayerParent( Transform parent )
    {
        if ( parent.parent == null )
            return;

        if ( parent.parent.GetComponent<PlayerController>() )
        {
            isChildOfPlayer = true;
        }
        else
            FindPlayerParent( parent.parent );

    }

    void PlayAnimation( string name )
    {
        if ( hasTreasureChest == true )
            animator.Play( name + "_With_TreasureChest" );
        else
            animator.Play( name );
    }       

    void KillParent( Transform parent )
    {
        foreach ( Transform trans in parent )
        {
            if ( trans.GetComponent<MeshRenderer>() )
            {
                if ( trans.GetComponent<Rigidbody>() == false )
                    trans.gameObject.AddComponent<Rigidbody>();
                trans.GetComponent<Rigidbody>().useGravity = true;
                trans.GetComponent<Rigidbody>().isKinematic = false;
                trans.GetComponent<Rigidbody>().AddForce( transform.forward * dashKnockback, ForceMode.Impulse );
            }

            if ( trans.childCount >= 1 )
                KillParent( trans );
        }
    }

    IEnumerator NotMoving()
    {
        yield return new WaitForSeconds(0.3f);
        moving = false;
        animator.SetBool( "walking", false );
        audioSources[ (int)AudioSources.AUDIO_WALKING ].Stop();
        isDashing = false;
    }

    IEnumerator DashingCoroutine( )
    {
        isDashing = true;
        yield return new WaitForSeconds( 0.15f );
        isDashing = false;
    }

    IEnumerator NotBalanced()
    {
        ableToBeBalanced = false;
        yield return new WaitForSeconds( 0.8f );
        ableToBeBalanced = true;
    }
}
