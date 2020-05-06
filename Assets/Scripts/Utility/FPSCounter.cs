using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public int avgFrameRate;
    public Text display_Text;
    int times = 0;
    private void Start()
    {
            display_Text.text = avgFrameRate.ToString() + " FPS";
    }

    public void Update()
    {
        float current = 0;
        current = (int)( 1f / Time.unscaledDeltaTime );
        avgFrameRate = (int)current;
        times++;
        if ( times >= 60 )
        {
            times = 0;
            display_Text.text = avgFrameRate.ToString() + " FPS";
        }
    }
}
