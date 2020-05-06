using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_Item : MonoBehaviour
{
    public int healthAmount = 50;
    private void OnTriggerEnter( Collider other )
    {
        if ( other.name.Contains( "Player" ) )
        {
            other.GetComponent<PlayerController>().Heal( healthAmount );
            transform.position = new Vector3( 800, 500, 500 );
        }
    }
}
