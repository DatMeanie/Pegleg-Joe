using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class Level : ScriptableObject
{
    public new string name;
    public string description;

    public int coinCost;

    public Texture selectImage;
}
