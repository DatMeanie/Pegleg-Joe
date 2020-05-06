using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Xml;
public struct SGame_Values
{
    public int difficulty;
    public int levelSelected;
    public float enemySpawnModifier;
    public float enemySpawnLimit;
    public float coinModifier;
    public float playerHealthModifier;
}

public struct SSave_Values
{
    public int coinsAmount;
    public int killsAmount;
    public int highest_Killstreak;
    public int highest_KillsAmount;
    public int highest_CoinsAmount;
    public int highest_TreasureChestAmount;
}

public enum EGame_Values_Keys
{
    DIFFICULTY,
    ENEMY_SPAWN_MODIFIER,
    ENEMY_SPAWN_LIMIT,
    COIN_MODIFIER,
    PLAYER_HEALTH_MODIFIER,
    LEVEL_SELECTED
}
public enum ESave_Values_Keys
{
    COINS_AMOUNT,
    KILLS_AMOUNT,
    HIGHEST_KILLSTREAK,
    HIGHEST_KILLSAMOUNT,
    HIGHEST_COINSAMOUNT,
    HIGHEST_TREASURECHESTAMOUNT
}
public class DataManager : MonoBehaviour
{
    
    public static DataManager singleton;
    public AudioMixer mixer;

    SGame_Values gameValues;
    SSave_Values saveValues;

    public Dictionary<int, string> highscores = new Dictionary<int, string>();
    List<int> highscore_Scores = new List<int>();
    List<string> highscore_Strings = new List<string>();

    [HideInInspector]
    public List<string> logs = new List<string>();
    [HideInInspector]
    public bool playedGame = false;

    public List<Quest> questList = new List<Quest>();
    XmlSerializer serializerQuest = new XmlSerializer(typeof(QuestDataBase));

    //databases
    public QuestDataBase questDB = new QuestDataBase();

    ////////////////////////////////////////////////////////////

    private void Awake()
    {
        if ( singleton == null )
        {
            DontDestroyOnLoad( gameObject );
            singleton = this;

            if ( Directory.Exists( Application.dataPath + "/SaveData/" ) == false )
                Directory.CreateDirectory( Application.dataPath + "/SaveData/" );

            if ( File.Exists( Application.dataPath + "/SaveData/quest_data.xml" ) == false )
                File.Create( Application.dataPath + "/SaveData/quest_data.xml" );
            
            //AddQuest();
            //Invoke("SaveQuestData", 0.2f);
            Invoke( "LoadQuestData", 0.2f);
        }
        else if ( singleton != this )
        {
            Destroy( gameObject );
        }

        ////////////////////////////////////////////////////////////
        // LOAD HIGHSCORES OR SAVE PLACEHOLDERS
        ////////////////////////////////////////////////////////////

        if ( File.Exists( Application.persistentDataPath + "/Highscores.dat" ) )
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Highscores.dat", FileMode.Open);
            highscore_Scores = (List<int>)bf.Deserialize( file );
            highscore_Strings = (List<string>)bf.Deserialize( file );
            file.Close();

            highscores.Clear();
            for ( int i = 0; i < highscore_Scores.Count; i++ )
                highscores.Add( highscore_Scores[ i ], highscore_Strings[ i ] );
        }
        else
        {
            for ( int i = -20; i < 0; i++ )
                highscores.Add( i, " " );

            for ( int i = -20; i < 0; i++ )
                highscore_Scores.Add( i );

            for ( int i = 0; i < 20; i++ )
                highscore_Strings.Add( "404" );

            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileHighscores = File.Create(Application.persistentDataPath + "/Highscores.dat");
            bf.Serialize( fileHighscores, highscore_Scores );
            bf.Serialize( fileHighscores, highscore_Strings );
            fileHighscores.Close();
        }

        ResetValues();
    }

    ////////////////////////////////////////////////////////////

    private void Start()
    {
        //////////////////////////////
        // LEVELS
        //////////////////////////////

        if ( PlayerPrefs.HasKey( "Level0_IsBought" ) == false )
            PlayerPrefs.SetInt( "Level0_IsBought", 1 );
        else
            if ( PlayerPrefs.GetInt( "Level0_IsBought" ) == 0 )
                PlayerPrefs.SetInt( "Level0_IsBought", 1 );

        //////////////////////////////
        // AUDIO
        //////////////////////////////

        if ( PlayerPrefs.HasKey( "MasterVolume" ) == true )
            mixer.SetFloat( "MasterVolume", Mathf.Log10( PlayerPrefs.GetFloat( "MasterVolume" ) ) * 20 );
        else
            PlayerPrefs.SetFloat( "MasterVolume", 1.0f );

        if ( PlayerPrefs.HasKey( "EnemyVolume" ) == true )
            mixer.SetFloat( "EnemyVolume", Mathf.Log10( PlayerPrefs.GetFloat( "EnemyVolume" ) ) * 20 );
        else
            PlayerPrefs.SetFloat( "EnemyVolume", 1.0f );

        if ( PlayerPrefs.HasKey( "MusicVolume" ) == true )
            mixer.SetFloat( "MusicVolume", Mathf.Log10( PlayerPrefs.GetFloat( "MusicVolume" ) ) * 20 );
        else
            PlayerPrefs.SetFloat( "MusicVolume", 1.0f );

        if ( PlayerPrefs.HasKey( "MusicVolume" ) == true )
            mixer.SetFloat( "MusicVolume", Mathf.Log10( PlayerPrefs.GetFloat( "MusicVolume" ) ) * 20 );
        else
            PlayerPrefs.SetFloat( "MusicVolume", 1.0f );

        //////////////////////////////
        // GAMEPLAY
        //////////////////////////////

        if ( PlayerPrefs.HasKey( "AimingXSensitivity" ) == false )
            PlayerPrefs.SetFloat( "AimingXSensitivity", 1.5f );

        if ( PlayerPrefs.HasKey( "AimingYSensitivity" ) == false )
            PlayerPrefs.SetFloat( "AimingYSensitivity", 1.5f );

        if ( PlayerPrefs.HasKey( "Autoshoot" ) == false )
            PlayerPrefs.SetInt( "Autoshoot", 0 );

        if ( PlayerPrefs.HasKey( "DayNightCycle" ) == false )
            PlayerPrefs.SetInt( "DayNightCycle", 1 );

        if ( PlayerPrefs.HasKey( "AimingEnabled" ) == false )
            PlayerPrefs.SetInt( "AimingEnabled", 1 );

        if ( PlayerPrefs.HasKey( "AimAssist" ) == false )
            PlayerPrefs.SetInt( "AimAssist", 0 );

        if ( PlayerPrefs.HasKey( "Cannonball_Amount" ) == false )
            PlayerPrefs.SetInt( "CoinsAmount", 100 );

        if ( PlayerPrefs.HasKey( "Beer_Multiplier" ) == false )
            PlayerPrefs.SetFloat( "Beer_Multiplier", 1.0f );

    }

    ////////////////////////////////////////////////////////////

    public void ResetProgression()
    {
        PlayerPrefs.SetInt( "CoinsAmount", 0 );
        saveValues.coinsAmount = 0;

        PlayerPrefs.SetFloat( "Beer_Multiplier", 1.0f );

        PlayerPrefs.SetInt( "Dash_Upgrade", 0 );
        PlayerPrefs.SetInt( "Cannonball_Amount", 50 );
        PlayerPrefs.SetInt( "Coconut_Amount", 100 );

        GameObject.Find( "TreasureChests_BackgroundLoot" ).GetComponent<CoinLoot>().ResetProgress();
        GameObject.Find( "TreasureChests_BackgroundLoot" ).GetComponent<CoinLoot>().FixChests();

        GameObject.Find( "coins_counter_text" ).GetComponent<Text>().text = saveValues.coinsAmount.ToString() + " coins";
    }

    ////////////////////////////////////////////////////////////

    public void ChangeGameValue( EGame_Values_Keys key, float _value )
    {
        switch ( key )
        {
            case EGame_Values_Keys.DIFFICULTY:
                gameValues.difficulty = (int)_value;
                switch( gameValues.difficulty )
                {
                    case 0:
                        gameValues.enemySpawnModifier = 1.2f;
                        gameValues.enemySpawnLimit = 1.2f;
                        gameValues.playerHealthModifier = 2.0f;
                        gameValues.coinModifier = 0.3f;
                        break;
                    case 1:
                        gameValues.enemySpawnModifier = 1.0f;
                        gameValues.enemySpawnLimit = 1.0f;
                        gameValues.playerHealthModifier = 1.0f;
                        gameValues.coinModifier = 1.0f;
                        break;
                    case 2:
                        gameValues.enemySpawnModifier = 0.7f;
                        gameValues.enemySpawnLimit = 0.7f;
                        gameValues.playerHealthModifier = 0.5f;
                        gameValues.coinModifier = 1.3f;
                        break;
                }
                break;

            case EGame_Values_Keys.ENEMY_SPAWN_MODIFIER:
                gameValues.enemySpawnModifier = _value;
                break;

            case EGame_Values_Keys.ENEMY_SPAWN_LIMIT:
                gameValues.enemySpawnModifier = _value;
                break;

            case EGame_Values_Keys.PLAYER_HEALTH_MODIFIER:
                gameValues.playerHealthModifier = _value;
                break;

            case EGame_Values_Keys.LEVEL_SELECTED:
                gameValues.levelSelected = (int)_value;
                break;
        }
    }

    ////////////////////////////////////////////////////////////

    public float GetGameValue( EGame_Values_Keys key )
    {
        switch ( key )
        {
            case EGame_Values_Keys.DIFFICULTY:
                return (float)gameValues.difficulty;

            case EGame_Values_Keys.ENEMY_SPAWN_MODIFIER:
                return gameValues.enemySpawnModifier;

            case EGame_Values_Keys.PLAYER_HEALTH_MODIFIER:
                return gameValues.playerHealthModifier;

            case EGame_Values_Keys.ENEMY_SPAWN_LIMIT:
                return gameValues.enemySpawnLimit;

            case EGame_Values_Keys.COIN_MODIFIER:
                return gameValues.coinModifier;

            case EGame_Values_Keys.LEVEL_SELECTED:
                return gameValues.levelSelected;

            default:
                return 404.0f;
        }
    }

    ////////////////////////////////////////////////////////////
    public void ChangeSaveValue( ESave_Values_Keys key, float _value )
    {
        switch ( key )
        {
            case ESave_Values_Keys.COINS_AMOUNT:
                saveValues.coinsAmount = (int)_value;
                PlayerPrefs.SetInt( "CoinsAmount", saveValues.coinsAmount );
                break;

            case ESave_Values_Keys.KILLS_AMOUNT:
                saveValues.killsAmount += (int)_value;
                PlayerPrefs.SetInt( "KillsAmount", saveValues.killsAmount );
                break;

            case ESave_Values_Keys.HIGHEST_KILLSAMOUNT:
                if( _value > saveValues.highest_KillsAmount )
                {
                    saveValues.highest_KillsAmount = (int)_value;
                    PlayerPrefs.SetInt( "Highest_KillsAmount", saveValues.highest_KillsAmount );
                }
                break;

            case ESave_Values_Keys.HIGHEST_COINSAMOUNT:
                if ( _value > saveValues.highest_CoinsAmount )
                {
                    saveValues.highest_CoinsAmount = (int)_value;
                    PlayerPrefs.SetInt( "Highest_CoinsAmount", saveValues.highest_CoinsAmount );
                }
                break;

            case ESave_Values_Keys.HIGHEST_KILLSTREAK:
                if ( _value > saveValues.highest_Killstreak )
                {
                    saveValues.highest_Killstreak = (int)_value;
                    PlayerPrefs.SetInt( "Highest_Killstreak", saveValues.highest_Killstreak );
                }
                break;

            case ESave_Values_Keys.HIGHEST_TREASURECHESTAMOUNT:
                if ( _value > saveValues.highest_TreasureChestAmount )
                {
                    saveValues.highest_TreasureChestAmount = (int)_value;
                    PlayerPrefs.SetInt( "Highest_TreasureChestAmount", saveValues.highest_TreasureChestAmount );
                }
                break;

        }
    }

    ////////////////////////////////////////////////////////////

    public float GetSaveValue( ESave_Values_Keys key )
    {
        switch ( key )
        {
            case ESave_Values_Keys.COINS_AMOUNT:
                return (float)saveValues.coinsAmount;

            case ESave_Values_Keys.HIGHEST_COINSAMOUNT:
                return (float)saveValues.highest_CoinsAmount;

            case ESave_Values_Keys.HIGHEST_KILLSAMOUNT:
                return (float)saveValues.highest_KillsAmount;

            case ESave_Values_Keys.HIGHEST_KILLSTREAK:
                return (float)saveValues.highest_Killstreak;

            case ESave_Values_Keys.HIGHEST_TREASURECHESTAMOUNT:
                return (float)saveValues.highest_TreasureChestAmount;

            case ESave_Values_Keys.KILLS_AMOUNT:
                return (float)saveValues.killsAmount;


            default:
                return 404.0f;
        }
    }

    ////////////////////////////////////////////////////////////
    public void SetHighScore( int _score )
    {
        bool higherThanOther = false;

        if ( _score == 0 )
            return;

        if ( highscores.ContainsKey( _score ) == true )
        {
            if(highscores[ _score ] == "404" )
            {
                AddHighScore( _score, _score );
                higherThanOther = true;
            }
            else
                return;
        }
        else if ( highscores.ContainsValue( "404" ) )
        {
            for ( int i = 0; i < highscore_Scores.Count; i++ )
            {
                if ( highscore_Strings[ i ] == "404" )
                {
                    AddHighScore( i, _score );
                    higherThanOther = true;
                    break;
                }
            }
        }
        else
        {
            for ( int i = 0; i < highscore_Scores.Count; i++ )
            {
                if ( _score > highscore_Scores[ i ] )
                {
                    AddHighScore( i, _score );
                    higherThanOther = true;
                    break;
                }
            }
        }

        if( higherThanOther )
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileHighscores = File.Create(Application.persistentDataPath + "/Highscores.dat");
            bf.Serialize( fileHighscores, highscore_Scores );
            bf.Serialize( fileHighscores, highscore_Strings );
            fileHighscores.Close();
        }
    }

    ////////////////////////////////////////////////////////////

    void AddHighScore( int index, int _score )
    {
        highscore_Scores[ index ] = _score;

        string deathState = " ";
        if ( GameObject.Find( "Player" ).GetComponent<PlayerController>().health < 0 )
            deathState = "Died ";
        else
            deathState = "Escaped ";

        string difficulty = "Normal";
        if ( gameValues.difficulty == 0 )
            difficulty = "Easy";
        else if ( gameValues.difficulty == 2 )
            difficulty = "Hard";

        string time = " Time " + System.DateTime.Now.ToString( "hh:mm:ss" );
        string date = " Date " + System.DateTime.Now.ToString( "yyyy/MM/dd" );

        //REMEMBER TO REPLACE VERSION!!!
        string scoreText = deathState  + "with "
                    + TreasureChestDropzone.coins
                    + " coins, killed " + Cannonball.GetKillCount()
                    + " enemies - Difficulty: " + difficulty
                    + "\n- Version Alpha 0.1.5 " + date + time;

        highscore_Strings[ index ] = scoreText;
        if( highscores.ContainsKey( _score ) )
        {
            highscores.Remove( _score );
            highscores.Add( _score, scoreText );
        }
        else
            highscores.Add( _score, scoreText );
    }

    public void AddLog( string text )
    {
        logs.Add( text );
    }

    ////////////////////////////////////////////////////////////

    public void FixValues()
    {
        if ( PlayerPrefs.HasKey( "CoinsAmount" ) == false )
            PlayerPrefs.SetInt( "CoinsAmount", 0 );
        else
            saveValues.coinsAmount = PlayerPrefs.GetInt( "CoinsAmount" );

        //////////////////////////////
        // STATS
        //////////////////////////////

        if ( PlayerPrefs.HasKey( "KillsAmount" ) == false )
            PlayerPrefs.SetInt( "KillsAmount", 0 );
        else
            saveValues.killsAmount = PlayerPrefs.GetInt( "KillsAmount" );

        if ( PlayerPrefs.HasKey( "Highest_CoinsAmount" ) == false )
            PlayerPrefs.SetInt( "Highest_CoinsAmount", 0 );
        else
            saveValues.highest_CoinsAmount = PlayerPrefs.GetInt( "Highest_CoinsAmount" );

        if ( PlayerPrefs.HasKey( "Highest_Killstreak" ) == false )
            PlayerPrefs.SetInt( "Highest_Killstreak", 0 );
        else
            saveValues.highest_Killstreak = PlayerPrefs.GetInt( "Highest_Killstreak" );

        if ( PlayerPrefs.HasKey( "Highest_KillsAmount" ) == false )
            PlayerPrefs.SetInt( "Highest_KillsAmount", 0 );
        else
            saveValues.highest_KillsAmount = PlayerPrefs.GetInt( "Highest_KillsAmount" );

        if ( PlayerPrefs.HasKey( "Highest_TreasureChestAmount" ) == false )
            PlayerPrefs.SetInt( "Highest_TreasureChestAmount", 0 );
        else
            saveValues.highest_TreasureChestAmount = PlayerPrefs.GetInt( "Highest_TreasureChestAmount" );

        //////////////////////////////
        // OTHER
        //////////////////////////////

        if ( GameObject.Find( "TreasureChests_BackgroundLoot" ) == true )
            GameObject.Find( "TreasureChests_BackgroundLoot" ).GetComponent<CoinLoot>().FixChests();

        playedGame = false;
    }

    ////////////////////////////////////////////////////////////

    private void ResetValues()
    {

        saveValues.coinsAmount = 0;
        gameValues.difficulty = 1;
        gameValues.enemySpawnModifier = 1.0f;
        gameValues.enemySpawnLimit = 1.0f;
        gameValues.playerHealthModifier = 1.0f;
        gameValues.levelSelected = 0;

    
    }

    ////////////////////////////////////////////////////////////
    //
    //                  QUEST RELATED FUNCTIONS
    //
    ////////////////////////////////////////////////////////////

    public void AddQuest()
    {
        //////////////////////////////
        // DEBUG
        //////////////////////////////

        QuestConditionEntry[] conditions = new QuestConditionEntry[ 1 ];
        conditions[ 0 ] = new QuestConditionEntry();
        conditions[ 0 ].description = "Do not delete!";
        conditions[ 0 ].conditionID = Quest_Conditions.KILLXOFY;
        conditions[ 0 ].x = 1;
        conditions[ 0 ].y = 5;
        conditions[ 0 ].z = 0;
        //conditions[ 0 ].time = 0.0f;
        Quest newQuest = new Quest( "Test quest", "null", -1, conditions, 1 );
        questList.Add( newQuest );

        questDB.list.Add( new QuestEntry() );
        questDB.list[ questDB.list.Count - 1 ].name = newQuest.name;
        questDB.list[ questDB.list.Count - 1 ].questGiverID = newQuest.questGiverID;
        questDB.list[ questDB.list.Count - 1 ].currentStage = newQuest.currentStage;
        questDB.list[ questDB.list.Count - 1 ].questConditions = conditions;
    }

    ////////////////////////////////////////////////////////////

    public void AddQuest( string _questID )
    {
        //add from database to list
        foreach ( QuestEntry quest in questDB.list )
        {
            if( _questID == quest.name )
            {
                Quest newQuest = new Quest( quest.name, quest.questGiverID, 0, quest.questConditions, quest.rewardDialogueID );
                if ( questList.Contains( newQuest ) == true )
                    break;
                questList.Add( newQuest );
                break;
            }
        }
    }

    ////////////////////////////////////////////////////////////

    public void SaveQuestData()
    {
        try
        {
            FileStream streamQuest = File.Open(Application.dataPath + "/SaveData/quest_data.xml", FileMode.Create);
            serializerQuest.Serialize( streamQuest, questDB );
            streamQuest.Close();
            Debug.Log( "Saving quests complete" );
        }
        catch
        {
            Debug.Log( "Error: Saving quests failed" );
        }
    }

    ////////////////////////////////////////////////////////////

    void LoadQuestData()
    {
        try
        {
            FileStream streamQuest = File.Open(Application.dataPath + "/SaveData/quest_data.xml", FileMode.Open);
            questDB = (QuestDataBase)serializerQuest.Deserialize( streamQuest );
            streamQuest.Close();

            //add from database to list
            foreach ( QuestEntry quest in questDB.list )
            {
                if ( quest.currentStage <= -1 || quest.questGiverID == "null" )
                    continue;
                questList.Add( new Quest( quest.name, quest.questGiverID, quest.currentStage, quest.questConditions, quest.rewardDialogueID ) );
            }
        }
        catch
        {
            //data cannot be opened
            Debug.Log( "Error: Loading quest failed" );
        }
    }

    ////////////////////////////////////////////////////////////

    public void ReplaceQuests( List<Quest> _newList )
    {
        questList = _newList;
    }

    ////////////////////////////////////////////////////////////

    public List<Quest> GetQuests( Quest_Conditions _condition )
    {
        List<Quest> returnQuests = new List<Quest>();
        foreach ( Quest quest in questList )
            if ( quest.questConditions[ quest.currentStage ].conditionID == _condition && quest.currentStage < quest.questConditions.Length )
                returnQuests.Add( quest );
        return returnQuests;
    }

    ////////////////////////////////////////////////////////////
    
    public List<Quest> GetQuests()
    {
        return questList;
    }

    ////////////////////////////////////////////////////////////
    
    public bool FindIfQuestIsTaken( string _questGiverID )
    {
        foreach ( Quest quest in questList )
            if ( quest.currentStage >= 0 && quest.questGiverID == _questGiverID )
                return true;
        return false;
    }

    ////////////////////////////////////////////////////////////

    public int FindIfQuestIsCompleted( string _questGiverID )
    {
        foreach ( Quest quest in questList )
            if ( quest.currentStage >= quest.questConditions.Length && quest.questGiverID == _questGiverID )
                return quest.rewardDialogueID;
        return -1;
    }

    ////////////////////////////////////////////////////////////
    public void CompleteQuest( string _questGiverID )
    {
        foreach ( Quest quest in questList )
        {
            if ( quest.currentStage >= quest.questConditions.Length && quest.questGiverID == _questGiverID )
            {
                questList.Remove( quest );
                ChangeSaveValue( ESave_Values_Keys.COINS_AMOUNT, GetSaveValue( ESave_Values_Keys.COINS_AMOUNT ) + 200 );
                break;
            }
        }
    }

    ////////////////////////////////////////////////////////////
}

////////////////////////////////////////////////////////////
//
//             QUEST CLASSES && XML
//
////////////////////////////////////////////////////////////

public class Quest
{
    public string name;
    public string questGiverID;
    public int currentStage;
    public int rewardDialogueID;
    public QuestConditionEntry[] questConditions;
    public Quest( string _name, string _questGiverID, int _currentStage, QuestConditionEntry[] _questConditions, int _rewardDialogueID )
    {
        name = _name;
        questGiverID = _questGiverID;
        currentStage = _currentStage;
        questConditions = _questConditions;
        rewardDialogueID = _rewardDialogueID;
    }
}

////////////////////////////////////////////////////////////

//{
//    public string description;
//    public int conditionID;
//    public int xID;
//    public int yID;
//    public int zID;

//    public QuestCondition( int _conditionID, string _description, int _xID, int _yID, int _zID )
//    {
//        description = _description;
//        conditionID = _conditionID;
//        xID = _xID;
//        yID = _yID;
//        zID = _zID;
//    }
//}

////////////////////////////////////////////////////////////

[System.Serializable]
public class QuestConditionEntry
{
    [XmlAttribute("Description", AttributeName = "Description")]
    public string description;
    [XmlAttribute("ConditionID", AttributeName = "ConditionID")]
    public Quest_Conditions conditionID;
    [XmlAttribute("x", AttributeName = "x")]
    public float x;
    [XmlAttribute("y", AttributeName = "y")]
    public int y;
    [XmlAttribute("z", AttributeName = "z")]
    public int z;
    [XmlAttribute("AdditionalDialogueID", AttributeName = "AdditionalDialogueID")]
    public int additionalDialogue;
}

////////////////////////////////////////////////////////////

[System.Serializable]
public class QuestEntry
{
    [XmlAttribute("Name")]
    public string name;
    [XmlAttribute("QuestGiver")]
    public string questGiverID;
    [XmlAttribute("CurrentStage")]
    public int currentStage;
    [XmlArray("QuestConditions")]
    public QuestConditionEntry[] questConditions;
    [XmlAttribute("RewardDialogueID")]
    public int rewardDialogueID;
}

////////////////////////////////////////////////////////////

[System.Serializable]
public class QuestDataBase
{
    [XmlArray("QuestList")]
    [XmlArrayItem("Quest")]
    public List<QuestEntry> list = new List<QuestEntry>();
}