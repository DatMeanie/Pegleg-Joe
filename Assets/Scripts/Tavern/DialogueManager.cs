using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    ////////////////////////////////////////////////////////////
    // PUBLIC COMPONENTS
    ////////////////////////////////////////////////////////////

    public GameObject choicesParent;
    public GameObject windowParent;
    public GameObject useTextObj;

    public Text npcName;
    public Text npcText;

    public Text choice1Text;
    public Text choice2Text;

    ////////////////////////////////////////////////////////////
    // PRIVATE COMPONENTS
    ////////////////////////////////////////////////////////////

    CameraController_Tavern cameraController;
    TavernPerson npc;
    NPC_Voiceline previousVoiceLine = null;
    DataManager dataManager;

    ////////////////////////////////////////////////////////////
    // PRIVATE VARIABLES
    ////////////////////////////////////////////////////////////

    [HideInInspector]
    public bool inDialogue = false;

    ////////////////////////////////////////////////////////////

    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController_Tavern>();

        if ( DataManager.singleton != null )
            dataManager = DataManager.singleton;

        windowParent.SetActive( false );
    }

    ////////////////////////////////////////////////////////////

    void Update()
    {
        if ( inDialogue == true && Input.GetKeyDown( KeyCode.Escape ) )
        {
            ExitDialogue();
        }
    }

    ////////////////////////////////////////////////////////////

    public void EnterDialogue( TavernPerson _npc, NPC_Voiceline _voiceline )
    {
        npc = _npc;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        UpdateText( _voiceline );

        windowParent.SetActive( true );
        useTextObj.SetActive( false );

        inDialogue = true;
    }

    ////////////////////////////////////////////////////////////

    public void UpdateText( NPC_Voiceline _voiceline )
    {
        npcName.text = npc.npcName;
        npcText.text = _voiceline.dialogue;

        choice1Text.text = _voiceline.choice1Text;
        choice2Text.text = _voiceline.choice2Text;

        previousVoiceLine = _voiceline;
    }

    ////////////////////////////////////////////////////////////

    public void TalkWithNPC( int choice )
    {
        if ( previousVoiceLine != null )
        {
            if ( previousVoiceLine.QuestID != "null" && choice == 1 )
                dataManager.AddQuest( previousVoiceLine.QuestID );

            if ( previousVoiceLine.Choice1ID == 0 && choice == 1 )
                previousVoiceLine = null;
            else if ( previousVoiceLine.Choice2ID == 0 && choice == 0 )
                previousVoiceLine = null;
        }
        npc.TalkWith( previousVoiceLine, choice );
    }

    ////////////////////////////////////////////////////////////

    public void ExitDialogue()
    {
        cameraController.ChangeState( true );
        windowParent.SetActive( false );
        useTextObj.SetActive( true );

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        npc.ExitDialogue();

        inDialogue = false;
    }

    ////////////////////////////////////////////////////////////
}
