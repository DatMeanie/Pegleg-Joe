using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TavernPerson : MonoBehaviour
{
    ////////////////////////////////////////////////////////////
    // COMPONENTS
    ////////////////////////////////////////////////////////////

    CameraController_Tavern cameraController;
    DialogueManager dialogueManager;
    DataManager dataManager;
    Usable_Button userPress;
    AudioSource audioSource;

    ////////////////////////////////////////////////////////////
    // VARIABLES
    ////////////////////////////////////////////////////////////

    public string npcName;
    public List<NPC_Voiceline> voicelines;
    public int npcID;

    ////////////////////////////////////////////////////////////

    private void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController_Tavern>();
        dialogueManager = GameObject.Find( "Dialogue_UI" ).GetComponent<DialogueManager>();

        if ( DataManager.singleton != null )
            dataManager = DataManager.singleton;

        audioSource = GetComponent<AudioSource>();
        userPress = GetComponent<Usable_Button>();
    }
    
    ////////////////////////////////////////////////////////////

    public void StartDialogue()
    {
        cameraController.ChangeState( false );
        NPC_Voiceline voiceLine = null;
        int rewardID = dataManager.FindIfQuestIsCompleted( npcName );
        if ( rewardID > -1 )
        {
            voiceLine = GetVoiceLine( rewardID );
            dataManager.CompleteQuest( npcName );
        }
        else
            voiceLine = GetVoiceLine();

        ////////////////////////////////////////////////////////////

        dialogueManager.EnterDialogue( this, voiceLine );

        audioSource.clip = voiceLine.voiceLine;
        audioSource.Play();

        userPress.active = false;
    }

    ////////////////////////////////////////////////////////////

    public void TalkWith( NPC_Voiceline _previousVoiceLine, int choice )
    {
        NPC_Voiceline voiceLine = null;
        if ( _previousVoiceLine == null )
            voiceLine = GetVoiceLine();
        else
        {
            if ( choice == 0 )
                voiceLine = GetVoiceLine( _previousVoiceLine.Choice2ID );
            else
                voiceLine = GetVoiceLine( _previousVoiceLine.Choice1ID );
        }

        if ( voiceLine == null )
            voiceLine = voicelines[ Random.Range( 0, voicelines.Count ) ];

        dialogueManager.UpdateText( voiceLine );

        audioSource.clip = voiceLine.voiceLine;
        audioSource.Play();
    }

    ////////////////////////////////////////////////////////////

    public void ExitDialogue()
    {
        userPress.active = true;
    }

    ////////////////////////////////////////////////////////////

    NPC_Voiceline GetVoiceLine()
    {
        NPC_Voiceline voiceLine = voicelines[ Random.Range( 0, voicelines.Count ) ];
        if( dataManager != null )
            while ( true )
            {
                if( voiceLine.QuestID != "null" )
                    if ( dataManager.FindIfQuestIsTaken( npcName ) == false && voiceLine.NotRandom == false )
                        break;
                    else
                        voiceLine = voicelines[ Random.Range( 0, voicelines.Count ) ];
                else
                    if( voiceLine.NotRandom == false )
                        break;
                    else
                        voiceLine = voicelines[ Random.Range( 0, voicelines.Count ) ];
            }

        return voiceLine;
    }

    ////////////////////////////////////////////////////////////

    NPC_Voiceline GetVoiceLine( int _index )
    {
        NPC_Voiceline voiceLine = null;
        foreach( NPC_Voiceline npcvl in voicelines )
        {
            if( npcvl.ID == _index )
            {
                voiceLine = npcvl;
                break;
            }
        }

        return voiceLine;
    }

    ////////////////////////////////////////////////////////////
}
