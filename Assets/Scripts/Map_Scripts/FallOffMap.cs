using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallOffMap : MonoBehaviour
{
    //player or enemy fall off map, dead
    //might happen due to bugs 

    private void OnTriggerEnter(Collider other)
    {
        if ( other.name.Contains( "Player" ) )
            other.GetComponent<PlayerController>().Damage( 10000 );
        else if ( other.name.Contains( "&&" ) )
            Destroy( other.gameObject );
        else if ( other.GetComponent<Cannonball>() )
            Destroy( other.gameObject );
        else if ( other.GetComponent<FallingObject>() )
            Destroy( other.gameObject );
    }
}
