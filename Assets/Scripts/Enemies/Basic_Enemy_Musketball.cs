using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic_Enemy_Musketball : MonoBehaviour
{
    //////////////////////////////
    // PUBLIC VARIABLES
    //////////////////////////////

    public int baseDamage = 50;
    public float knockback = 400.0f;

    public float speedMultiplier = 70.0f;
    public float slowdownMultiplier = 0.05f;
    public float gravityMultiplier = 1.0f;

    //////////////////////////////
    // COMPONENTS AND OBJECTS
    //////////////////////////////

    TrashCollector trashCollector;

    //////////////////////////////
    // VARIABLES
    //////////////////////////////

    Vector3 forward;
    float lifeTimeTimer = 10.0f;

    //////////////////////////////
    // BOOLS
    //////////////////////////////

    bool enemyParentExist = false;

    private void Start()
    {
        trashCollector = GameObject.Find( "Coding_Trash" ).GetComponent<TrashCollector>();
    }
    public void Activate()
    {
        forward = transform.forward;
        speedMultiplier = 70.0f;
        enemyParentExist = false;
        GetComponent<MeshRenderer>().enabled = true;
    }
    private void OnTriggerEnter( Collider other )
    {
        //////////////////////////////
        // HIT PLAYER
        //////////////////////////////

        if ( other.gameObject.name.Contains( "Player" ) && GetComponent<MeshRenderer>().enabled == true )
        {
            int damage = GetDamageOutput();
            other.gameObject.GetComponent<PlayerController>().Damage( damage );
            GetComponent<MeshRenderer>().enabled = false;
            transform.position = new Vector3( 500.0f, 500.0f, 500.0f );
        }

        //////////////////////////////
        // HIT PHYSIC OBJECT
        //////////////////////////////

        if ( other.gameObject.name.Contains( "&&" ) 
            && other.gameObject.tag != "Enemy" 
            && GetComponent<MeshRenderer>().enabled == true )
        {
            FindEnemyParent( other.transform );
            if ( enemyParentExist == false )
            {
                other.transform.parent = transform;

                if ( other.GetComponent<Rigidbody>() )
                    other.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * knockback, ForceMode.Impulse);
            }

            GetComponent<MeshRenderer>().enabled = false;
            transform.position = new Vector3( 500.0f, 500.0f, 500.0f );
        }
    }

    private void OnCollisionEnter( Collision collision )
    {
        if ( collision.gameObject.name.Contains( "&&" ) == false && collision.gameObject.tag != "Enemy" )
        {
            //forward = transform.forward;
            speedMultiplier -= 10.0f;
        }
    }

    private void Update()
    {
        if ( speedMultiplier > 0.0f )
        {
            speedMultiplier -= Time.deltaTime * slowdownMultiplier;
            //transform.rotation = Quaternion.Euler( new Vector3( transform.eulerAngles.x, transform.eulerAngles.y + Time.deltaTime * speedMultiplier, transform.eulerAngles.z ) );
            float gravity = 20.0f - speedMultiplier;
            if ( gravity < 0.0f )
                gravity = 0.0f;

            forward.y -= Time.deltaTime * gravityMultiplier * transform.forward.normalized.y - gravity;
            GetComponent<Rigidbody>().velocity = forward * speedMultiplier;
        }

        lifeTimeTimer -= Time.deltaTime;
        if ( lifeTimeTimer < 0.0f )
            Destroy( gameObject );
    }

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

    int GetDamageOutput()
    {
        return baseDamage * Mathf.RoundToInt( speedMultiplier / 70.0f );
    }
}
