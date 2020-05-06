using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public enum ELootRarities
{
    RARITY_COMMON,
    RARITY_RARE,
    RARITY_EPIC,
    RARITY_LEGENDARY
}

[System.Serializable]
public struct SLootbox_Values
{
    public string Box;
    public string Texture;
    public ELootRarities Rarity;
}

public class LootboxManager : MonoBehaviour
{

    /////////////////////////////////////////////
    // PUBLIC VARIABLES
    /////////////////////////////////////////////

    public Transform content;
    public Text coinAmountText;
    public GameObject mainScreen;
    public GameObject openerParent;
    public GameObject openerContent;
    public GameObject wonContinueButton;

    /////////////////////////////////////////////
    // PRIVATE VARIABLES
    /////////////////////////////////////////////

    float duration = 12000.0f;
    float timer;
    float constant = 2f;
    float variable = 6;
    float speed = 1.0f;
    int amount = 40;

    Vector3 startPos;
    Vector3 distance;
    Vector3 newPos;

    bool lootBoxActive;
    bool createdBoxes = false;

    Dictionary<string, SLootbox_Values> boxes = new Dictionary<string, SLootbox_Values>();

    public void UpdateLootBoxScreen()
    {
        /////////////////////////////////////////////
        // UPDATE UI AND SET VALUES
        /////////////////////////////////////////////

        coinAmountText.text = "Coins: " + DataManager.singleton.GetSaveValue(ESave_Values_Keys.COINS_AMOUNT).ToString();
       
        LoadBoxes();

        LoadLootboxOpener();
        createdBoxes = true;

        startPos = openerContent.transform.GetChild(0).GetComponent<RectTransform>().position;
        newPos = startPos;
        distance = openerContent.transform.GetChild(0).GetComponent<RectTransform>().position - openerContent.transform.GetChild(1).GetComponent<RectTransform>().position;
    }

    private void Update()
    {
        if ( lootBoxActive )
        {
            /////////////////////////////////////////////
            // LOOTBOX COUNTDOWN
            /////////////////////////////////////////////
           
            variable -= Time.deltaTime;
            timer -= ( Mathf.Pow( constant, variable ) );
            MoveLootboxes();
          
            /////////////////////////////////////////////
            // LOOTBOX FINISHED, DO RESET
            /////////////////////////////////////////////

            if ( timer < 0.0f )
            {
                newPos = startPos + distance;

                int children = openerContent.transform.childCount;
                for ( int i = 0; i > children; i -= -1 )
                {
                   newPos -= distance;
                   openerContent.transform.GetChild( i ).GetComponent<RectTransform>().position = newPos;
                }

                newPos = startPos;

                wonContinueButton.SetActive( true );
                lootBoxActive = false;
            }
        }
    }

    public void OpenLootbox()
    {
        LoadLootboxOpener();
        if ( lootBoxActive == false )
        {
            lootBoxActive = true;
        }
    }

    public void LoadLootboxOpener()
    {
        /////////////////////////////////////////////
        // GET VARIABLES AND BOX UI
        /////////////////////////////////////////////

        GameObject item;
        if ( createdBoxes == false )
        {
            item = Instantiate( openerContent.transform.GetChild(0).gameObject );
            item.transform.SetParent( openerContent.transform, false );
            item.GetComponent<RectTransform>().position = startPos;
        }
        else
        {
            item = openerContent.transform.GetChild( 0 ).gameObject;
        }

        /////////////////////////////////////////////
        // CREATE NEW UI BOXES
        /////////////////////////////////////////////

        for ( int i = 0; i < amount; i++ )
        {
            newPos -= distance;
            GameObject newBox;
            if ( createdBoxes == false )
            {
                newBox = Instantiate( item );
                newBox.transform.SetParent( openerContent.transform, false );
            }
            else
            {
                newBox = openerContent.transform.GetChild( i ).gameObject;
            }

            newBox.GetComponent<RectTransform>().position = newPos;
            //change image
            //change color
        }
    }

    void MoveLootboxes()
    {
        /////////////////////////////////////////////
        // MOVE BOXES, SPEED DEPENDING ON TIMER
        /////////////////////////////////////////////

        int children = openerContent.transform.childCount;
        for ( int i = 0; i < children; i++ )
        {
            Transform child = openerContent.transform.GetChild( i );
            if ( timer > 0.0f )
                child.position = new Vector3
                    ( child.GetComponent<RectTransform>().position.x - ( ( timer / 10 ) / amount ) * speed
                    , child.GetComponent<RectTransform>().position.y
                    , child.GetComponent<RectTransform>().position.z
                    );
            else
                break;
        }
    }
    public void OpenOpening()
    {
        mainScreen.SetActive( false );
        openerParent.SetActive( true );
        timer = duration;
    }
    public void FinishOpening()
    {
        wonContinueButton.SetActive( false ); 
        openerParent.SetActive( false );
        mainScreen.SetActive( true );
    }

    void LoadBoxes()
    {
        // load stuff from file
        BinaryFormatter bf = new BinaryFormatter();
        if ( File.Exists( Application.dataPath + "/lootboxes.dat" ) )
        {
            string json = File.ReadAllText(Application.dataPath + "/lootboxes.dat");
            boxes = JsonUtility.FromJson<Dictionary<string, SLootbox_Values>>( json );
        }
        else
        {
            SaveBoxes();
        }
    }
    public void SaveBoxes()
    {
        //save to file
        //binaryformatter to save lists and etc

        SLootbox_Values values;
        values.Box = "Meme-Box";
        values.Texture = "BigChungus.png";
        values.Rarity = ELootRarities.RARITY_COMMON;

        boxes.Add( "Test", values );

        string json = JsonUtility.ToJson(values);
        Debug.Log( json );
        File.WriteAllText( Application.dataPath + "/lootboxes.dat", json );

        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Create(Application.persistentDataPath + "/lootboxes.dat");
        //bf.Serialize( file, values );
        //file.Close();
    }
}
