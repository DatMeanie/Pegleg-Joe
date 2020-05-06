using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Usable_Button : Usable
{
    //customizable button

    public UnityEvent invokeMethod; //method set in editor

    public Usable_Button( bool hold, bool active) : base(hold, active)
    {

    }

    public override void Use()
    {
        invokeMethod.Invoke();
    }
}
