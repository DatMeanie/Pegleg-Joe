using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Usable : MonoBehaviour
{
    //usable base script

    public bool hold;
    public bool active = true;
    public Usable(bool hold, bool active)
    {
        this.hold = hold;
        this.hold = active;
    }
    public abstract void Use();
}
