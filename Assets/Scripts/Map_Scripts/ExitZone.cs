using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitZone : MonoBehaviour
{
    public GameObject exitText;
    GameManager gameManager;

    bool isActive = false;

    //TreasureChestDropzone treasureChestDropzone;
    private void Start()
    {
        gameManager = GameObject.Find( "GameManager" ).GetComponent<GameManager>();
        //treasureChestDropzone = GameObject.Find( "TreasureChest_Dropzone" ).GetComponent<TreasureChestDropzone>();
    }
    private void OnTriggerEnter( Collider other )
    {
        if ( other.name.Contains( "Player" ) == true )
        {
            exitText.SetActive( true );
            isActive = true;
        }
    }

    private void OnTriggerExit( Collider other )
    {
        if ( other.name.Contains( "Player" ) == true  )
        {
            exitText.SetActive( false );
            isActive = false;
        }
    }
    private void Update()
    {
        if ( isActive == true )
        {
            if( Input.GetKeyDown( KeyCode.E ) )
            {
                if ( GameObject.Find( "DataManager" ) )
                    DataManager.singleton.ChangeSaveValue( ESave_Values_Keys.COINS_AMOUNT, DataManager.singleton.GetSaveValue( ESave_Values_Keys.COINS_AMOUNT ) + TreasureChestDropzone.coins );

                gameManager.EndGame();
            }
        }
    }
}
