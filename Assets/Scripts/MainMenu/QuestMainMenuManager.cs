using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class QuestMainMenuManager : MonoBehaviour
{
    public Transform content;

    List<Quest> quests;
    Vector3 startPos;
    float distance;

    bool alreadyInitialized = false;

    ////////////////////////////////////////////////////////////

    public void Initialize()
    {
        if ( alreadyInitialized == true )
            return;
        alreadyInitialized = true;

        GameObject firstChild = content.GetChild( 0 ).gameObject;
        GameObject secondChild = content.GetChild( 1 ).gameObject;
        startPos = content.GetChild( 0 ).GetComponent<RectTransform>().position;
        distance = ( secondChild.GetComponent<Transform>().position - firstChild.GetComponent<Transform>().position ).y;
        quests = DataManager.singleton.GetQuests();

        ////////////////////////////////////////////////////////////

        foreach( Quest quest in quests )
        {
            Transform textObj = Instantiate( firstChild.transform, content );

            textObj.position = startPos;
            startPos = new Vector3( startPos.x, startPos.y + distance, startPos.z );

            if ( quest.currentStage >= quest.questConditions.Length )
            {
                string descriptionText = quest.questConditions[ quest.questConditions.Length - 1 ].description;
                if ( descriptionText.Contains( "XAMOUNT " ) == true )
                    descriptionText = descriptionText.Replace( "XAMOUNT", quest.questConditions[ quest.questConditions.Length - 1 ].x.ToString() );
                textObj.GetChild( 0 ).gameObject.SetActive( false );
                textObj.GetChild( 3 ).GetChild( 0 ).GetComponent<Text>().text = descriptionText;
            }
            else
            {
                string descriptionText = quest.questConditions[ quest.currentStage ].description;
                if ( descriptionText.Contains( "XAMOUNT " ) == true )
                    descriptionText = descriptionText.Replace( "XAMOUNT", quest.questConditions[ quest.currentStage ].x.ToString() );
                textObj.GetChild( 1 ).gameObject.SetActive( false );
                textObj.GetChild( 3 ).GetChild( 0 ).GetComponent<Text>().text = descriptionText;
            }
        }

        ////////////////////////////////////////////////////////////
        
        Destroy( firstChild );
        Destroy( secondChild );
    }
}
