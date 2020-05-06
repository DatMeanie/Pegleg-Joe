using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectorMenu : MonoBehaviour
{
    //////////////////////////////
    // VARIABLES
    //////////////////////////////

    //private
    int levelSelected = 0;
    int difficultyIndex = 1;

    float distance = 0.0f;
    Vector3 levelPosition;

    //////////////////////////////
    // OBJECTS AND COMPONENTS
    //////////////////////////////
    
    //public
    public Transform levelsContent;
    public Text difficultyBtnText;

    public Text titleText;
    public Text descriptionText;

    //private
    DataManager dataManager;
    CoinLoot backgroundLoot;

    GameObject placeHolderLevel;

    List<Image> imageBackgrounds = new List<Image>();
    List<Transform> locks = new List<Transform>();
    List<Level> levels = new List<Level>();
    List<Button> selectButtons = new List<Button>();

    private void Start()
    {
        /////////////////////////////////////////
        // GET STUFF
        /////////////////////////////////////////

        dataManager = GameObject.Find( "DataManager" ).GetComponent<DataManager>();
        backgroundLoot = GameObject.Find( "TreasureChests_BackgroundLoot" ).GetComponent<CoinLoot>();
        difficultyIndex = (int)dataManager.GetGameValue( EGame_Values_Keys.DIFFICULTY );
        levelSelected = (int)dataManager.GetGameValue( EGame_Values_Keys.LEVEL_SELECTED );
        levels = GetComponent<LevelsList>().levels;

        /////////////////////////////////////////
        // SET TEXT INFO
        /////////////////////////////////////////

        if ( difficultyIndex == 0 )
            difficultyBtnText.text = "Easy";
        if ( difficultyIndex == 1 )
            difficultyBtnText.text = "Normal";
        if ( difficultyIndex == 2 )
            difficultyBtnText.text = "Hard";

        titleText.text = levels[ levelSelected ].name;
        descriptionText.text = levels[ levelSelected ].description;

        /////////////////////////////////////////
        // UPDATE LEVEL SELECTOR SCREEN
        /////////////////////////////////////////

        foreach ( Transform child in levelsContent )
        {
            if ( child.name.Contains( "Level" ) )
            {
                placeHolderLevel = child.gameObject;
                levelPosition = child.gameObject.GetComponent<RectTransform>().position;
                distance = ( child.gameObject.GetComponent<Transform>().position - levelsContent.Find( "Level2" ).position ).y;
                break;
            }
        }

        Destroy( placeHolderLevel );
        Destroy( levelsContent.Find( "Level2" ).gameObject );

        int count = 0;
        foreach(Level level in levels )
        {
            int correctLevel = count;
            GameObject newLevel;
            newLevel = Instantiate( placeHolderLevel, levelsContent );
            RectTransform newLevelTrans = newLevel.GetComponent<RectTransform>();

            newLevelTrans.position = levelPosition;
            newLevelTrans.name = "Level" + correctLevel;
            newLevelTrans.Find( "select_image" ).GetComponent<RawImage>().texture = levels[ count ].selectImage;
            newLevelTrans.Find( "Lock_Overlay" ).Find( "lock_text" ).GetComponent<Text>().text = levels[ count ].coinCost + " coins";
            //newLevelTrans.Find( "Lock_Overlay" ).Find( "lock_button" ).GetComponent<Button>().onClick.AddListener( delegate { BuyLevel( -content.childCount - count ); } );
            newLevelTrans.Find( "Lock_Overlay" ).Find( "lock_button" ).GetComponent<Button>().onClick.AddListener( () => BuyLevel( correctLevel ) );

            foreach ( Transform child in newLevelTrans )
            {
                if ( child.GetComponent<Text>() )
                    child.GetComponent<Text>().enabled = true;

                if ( child.GetComponent<RawImage>() )
                    child.GetComponent<RawImage>().enabled = true;

                if ( child.GetComponent<Image>() )
                    child.GetComponent<Image>().enabled = true;

                if ( child.GetComponent<Button>() )
                    child.GetComponent<Button>().enabled = true;
            }

            imageBackgrounds.Add( newLevelTrans.Find( "Image_Background" ).GetComponent<Image>() );
            locks.Add( newLevelTrans.Find( "Lock_Overlay" ) );
            imageBackgrounds[ count ].color = new Color( 255, 255, 255, 70 );
            selectButtons.Add( newLevelTrans.Find( "select_button" ).GetComponent<Button>() );
            //selectButtons[count].onClick.AddListener( delegate { ChangeLevel( count - 3); } );
            selectButtons[ count ].onClick.AddListener( () => ChangeLevel( correctLevel ) );

            levelPosition = new Vector3( levelPosition.x, levelPosition.y - distance, levelPosition.z );
            count++;
        }

        count = 0;
        foreach ( Transform levelTrans in levelsContent )
        {
            if( levelTrans.name.Contains("Level") )
            {
                if ( PlayerPrefs.HasKey( "Level" + count + "_IsBought" ) == false )
                    PlayerPrefs.SetInt( "Level" + count + "_IsBought", 0 );
                else
                {
                    if ( PlayerPrefs.GetInt( "Level" + count + "_IsBought" ) == 1 )
                    {
                        locks[ count ].gameObject.SetActive( false );
                        selectButtons[ count ].interactable = true;
                    }
                }

                count++;
            }
        }

        imageBackgrounds[ levelSelected ].color = Color.red;

        /////////////////////////////////////////

    }

    public void ChangeDifficulty()
    {
        difficultyIndex++;
        if ( difficultyIndex >= 3 )
            difficultyIndex = 0;

        if ( difficultyIndex == 0 )
            difficultyBtnText.text = "Easy";
        if ( difficultyIndex == 1 )
            difficultyBtnText.text = "Normal";
        if ( difficultyIndex == 2 )
            difficultyBtnText.text = "Hard";

        dataManager.ChangeGameValue( EGame_Values_Keys.DIFFICULTY, difficultyIndex );
    }

    public void ChangeLevel( int _level )
    {
        if ( PlayerPrefs.GetInt( "Level" + _level + "_IsBought" ) == 0 )
            return;

        imageBackgrounds[ levelSelected ].color = new Color( 255, 255, 255, 70 );
        levelSelected = _level;
        imageBackgrounds[ levelSelected ].color = Color.red;
        titleText.text = levels[ levelSelected ].name;
        descriptionText.text = levels[ levelSelected ].description;
    }

    public void BuyLevel( int _level )
    {
        float coins = dataManager.GetSaveValue(ESave_Values_Keys.COINS_AMOUNT);
        if ( coins >= levels[ _level ].coinCost )
        {
            coins -= levels[ _level ].coinCost;
            dataManager.ChangeSaveValue(ESave_Values_Keys.COINS_AMOUNT, coins );
            backgroundLoot.FixChests();

            selectButtons[ _level ].interactable = true;
            locks[ _level ].gameObject.SetActive( false );

            ChangeLevel( _level );

            PlayerPrefs.SetInt( "Level" + _level + "_IsBought", 1 );
            GameObject.Find( "coins_counter_text" ).GetComponent<Text>().text = coins.ToString() + " coins";
        }
    }

    public void ResetProgression()
    {
        int count = 2;
        foreach ( Transform levelTrans in levelsContent )
        {
            if ( levelTrans.name.Contains( "Level" ) && levelTrans.name.Contains("Level1") == false )
            {
                PlayerPrefs.SetInt( "Level" + count + "_IsBought", 0 );
                locks[ count - 1 ].gameObject.SetActive( true );
                selectButtons[ count - 1 ].interactable = false;

                count++;
            }
        }
    }

    public void PlayGame()
    {
        dataManager.ChangeGameValue( EGame_Values_Keys.LEVEL_SELECTED, levelSelected );
        SceneManager.LoadScene( "Level" + ( levelSelected + 1 ) );
    }
}
