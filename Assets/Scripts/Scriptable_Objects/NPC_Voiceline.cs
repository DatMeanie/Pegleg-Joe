using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class NPC_Voiceline : ScriptableObject
{
    public string dialogue;
    public string choice1Text = "Talk";
    public string choice2Text = "Talk";

    public int ID;

    public int Choice1ID;
    public int Choice2ID;

    public AudioClip voiceLine;
    public bool NotRandom;

    public string QuestID;
}
