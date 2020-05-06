using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HighscoresManager : MonoBehaviour
{
    public Transform content;

    Dictionary<int, string> highscores;
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

        Invoke( "UpdateText", 0.06f );
    }

    public void UpdateText()
    {
        highscores = DataManager.singleton.highscores;

        sortedScores = highscores.Keys.ToList();
        sortedScores.Sort();

        for ( int i = 0; i < content.childCount; i++ )
        {
            if ( highscores[ sortedScores[ sortedScores.Count - i - 1 ] ] != "404" )
                content.GetChild( i ).GetComponent<Text>().text = highscores[ sortedScores[ sortedScores.Count - i - 1 ] ];
            else
                content.GetChild( i ).GetComponent<Text>().text = " ";
        }
    }
}
