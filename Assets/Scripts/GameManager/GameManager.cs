using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    ////////////////////////////////////////////////////////////

    public GameObject healthItem;

    GameObject[] healthItems = new GameObject[8];
    int healthItemIndex = 0;

    ////////////////////////////////////////////////////////////

    DataManager dataManager;
    List<Quest> questList = new List<Quest>();
    List<float> questTimers = new List<float>();

    ////////////////////////////////////////////////////////////

    void Start()
    {
        TreasureChestDropzone.coins = 0;
        TreasureChestDropzone.chestsStolen = 0;
        Cannonball.enemyKilledCount = 0;

        if ( GameObject.Find( "DataManager" ) )
        {
            dataManager = DataManager.singleton;
            dataManager.playedGame = true;
            dataManager.logs.Clear();
            questList = dataManager.GetQuests();

            foreach ( Quest quest in questList )
                if ( quest.questConditions[ quest.currentStage ].conditionID == Quest_Conditions.XTIMEONYMAP )
                    questTimers.Add( quest.questConditions[ quest.currentStage ].x );
                else
                    questTimers.Add( 0.0f );
        }

        for ( int i = 0; i < 8; i++ )
            healthItems[ i ] = Instantiate(healthItem, new Vector3(700, 400, 1000), Quaternion.identity, GameObject.Find("Health_Items").transform );

        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach ( Enemy enemy in enemies )
            enemy.Initialize();

        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach ( PlayerController player in players )
            player.Initialize();

        TreasureChestDropzone[] dropzones = FindObjectsOfType<TreasureChestDropzone>();
        foreach ( TreasureChestDropzone zone in dropzones )
            zone.Initialize();

        GetComponent<TreasureChestSpawner>().Initialize();
    }

    ////////////////////////////////////////////////////////////

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        int count = -1;
        foreach ( Quest quest in questList )
        {
            count++;
            if ( quest.currentStage >= quest.questConditions.Length )
                continue;

            if ( quest.questConditions[ quest.currentStage ].conditionID == Quest_Conditions.XTIMEONYMAP
                && quest.questConditions[ quest.currentStage ].y == dataManager.GetGameValue( EGame_Values_Keys.LEVEL_SELECTED ) )
            {
                questTimers[ count ] -= deltaTime;
                if ( questTimers[ count ] <= 0.0f )
                    quest.currentStage++;
            }
        }
    }

    ////////////////////////////////////////////////////////////

    public void UpdateQuests( Enemy_Types _type )
    {
        foreach( Quest quest in questList )
        {
            if ( quest.currentStage >= quest.questConditions.Length )
                continue;

            if ( quest.questConditions[ quest.currentStage ].conditionID == Quest_Conditions.KILLXOFY 
                && quest.questConditions[ quest.currentStage ].y == (int)_type )
            {
                quest.questConditions[ quest.currentStage ].x--;
                if( quest.questConditions[ quest.currentStage ].x <= 0.0f )
                    quest.currentStage++;
            }
        }
    }

    ////////////////////////////////////////////////////////////

    public void SpawnHealthItem( Vector3 pos )
    {
        healthItems[ healthItemIndex ].transform.position = pos;

        healthItemIndex++;
        if ( healthItemIndex >= healthItems.Length )
            healthItemIndex = 0;
    }

    ////////////////////////////////////////////////////////////

    public void EndGame()
    {
        if ( dataManager != null )
        {
            dataManager.AddLog( "Killed: " + Cannonball.GetKillCount() );
            dataManager.AddLog( "Coins collected : " + TreasureChestDropzone.coins );
            dataManager.AddLog( "Chests stolen: " + TreasureChestDropzone.chestsStolen );

            dataManager.ChangeSaveValue( ESave_Values_Keys.HIGHEST_KILLSAMOUNT, Cannonball.GetKillCount() );
            dataManager.ChangeSaveValue( ESave_Values_Keys.HIGHEST_COINSAMOUNT, TreasureChestDropzone.coins );
            dataManager.ChangeSaveValue( ESave_Values_Keys.HIGHEST_TREASURECHESTAMOUNT, TreasureChestDropzone.chestsStolen );

            dataManager.SetHighScore( Cannonball.GetKillCount() + TreasureChestDropzone.coins );

            foreach ( Quest quest in questList )
            {
                if ( quest.currentStage >= quest.questConditions.Length )
                    continue;

                if ( quest.questConditions[ quest.currentStage ].conditionID == Quest_Conditions.EARNEDXCOINSONEPLAY && TreasureChestDropzone.coins >= quest.questConditions[ quest.currentStage ].x )
                    quest.currentStage++;
            }

            dataManager.ReplaceQuests( questList );
        }

        PlayerPrefs.SetFloat( "Beer_Multiplier", 1.0f );

        SceneManager.LoadScene( 0 );
    }
}
