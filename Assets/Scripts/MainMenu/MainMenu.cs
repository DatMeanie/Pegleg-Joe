using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject levelSelector;
    public GameObject tutorialMenu;
    public GameObject highscoresMenu;
    public GameObject gameEndMenu;
    public GameObject questMenu;
    public Text coinsText;

    DataManager dataManager;
    CoinLoot backgroundLoot;

    private void Start()
    {
        backgroundLoot = GameObject.Find( "TreasureChests_BackgroundLoot" ).GetComponent<CoinLoot>();
        
        dataManager = DataManager.singleton;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if ( dataManager.playedGame == true )
        {
            gameEndMenu.SetActive( true );
            GetComponent<GameEndManager>().Initialize();
            GetComponent<GameEndManager>().UpdateText();
            mainMenu.SetActive( false );
        }
        else
        {
            gameEndMenu.SetActive( false );
        }

        dataManager.FixValues();
        coinsText.text = dataManager.GetSaveValue( ESave_Values_Keys.COINS_AMOUNT ).ToString() + " coins";
        Cannonball.ResetKillCounter();
        GetComponent<HighscoresManager>().Initialize();
        GetComponent<StatsManager>().Initialize();
    }

    private void Update()
    {
        if ( Input.GetKeyDown( KeyCode.P ) )
        {
            dataManager.ChangeSaveValue( ESave_Values_Keys.COINS_AMOUNT, dataManager.GetSaveValue( ESave_Values_Keys.COINS_AMOUNT ) + 500 );
            coinsText.text = dataManager.GetSaveValue( ESave_Values_Keys.COINS_AMOUNT ).ToString() + " coins";
            backgroundLoot.FixChests();
        }
    }

    public void EnterLevelSelector()
    {
        levelSelector.SetActive( true );
        mainMenu.SetActive( false );
    }
    public void ExitLevelSelector()
    {
        levelSelector.SetActive( false );
        mainMenu.SetActive( true );
    }
    public void EnterOptionsMenu()
    {
        optionsMenu.SetActive( true );
        mainMenu.SetActive( false );
    }
    public void ExitOptionsMenu()
    {
        optionsMenu.SetActive( false );
        mainMenu.SetActive( true );
    }

    public void EnterTutorialMenu()
    {
        tutorialMenu.SetActive( true );
        mainMenu.SetActive( false );
    }
    public void ExitTutorialMenu()
    {
        tutorialMenu.SetActive( false );
        mainMenu.SetActive( true );
    }
    public void EnterHighscoresMenu()
    {
        highscoresMenu.SetActive( true );
        mainMenu.SetActive( false );

        GetComponent<HighscoresManager>().UpdateText();
    }
    public void EnterQuestMenu()
    {
        GetComponent<QuestMainMenuManager>().Initialize();
        questMenu.SetActive( true );
        mainMenu.SetActive( false );
    }
    public void ExitHighscoresMenu()
    {
        highscoresMenu.SetActive( false );
        mainMenu.SetActive( true );
    }
    public void ExitGameEndMenu()
    {
        gameEndMenu.SetActive( false );
        mainMenu.SetActive( true );
    }
    public void ExitQuestMenu()
    {
        questMenu.SetActive( false );
        mainMenu.SetActive( true );
    }
    public void GoToTavern()
    {
        SceneManager.LoadScene( "Tavern" );
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
