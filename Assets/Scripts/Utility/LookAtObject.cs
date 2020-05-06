using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtObject : MonoBehaviour
{
    public GameObject lookObject;
    public float turnRate = 2.0f;

    public bool isActive = false;
    public bool lockX = false;
    public bool lockY = false; 
    public bool lockZ = false; 
    public bool backToOriginalPosition = false;

    Vector3 originalRot;
    private void Start()
    {
        originalRot = transform.rotation.eulerAngles;
    }
    void Update()
    {
        if ( isActive )
        {
            Vector3 rot = Vector3.zero;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation( lookObject.transform.position - transform.position), turnRate * Time.deltaTime);

            if (lockX == false)
                rot.x = transform.rotation.eulerAngles.x;
            if (lockY == false)
                rot.y = transform.rotation.eulerAngles.y;
            if (lockZ == false)
                rot.z = transform.rotation.eulerAngles.z;

            transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
        }
        if( isActive == false && backToOriginalPosition == true)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(originalRot), turnRate * Time.deltaTime);
        }
    }
}
