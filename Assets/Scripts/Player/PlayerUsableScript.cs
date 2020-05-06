using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUsableScript : MonoBehaviour
{
    //use usables script
    //use the one nearest

    Usable usable;
    public GameObject useText;
    private void Update()
    {
        if( usable != null )
            if ( usable.active == false )
                usable = null;

        //hold usable
        if (usable != null && usable.hold == false && Input.GetKeyDown(KeyCode.E) )
            usable.Use();
        //not hold usable
        if (usable != null && usable.hold == true && Input.GetKey(KeyCode.E))
            usable.Use();

        if ( usable == null )
            useText.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Usable>() != false )
        {
            if ( other.GetComponent<Usable>().active == false )
                return;

            if ( usable != null )
            {
                //is closer than current usable?
                if ( Vector3.Distance( other.transform.position, transform.position ) < Vector3.Distance( usable.transform.position, transform.position ) )
                    usable = other.GetComponent<Usable>();

            }
            else
                usable = other.GetComponent<Usable>();
        }

        useText.SetActive( true );
    }
    private void OnTriggerStay(Collider other)
    {


        if (other.GetComponent<Usable>() != null)
        {
            if ( usable != null )
            {
                //compare usables in area
                if ( Vector3.Distance( other.transform.position, transform.position ) < Vector3.Distance( usable.transform.position, transform.position ) )
                    usable = other.gameObject.GetComponent<Usable>();
            }
            else
                usable = other.GetComponent<Usable>();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        usable = null;
        useText.SetActive(false);
    }
}
