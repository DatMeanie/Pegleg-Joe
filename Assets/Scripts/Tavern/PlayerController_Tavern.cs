using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController_Tavern : MonoBehaviour
{

    //playercontroller from Room shooter
    //able to walk, jump, crouch
    //a bit updated

    //////////////////////////////
    // VARIABLES
    //////////////////////////////

    //Public variables
    [Header("Variables")]
    public float movementSpeed = 10.0f;

    public float fallSpeed = 1.0f;

    //////////////////////////////////////////
    // PUBLIC COMPONENTS AND OBJECTS
    //////////////////////////////////////////

    [Header("Game Objects")]
    CameraController_Tavern playerPerspectiveCamera;

    //////////////////////////////
    // BOOLS
    //////////////////////////////

    bool active = true;
    bool flying = true;
    bool ableToBeBalanced = true;
    [HideInInspector]
    public bool moving = false;

    //////////////////////////////
    // COMPONENTS AND OBJECTS
    //////////////////////////////

    TrashCollector trashCollector;
    GameManager gameManager;
    DialogueManager dialogueManager;

    Animator animator;

    //////////////////////////////
    // AUDIO
    //////////////////////////////

    AudioSource[] audioSources;

    public void Start()
    {
        Physics.IgnoreLayerCollision( 9, 12 );

        audioSources = GetComponents<AudioSource>();
        animator = GetComponent<Animator>();

        playerPerspectiveCamera = Camera.main.GetComponent<CameraController_Tavern>();
        dialogueManager = GameObject.Find( "Dialogue_UI" ).GetComponent<DialogueManager>();
    }

    void Update()
    {
        if ( active )
        {
            Input_ThirdPerson();
        }

        if ( Input.GetKeyDown( KeyCode.Escape ) && dialogueManager.inDialogue == true )
        {
            SceneManager.LoadScene( 0 );
        }
    }

    private void OnCollisionEnter( Collision collision )
    {

    }

    void Input_ThirdPerson()
    {
        //////////////////////////////
        // DIRECTIONAL MOVEMENT
        //////////////////////////////

        if ( moving && audioSources[ (int)AudioSources.AUDIO_WALKING ].isPlaying == false )
        {
            audioSources[ (int)AudioSources.AUDIO_WALKING ].Play();
        }

        if ( Input.GetKey( KeyCode.W ) )
        {
            transform.position += transform.forward * movementSpeed * Time.deltaTime;
            moving = true;
            animator.SetBool( "walking", true );
        }
        else if ( Input.GetKey( KeyCode.S ) )
        {
            transform.position -= transform.forward * movementSpeed * Time.deltaTime;
            moving = true;
            animator.SetBool( "walking", true );
        }

        if ( Input.GetKey( KeyCode.D ) )
        {
            transform.position += transform.right * movementSpeed * Time.deltaTime;
            moving = true;
            animator.SetBool( "walking", true );
        }
        else if ( Input.GetKey( KeyCode.A ) )
        {
            transform.position -= transform.right * movementSpeed * Time.deltaTime;
            moving = true;
            animator.SetBool( "walking", true );
        }

        //////////////////////////////
        // IS IDLE
        //////////////////////////////

        if ( Input.GetKeyUp( KeyCode.W ) || Input.GetKeyUp( KeyCode.S ) || Input.GetKeyUp( KeyCode.D ) || Input.GetKeyUp( KeyCode.A ) )
        {
            StartCoroutine( NotMoving() );
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
            && transform.position.y > -4.0f )
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

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

    public void ChangeState( bool newState )
    {
        active = newState;
    }

    void PlayAnimation( string name )
    {
        animator.Play( name );
    }

    IEnumerator NotMoving()
    {
        yield return new WaitForSeconds( 0.3f );
        moving = false;
        animator.SetBool( "walking", false );
        audioSources[ (int)AudioSources.AUDIO_WALKING ].Stop();
    }
}