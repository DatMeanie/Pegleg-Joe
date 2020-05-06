using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndirectObjectConnection : MonoBehaviour
{
    public GameObject parentObject;
    Vector3 offsetPos;
    Vector3 offsetRot;
    public bool followPosition;
    public bool followRotation;
    private void Start()
    {
        if( followPosition )
            offsetPos = transform.position - parentObject.transform.position;
        if( followRotation )
            offsetRot = transform.eulerAngles - parentObject.transform.eulerAngles;
    }
    void Update()
    {
        if( followPosition )
            transform.position = parentObject.transform.position + offsetPos;
        if( followRotation )
            transform.rotation = Quaternion.Euler(parentObject.transform.eulerAngles + offsetRot);
    }
}
