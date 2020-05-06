using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class BuyUpgrade : MonoBehaviour
{
    public Upgrade upgrade;
    public bool BuyMultipleTimes = false;
    public bool isFloat = false;
    public float defaultNumber = 0;

    TavernManager tavernManager;
    DataManager dataManager;

    Text coinCost;
    Text upgradeName;

    private void Start()
    {
        if ( GameObject.Find( "DataManager" ) )
            dataManager = GameObject.Find( "DataManager" ).GetComponent<DataManager>();

        tavernManager = GameObject.Find( "TavernManager" ).GetComponent<TavernManager>();

        upgradeName = transform.Find( "upgrade_name" ).GetComponent<Text>();
        upgradeName.text = upgrade.fancyName;
        coinCost = transform.Find( "buy_button" ).GetChild( 0 ).GetComponent<Text>();
        coinCost.text = "Buy: " + upgrade.coinCost.ToString() + " Coins";
        if ( transform.Find( "buy20_button" ) )
            transform.Find( "buy20_button" ).GetChild( 0 ).GetComponent<Text>().text = "Buy 20: " + ( upgrade.coinCost * 20 ).ToString() + " Coins";

        if ( PlayerPrefs.HasKey( upgrade.name ) == true && BuyMultipleTimes == false )
        {
            if ( PlayerPrefs.GetInt( upgrade.name ) == 1 )
                coinCost.text = "Bought";
        }
        else if ( PlayerPrefs.HasKey( upgrade.name ) == true )
        {
            if( isFloat == false)
                upgradeName.text = upgrade.fancyName + " : " + PlayerPrefs.GetInt( upgrade.name );
            else
                upgradeName.text = upgrade.fancyName + " : " + PlayerPrefs.GetFloat( upgrade.name );
        }
        else
            upgradeName.text = upgrade.fancyName + " : " + defaultNumber;
    }
    public void PurchaseUpgrade( float amount )
    {
        if ( dataManager == null )
            return;

        if ( PlayerPrefs.HasKey( upgrade.name ) ==  true && BuyMultipleTimes == false )
            if ( PlayerPrefs.GetInt( upgrade.name ) == 1 )
                return;

        float coins = dataManager.GetSaveValue(ESave_Values_Keys.COINS_AMOUNT);
        float coinsNeeded = upgrade.coinCost * amount;
        if ( amount < 1.0f )
        {
            coinsNeeded = upgrade.coinCost;
        }
        if ( coins >= coinsNeeded )
        {
            coins -= coinsNeeded;
            dataManager.ChangeSaveValue( ESave_Values_Keys.COINS_AMOUNT, coins );

            if( BuyMultipleTimes == false )
            {
                PlayerPrefs.SetInt( upgrade.name, 1 * (int)amount );
                coinCost.text = "Bought";
            }
            else
            {
                if ( PlayerPrefs.HasKey( upgrade.name ) == false )
                {
                    if( isFloat == false )
                    {
                        PlayerPrefs.SetInt( upgrade.name, 1 * (int)amount );
                        upgradeName.text = upgrade.fancyName + " : " + ( 1 * amount ).ToString();
                    }
                    else
                    {
                        PlayerPrefs.SetFloat( upgrade.name, 1 * amount );
                        upgradeName.text = upgrade.fancyName + " : " + ( 1 * amount ).ToString();
                    }

                }
                else
                {
                    if ( isFloat == false )
                    {
                        PlayerPrefs.SetInt( upgrade.name, PlayerPrefs.GetInt( upgrade.name ) + ( 1 * (int)amount ) );
                        upgradeName.text = upgrade.fancyName + " : " + PlayerPrefs.GetInt( upgrade.name );
                    }
                    else
                    {
                        PlayerPrefs.SetFloat( upgrade.name, PlayerPrefs.GetFloat( upgrade.name ) + amount );
                        upgradeName.text = upgrade.fancyName + " : " + PlayerPrefs.GetFloat( upgrade.name ).ToString();
                    }
                }
            }


            
            GameObject.Find( "coins_text" ).GetComponent<Text>().text = coins.ToString() + " coins";
        }
    }
}
