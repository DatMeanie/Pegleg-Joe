using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    bool isOpen = false;

    //////////////////////////////
    // GAMEPLAY SETTINGS
    //////////////////////////////    

    [Header("Gameplay")]

    public Toggle autoshootToggle;
    public Toggle dayNightCycleToggle;
    public Toggle aimToggle;
    public Toggle aimAssistToggle;

    public Slider aimingXSlider;
    public Text aimingXText;

    public Slider aimingYSlider;
    public Text aimingYText;

    //////////////////////////////
    // AUDIO SETTINGS
    //////////////////////////////    

    [Header("Audio")]
    public AudioMixer mixer;

    public Slider masterVolumeSlider;
    public Text masterVolumeText;

    public Slider enemyVolumeSlider;
    public Text enemyVolumeText;

    public Slider musicVolumeSlider;
    public Text musicVolumeText;

    public void OpenMenu()
    {
        masterVolumeText.text = "Volume: " + PlayerPrefs.GetFloat( "MasterVolume" ).ToString();
        masterVolumeSlider.value = PlayerPrefs.GetFloat( "MasterVolume" );

        enemyVolumeText.text = "Enemy volume: " + PlayerPrefs.GetFloat( "EnemyVolume" ).ToString();
        enemyVolumeSlider.value = PlayerPrefs.GetFloat( "EnemyVolume" );

        musicVolumeText.text = "Music volume: " + PlayerPrefs.GetFloat( "MusicVolume" ).ToString();
        musicVolumeSlider.value = PlayerPrefs.GetFloat( "MusicVolume" );

        aimingXText.text = "Aiming X Sensitivity: " + PlayerPrefs.GetFloat( "AimingYSensitivity" ).ToString();
        aimingXSlider.value = PlayerPrefs.GetFloat( "AimingYSensitivity" );

        aimingYText.text = "Aiming Y sensitivity: " + PlayerPrefs.GetFloat( "AimingXSensitivity" ).ToString();
        aimingYSlider.value = PlayerPrefs.GetFloat( "AimingXSensitivity" );

        isOpen = true;

        if ( PlayerPrefs.GetInt( "Autoshoot" ) == 1 )
            autoshootToggle.isOn = true;
        else
            autoshootToggle.isOn = false;

        if ( PlayerPrefs.GetInt( "AimingEnabled" ) == 1 )
            aimToggle.isOn = true;
        else
            aimToggle.isOn = false;

        if ( PlayerPrefs.GetInt( "DayNightCycle" ) == 1 )
            dayNightCycleToggle.isOn = true;
        else
            dayNightCycleToggle.isOn = false;

        if ( PlayerPrefs.GetInt( "AimAssist" ) == 1 )
            aimAssistToggle.isOn = true;
        else
            aimAssistToggle.isOn = false;
    }

    public void SetMasterVolumeLevel( )
    {
        mixer.SetFloat( "MasterVolume", Mathf.Log10( masterVolumeSlider.value ) * 20 );
        masterVolumeText.text = "Volume: " + ( Mathf.Round( masterVolumeSlider.value * 100f ) / 100f ).ToString();
        float saveValue = Mathf.Round( masterVolumeSlider.value * 100f ) / 100f;
        if ( saveValue <= 0 )
            saveValue = 0.001f;
        PlayerPrefs.SetFloat( "MasterVolume", saveValue );
    }

    public void SetEnemyVolumeLevel()
    {
        mixer.SetFloat( "EnemyVolume", Mathf.Log10( enemyVolumeSlider.value ) * 20 );
        enemyVolumeText.text = "Enemy volume: " + ( Mathf.Round( enemyVolumeSlider.value * 100f ) / 100f ).ToString();
        float saveValue = Mathf.Round( enemyVolumeSlider.value * 100f ) / 100f;
        if ( saveValue <= 0 )
            saveValue = 0.001f;

        PlayerPrefs.SetFloat( "EnemyVolume", saveValue );


        if( isOpen == true )
        {
            enemyVolumeSlider.GetComponent<AudioSource>().clip = GameObject.Find( "Enemy_Human_Death_Sounds" ).GetComponent<AudioList>().GetRandomSound();
            enemyVolumeSlider.GetComponent<AudioSource>().Play();
        }
    }

    public void SetMusicVolumeLevel()
    {
        mixer.SetFloat( "MusicVolume", Mathf.Log10( musicVolumeSlider.value ) * 20 );
        musicVolumeText.text = "Music volume: " + ( Mathf.Round( musicVolumeSlider.value * 100f ) / 100f ).ToString();
        float saveValue = Mathf.Round( musicVolumeSlider.value * 100f ) / 100f;
        if ( saveValue <= 0 )
            saveValue = 0.001f;
        PlayerPrefs.SetFloat( "MusicVolume", saveValue );
    }

    public void SetAimingXSensitivity()
    {
        aimingXText.text = "Aiming X sensitivity: " + ( Mathf.Round( aimingXSlider.value * 100f ) / 100f ).ToString();
        float saveValue = Mathf.Round( aimingXSlider.value * 100f ) / 100f;
        if ( saveValue <= 0 )
            saveValue = 0.001f;
        PlayerPrefs.SetFloat( "AimingXSensitivity", saveValue );
    }

    public void SetAimingYSensitivity()
    {
        aimingYText.text = "Aiming Y sensitivity: " + ( Mathf.Round( aimingYSlider.value * 100f ) / 100f ).ToString();
        float saveValue = Mathf.Round( aimingYSlider.value * 100f ) / 100f;
        if ( saveValue <= 0 )
            saveValue = 0.001f;
        PlayerPrefs.SetFloat( "AimingYSensitivity", saveValue );
    }

    public void SetAutoshoot()
    {
        int result = 0;
        if ( aimToggle.isOn == true )
            result = 1;

        PlayerPrefs.SetInt( "Autoshoot", result );
    }
    public void SetAimAssist()
    {
        int result = 0;
        if ( aimAssistToggle.isOn == true )
            result = 1;

        PlayerPrefs.SetInt( "AimAssist", result );
    }

    public void SetDayNightCycle()
    {
        int result = 0;
        if ( dayNightCycleToggle.isOn == true )
            result = 1;

        PlayerPrefs.SetInt( "DayNightCycle", result );
    }
    public void SetAimingToggle()
    {
        int result = 0;
        if ( aimToggle.isOn == true )
            result = 1;

        PlayerPrefs.SetInt( "AimingEnabled", result );
    }

    public void CloseMenu()
    {
        isOpen = false;
    }
}
