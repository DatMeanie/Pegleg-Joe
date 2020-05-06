using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class Upgrade : ScriptableObject
{
    public new string name;
    public string fancyName;
    public string description;

    public int coinCost;
}
