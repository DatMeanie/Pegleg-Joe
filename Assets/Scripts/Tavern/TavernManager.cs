using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TavernManager : MonoBehaviour
{
    public GameObject upgradeUI;
    public Usable_Button upgrader;

    DataManager dataManager;
    CameraController_Tavern cameraController;
    Text coinsText;

    private void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController_Tavern>();
        coinsText = GameObject.Find( "coins_text" ).GetComponent<Text>();
        if( GameObject.Find( "DataManager" ) )
        {
            dataManager = GameObject.Find( "DataManager" ).GetComponent<DataManager>();
            coinsText.text = dataManager.GetSaveValue( ESave_Values_Keys.COINS_AMOUNT ).ToString() + " Coins";
        }
        upgradeUI.SetActive( false );
    }

    public void UpdateUI()
    {
        if( dataManager != null )
            coinsText.text = dataManager.GetSaveValue( ESave_Values_Keys.COINS_AMOUNT ).ToString() + " Coins";
    }

    public void LeaveTavern()
    {
        SceneManager.LoadScene( 0 );
    }
    public void OpenUpgradeMenu()
    {
        upgradeUI.SetActive( true );

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        cameraController.ChangeState( false );
        upgrader.active = false;
    }
    public void ExitUpgradeMenu()
    {
        upgradeUI.SetActive( false );

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        cameraController.ChangeState( true );
        upgrader.active = true;
    }
}
