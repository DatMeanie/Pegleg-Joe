using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    TreasureChestDropzone treasureChestDropzone;
    TreasureChestSpawner treasureChestSpawner;
    Animator goToBoatPanelAnimator;
    bool isActive = true;
    private void Start()
    {
        treasureChestDropzone = GameObject.Find( "TreasureChest_Dropzone" ).GetComponent<TreasureChestDropzone>();
        treasureChestSpawner = GameObject.Find( "GameManager" ).GetComponent<TreasureChestSpawner>();
        goToBoatPanelAnimator = GameObject.Find( "GoToBoat_Panel" ).GetComponent<Animator>();

    }
    public void Initialize( Vector3 pos )
    {
        transform.position = pos;
        gameObject.SetActive( true );
        isActive = true;
    }
    private void OnTriggerEnter( Collider other )
    {
        if( other.name.Contains( "Player") == true && isActive == true )
        {
            if ( other.GetComponent<PlayerController>().hasTreasureChest == false )
            {
                gameObject.SetActive( false );
                isActive = false;
                other.GetComponent<PlayerController>().ChangeIsHoldingTreasure( 1 );
                treasureChestSpawner.MakeSpawnerAvailable( transform.position );
                transform.position = new Vector3( 500, 500, 500 );
                treasureChestDropzone.ActivateZone();
                goToBoatPanelAnimator.Play( "GoToBoat_Flash" );
            }
        }
    }
}
