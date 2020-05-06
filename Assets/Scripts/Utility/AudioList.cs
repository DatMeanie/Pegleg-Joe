using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioList : MonoBehaviour
{
    public List<AudioClip> sounds = new List<AudioClip>();
    public AudioClip GetRandomSound()
    {
        return sounds[ Random.Range( 0, sounds.Count ) ];
    }
}
