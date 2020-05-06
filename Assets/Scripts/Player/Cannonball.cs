using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Cannonball : MonoBehaviour
{
    //////////////////////////////
    // PUBLIC VARIABLES
    //////////////////////////////

    public float baseDamage = 50.0f;
    public float health = 50.0f;
    public float hardnessMultiplier = 1.0f;
    public float knockback = 400.0f;
    public float speedLimit = 15.0f;

    public float speedMultiplier = 70.0f;
    public float slowdownMultiplier = 0.05f;
    public float hitSlowdownMultiplier = 1.0f;
    public float gravityMultiplier = 1.0f;

    public bool ableToBeDestroyed = false;

    public static int enemyKilledCount = 0;

    //////////////////////////////
    // PRIVATE VARIABLES
    //////////////////////////////

    float defaultMass;
    float defaultSpeedMultiplier;

    int childMultiplier = 1;
    int killedMultiplier = 1;
    int chanceToGetCoin = 70;

    int destructableParentIndex = -1;
    float beerMultiplier = 1.0f;

    Vector3 forward;

    //////////////////////////////
    // COMPONENTS AND OBJECTS
    //////////////////////////////

    GameObject cannonballDestroyed;

    TrashCollector trashCollector;
    GameManager gameManager;
    DataManager dataManager;

    Text killcounterText;

    Rigidbody rb;

    List<DestructableObject> destructableParents = new List<DestructableObject>();
    TreasureChestDropzone treasureChestDropzone;

    //////////////////////////////
    // BOOLS
    //////////////////////////////

    bool enemyParentExist = false;
    bool destructableParentExist = false;
    bool ableToHit = true;

    ////////////////////////////////////////////////////////////

    private void Start()
    {
        beerMultiplier = PlayerPrefs.GetFloat( "Beer_Multiplier" );
        trashCollector = GameObject.Find( "Coding_Trash" ).GetComponent<TrashCollector>();
        gameManager = GameObject.Find( "GameManager" ).GetComponent<GameManager>();
        killcounterText = GameObject.Find( "killcounter_text" ).GetComponent<Text>();
        rb = GetComponent<Rigidbody>();

        defaultMass = rb.mass;
        defaultSpeedMultiplier = speedMultiplier;

        if ( GameObject.Find( "DataManager" ) )
        {
            dataManager = GameObject.Find( "DataManager" ).GetComponent<DataManager>();
            chanceToGetCoin = Mathf.RoundToInt( chanceToGetCoin * dataManager.GetGameValue( EGame_Values_Keys.COIN_MODIFIER ) );
        }

        treasureChestDropzone = GameObject.Find( "TreasureChest_Dropzone" ).GetComponent<TreasureChestDropzone>();

        if( ableToBeDestroyed == true )
        {
            cannonballDestroyed = transform.Find( "Cannonball_Destroyed" ).gameObject;
            cannonballDestroyed.SetActive( false );
        }

        Activate();
    }

    ////////////////////////////////////////////////////////////
    
    public void Activate()
    {
        speedMultiplier = defaultSpeedMultiplier;
        childMultiplier = 1;
        killedMultiplier = 1;
        destructableParentIndex = -1;

        rb.mass = defaultMass;
        forward = transform.forward;

        enemyParentExist = false;
        ableToHit = true;

        destructableParents.Clear();
    }

    ////////////////////////////////////////////////////////////

    private void OnTriggerEnter( Collider other )
    {
        if ( speedMultiplier > speedLimit - 15.0f )
        {
            GameObject otherObject = other.gameObject;
            
            //////////////////////////////
            // HIT ENEMY
            //////////////////////////////

            if ( otherObject.tag == "Enemy" )
            {
                int damage = GetDamageOutput();
                Enemy enemy = otherObject.GetComponent<Enemy>();

                if ( enemy.health - damage <= 0
                    && enemy.ableToBeDamaged == true )
                {
                    enemy.Damage( damage, knockback );
                    otherObject.tag = "Untagged";
                    Destroy( otherObject.GetComponent<Enemy>() );
                    Destroy( otherObject.GetComponent<Animator>() );
                    Destroy( otherObject.GetComponent<BoxCollider>() );
                    Destroy( otherObject.GetComponent<NavMeshAgent>() );
                    KillParent( other.transform, 2.0f );
                    trashCollector.AddToTrash( otherObject );

                    enemyKilledCount++;
                    killedMultiplier++;
                    killcounterText.text = "Kill counter: " + enemyKilledCount.ToString();

                    if ( dataManager != null )
                    {
                        dataManager.ChangeSaveValue( ESave_Values_Keys.HIGHEST_KILLSTREAK, killedMultiplier - 1 );
                        dataManager.ChangeSaveValue( ESave_Values_Keys.KILLS_AMOUNT, 1 );
                        gameManager.UpdateQuests( enemy.enemyType );
                    }

                    if ( Random.Range( 0, 100 ) > chanceToGetCoin - ( ( killedMultiplier - 1 ) * 10 ) )
                        treasureChestDropzone.AddCoins( Mathf.RoundToInt( Random.Range( 0, killedMultiplier * 4 * beerMultiplier ) ) );
                }
                else
                    enemy.Damage( damage, knockback );

                speedMultiplier -= 3.0f * hitSlowdownMultiplier;
                health -= 3.0f * hardnessMultiplier;
            }

            //////////////////////////////
            // HIT PHYSIC OBJECT
            //////////////////////////////

            if ( otherObject.name.Contains( "&&" ) && otherObject.gameObject.tag != "Enemy" )
            {
                FindEnemyParent( otherObject.transform );

                if ( enemyParentExist == false )
                {
                    FindDestructableParent( otherObject.transform );

                    float chance = -10.0f;
                    float otherMass = 3.0f;
                    if ( otherObject.GetComponent<Rigidbody>() )
                    {
                        Rigidbody otherRB;
                        otherRB = otherObject.GetComponent<Rigidbody>();
                        otherMass = otherRB.mass;

                        chance = speedMultiplier - ( otherMass / 10 );
                    }

                    if ( destructableParentExist == true
                        && ableToHit == true
                        && destructableParentIndex < destructableParents.Count )
                    {
                        destructableParentExist = false;
                        destructableParents[ destructableParentIndex ].Explode();
                        if ( destructableParents[ destructableParentIndex ].hasExploded == true )
                            AddChild( otherObject );
                    }
                    else if ( destructableParentExist == true
                        && ableToHit == false
                        && Random.Range( 0, 70 ) < chance
                        && otherObject.name.Contains( "##" ) == false )
                    {
                        destructableParentExist = false;
                        AddChild( otherObject );
                    }
                    else if ( ableToHit == true )
                        AddChild( otherObject );
                    else
                        speedMultiplier = 0.0f;

                    speedMultiplier -= ( otherMass / 10 ) * hitSlowdownMultiplier;
                    health -= ( otherMass / 10 ) * hardnessMultiplier;
                }

                ableToHit = true;
            }
        }
    }

    ////////////////////////////////////////////////////////////

    private void OnCollisionEnter( Collision collision )
    {
        if( collision.gameObject.name.Contains( "&&" ) == false && collision.gameObject.tag != "Enemy" )
        {
            FindEnemyParent( collision.transform );
            //forward = transform.forward;
            if ( enemyParentExist == false )
            {
                speedMultiplier -= 30.0f * hitSlowdownMultiplier;
                health -= 30.0f * hardnessMultiplier;
            }
        }
    }

    ////////////////////////////////////////////////////////////

    private void OnCollisionStay( Collision collision )
    {
        if ( collision.gameObject.name.Contains( "&&" ) == false && collision.gameObject.tag != "Enemy" )
        {
            FindEnemyParent( collision.transform );
            //forward = transform.forward;
            if ( enemyParentExist == false )
            {
                speedMultiplier -= 5.0f * hitSlowdownMultiplier;
                health -= 30.0f * hardnessMultiplier;
            }
        }
    }

    ////////////////////////////////////////////////////////////

    private void Update()
    {
        if ( speedMultiplier > 0.0f )
        { 
            speedMultiplier -= Time.deltaTime * slowdownMultiplier * childMultiplier;
        }
    }

    ////////////////////////////////////////////////////////////

    private void FixedUpdate()
    {
        if( speedMultiplier > 0.0f )
        {
            //transform.rotation = Quaternion.Euler( new Vector3( transform.eulerAngles.x, transform.eulerAngles.y + Time.deltaTime * speedMultiplier, transform.eulerAngles.z ) );
            float gravity = 40.0f / speedMultiplier;
            if ( gravity < 0.0f )
                gravity = 0.0f;

            forward.y -= Time.deltaTime * gravityMultiplier * gravity;
            rb.velocity = forward * speedMultiplier;
            if ( rb.velocity.y > 150.0f )
                rb.velocity = new Vector3(forward.x, 0.0f, forward.z);
        }

        CheckMultiplier();

    }

    ////////////////////////////////////////////////////////////

    //Add rigidbody to children
    void KillParent( Transform parent, float force )
    {
        foreach ( Transform trans in parent )
        {
            if ( trans.GetComponent<MeshRenderer>() )
            {
                if( trans.GetComponent<Rigidbody>() == false )
                    trans.gameObject.AddComponent<Rigidbody>();

                if ( trans.GetComponent<Collider>() )
                {
                    int colliderCount = trans.GetComponents<Collider>().Length;
                    for ( int colliderIndex = 0; colliderIndex < colliderCount; colliderIndex++ )
                        trans.GetComponent<Collider>().enabled = true;
                }

                Rigidbody transRB = trans.GetComponent<Rigidbody>();

                transRB.useGravity = true;
                transRB.isKinematic = false;
                trans.GetComponent<Rigidbody>().AddForce( transform.forward * force, ForceMode.Impulse );
            }

            if ( trans.childCount >= 1 )
                KillParent( trans, force );
        }
    }

    ////////////////////////////////////////////////////////////

    //Find if object has enemy as parent
    void FindEnemyParent( Transform parent )
    {
        if ( parent.parent == null )
            return;

        if ( parent.parent.GetComponent<Enemy>() )
        {
            int y = parent.parent.GetComponent<Enemy>().health;
            enemyParentExist = true;
            if ( parent.parent.GetComponent<Enemy>().health <= 0 )
                enemyParentExist = false;
        }
        else
            FindEnemyParent( parent.parent );
    }

    ////////////////////////////////////////////////////////////

    //Find if object has destructable object as parent
    void FindDestructableParent( Transform parent )
    {
        if ( parent.parent == null )
            return;

        if ( parent.parent.GetComponent<DestructableObject>() )
        {
            destructableParentExist = true;
            if ( destructableParents.Contains( parent.parent.GetComponent<DestructableObject>() ) == false )
            {
                destructableParentIndex++;
                destructableParents.Add( parent.parent.GetComponent<DestructableObject>() );
            }
            else 
                ableToHit = false;
        }
        else
            FindDestructableParent( parent.parent );
    }

    ////////////////////////////////////////////////////////////

    void AddChild( GameObject child )
    {
        child.transform.parent = transform;
        childMultiplier++;

        if ( child.GetComponent<Rigidbody>() ) 
        {
            Rigidbody childRB = child.GetComponent<Rigidbody>();
            childRB.isKinematic = true;
            rb.mass += childRB.mass;
        }
        if ( child.GetComponent<Collider>() )
        {
            int colliderCount = child.GetComponents<Collider>().Length;
            for ( int colliderIndex = 0; colliderIndex < colliderCount; colliderIndex++ )
                child.GetComponent<Collider>().enabled = false;
        }
    }

    ////////////////////////////////////////////////////////////

    int GetDamageOutput()
    {
        float power = speedMultiplier * rb.mass;
        power /= defaultMass * 50;
        float speedMod = speedMultiplier / defaultSpeedMultiplier;

        int childPower = Mathf.RoundToInt( childMultiplier / 4 );

        int damage = Mathf.RoundToInt( baseDamage  * killedMultiplier * power ) + childPower;
        //Debug.Log( "DAMAGE: " + damage + "CHILD: " + childMultiplier + " KILLED: " + killedMultiplier + " POWER: " + power + " REAL POWER: " + ( speedMultiplier * rb.mass ).ToString() );
        return damage;
    }

    ////////////////////////////////////////////////////////////

    void CheckMultiplier()
    {
        if ( health < 0 && ableToBeDestroyed == true )
        {
            List<Transform> children = new List<Transform>();
            foreach ( Transform trans in transform )
                if ( trans.name.Contains( "Ammo_Stage" ) == false )
                    children.Add( trans );
            foreach ( Transform trans in children )
            {
                if ( trans.GetComponent<Collider>() )
                {
                    int colliderCount = trans.GetComponents<Collider>().Length;
                    for ( int colliderIndex = 0; colliderIndex < colliderCount; colliderIndex++ )
                        trans.GetComponent<Collider>().enabled = true;

                }

                if ( trans.GetComponent<Rigidbody>() )
                    rb.mass -= trans.GetComponent<Rigidbody>().mass;

                trashCollector.AddToTrash( trans.gameObject );
            }

            gameObject.SetActive( false );
            cannonballDestroyed.SetActive( true );
            KillParent( cannonballDestroyed.transform, 20.0f );
            //killedChildren = true;
        }
        else if ( speedMultiplier < speedLimit )
        {
            List<Transform> children = new List<Transform>();
            foreach ( Transform trans in transform )
                if ( trans.name.Contains( "Ammo_Stage" ) == false )
                    children.Add( trans );
            foreach ( Transform trans in children )
            {
                if ( trans.GetComponent<Collider>() )
                {
                    int colliderCount = trans.GetComponents<Collider>().Length;
                    for ( int colliderIndex = 0; colliderIndex < colliderCount; colliderIndex++ )
                        trans.GetComponent<Collider>().enabled = true;

                }

                if ( trans.GetComponent<Rigidbody>() )
                    rb.mass -= trans.GetComponent<Rigidbody>().mass;

                trashCollector.AddToTrash( trans.gameObject );
            }
        }
    }

    ////////////////////////////////////////////////////////////

    public static void ResetKillCounter()
    {
        enemyKilledCount = 0;
    }

    ////////////////////////////////////////////////////////////

    public static int GetKillCount()
    {
        return enemyKilledCount;
    }
}
