using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinLoot : MonoBehaviour
{
    List<GameObject> children = new List<GameObject>();
    public void Initialize()
    {
        transform.position = Vector3.zero;

        for ( int childIndex = 0; childIndex < transform.childCount; childIndex++ )
            children.Add( transform.GetChild( childIndex ).gameObject );
    }
    public void FixChests()
    {
        if ( children.Count < 1 )
            Initialize();

        if ( PlayerPrefs.HasKey( "CoinsAmount" ) == true && GameObject.Find( "DataManager" ) )
        {
            int startValue = Mathf.RoundToInt( GameObject.Find("DataManager").GetComponent<DataManager>().GetSaveValue(ESave_Values_Keys.COINS_AMOUNT) / 300.0f );
            if ( startValue < transform.childCount )
                for ( int chestIndex = 0; chestIndex < transform.childCount; chestIndex++ )
                    if( chestIndex >= startValue )
                        children[ chestIndex ].SetActive( false );
                    else
                        children[ chestIndex ].SetActive( true );
        }
    }

    public void ResetProgress()
    {
        for ( int chestIndex = 0; chestIndex < transform.childCount; chestIndex++ )
            children[ chestIndex ].SetActive( true );
    }
}
