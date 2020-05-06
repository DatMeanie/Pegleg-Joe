using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //angle minimums
    float xMin = -100f;
    float xMax = 100f;

    //variables
    public float sens = 1.5f;
    public float aimingSens = 1.5f;
    float rotX;
    float rotY;

    GameObject playerCannon;
    PlayerController player;

    Vector3 offset;

    bool scriptEnabled = true;
    bool aimingEnabled = true;

    public GameObject mouseCursor;
    RectTransform cursorRectTransform;
    public GameObject mouseLimitPanel;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        playerCannon = player.cannon;

        //cursorRectTransform = mouseCursor.GetComponent<RectTransform>();

        //offset from player
        offset = transform.position - player.transform.position;
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (PlayerPrefs.HasKey("Sensitivity"))
            sens = PlayerPrefs.GetFloat("Sensitivity");

        if ( PlayerPrefs.HasKey( "AimingXSensitivity" ) )
            sens = PlayerPrefs.GetFloat( "AimingXSensitivity" );

        if ( PlayerPrefs.HasKey( "AimingYSensitivity" ) )
            aimingSens = PlayerPrefs.GetFloat( "AimingYSensitivity" );

        if ( PlayerPrefs.HasKey( "AimingEnabled" ) )
            if( PlayerPrefs.GetInt( "AimingEnabled" ) == 0)
                aimingEnabled = false;
    }

    void Update()
    {
        //RectTransform limitRectTransform = mouseLimitPanel.GetComponent<RectTransform>();
        //float width = limitRectTransform.sizeDelta.x;
        //float height = limitRectTransform.sizeDelta.y;

        //Debug.Log( " limit : " + limitRectTransform.anchoredPosition + " cursor : " + cursorRectTransform.anchoredPosition );

        //if ( cursorRectTransform.anchoredPosition.x < limitRectTransform.transform.position.x )
        //{
        //    cursorRectTransform.anchoredPosition = new Vector2(
        //        limitRectTransform.anchoredPosition.x - ( width / 2), 
        //        mouseCursor.transform.position.y );
        //}
        //if ( mouseCursor.transform.position.x > mouseLimitPanel.transform.position.x + ( width / 2 ) )
        //{
        //    mouseCursor.transform.position = new Vector3(
        //        mouseLimitPanel.transform.position.x + ( width / 2 ),
        //        mouseCursor.transform.position.y,
        //        mouseCursor.transform.position.z );
        //}
        //if ( mouseCursor.transform.position.y < mouseLimitPanel.transform.position.y + ( height / 2 ) )
        //{
        //    mouseCursor.transform.position = new Vector3(
        //        mouseCursor.transform.position.x,
        //        mouseLimitPanel.transform.position.y + ( height / 2 ),
        //        mouseCursor.transform.position.z );
        //}
        //if ( mouseCursor.transform.position.y > mouseLimitPanel.transform.position.y - ( height / 2 ) )
        //{
        //    mouseCursor.transform.position = new Vector3(
        //        mouseCursor.transform.position.x,
        //        mouseLimitPanel.transform.position.y - ( height / 2 ),
        //        mouseCursor.transform.position.z );
        //}

        if (scriptEnabled == false
            && player.health <= 0 )
            return;

        CheckScrolling();

        //rotations
        rotX += Input.GetAxis("Mouse Y") * aimingSens;
        rotY += Input.GetAxis("Mouse X") * sens;

        rotX = Mathf.Clamp(rotX, xMin, xMax); //limit player

        //cursorRectTransform.position = new Vector2( cursorRectTransform.position.x, cursorRectTransform.position.y - Input.GetAxisRaw( "Mouse Y" ) );

        Vector3 realPos = new Vector3(offset.x, offset.y - ( rotX / 200 ), offset.z);
        transform.position = Vector3.Lerp( transform.position, player.transform.TransformPoint( realPos ), 15.0f * Time.deltaTime );
        Quaternion newRot = Quaternion.Euler( 
            new Vector3( transform.eulerAngles.x, player.transform.eulerAngles.y, transform.eulerAngles.z ) );
        transform.rotation = Quaternion.Lerp( transform.rotation, newRot, 20.0f * Time.deltaTime );


        //change rotations
        //player.transform.eulerAngles = new Vector3( -rotX, rotY, player.transform.eulerAngles.z );
        if( player.isDashing == false )
        {
            if( aimingEnabled )
                playerCannon.transform.localRotation = Quaternion.Euler( rotX, playerCannon.transform.rotation.y, playerCannon.transform.rotation.z );
            player.transform.rotation = Quaternion.Euler( player.transform.rotation.x, rotY, player.transform.rotation.z );
        }
    }

    //change scriptenabled
    public void ChangeState(bool newState)
    {
        scriptEnabled = newState;
    }

    void CheckScrolling()
    {
        if ( Input.GetAxis( "Mouse ScrollWheel" ) > 0f ) // forward
        {
            if ( offset.z < -1.1f )
            {
                offset.y -= 0.5f;
                offset.z += 1.0f;
            }
        }
        else if ( Input.GetAxis( "Mouse ScrollWheel" ) < 0f ) // backwards
        {
            if ( offset.z > -20.1f )
            {
                offset.y += 0.5f;
                offset.z -= 1.0f;
            }
        }
    }
}