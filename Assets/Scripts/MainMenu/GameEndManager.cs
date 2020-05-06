using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameEndManager : MonoBehaviour
{
    public Transform content;

    List<string>  logs;
    List<int> sortedScores;

    Vector3 startPos;
    float distance;

    public void Initialize()
    {
        startPos = content.GetChild( 0 ).GetComponent<RectTransform>().position;
        distance = ( content.GetChild( 1 ).GetComponent<Transform>().position - content.GetChild( 2 ).position ).y;

        for ( int i = 0; i < content.childCount; i++ )
        {
            Transform textObj = content.GetChild(i);
            textObj.position = startPos;
            startPos = new Vector3( startPos.x, startPos.y + distance, startPos.z );
        }

        logs = DataManager.singleton.logs;
        Invoke( "UpdateText", 0.06f );
    }

    public void UpdateText()
    {

        for ( int i = 0; i < content.childCount; i++ )
        {
            if ( i < logs.Count )
                content.GetChild( i ).GetComponent<Text>().text = logs[ i ];
            else
                content.GetChild( i ).GetComponent<Text>().text = " ";
        }

    }

}
