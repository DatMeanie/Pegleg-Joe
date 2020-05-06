using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureChestDropzone : MonoBehaviour
{
    [HideInInspector]
    public static int coins = 0;
    public static int chestsStolen = 0;
    GameObject child;
    bool isActive = false;

    public Text coinsText;

    public int coinMax = 100;
    float beerMultiplier = 1.0f;

    DataManager dataManager;

    public void Initialize()
    {
        child = transform.GetChild( 0 ).gameObject;
        child.SetActive( false );
        coinsText = GameObject.Find( "coins_text" ).GetComponent<Text>();
        coinsText.text = coins.ToString() + " Coins";

        beerMultiplier = PlayerPrefs.GetFloat( "Beer_Multiplier" );

        if ( GameObject.Find( "DataManager" ) )
        {
            dataManager = GameObject.Find( "DataManager" ).GetComponent<DataManager>();
            coinMax = Mathf.RoundToInt( coinMax * dataManager.GetGameValue( EGame_Values_Keys.COIN_MODIFIER ) );
        }
    }
    private void OnTriggerEnter( Collider other )
    {
        if ( other.name.Contains( "Player" ) == true && isActive == true )
        {
            //PlayerController player = other.GetComponent<PlayerController>(); 
            child.SetActive( false );
            isActive = false;
            other.GetComponent<PlayerController>().ChangeIsHoldingTreasure( 0 );
            coins += Mathf.RoundToInt( Random.Range( 40, coinMax ) * beerMultiplier );
            chestsStolen++;
            coinsText.text = coins.ToString() + " Coins";
        }
    }

    public void ActivateZone()
    {
        isActive = true;
        child.SetActive( true );
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        coinsText.text = coins.ToString() + " Coins";
    }

    public void SaveCoins()
    {
        if( GameObject.Find("DataManager") )
        {
            dataManager =  GameObject.Find( "DataManager" ).GetComponent<DataManager>();
            dataManager.ChangeSaveValue( ESave_Values_Keys.COINS_AMOUNT, dataManager.GetSaveValue( ESave_Values_Keys.COINS_AMOUNT ) + coins );
        }
    }
}
