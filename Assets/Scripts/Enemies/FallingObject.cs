using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObject : MonoBehaviour
{
    public int damage = 20;
    bool alreadyDamaged = false;
    private void OnTriggerEnter( Collider other )
    {
        if (other.name.Contains( "Player" ) == true && alreadyDamaged  == false )
        {
            other.GetComponent<PlayerController>().Damage( damage );
            alreadyDamaged = true;
        }
    }
}
