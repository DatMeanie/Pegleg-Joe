using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnXAxis : MonoBehaviour
{
    public float rotationSpeed = 10.0f;

    bool isActive;

    private void Start()
    {
        if ( PlayerPrefs.GetInt( "DayNightCycle" ) == 1 )
            isActive = true;
    }

    void Update()
    {
        if( isActive == true )
            transform.Rotate( new Vector3( Time.deltaTime * rotationSpeed, 0, 0 ) );
    }
}
