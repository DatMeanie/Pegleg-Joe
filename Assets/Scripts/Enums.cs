using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
public enum Quest_Conditions
{
    [XmlEnum(Name = "X_TIMEON_Y_MAP")]
    XTIMEONYMAP,
    [XmlEnum(Name = "KILL_X_OF_Y")]
    KILLXOFY,
    [XmlEnum(Name = "EARNED_X_COINS_ONEPLAY")]
    EARNEDXCOINSONEPLAY,
    [XmlEnum(Name = "FIND_X_ON_Y_MAP")]
    FINDXONYMAP
}

public enum Enemy_Types
{
    NORMAL,
    UNDEAD,
    ANIMAL,
    MIDGET,
    MEDIEVAL,
    BOSS
}